using Microsoft.AspNetCore.Identity;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.Entities;

public class User : IdentityUser<int>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime? DeletedAt { get; set; }
    internal ICollection<ClientMeeting> ClientMeetings { get; set; }
}

public class Role : IdentityRole<int> 
{
    public ICollection<Permission> Permissions { get; set; }
}

public class Permission : StrongEntity
{
    public string Name { get; set; }
}

public class RolePermission : WeakCompositeEntity<int, int>
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}
