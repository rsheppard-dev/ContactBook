using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ContactBook.Data;
using ContactBook.Models;
using ContactBook.Models.ViewModels;
using ContactBook.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ContactBook.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailService;

        public CategoriesController(ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IEmailSender emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: Categories
        [Authorize]
        public async Task<IActionResult> Index(string swalMessage = null)
        {
            ViewData["swalMessage"] = swalMessage;
            string appUserId = _userManager.GetUserId(User);

            var categories = await _context.Categories
                .Where(c => c.AppUserId == appUserId)
                .Include(c => c.AppUser)
                .ToListAsync();
                
            return View(categories);
        }
        
        // GET: Categories/EmailCategory/5
        [Authorize]
        public async Task<IActionResult> EmailCategory(int id)
        {
            string appUserId = _userManager.GetUserId(User);
            
            Category category = await _context.Categories
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == appUserId);

            List<string> emails = category.Contacts.Select(c => c.Email).ToList();

            EmailData emailData = new EmailData()
            {
                GroupName = category.Name,
                EmailAddress = String.Join(";", emails),
                Subject = $"Group Message: {category.Name}"
            };

            EmailCategoryViewModel model = new EmailCategoryViewModel()
            {
                Contacts = category.Contacts.ToList(),
                EmailData = emailData
            };
            return View(model);
        }
        
        // POST: Categories/EmailCategory/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailCategory(EmailCategoryViewModel ecvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _emailService.SendEmailAsync(ecvm.EmailData.EmailAddress,
                        ecvm.EmailData.Subject,
                        ecvm.EmailData.Body);

                    return RedirectToAction("Index", "Categories", new { swalMessage = "Success: Email sent!" });
                }
                catch
                {
                    return RedirectToAction("Index", "Categories", new { swalMessage = "Error: Failed to send email!"});
                    throw;
                }
            }
            
            return View(ecvm);
        }

        // GET: Categories/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppUserId,Name")] Category category)
        {
            ModelState.Remove("AppUserId");
            
            if (ModelState.IsValid)
            {
                string appUserId = _userManager.GetUserId(User);
                category.AppUserId = appUserId;
                
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string appUserId = _userManager.GetUserId(User);

            var category = await _context.Categories
                .Where(c => c.Id == id && c.AppUserId == appUserId)
                .FirstOrDefaultAsync();
            
            if (category == null)
            {
                return NotFound();
            }
           
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string appUserId = _userManager.GetUserId(User);
                    category.AppUserId = appUserId;
                    
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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

            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            string appUserId = _userManager.GetUserId(User);
            
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == appUserId);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string appUserId = _userManager.GetUserId(User);
            
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == appUserId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
