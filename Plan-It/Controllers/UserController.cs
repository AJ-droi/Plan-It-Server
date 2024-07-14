using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan_It.Dto.Query;
using Plan_It.Interfaces;
using Plan_It.Models;
using Plan_It.Repository;

namespace Plan_It.Controllers
{

    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userRepo;
        public UserController(IUser userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserDto queryDto)
        {
            var users = await _userRepo.GetAllUsers(queryDto);

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetSingleUser([FromRoute] string id)
        {
            try
            {
                var user = await _userRepo.UserInfo(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] ApplicationUser updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userInfo = await _userRepo.UserInfo(id);

                if (userInfo == null)
                {
                    return NotFound();
                }


                userInfo.BirthDay = updateUserDto.BirthDay;
                userInfo.UserStatus = updateUserDto.UserStatus;
                userInfo.TaskId = updateUserDto.TaskId;
                userInfo.GroupId = updateUserDto.GroupId;

                var updatedUser = await _userRepo.UpdateUser(id, userInfo);

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            try
            {
                var user = await _userRepo.RemoveUser(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}