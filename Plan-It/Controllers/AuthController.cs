
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan_It.Data;
using Plan_It.Dto.Authentication;
using Plan_It.Interfaces;
using Plan_It.Models;
using Task = System.Threading.Tasks.Task;

namespace Plan_It.Controllers
{

    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDBContext _context;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager, ApplicationDBContext context, IEmailService emailService, ITokenService tokenService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto { Status = "Error", Message = "Invalid model state" });
            }

            try
            {
                var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User already exists!" });


                var user = new ApplicationUser
                {
                    UserName = registerDto.Email, // Assuming username is the email
                    Email = registerDto.Email,
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = $"User creation failed! Errors: {errors}" });
                }

                // Send OTP to user's email
                await SendConfirmationEmail(registerDto.Email, user);

                return Ok(new ResponseDto { Status = "Success", Message = "An Otp has been sent to your email addresss" });
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = ex.Message });
            }

        }

        private async Task SendConfirmationEmail(string? email, ApplicationUser? user)
        {
            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Save OTP in the database
            var otpEntry = new OtpEntry
            {
                UserId = user.Id,
                OtpCode = otp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(15) // OTP expires in 15 minutes
            };
            _context.OtpEntries.Add(otpEntry);
            await _context.SaveChangesAsync();
            await _emailService.SendEmailAsync(email, "Confirm Your Email", $"Please confirm your account by using this OTP {otp}.", true);
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] OtpDto otpDto)
        {
            try
            {
                var otpEntry = await _context.OtpEntries.FirstOrDefaultAsync(o => o.OtpCode == otpDto.Otp);
                if (otpEntry == null)
                    return BadRequest(new ResponseDto { Status = "Error", Message = "Invalid OTP" });

                if (otpEntry.ExpirationTime < DateTime.UtcNow)
                    return BadRequest(new ResponseDto { Status = "Error", Message = "OTP has expired" });

                var user = await _userManager.FindByIdAsync(otpEntry.UserId);
                if (user == null)
                    return BadRequest(new ResponseDto { Status = "Error", Message = "User not found" });

                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                _context.OtpEntries.Remove(otpEntry);
                await _context.SaveChangesAsync();

                return Ok(new ResponseDto { Status = "Success", Message = "Email confirmed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = ex.Message });
            }

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user == null) return Unauthorized(new ResponseDto{
                Status = "Email Error",
                Message = "Email not found"
            });

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized(new ResponseDto{
                Status = "Password Error",
                Message = "Password not correct"
            });

            return Ok(
                new NewUserDto
                {
                    Status = "Success",
                    Message = "Login Successful",
                    Token = _tokenService.CreateToken(user)
                }
            );
        }

    }

}

