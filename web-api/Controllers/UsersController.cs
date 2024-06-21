using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using web_api.Model;
using web_api.Services;
using web_api.Interfaces;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UsersService _userService;

        public UsersController(IConfiguration configuration, UsersService userService)
        {
            _userService = userService;
            _configuration = configuration;
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
            // Verificar se o modelo é válido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userService.ValidateUser(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized("Credenciais inválidas.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [Authorize]
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

        [Authorize]
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
            // Obter a chave secreta do appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]; // Obter como string

            // Converter a chave de string para bytes
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            // Usar a chave secreta para gerar as credenciais de assinatura
            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            // Construir as reivindicações (claims) para o token JWT
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email)
    };

            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            // Criar e configurar o token JWT
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            // Escrever o token JWT como uma string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class LoginRequest
        {
            [Required(ErrorMessage = "O campo Email é obrigatório.")]
            [EmailAddress(ErrorMessage = "O campo Email deve conter um endereço de e-mail válido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "O campo Senha é obrigatório.")]
            public string Password { get; set; }
        }
    }
}
