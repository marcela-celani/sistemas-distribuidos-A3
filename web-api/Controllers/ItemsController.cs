using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
        private readonly UsersService _userService;

        public ItemsController(ItemsService itemsService, UsersService userService)
        {
            _itemsService = itemsService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<Items>> Get() => _itemsService.Get();

        [Authorize]
        [HttpGet("user/{userId}")]
        public ActionResult<List<Items>> GetByUserId(string userId)
        {
            // Verificar se o ID fornecido é válido
            if (!ObjectId.TryParse(userId, out _))
            {
                return BadRequest("ID inválido.");
            }

            var items = _itemsService.GetByUserId(userId);

            if (items == null || items.Count == 0)
            {
                return NotFound("Itens não encontrados.");
            }

            return items;
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetItem")]
        public ActionResult<Items> Get(string id)
        {
            // Verificar se o ID fornecido é válido
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("ID inválido.");
            }

            var item = _itemsService.Get(id);

            if (item == null)
            {
                return NotFound("Item não encontrado.");
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
            catch (Exception)
            {
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] Items itemIn)
        {
            // Obter o UserId do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar se o ID do item fornecido é válido
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("ID do item inválido.");
            }

            // Verificar se o item existe no banco de dados
            var existingItem = _itemsService.Get(id);
            if (existingItem == null)
            {
                return NotFound("Item não encontrado.");
            }

            try
            {
                // Verificar se o UserId fornecido existe no banco de dados e se pertence ao usuário autenticado
                var userExists = _userService.Get(userId); // Assumindo que _userService.Get(userId) retorna o usuário ou null se não encontrado
                if (userExists == null)
                {
                    return NotFound("UserId inexistente.");
                }

                // Verificar se o item pertence ao usuário autenticado
                if (existingItem.UserId != userId)
                {
                    return Forbid("UserId não autorizado.");
                }

                // Garantir que o Id do item não seja alterado
                itemIn.Id = id;
                _itemsService.Update(id, itemIn);
                return Ok("Item atualizado com sucesso.");
            }
            catch (ArgumentException)
            {
                return BadRequest("Dados inválidos fornecidos.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("Usuário não autorizado.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno do servidor.");
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
            catch (Exception)
            {
                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }
}
