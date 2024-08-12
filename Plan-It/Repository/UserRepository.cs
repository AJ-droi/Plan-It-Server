using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Plan_It.Data;
using Plan_It.Dto.Authentication;
using Plan_It.Dto.Query;
using Plan_It.Interfaces;
using Plan_It.Models;

namespace Plan_It.Repository
{
    public class UserRepository : IUser
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> UserInfo(string id)
        {
            var userInfo = await _userManager.Users.FirstOrDefaultAsync(t => t.Id == id);

            if (userInfo == null)
            {
                return null;
            }

            return userInfo;
        }

        public async Task<List<ApplicationUser>> GetAllUsers(UserDto query)
        {
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            // Start building the query
            var userQuery = _userManager.Users.AsQueryable();

            // Apply search term filtering
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                userQuery = userQuery.Where(u => u.UserName.Contains(query.SearchTerm)
                                              || u.Email.Contains(query.SearchTerm)
                                              || u.Group.Name.Contains(query.SearchTerm));
            }

            // Apply GroupId filtering
            if (!string.IsNullOrEmpty(query.GroupId))
            {
                var groupId = Guid.Parse(query.GroupId);
                userQuery = userQuery.Where(u => u.GroupId == groupId);
            }

            // Apply sorting
            switch (query.SortField.ToLower())
            {
                case "username":
                    userQuery = query.SortOrder == "desc"
                        ? userQuery.OrderByDescending(u => u.UserName)
                        : userQuery.OrderBy(u => u.UserName);
                    break;
                case "email":
                    userQuery = query.SortOrder == "desc"
                        ? userQuery.OrderByDescending(u => u.Email)
                        : userQuery.OrderBy(u => u.Email);
                    break;
                default: // Default to sorting by CreatedAt
                    userQuery = query.SortOrder == "desc"
                        ? userQuery.OrderByDescending(u => u.CreatedAt)
                        : userQuery.OrderBy(u => u.CreatedAt);
                    break;
            }

            // Apply pagination
            userQuery = userQuery.Skip(skipNumber).Take(query.PageSize);

            // Include related data
            userQuery = userQuery
                            .Include(user => user.Group);
                            
            // Execute the query and return the results
            return await userQuery.ToListAsync();
        }


        public async Task<ApplicationUser> UpdateUser(string id, ApplicationUser user)
        {
            var userInfo = await _userManager.Users.FirstOrDefaultAsync(t => t.Id == id);

            if (userInfo == null)
            {
                return null;
            }

            userInfo.BirthDay = user.BirthDay;
            userInfo.UserStatus = user.UserStatus;
            userInfo.GroupId = user.GroupId;


            await _userManager.UpdateAsync(userInfo);
            return userInfo;
        }

        public async Task<ApplicationUser> RemoveUser(string id)
        {
            var userInfo = await _userManager.Users.FirstOrDefaultAsync(t => t.Id == id);

            if (userInfo == null)
            {
                return null;
            }

            await _userManager.DeleteAsync(userInfo);
            return userInfo;
        }


        public async Task<ApplicationUser> AssignTaskInstruction(string id, AssignTaskInstructionDto assignTaskInstruction){
            var userInfo = await _userManager.Users.FirstOrDefaultAsync(t => t.Id == id);

            if(userInfo == null){
                return null;
            }

            userInfo.TaskInstruction = assignTaskInstruction.instruction;

            await _userManager.UpdateAsync(userInfo);
            return userInfo;
        }


    }
}