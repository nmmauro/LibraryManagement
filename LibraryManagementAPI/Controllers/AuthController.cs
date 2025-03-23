using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagementAPI.DTOs;
using LibraryManagementAPI.Models;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            // Check if user exists
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest(new { Status = "Error", Message = "User already exists!" });

            // Create new user
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { Status = "Error", Message = "User creation failed! Please check user details and try again.", Errors = result.Errors });

            // Ensure roles exist
            if (!await _roleManager.RoleExistsAsync("Librarian"))
                await _roleManager.CreateAsync(new IdentityRole("Librarian"));
            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new IdentityRole("Customer"));

            // Assign role based on registration
            if (model.Role == "Librarian")
                await _userManager.AddToRoleAsync(user, "Librarian");
            else
                await _userManager.AddToRoleAsync(user, "Customer");

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { Status = "Error", Message = "Invalid credentials" });

            // Check password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return Unauthorized(new { Status = "Error", Message = "Invalid credentials" });

            // Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // Create claims for JWT
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // Generate token
            var token = GetToken(authClaims);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new AuthResponseDTO
            {
                Username = user.UserName,
                Email = user.Email,
                Role = userRoles.FirstOrDefault(),
                Token = tokenString,
                Expiration = token.ValidTo
            });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                claims: authClaims,
                signingCredentials: signIn);

            return token;
        }
    }
}