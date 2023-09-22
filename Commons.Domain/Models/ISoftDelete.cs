namespace Commons.Domain.Models;

public interface ISoftDelete
{
    bool IsDeleted { get;}
    void SoftDelete();
}
