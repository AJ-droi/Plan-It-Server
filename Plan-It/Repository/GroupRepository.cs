using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Plan_It.Data;
using Plan_It.Dto.Query;
using Plan_It.Interfaces;
using Plan_It.Models;

namespace Plan_It.Repository{
    public class GroupRepository : IGroup{
        private readonly ApplicationDBContext _context;

        public GroupRepository(ApplicationDBContext context){
            _context = context;
        }
        public async Task<Group> CreateAsync(Group group){
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<Group> GroupInfo(Guid id){
            var groupInfo = await _context.Groups.FirstOrDefaultAsync(t => t.Id == id);

            if(groupInfo == null){
                return null;
            }

            return groupInfo;
        }

         public async Task<List<Group>> GetAllGroups(GroupDto query){
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            // Start building the query
            var groupQuery = _context.Groups.AsQueryable();

            // Apply search term filtering
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                groupQuery = groupQuery.Where(u => u.Name.Contains(query.SearchTerm)
                                              || u.Description.Contains(query.SearchTerm));

            }

            
            // Apply sorting
            switch (query.SortField.ToLower())
            {
                case "name":
                    groupQuery = query.SortOrder == "desc"
                        ? groupQuery.OrderByDescending(u => u.Name)
                        : groupQuery.OrderBy(u => u.Name);
                    break;
                case "createdat":
                    groupQuery = query.SortOrder == "desc"
                        ? groupQuery.OrderByDescending(t => t.CreatedAt)
                        : groupQuery.OrderBy(t => t.CreatedAt);
                    break;
                default: // Default to sorting by CreatedAt
                    groupQuery = query.SortOrder == "desc"
                        ? groupQuery.OrderByDescending(t => t.CreatedAt)
                        : groupQuery.OrderBy(t => t.CreatedAt);
                    break;
            }

            // Apply pagination
            groupQuery = groupQuery.Skip(skipNumber).Take(query.PageSize);

            // Include related data
            groupQuery = groupQuery.Include(task => task.GroupTasks);

            // Execute the query and return the results
            return await groupQuery.ToListAsync();
        }

        public async Task<Group> UpdateGroup(Guid id, Group group){
            var groupInfo = await _context.Groups.FirstOrDefaultAsync(t => t.Id == id);

            if(groupInfo == null){
                return null;
            }

            groupInfo.Name = group.Name;
            groupInfo.Description = group.Description;
            groupInfo.UserId = group.UserId;


            await _context.SaveChangesAsync();
            return groupInfo;
        }

        public async Task<Group> RemoveTask(Guid id){
            var groupInfo = await _context.Groups.FirstOrDefaultAsync(t => t.Id == id);

            if(groupInfo == null){
                return null;
            }

            _context.Groups.Remove(groupInfo);
            await _context.SaveChangesAsync();
            
            return groupInfo;
        }

        
    }
}