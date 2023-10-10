using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Commons.ASPNETCore;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class UnitOfWorkFilterAttribute : ActionFilterAttribute
{
    public Type[] DbContextTypes { get; set; }
    public UnitOfWorkFilterAttribute(params Type[] dbContextTypes)
    {
        DbContextTypes = dbContextTypes;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var result = await next();
        if (result.Exception != null) return;
        var sp = context.HttpContext.RequestServices;
        foreach (var dbCtxType in DbContextTypes)
        {
            DbContext dbCtx = (DbContext)sp.GetRequiredService(dbCtxType);
            await dbCtx.SaveChangesAsync();
        }
    }
}
