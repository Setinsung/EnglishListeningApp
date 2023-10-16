using Commons.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

public class User : IdentityUser<Guid>, ICreatedTime, IDeletedTime, ISoftDelete
{
    public DateTime CreatedTime { get; init; }

    public DateTime? DeletedTime { get; private set; }

    public bool IsDeleted { get; private set; }

    public User(string userName) : base(userName)
    {
        this.Id = Guid.NewGuid();
        this.CreatedTime = DateTime.Now;
    }

    public void SoftDelete()
    {
        this.IsDeleted = true;
        this.DeletedTime = DateTime.Now;
    }
}
