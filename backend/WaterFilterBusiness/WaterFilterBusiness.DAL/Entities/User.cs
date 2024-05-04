using Microsoft.AspNetCore.Identity;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.Entities;

public class User : IdentityUser<int>
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;

    public DateTime? DeletedAt { get; set; }
    internal IList<ClientMeeting> ClientMeetings { get; set; } = new List<ClientMeeting>();
}

public class Role : IdentityRole<int> 
{
    public IList<Permission> Permissions { get; set; } = new List<Permission>();
}

public class Permission : StrongEntity
{
    public string Name { get; set; } = null!;
}

public class RolePermission : CompositeEntity<int, int>
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}
