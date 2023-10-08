using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Commons.Helpers;

public static class ReflectionHelper
{
    /// <summary>
    /// 据产品名称获取AppDomain中的程序集
    /// </summary>
    /// <param name="productName"></param>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetAssembliesByProductName(string productName)
    {
        var asms = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var asm in asms)
        {
            var asmCompanyAttr = asm.GetCustomAttribute<AssemblyProductAttribute>();
            if (asmCompanyAttr != null && asmCompanyAttr.Product == productName)
            {
                yield return asm;
            }
        }
    }

    /// <summary>
    /// 获取当前应用程序中引用的所有程序集。
    /// </summary>
    /// <param name="skipSystemAssemblies">是否跳过系统程序集，默认为true。</param>
    /// <returns>引用的所有程序集集合。</returns>
    public static IEnumerable<Assembly> GetAllReferencedAssemblies(bool skipSystemAssemblies = true)
    {
        // 获取根程序集
        Assembly? rootAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        var returnAssemblies = new HashSet<Assembly>(new AssemblyEquality());
        var loadedAssemblies = new HashSet<string>();
        var assembliesToCheck = new Queue<Assembly>();
        
        // 如果不跳过系统程序集，或者跳过系统程序集但根程序集不是系统程序集，则将根程序集添加到返回集合中
        if (!skipSystemAssemblies || skipSystemAssemblies && !IsSystemAssembly(rootAssembly))
        {
            if(IsValid(rootAssembly)) returnAssemblies.Add(rootAssembly);
        }

        // 遍历待检查的程序集队列获取所有其引用的程序集，从根程序集开始
        assembliesToCheck.Enqueue(rootAssembly);
        while (assembliesToCheck.Any())
        {
            var assemblyToCheck = assembliesToCheck.Dequeue();
            foreach (AssemblyName reference in assemblyToCheck.GetReferencedAssemblies())
            {
                // 如果已加载的程序集中不包含当前的引用程序集，则加载引用程序集并添加到待检查的程序集队列和已加载程序集集合中
                if (!loadedAssemblies.Contains(reference.FullName))
                {
                    var asm = Assembly.Load(reference);
                    if (skipSystemAssemblies && IsSystemAssembly(asm)) continue;
                    assembliesToCheck.Enqueue(asm);
                    loadedAssemblies.Add(reference.FullName);
                    if (IsValid(asm)) returnAssemblies.Add(asm);
                }
            }
        }

        // 获取基础目录下的所有dll文件
        var asmInBaseDir = Directory.EnumerateDirectories(AppContext.BaseDirectory, "*.dll", new EnumerationOptions { RecurseSubdirectories = true });
        foreach (var asmPath in asmInBaseDir)
        {
            // 如果不是托管程序集，则继续下一个循环
            if (!IsManagedAssembly(asmPath)) continue;
            AssemblyName asmName = AssemblyName.GetAssemblyName(asmPath);
            // 如果返回集合中已经存在相同定义的程序集，则继续下一个循环
            if (returnAssemblies.Any(x => AssemblyName.ReferenceMatchesDefinition(x.GetName(), asmName))) continue;
            if (skipSystemAssemblies && IsSystemAssembly(asmPath)) continue;
            // 尝试加载程序集
            Assembly? asm = TryLoadAssembly(asmPath);
            if (asm == null) continue;
            if (!IsValid(asm)) continue;
            if (skipSystemAssemblies && IsSystemAssembly(asm)) continue;
            // 添加到返回集合中
            returnAssemblies.Add(asm);
        }
        return returnAssemblies;
    }


    /// <summary>
    /// 判断指定的程序集是否为系统程序集。
    /// </summary>
    /// <param name="asm">指定程序集</param>
    /// <returns>如果程序集为系统程序集，则返回true；否则返回false。</returns>
    private static bool IsSystemAssembly(Assembly asm)
    {
        var asmCompanyAttr = asm.GetCustomAttribute<AssemblyCompanyAttribute>();
        if (asmCompanyAttr == null) return false;
        else
        {
            string companyName = asmCompanyAttr.Company;
            return companyName.Contains("Microsoft");
        }
    }

    /// <summary>
    /// 判断指定的程序集是否为系统程序集。
    /// </summary>
    /// <param name="asmPath">要检查的程序集文件路径。</param>
    /// <returns>如果程序集为系统程序集，则返回true；否则返回false。</returns>
    private static bool IsSystemAssembly(string asmPath)
    {
        var moduleDef = AsmResolver.DotNet.ModuleDefinition.FromFile(asmPath);
        var asm = moduleDef.Assembly;
        if (asm == null) return false;
        var asmCompanyAttr = asm.CustomAttributes.FirstOrDefault(c => c.Constructor?.DeclaringType?.FullName == typeof(AssemblyCompanyAttribute).FullName);
        if (asmCompanyAttr == null) return false;
        var companyName = ((AsmResolver.Utf8String?)asmCompanyAttr.Signature?.FixedArguments[0]?.Element)?.Value;
        if (companyName == null) return false;
        return companyName.Contains("Microsoft");
    }

    /// <summary>
    /// 判断指定的文件是否为托管程序集。
    /// </summary>
    /// <param name="file">要检查的文件路径。</param>
    /// <returns>如果文件为托管程序集，则返回true；否则返回false。</returns>
    private static bool IsManagedAssembly(string file)
    {
        using var fs = File.OpenRead(file);
        using PEReader peReader = new PEReader(fs);
        return peReader.HasMetadata && peReader.GetMetadataReader().IsAssembly;
    }

    /// <summary>
    /// 尝试加载给定路径的程序集
    /// </summary>
    /// <param name="asmPath">程序集文件的路径</param>
    /// <returns>加载成功时返回Assembly对象，否则返回null</returns>
    private static Assembly? TryLoadAssembly(string asmPath)
    {
        AssemblyName asmName = AssemblyName.GetAssemblyName(asmPath);
        Assembly? asm = null;
        try
        {
            asm = Assembly.Load(asmName);
        }
        catch (BadImageFormatException ex)
        {
            Debug.WriteLine(ex);
        }
        catch (FileLoadException ex)
        {
            Debug.WriteLine(ex);
        }
        if (asm == null)
        {
            try
            {
                asm = Assembly.LoadFile(asmPath);
            }
            catch (BadImageFormatException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (FileLoadException ex)
            {
                Debug.WriteLine(ex);
            }
        }
        return asm;
    }

    /// <summary>
    /// 验证给定的程序集是否有效。
    /// </summary>
    /// <param name="asm">要验证的程序集。</param>
    /// <returns>如果程序集有效，则为 true；否则为 false。</returns>
    private static bool IsValid(Assembly asm)
    {
        try
        {
            asm.GetTypes();
            asm.DefinedTypes.ToList();
            return true;
        }
        catch (ReflectionTypeLoadException ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
    }
}



class AssemblyEquality : EqualityComparer<Assembly>
{
    public override bool Equals(Assembly? x, Assembly? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return AssemblyName.ReferenceMatchesDefinition(x.GetName(), y.GetName());
    }

    public override int GetHashCode([DisallowNull] Assembly obj)
    {
        return obj.GetName().FullName.GetHashCode();
    }
}