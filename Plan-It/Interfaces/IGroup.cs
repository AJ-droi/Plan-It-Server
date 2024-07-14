using Plan_It.Dto.Query;
using Plan_It.Models;

namespace Plan_It.Interfaces{
    public interface IGroup{
        Task<Group> CreateAsync(Group group);
        Task<Group> GroupInfo(Guid id);
        Task<List<Group>> GetAllGroups(GroupDto query);
         Task<Group> UpdateGroup(Guid id, Group group);
         Task<Group> RemoveTask(Guid id);
    }
}