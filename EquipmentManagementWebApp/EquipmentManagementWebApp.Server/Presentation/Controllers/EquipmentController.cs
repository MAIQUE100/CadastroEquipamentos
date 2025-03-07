using Microsoft.AspNetCore.Mvc;
using EquipmentManagementWebApp.Server.Application;
using EquipmentManagementWebApp.Server.Domain.Interfaces;
using EquipmentManagementWebApp.Server.Presentation.ViewModels;


namespace EquipmentManagementWebApp.Server.Presentation.Controllers
{
    [Route("api/equipments")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService; 
        
        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var equipment = await _equipmentService.GetAllAsync();
            return Ok(equipment);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var equipment = await _equipmentService.GetByIdAsync(id);
            if (equipment == null)
                return NotFound("Equipment not found.");

            return Ok(equipment);
        }

        [HttpPost]
        public IActionResult Create([FromBody] EquipmentViewModel equipmentViewModel)
        {
            if (equipmentViewModel == null)
            {
                return BadRequest("Equipment cannot be empty.");
            }

            var result = _equipmentService.AddAsync(equipmentViewModel);

            if (result != null)
            {
                return Ok("Equipment added successfully.");
            }
            else
            {
                return StatusCode(500, "\r\nError adding equipment.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentViewModel equipmentViewModel)
        {
            Console.WriteLine($"Recebida solicitação para atualizar o equipamento com ID: {id}");

            var equipmentUpdated = await _equipmentService.GetByIdAsync(id);
            if (equipmentUpdated == null)
            {
                return NotFound("Equipment not found.");
            }

            equipmentUpdated.Installation = equipmentViewModel.Installation;
            equipmentUpdated.Batch = equipmentViewModel.Batch;
            equipmentUpdated.Operator = equipmentViewModel.Operator;
            equipmentUpdated.Manufacturer = equipmentViewModel.Manufacturer;
            equipmentUpdated.Model = equipmentViewModel.Model;
            equipmentUpdated.Version = equipmentViewModel.Version;

            var updated = await _equipmentService.UpdateAsync(equipmentUpdated);

            if (!updated)
            {
                return BadRequest("Error updating equipment.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine($"Recebida solicitação para excluir o equipamento com ID: {id}");

            var deleted = await _equipmentService.DeleteAsync(id);
            if (!deleted)
                return NotFound("Equipment not found.");

            return NoContent();
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] IEnumerable<EquipmentViewModel> equipmentViewModel)
        {
            if(equipmentViewModel == null || !equipmentViewModel.Any())
            {
                return BadRequest("Equipments cannot be empty.");
            }

            try
            {
                await _equipmentService.ImportEquipmentsAsync(equipmentViewModel);

                return Ok("Equipments imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao importar equipamentos: {ex.Message}");
            }
        }
    }
}
