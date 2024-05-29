// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using web_api.Model;
using web_api.Services;

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
        public ActionResult<User> Create(User user)
        {
            var createdUser = _userService.Create(user);

            return CreatedAtRoute("GetUser", new { id = createdUser.Id }, createdUser);
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
                return NotFound();
            }

            _userService.Update(id, userIn);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Remove(id);

            return NoContent();
        }
    }
}
