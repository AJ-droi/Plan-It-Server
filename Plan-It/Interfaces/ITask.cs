
using Plan_It.Dto.Query;
using Plan_It.Models;

namespace Plan_It.Interfaces{
    public interface ITask
    {
        Task<Models.Task> CreateAsync(Models.Task task);
        Task<Models.Task> TaskInfo(Guid id);
        Task<List<Models.Task>> GetAllTasks(TaskDto query);
        Task<Models.Task> RemoveTask(Guid id);
        Task<Models.Task> UpdateTask(Guid id, Models.Task task);
        Task<object> AssignGroupToTask(Guid id, Guid groupId);
}
}