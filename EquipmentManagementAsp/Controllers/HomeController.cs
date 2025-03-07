using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EquipmentManagementAsp.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Text;
using EquipmentManagementAsp.Services;

namespace EquipmentManagementAsp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Create()
        {
            // Lógica para adicionar
            return View();
        }
        
        public IActionResult Delete()
        {
            // Lógica para excluir
            return View();
        }

        public IActionResult CSVImport()
        {
            return View();
        } 
    }
}
