// Controllers/ItemsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using web_api.Model;
using web_api.Services;

namespace web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ItemsService _itemsService;

        public ItemsController(ItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<Items>> Get() => _itemsService.Get();

        [Authorize]
        [HttpGet("user/{userId}")]
        public ActionResult<List<Items>> GetByUserId(string userId) => _itemsService.GetByUserId(userId);

        [Authorize]
        [HttpGet("{id}", Name = "GetItem")]
        public ActionResult<Items> Get(string id)
        {
            var item = _itemsService.Get(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Items> Create(Items item)
        {
            try
            {
                _itemsService.Create(item);
                return CreatedAtRoute("GetItem", new { id = item.Id.ToString() }, item);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(string id, Items itemIn)
        {
            var item = _itemsService.Get(id);

            if (item == null)
            {
                return NotFound("Tarefa não encontrada.");
            }

            _itemsService.Update(id, itemIn);

            return Ok("Tarefa atualizada com sucesso.");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var item = _itemsService.Get(id);

            if (item == null)
            {
                return NotFound("Tarefa não encontrada.");
            }

            _itemsService.Remove(item.Id);

            return Ok("Tarefa excluida com sucesso.");
        }
    }
}