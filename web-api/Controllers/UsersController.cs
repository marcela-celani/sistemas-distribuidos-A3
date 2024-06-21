using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using web_api.Model;
using web_api.Services;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using System;
using web_api.Interfaces;

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
            try
            {
                var user = _userService.Get(id);

                if (user == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                return user;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Sign Up")]
        public ActionResult<IUser> Register([FromBody] User user)
        {
            try
            {
                var createdUser = _userService.Create(user);
                return CreatedAtRoute("GetUser", new { id = createdUser.Id }, createdUser);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] User userIn)
        {
            // Obter o UserId do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar se o ID fornecido é válido
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID inválido.");
            }

            // Verificar se o ID fornecido é diferente do UserId do usuário autenticado
            if (id != userId)
            {
                return Forbid("Você não tem permissão para atualizar este usuário.");
            }

            try
            {
                // Garantir que o ID do usuário não seja alterado
                userIn.Id = id;
                _userService.Update(id, userIn);
                return Ok("Usuário atualizado com sucesso.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            // Obter o UserId do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar se o ID fornecido é válido
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID inválido.");
            }

            // Verificar se o ID fornecido é diferente do UserId do usuário autenticado
            if (id != userId)
            {
                return Forbid("Você não tem permissão para excluir este usuário.");
            }

            try
            {
                _userService.DeleteUserAndItems(id);
                return Ok("Usuário e itens relacionados excluídos com sucesso.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);
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
