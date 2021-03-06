using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class SpecialTagsController : Controller
    {
        public readonly ApplicationDbContext _db;

        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.SpecialTags.ToList());
        }

        //Get Create Action Method 
        public IActionResult Create()
        {
            return View();
        }

        //Post Create Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTags specialTags)
        {

            if (ModelState.IsValid)
            {
                _db.Add(specialTags);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTags);
        }

        //Get Edit Action Method 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        //Post Edit Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTags specialTag)
        {
            if (id != specialTag.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _db.Update(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }

        //Get Details Action Method 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        //Get Delete Action Method 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        //Post Delete Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var specialTag = await _db.SpecialTags.FindAsync(id);
            _db.SpecialTags.Remove(specialTag);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}