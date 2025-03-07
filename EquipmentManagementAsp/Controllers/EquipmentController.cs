using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EquipmentManagementAsp.Models;
using Newtonsoft.Json;
using System.Text;
using EquipmentManagementAsp.Services;

namespace EquipmentManagementAsp.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService, HttpClient httpClient)
        {
            _equipmentService = equipmentService;
            _httpClient = httpClient;
            httpClient.BaseAddress = new Uri("http://localhost:5090/api/equipments/");
        }

        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var response = await _httpClient.GetStringAsync("");
            var equipments = JsonConvert.DeserializeObject<List<Equipment>>(response);
            return View(equipments);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetStringAsync($"{id}");
            var equipment = JsonConvert.DeserializeObject<Equipment>(response);
            return View(equipment);
        }

        public IActionResult GetById()
        {
            return View();
        }
        [HttpGet()]
        public async Task<IActionResult> GetById(int id)
        {

            if (id <= 0)
            {
                return View();
            }

            try
            {
                var response = await _httpClient.GetAsync(id.ToString());

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.ErrorMessage = "Equipamento não encontrado.";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Erro ao buscar equipamento: {response.ReasonPhrase}";
                    }
                    return View();
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var equipment = JsonConvert.DeserializeObject<Equipment>(responseString);

                ViewData["Equipment"] = equipment;
                return View(equipment);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Erro ao conectar com o servidor. Tente novamente mais tarde.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Equipment equipment)
        {
            Console.WriteLine(JsonConvert.SerializeObject(equipment));
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("", equipment);

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.SuccessMessage = "Equipamento criado com sucesso";
                        return RedirectToAction("GetAll");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Erro ao adicionar o equipamento. Tente novamente.";
                        return View(equipment);
                    }
                }
                catch (HttpRequestException)
                {
                    ViewBag.ErrorMessage = "Erro ao se comunicar com a API. Verifique a conexão.";
                    return View(equipment);
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(equipment);
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id < 0)
            {
                return View();
            }

            try
            {
                var response = await _httpClient.GetAsync(id.ToString());

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "Equipamento não encontrado.";
                    return View();
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var equipment = JsonConvert.DeserializeObject<Equipment>(responseString);

                return View(equipment);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Erro ao se conectar com o servidor.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Equipment equipment)
        {
            if (equipment == null || equipment.Id <= 0)
            {
                ViewBag.ErrorMessage = "Os dados do equipamento são inválidos.";
                return View(equipment);
            }

            if (!ModelState.IsValid)
            {
                return View(equipment);
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{equipment.Id}", equipment);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "Equipamento atualizado com sucesso.";
                    return RedirectToAction("GetAll");
                }
                else
                {
                    ViewBag.ErrorMessage = "Erro ao atualizar o equipamento. Tente novamente.";
                    return View(equipment);
                }
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Erro ao se comunicar com a API.";
                return View(equipment);
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id <= 0)
            {
                ViewBag.ErrorMessage = "ID inválido.";
                return View();
            }
            if(id == null) 
            {
                return View();
            }

            ViewBag.Id = id;
            ViewBag.ReadOnly = true;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                ViewBag.ErrorMessage = "ID inválido.";
                return View("Delete");
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"{id}");

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "Equipamento excluído com sucesso.";
                    return View("Delete");
                }
                else
                {
                    ViewBag.ErrorMessage = "Erro ao deletar o equipamento. Tente novamente.";
                    return View("Delete");
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Erro ao se comunicar com a API. Verifique a conexão.");
                return StatusCode(500, ModelState);
            }
        }

        public IActionResult ImportEquipments()
        {
            return View(new List<Equipment>());
        }

        [HttpPost]
        public async Task<IActionResult> ImportCSV(IFormFile file)
        {
            var (success, message, equipments) = await _equipmentService.ImportCSVAsync(file);

            if (!success)
            {
                ModelState.AddModelError("", message);
                return View("ImportEquipments");
            }

            return View("ImportEquipments",equipments);
        }

        [HttpPost]
        public async Task<IActionResult> ImportEquipments(IEnumerable<Equipment> equipments)
        {
            if (equipments == null || !equipments.Any())
            {
                ModelState.AddModelError("", "Nenhum equipamento encontrado.");
                return View("ImportEquipments", new List<Equipment>());
            }

            try
            {
                var jsonEquipments = new StringContent(JsonConvert.SerializeObject(equipments), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("import", jsonEquipments);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "Equipamentos importados com sucesso!";
                    return View("ImportEquipments", new List<Equipment>());
                }
                else
                {
                    ModelState.AddModelError("", "Erro ao importar equipamentos.");
                    return View("ImportEquipments", equipments);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao se comunicar com a API: {ex.Message}");
                return View("ImportEquipments", equipments);
            }
        }
    }
}
