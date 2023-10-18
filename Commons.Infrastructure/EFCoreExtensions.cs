using Commons.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace Commons.Infrastructure;

public static class EFCoreExtensions
{
    /// <summary>
    /// 启用全局软删除过滤器的方法，用于在模型构建器中遍历所有实现ISoftDelete接口的实体对它们设置软删除过滤器。
    /// </summary>
    /// <param name="modelBuilder">模型构建器</param>
    public static void EnableSoftDeletionGlobalFilter(this ModelBuilder modelBuilder)
    {
        var entityTypesHasSoftDeletion = modelBuilder.Model.GetEntityTypes()
            .Where(e => e.ClrType.IsAssignableTo(typeof(ISoftDelete)));

        foreach (var entityType in entityTypesHasSoftDeletion)
        {
            IMutableProperty? isDeletedProperty = entityType.FindProperty(nameof(ISoftDelete.IsDeleted));
            ParameterExpression parameter = Expression.Parameter(entityType.ClrType, "p");
            var filter = Expression.Lambda(Expression.Not(Expression.Property(parameter, isDeletedProperty.PropertyInfo)), parameter); // 定义过滤器。排除isDelete标记为true即被删除的实体
            entityType.SetQueryFilter(filter);
        }
    }

    /// <summary>
    /// 执行查询的扩展方法，返回一个可查询的实体集合。
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="ctx">数据库上下文</param>
    /// <returns>可查询的实体集合</returns>
    public static IQueryable<T> Query<T>(this DbContext ctx) 
        where T : class, IEntity
    {
        return ctx.Set<T>().AsNoTracking();
    }
}
