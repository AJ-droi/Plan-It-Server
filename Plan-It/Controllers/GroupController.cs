using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan_It.Dto.Query;
using Plan_It.Dto.Group;
using Plan_It.Interfaces;
using Plan_It.Models;

namespace Plan_It.Controllers
{

    [Route("group")]
    [ApiController]

    public class GroupController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroup _groupRepo;
        private readonly ILogger<GroupController> _logger;
        public GroupController(UserManager<ApplicationUser> userManager, IGroup groupRepo, ILogger<GroupController> logger)
        {
            _userManager = userManager;
            _groupRepo = groupRepo;
            _logger = logger;
        }

        [HttpPost("create-group")]
        [Authorize]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto createGroupDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                {
                    _logger.LogWarning("Unauthorized attempt to create group. Email claim not found.");
                    return Unauthorized();
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    _logger.LogWarning("Unauthorized attempt to create group. User not found.");
                    return Unauthorized();
                }

                var newGroup = new Group
                {
                    Name = createGroupDto.Name,
                    UserId = user.Id
                };


                var createdGroup = await _groupRepo.CreateAsync(newGroup);

                // update the userId in applicationUser, if the id is located in createGroupDto.Members
                foreach (var userId in createGroupDto.Members)
                {
                    _logger.LogInformation(userId);
                    var userToUpdate = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
                    if (userToUpdate != null)
                    {
                        userToUpdate.GroupId = createdGroup.Id;
                        await _userManager.UpdateAsync(userToUpdate);
                    }
                }
                return Ok(createdGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating group.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("assign-members/{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> AssignMembers([FromRoute] Guid id, [FromBody] List<string> assignMembersDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var group = await _groupRepo.GroupInfo(id);
                if (group == null)
                {
                    _logger.LogWarning("Group not found.");
                    return NotFound();
                }

                // update the userId in applicationUser, if the id is located in assignMembersDto.Members
                foreach (var userId in assignMembersDto)
                {
                    var userToUpdate = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
                    if (userToUpdate != null)
                    {
                        userToUpdate.GroupId = group.Id;
                        await _userManager.UpdateAsync(userToUpdate);
                    }
                }
                return Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning members to group.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("get-all-groups")]
        [Authorize]
        public async Task<IActionResult> GetAllGroups([FromQuery] GroupDto query)
        {
            var tasks = await _groupRepo.GetAllGroups(query);

            return Ok(tasks);
        }

       [HttpGet("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetSingleGroup([FromRoute] Guid id)
        {
            try
            {
                var group = await _groupRepo.GroupInfo(id);

                if (group == null)
                {
                    return NotFound();
                }

                return Ok(group);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateGroup([FromRoute] Guid id, [FromBody] UpdateGroupDto updateGroupDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var groupInfo = await _groupRepo.GroupInfo(id);

                if (groupInfo == null)
                {
                    return NotFound();
                }


                groupInfo.Name = updateGroupDto.Name;
                groupInfo.Description = updateGroupDto.Description;
                groupInfo.UserId = updateGroupDto.UserId;

                var updatedGroup = await _groupRepo.UpdateGroup(id, groupInfo);

                return Ok(updatedGroup);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
        {
            try
            {
                var group = await _groupRepo.RemoveTask(id);

                if (group == null)
                {
                    return NotFound();
                }

                return Ok(group);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}