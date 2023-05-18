using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactBook.Data;
using ContactBook.Models;
using ContactBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using ContactBook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ContactBook.Categories
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IEmailSender _emailService;

        public CategoriesController(ApplicationDbContext context, UserManager<AppUser> userManager, IImageService imageService, IEmailSender emailService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _emailService = emailService;
        }

        // GET: Categories
        [Authorize]
        public async Task<IActionResult> Index(string swalMessage = null)
        {
            string appUserId = _userManager.GetUserId(User);

            var categories = await _context.Categories
                .Where(c => c.AppUserId == appUserId)
                .Include(c => c.AppUser)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewData["SwalMessage"] = swalMessage;   
            return View(categories);
        }

        // GET: SearchContacts
        [Authorize]
        public IActionResult SearchContacts(string searchString)
        {
            string appUserId = _userManager.GetUserId(User);
            var categories = new List<Category>();

            AppUser appUser = _context.Users
                .Include(c => c.Categories)
                .ThenInclude(c => c.Contacts)
                .FirstOrDefault(u => u.Id == appUserId)!;

            if (String.IsNullOrEmpty(searchString))
            {
                categories = appUser.Categories
                    .OrderBy(c => c.Name)
                    .ToList();
            }
            else
            {
                categories = appUser.Categories
                    .Where(c => c.Name!.ToLower().Contains(searchString.ToLower()))
                    .OrderBy(c => c.Name)
                    .ToList();
            }

            return View(nameof(Index), categories);
        }

        // GET: EmailCategory/5
        [Authorize]
        public async Task<IActionResult> EmailCategory(int id)
        {
            string appUserId = _userManager.GetUserId(User);

            Category? category = await _context.Categories
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == appUserId);

            List<string> emails = category!.Contacts
                .Select(c => c.Email)
                .ToList()!;

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

        // POST: EmailCategory/5
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EmailCategory(EmailCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _emailService.SendEmailAsync(model.EmailData!.EmailAddress, model.EmailData.Subject, model.EmailData.Body);
                    return RedirectToAction("Index", "Categories", new { swalMessage = "Success: Email sent!" });
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Categories", new { swalMessage = "Error: Failed to send message!" });
                    throw;
                }
            }
            
            return View(model);
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
        public async Task<IActionResult> Create([Bind("Id,Name,ImageFile")] Category category)
        {
            ModelState.Remove("AppUserId");

            if (ModelState.IsValid)
            {
                string appUserId = _userManager.GetUserId(User);
                category.AppUserId = appUserId;

                if (category.ImageFile != null)
                {
                    category.ImageData = await _imageService.ConvertFileToByteArrayAsync(category.ImageFile);
                    category.ImageType = category.ImageFile.ContentType;
                }

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,Name,ImageData,ImageType,ImageFile")] Category category)
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

                    if (category.ImageFile != null)
                    {
                        category.ImageData = await _imageService.ConvertFileToByteArrayAsync(category.ImageFile);
                        category.ImageType = category.ImageFile.ContentType;
                    }

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
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            string appUserId = _userManager.GetUserId(User);

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

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == appUserId);

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
