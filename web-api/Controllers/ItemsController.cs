// Controllers/ItemsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
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
                // Obter o UserId do usuário autenticado
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Validar se o userId no corpo da requisição é o mesmo do usuário autenticado
                if (item.UserId != userId)
                {
                    return BadRequest("Você só pode criar itens para o seu próprio usuário.");
                }

                // Continuar com a criação do item
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
        public IActionResult Update(string id, [FromBody] Items itemIn)
        {
            // Obter o UserId do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar se o ID fornecido é válido
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID inválido.");
            }

            // Verificar se o item existe no banco de dados
            var existingItem = _itemsService.Get(id);
            if (existingItem == null)
            {
                return NotFound("Item não encontrado.");
            }

            // Verificar se o item pertence ao usuário autenticado
            if (existingItem.UserId != userId)
            {
                return Forbid("Você não tem permissão para atualizar este item.");
            }

            try
            {
                // Garantir que o Id do item não seja alterado
                itemIn.Id = id;
                _itemsService.Update(id, itemIn);
                return Ok("Item atualizado com sucesso.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

            try
            {
                // Remover o item, verificando se o usuário tem permissão
                _itemsService.Remove(id, userId);
                return Ok("Item excluído com sucesso.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("Usuário não autorizado.");
            }
            catch (ArgumentException)
            {
                return NotFound("Item não encontrado.");
            }
        }

    }
}