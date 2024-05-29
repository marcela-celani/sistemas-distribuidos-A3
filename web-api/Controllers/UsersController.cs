// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using web_api.Model;
using web_api.Services;
using web_api.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpPost]
        public ActionResult<IUser> Create(User user)  // Alterado para User
        {
            // Validações
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

            return Ok("Usuário excluido com sucesso.");
        }
    }
}
