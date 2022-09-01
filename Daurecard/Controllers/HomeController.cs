using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Daurecard.Models;
using Microsoft.AspNetCore.Authorization;


using SKM.V3;
using SKM.V3.Methods;
using SKM.V3.Models;
using Daurecard.Data;

namespace Daurecard.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            Subscribe _subscribe = new Subscribe();
            var licenseKey = "KBGKO-AAGWB-WIEMS-QIHKB"; // <--  remember to change this to your license key
            var RSAPubKey = "<RSAKeyValue><Modulus>vByFUbAkl1KIRFkFeRfqPvDeiWBO+J435D2Vkp9Yz3UyauER+y+MNHNChE7AkJrfVjmk08pIIEQf4bosqW54WdlXlcdnX2nW5O3rJhhxwoosSbl71/vFcLdeYtSLFsNXdu2zwYIbeugMILnLKKZfchn37PX4c8PI5MyvwgIt1tDOb3rGEG6k+3e2gyS12jD4vOnOkmkOOu3OwcLDn6qaHMpau3W4lB3JaOFld93xL6uxeoqDqqTa/EyIJQK3h4KSdZbuqpkpZ0khILbG0MMBYv0odtL4IxKKxeev2ADgULgOuiCQStk0p9MMX85PhDUWBed8NVNJmP1yFv5oCZ429w==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            var auth = "WyIyNDAzNDM1NSIsImJqYnNNYTRlRDdrMFo0TzVKS2FOaVhiZ1ZCTkJjRnFGNUhPTGZ6NmkiXQ==";

            var licence = _context.Subscribes.FirstOrDefault().LicenceKey;
            var product = _context.Subscribes.FirstOrDefault().ProductId;

            var result = Key.Activate(token: auth, parameters: new ActivateModel()
            {
                Key = licence,
                ProductId = product,  // <--  remember to change this to your Product Id 16455
                Sign = true,
                MachineCode = Helpers.GetMachineCodePI(v: 2)
            });
            var b = result.LicenseKey.HasNotExpired();
            if (result.LicenseKey.Expires.Month == DateTime.Now.Month &&
                result.LicenseKey.Expires.Day != DateTime.Now.Day)
            {
                Console.WriteLine("The license has expired !");
                return RedirectToAction("Subscribe", "Subscribe");
            }


            if (result == null || result.Result == ResultType.Error ||
                !result.LicenseKey.HasValidSignature(RSAPubKey).IsValid() )
            {
                Console.WriteLine("The license does not work.");
                Console.WriteLine(result.Message);
                return RedirectToAction("Subscribe", "Subscribe");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
