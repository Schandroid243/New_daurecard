using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daurecard.Data;
using Daurecard.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Daurecard.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscribeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View("SubscribePage");
        }

        // GET: Subscribe/Subscription
        public IActionResult Subscribe()
        {
            return View("SubscribePage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(Subscribe obj)
        {
            _context.Subscribes.Add(obj);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
