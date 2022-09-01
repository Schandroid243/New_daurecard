using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Daurecard.Data;
using Daurecard.Models;
using Daurecard.Models.ViewModels;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using static QRCoder.PayloadGenerator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace Daurecard.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ClientController(ApplicationDbContext context, IWebHostEnvironment hostEnvironement)
        {
            _context = context;
            _hostEnvironment = hostEnvironement;
        }

        // GET: Client
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber, string currentFilter)
        {
            var applicationDbContext = _context.Clients.Include(c => c.Entreprise);
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var client = from c in applicationDbContext
                           select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                client = client.Where(s => s.PostNom.Contains(searchString)
                                       || s.Nom.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    client = client.OrderByDescending(s => s.PostNom);
                    break;
                case "Date":
                    client = client.OrderBy(s => s.Nom);
                    break;
                case "date_desc":
                    client = client.OrderByDescending(s => s.Prenom);
                    break;
                default:
                    client = client.OrderBy(s => s.PostNom);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Client>.CreateAsync(client.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        // GET: Client/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Entreprise)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        
        // GET: Client/Create
        public IActionResult Create()
        {
            ClientViewModel clientVM = new ClientViewModel()
            {
                Client = new Client(),
                EntreprisesList = _context.Entreprises.Select(i => new SelectListItem
                {
                    Text = i.Nom,
                    Value = i.Id.ToString()
                })
            };
            return View(clientVM);
        }

        // POST: Client/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel obj)
        {
            if (ModelState.IsValid)
            {
                
                //Save image to wwwroot/Image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(obj.Client.ImageFile.FileName);
                string extension = Path.GetExtension(obj.Client.ImageFile.FileName);

                obj.Client.ProfileImage = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await obj.Client.ImageFile.CopyToAsync(fileStream);
                }
                _context.Clients.Add(obj.Client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }


      
        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ClientViewModel clientVM = new ClientViewModel()
            {
                Client = new Client(),
                EntreprisesList = _context.Entreprises.Select(i => new SelectListItem
                {
                    Text = i.Nom,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                return NotFound();
            }

            clientVM.Client = await _context.Clients.FindAsync(id);
            if (clientVM.Client == null)
            {
                return NotFound();
            }
            return View(clientVM);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientViewModel obj)
        {
            if (id != obj.Client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var client = await _context.Clients.FindAsync(obj.Client.Id);
                    client.Nom = obj.Client.Nom;
                    client.Prenom = obj.Client.Prenom;
                    client.PostNom = obj.Client.PostNom;
                    client.Tel = obj.Client.Tel;
                    client.EntrepriseId = obj.Client.EntrepriseId;
                    

                    if (obj.Client.ImageFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string path = "";
                        string fileName = Path.GetFileNameWithoutExtension(obj.Client.ImageFile.FileName);
                        string extension = Path.GetExtension(obj.Client.ImageFile.FileName);
                        obj.Client.ProfileImage = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await obj.Client.ImageFile.CopyToAsync(fileStream);
                        }
                        client.ProfileImage = obj.Client.ProfileImage;
                    }
                    _context.Clients.Update(client);
                        await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(obj.Client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(obj);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Entreprise)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            TempData["AlertMessage"] = "Client supprimé avec succès";
            return RedirectToAction(nameof(Index));
        }


        //GET
        public IActionResult CreateQrCode()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQrCode(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Entreprise)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }



            var downloadVcardVM = await _context.DownloadVCards.FirstOrDefaultAsync(x => x.ClientId == client.Id);
            var dateTime = DateTime.Now;

            if(downloadVcardVM == null)
            {
                var myDownload = new DownloadVCard
                {
                    Id = Guid.NewGuid(),
                    ClientId = client.Id,
                    StartDate = dateTime.Date
                };
                _context.DownloadVCards.Add(myDownload);
                await _context.SaveChangesAsync();

                Url generator1 = new("https://localhost:5001/downloadvcard/downloadvcard/" + myDownload.Id + "/");
                using (MemoryStream ms = new())
                {
                    string payload = generator1.ToString();
                    QRCodeGenerator _qrCode = new();
                    QRCodeData _qrCodeData = _qrCode.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new(_qrCodeData);
                    double targetSize = 210d;
                    var ppm = (int)Math.Floor(targetSize / _qrCodeData.ModuleMatrix.Count());
                    Bitmap bitMap = qrCode.GetGraphic(ppm);

                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QrCodeUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray()));
                }
            }
            else
            {
                var download = await _context.DownloadVCards.FirstOrDefaultAsync(x => x.ClientId == client.Id);
                
                Url generator2 = new("https://localhost:5001/downloadvcard/downloadvcard/" + download.Id + "/");
                using (MemoryStream ms = new())
                {
                    string payload = generator2.ToString();
                    QRCodeGenerator _qrCode = new();
                    QRCodeData _qrCodeData = _qrCode.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new(_qrCodeData);
                    double targetSize = 210d;
                    var ppm = (int)Math.Floor(targetSize / _qrCodeData.ModuleMatrix.Count());
                    Bitmap bitMap = qrCode.GetGraphic(ppm);

                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QrCodeUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray()));
                }
            }
            

            //Url generator = new("https://localhost:5001/downloadvcard/downloadvcard/" +downloadVcardVM.Id + "/");
            //using (MemoryStream ms = new())
            //{
            //    string payload = generator.ToString();
            //    QRCodeGenerator _qrCode = new();
            //    QRCodeData _qrCodeData = _qrCode.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            //    QRCode qrCode = new(_qrCodeData);
            //    double targetSize = 210d;
            //    var ppm = (int)Math.Floor(targetSize / _qrCodeData.ModuleMatrix.Count());
            //    Bitmap bitMap = qrCode.GetGraphic(ppm);
                
            //    bitMap.Save(ms, ImageFormat.Png);
            //    ViewBag.QrCodeUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray()));
            //}

            return View();
        }

        //Need a refactor
        public IActionResult DownloadQrCode()
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData("data:image/png;base64,{0}");

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var yourImage = Image.FromStream(mem))
                    {
                        yourImage.Save("~/Documents/PrivateProject/", ImageFormat.Png);
                    }
                }
            }
            return RedirectToAction("CreateQrCode");
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
