using System;
using Microsoft.AspNetCore.Mvc;

namespace Postcore.Web.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            string name = "dude";
            var date = DateTime.Now;

            return this.Content($"Hey, {name}! Today is {date.DayOfWeek}, it's {date:HH:mm} now.");
        }        
    }
}