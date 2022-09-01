using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daurecard.Data;
using Daurecard.Models.ViewModels;
using Daurecard.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Daurecard.Controllers
{
    public class DownloadVCardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DownloadVCardController(ApplicationDbContext context, IWebHostEnvironment hostEnvironement)
        {
            _context = context;
            _hostEnvironment = hostEnvironement;
        }
        //GET
        public async Task<IActionResult> DownloadVCard(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var download = await _context.DownloadVCards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (download == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.Include(c => c.Entreprise).FirstOrDefaultAsync(x => x.Id == download.ClientId);
            ViewBag.Facebook = client.Facebook;
            ViewBag.Twitter = client.Twitter;
            ViewBag.LinkedIn = client.LinkdeIn;
            var days = (DateTime.Now - download.StartDate).Days;

            if(days > 30)
            {
                return View("ExpiryDate");
            }
            else
            {
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadVCard(Guid? id, DownloadVCardViewModel obj)
        {
            if (id == null)
            {
                return NotFound();
            }

            var download = await _context.DownloadVCards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (download == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.Include(c => c.Entreprise).FirstOrDefaultAsync(x => x.Id == download.ClientId);
            var vcard = new VCard()
            {
                FirstName = client.Nom,
                LastName = client.PostNom,
                Phone = client.Tel,
                Organization = client.Entreprise.Nom,
            };
            vcard.GetFullName();

            string wwwRootPath = _hostEnvironment.WebRootPath;

            var fileName = wwwRootPath + "/VCF/" + client.Nom + ".vcf";
            var disposition = "attachment; filename=" + fileName;

            System.IO.File.WriteAllText(fileName, vcard.ToString());

            ViewBag.FileName = fileName;

            ////get a reference to the response
            HttpResponse response = HttpContext.Response;

            response.ContentType = "text/vcard";
            response.Headers.Add("Content-Disposition", "attachment; filename=" + Path.GetFileName(fileName));
            response.Headers.Add("Transfer-Encoding", "identity");
            var bytes = Encoding.UTF8.GetBytes(vcard.ToString());
            await response.Body.WriteAsync(bytes.AsMemory(0, bytes.Length));

            return View();
        }
    }
}
