using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using web_api.Model;
using web_api.Services;
using web_api.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;

namespace web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _userService;

        public UsersController(UsersService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<List<User>> Get() => _userService.Get();

        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost("Sign Up")]
        public ActionResult<IUser> Register([FromBody] User user)
        {
            // Validações do usuário
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                return BadRequest("O nome de usuário é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("O email é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("A senha é obrigatória.");
            }

            try
            {
                var createdUser = _userService.Create(user);
                return CreatedAtRoute("GetUser", new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var user = _userService.ValidateUser(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized("Credenciais inválidas.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, User userIn)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userIn.Id) || id != userIn.Id)
            {
                return BadRequest();
            }

            var existingUser = _userService.Get(id);

            if (existingUser == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            _userService.Update(id, userIn);

            return Ok("Usuário atualizado com sucesso.");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            _userService.Remove(id);

            return Ok("Usuário excluído com sucesso.");
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = new byte[32]; // 256 bits = 32 bytes
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(secretKey);
            }

            var credentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
    };

            var issuer = "cadastrei";
            var audience = "credenciais_usuario";

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
