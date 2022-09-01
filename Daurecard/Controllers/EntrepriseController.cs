using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Daurecard.Data;
using Daurecard.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Daurecard.Controllers
{
    [Authorize]
    public class EntrepriseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EntrepriseController(ApplicationDbContext context, IWebHostEnvironment hostEnvironement)
        {
            _context = context;
            _hostEnvironment = hostEnvironement;
        }

        // GET: Entreprise
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber, string currentFilter)
        {
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
            var entreprise = from c in _context.Entreprises
                         select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                entreprise = entreprise.Where(s => s.Nom.Contains(searchString)
                                       || s.SecteurActivite.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    entreprise = entreprise.OrderByDescending(s => s.Nom);
                    break;
                
                default:
                    entreprise = entreprise.OrderBy(s => s.SecteurActivite);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Entreprise>.CreateAsync(entreprise.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return View(await _context.Entreprises.ToListAsync());
        }

        // GET: Entreprise/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entreprise = await _context.Entreprises
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entreprise == null)
            {
                return NotFound();
            }

            return View(entreprise);
        }

        // GET: Entreprise/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entreprise/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Entreprise entreprise)
        {
            if (ModelState.IsValid)
            {
                    //Save image to wwwroot/Logo
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(entreprise.LogoFile.FileName);
                    string extension = Path.GetExtension(entreprise.LogoFile.FileName);

                    entreprise.Logo = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Logo/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await entreprise.LogoFile.CopyToAsync(fileStream);
                    }
                    _context.Add(entreprise);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                
            }
            return View(entreprise);
        }

        // GET: Entreprise/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entreprise = await _context.Entreprises.FindAsync(id);
            if (entreprise == null)
            {
                return NotFound();
            }
            return View(entreprise);
        }

        // POST: Entreprise/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Entreprise entreprise)
        {
            if (id != entreprise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var company = await _context.Entreprises.FindAsync(entreprise.Id);
                    company.Nom = entreprise.Nom;
                    company.Email = entreprise.Email;
                    company.Adresse = entreprise.Adresse;
                    company.SecteurActivite = entreprise.SecteurActivite;
                    company.Tel = entreprise.Tel;

                    if(entreprise.LogoFile != null)
                    {
                        //Save image to wwwroot/Logo
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(entreprise.LogoFile.FileName);
                        string extension = Path.GetExtension(entreprise.LogoFile.FileName);

                        entreprise.Logo = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Logo/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await entreprise.LogoFile.CopyToAsync(fileStream);
                        }
                        company.Logo = entreprise.Logo;
                    }

                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntrepriseExists(entreprise.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    } 
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entreprise);
        }

        // GET: Entreprise/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var entreprise = await _context.Entreprises
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (entreprise == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(entreprise);
        //}

        //// POST: Entreprise/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var entreprise = await _context.Entreprises.FindAsync(id);
        //    _context.Entreprises.Remove(entreprise);
        //    await _context.SaveChangesAsync();
        //    TempData["AlertMessage"] = "Entreprise supprimé avec succès !";
        //    return RedirectToAction(nameof(Index));
        //}

        private bool EntrepriseExists(int id)
        {
            return _context.Entreprises.Any(e => e.Id == id);
        }
    }
}
