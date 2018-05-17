using System.Collections.Generic;
using System.Diagnostics;
using DynamicUIForModels.Dynamic;
using Microsoft.AspNetCore.Mvc;
using DynamicUIForModels.Models;

namespace DynamicUIForModels.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(DynamicHelper.GetModel(typeof(User)));
        }

        [HttpPost]
        public IActionResult Save(List<GenericModel> data)
        {
            User user = new User();
            foreach (var item in data)
            {
                DynamicHelper.SetObjectProperty(item.Key, item.Value, user);
            }

            return View(DynamicHelper.GetModel(user));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
