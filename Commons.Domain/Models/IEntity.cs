namespace Commons.Domain.Models
{
    public interface IEntity
    {
        public Guid Id { get; } // 应该物理上用自增的列做主键。逻辑上关联都用Guid
    }
}
