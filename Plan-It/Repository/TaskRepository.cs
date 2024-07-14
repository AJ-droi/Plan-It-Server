using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Plan_It.Data;
using Plan_It.Dto.Query;
using Plan_It.Enum;
using Plan_It.Interfaces;
using Plan_It.Models;

namespace Plan_It.Repository
{
    public class TaskRepository : ITask
    {
        private readonly ApplicationDBContext _context;

        public TaskRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Models.Task> CreateAsync(Models.Task task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Models.Task> TaskInfo(Guid id)
        {
            var taskInfo = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (taskInfo == null)
            {
                return null;
            }

            return taskInfo;
        }

        public async Task<List<Models.Task>> GetAllTasks(TaskDto query)
        {
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            // Start building the query
            var taskQuery = _context.Tasks.AsQueryable();

            // Apply search term filtering
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                taskQuery = taskQuery.Where(u => u.Title.Contains(query.SearchTerm)
                                              || u.Description.Contains(query.SearchTerm));

            }

            // Apply Category filtering
            if (!string.IsNullOrEmpty(query.Category))
            {
                taskQuery = taskQuery.Where(t => t.Category.ToString() == query.Category);
                
            }
            
            // Apply sorting
            switch (query.SortField.ToLower())
            {
                case "title":
                    taskQuery = query.SortOrder == "desc"
                        ? taskQuery.OrderByDescending(u => u.Title)
                        : taskQuery.OrderBy(u => u.Title);
                    break;
                case "createdat":
                    taskQuery = query.SortOrder == "desc"
                        ? taskQuery.OrderByDescending(t => t.CreatedAt)
                        : taskQuery.OrderBy(t => t.CreatedAt);
                    break;
                case "duedate":
                    taskQuery = query.SortOrder == "desc"
                        ? taskQuery.OrderByDescending(t => t.DueDate)
                        : taskQuery.OrderBy(t => t.DueDate);
                    break;
                default: // Default to sorting by CreatedAt
                    taskQuery = query.SortOrder == "desc"
                        ? taskQuery.OrderByDescending(t => t.CreatedAt)
                        : taskQuery.OrderBy(t => t.CreatedAt);
                    break;
            }

            // Apply pagination
            taskQuery = taskQuery.Skip(skipNumber).Take(query.PageSize);

            // Include related data
            taskQuery = taskQuery.Include(task => task.GroupTasks);

            // Execute the query and return the results
            return await taskQuery.ToListAsync();
        }

        public async Task<Models.Task> UpdateTask(Guid id, Models.Task task)
        {
            var taskInfo = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (taskInfo == null)
            {
                return null;
            }

            taskInfo.Title = task.Title;
            taskInfo.Description = task.Description;
            taskInfo.Category = task.Category;
            taskInfo.StartDate = task.StartDate;
            taskInfo.DueDate = task.DueDate;
            taskInfo.TaskStatus = task.TaskStatus;
            taskInfo.StartTime = task.StartTime;


            await _context.SaveChangesAsync();
            return taskInfo;
        }

        public async Task<Models.Task> RemoveTask(Guid id)
        {
            var taskInfo = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (taskInfo == null)
            {
                return null;
            }

            _context.Tasks.Remove(taskInfo);
            await _context.SaveChangesAsync();

            return taskInfo;
        }

        public async Task<object> AssignGroupToTask(Guid id, Guid groupId)
        {
            var taskInfo = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (taskInfo == null)
            {
                return null;
            }

            if (taskInfo.Category == Enum.Category.Personal)
            {
                return "This is not a group Task";
            }

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
            {
                return "Group not found";
            }

            var groupTask = new GroupTask
            {
                TaskId = id,
                GroupId = groupId
            };

            taskInfo.GroupTasks.Add(groupTask);
            await _context.SaveChangesAsync();
            return taskInfo;

        }


    }
}