using Microsoft.EntityFrameworkCore;

namespace Commons.CommonInitializer;

/// <summary>
/// 用于构建设计时工厂时创建统一配置的DbContextOptionsBuilder
/// </summary>
public static class DbContextOptionsBuilderFactory
{
    public static DbContextOptionsBuilder<TDbContext> Create<TDbContext>()
         where TDbContext : DbContext
    {
        var connStr = Environment.GetEnvironmentVariable("DefaultDB:ConnStr");
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        //optionsBuilder.UseSqlServer(connStr);
        optionsBuilder.UseSqlServer("server=.;database=EnglishListeningApp;trusted_connection=true;multipleActiveResultSets=true;encrypt=false");
        return optionsBuilder;
    }
}
