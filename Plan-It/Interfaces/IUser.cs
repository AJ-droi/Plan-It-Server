using Plan_It.Dto.Query;
using Plan_It.Models;

namespace Plan_It.Interfaces{
    public interface IUser{
       Task<ApplicationUser> UserInfo(string id);
       Task<List<ApplicationUser>> GetAllUsers(UserDto query);
       Task<ApplicationUser> UpdateUser(string id, ApplicationUser user);
       Task<ApplicationUser> RemoveUser(string id);
    }
}