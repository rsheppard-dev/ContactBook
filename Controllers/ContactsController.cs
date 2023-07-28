using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactBook.Data;
using ContactBook.Models;
using Microsoft.AspNetCore.Authorization;
using ContactBook.Enums;
using Microsoft.AspNetCore.Identity;
using ContactBook.Services.Interfaces;
using ContactBook.Models.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ContactBook.Contacts
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IAddressBookService _addressBookService;
        private readonly IEmailSender _emailService;

        public ContactsController(ApplicationDbContext context, UserManager<AppUser> userManager, IImageService imageService, IAddressBookService addressBookService, IEmailSender emailService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _addressBookService = addressBookService;
            _emailService = emailService;
        }

        // GET: Contacts
        [Authorize]
        public IActionResult Index(int categoryId, string? swalMessage = null)
        {
            ViewData["SwalMessage"] = swalMessage;
            
            var contacts = new List<Contact>();
            string appUserId = _userManager.GetUserId(User);

            // return the user id and their associated contacts and categories
            AppUser appUser = _context.Users
                .Include(u => u.Contacts)
                .ThenInclude(u => u.Categories)
                .FirstOrDefault(u => u.Id == appUserId)!;

            var categories = appUser.Categories;

            if (categoryId == 0)
            {
                // if all contacts selected
                contacts = appUser.Contacts
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToList();
            } 
            else if (categoryId == 999)
            {
                // if favourite contacts selected
                contacts = appUser.Contacts
                    .Where(c => c.Favourite)
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToList();
            }
            else
            {
                // if specific category filter applied
                contacts = appUser.Categories
                    .FirstOrDefault(c => c.Id == categoryId)!
                    .Contacts
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToList();
            }

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", categoryId);

            return View(contacts);
        }

        // PATCH: ToggleFavourite
        [Authorize]
        public ActionResult ToggleFavourite(int id)
        {
            var category = _context.Contacts.Find(id)!;
            category.Favourite = !category.Favourite;
            _context.Update(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: SearchContacts
        public IActionResult SearchContacts(string searchString)
        {
            string appUserId = _userManager.GetUserId(User);
            var contacts = new List<Contact>();

            AppUser appUser = _context.Users
                .Include(c => c.Contacts)
                .ThenInclude(c => c.Categories)
                .FirstOrDefault(u => u.Id == appUserId)!;

            if (String.IsNullOrEmpty(searchString))
            {
                contacts = appUser.Contacts
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToList();
            }
            else
            {
                contacts = appUser.Contacts
                    .Where(c => c.FullName!.ToLower().Contains(searchString.ToLower()))
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToList();
            }

            ViewData["CateogryId"] = new SelectList(appUser.Categories, "Id", "Name", 0);

            return View(nameof(Index), contacts);
        }

        // GET: Contacts/EmailContact/5
        [Authorize]
        public async Task<IActionResult> EmailContact(int? id)
        {
            string appUserId = _userManager.GetUserId(User);
            Contact? contact = await _context.Contacts
                .Where(c => c.Id == id && c.AppUserId == appUserId)
                .FirstOrDefaultAsync();
            
            if (contact == null)
            {
                return NotFound(contact);
            }

            EmailData emailData = new()
            {
                EmailAddress = contact.Email!,
                FirstName = contact.FirstName,
                LastName = contact.LastName
            };

            EmailContactViewModel model = new()
            {
                Contact = contact,
                EmailData = emailData
            };

            return View(model);
        }

        //POST: Contacts/EmailContact/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EmailContact(EmailContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Email == model.EmailData!.EmailAddress)!;

                    if(contact == null)
                    {
                        return NotFound();
                    }

                    await _emailService.SendEmailAsync(model.EmailData!.EmailAddress, model.EmailData.Subject, model.EmailData.Body);

                    // update last contact
                    contact.LastContact = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    _context.Update(contact);
                    _context.SaveChanges();

                    return RedirectToAction("Index", "Contacts", new { swalMessage = "Success: Email sent!"});
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Contacts", new { swalMessage = "Error: Email failed to send!"});
                    throw;
                }
            }

            return View(model);
        }

        // GET: Contacts/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            string appUserId = _userManager.GetUserId(User);

            List<Counties> countiesList = Enum.GetValues(typeof(Counties)).Cast<Counties>().ToList();
            IEnumerable<string> counties = countiesList.Select(county => county.ToString().Replace("_", " "));

            ViewData["CountiesList"] = new SelectList(counties);
            ViewData["CategoryList"] = new MultiSelectList(await _addressBookService.GetUserCategoriesAsync(appUserId), "Id", "Name");

            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,BirthDate,Address1,Address2,City,County,PostCode,Email,PhoneNumber,ImageFile")] Contact contact, List<int> CategoryList)
        {
            ModelState.Remove("AppUserId");
            
            if (ModelState.IsValid)
            {
                contact.AppUserId = _userManager.GetUserId(User);
                contact.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                if (contact.BirthDate != null)
                {
                    contact.BirthDate = DateTime.SpecifyKind(contact.BirthDate.Value, DateTimeKind.Utc);
                }

                if (contact.ImageFile != null)
                {
                    contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.ImageFile);
                    contact.ImageType = contact.ImageFile.ContentType;
                }

                _context.Add(contact);
                await _context.SaveChangesAsync();

                // loop over selected categories
                foreach (int categoryId in CategoryList)
                {
                    // save each category selected to the ContactCategories table
                    await _addressBookService.AddContactToCategoryAsync(categoryId, contact.Id);
                }

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Contacts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            string appUserId = _userManager.GetUserId(User);

            var contact = await _context.Contacts
                .Where(c => c.AppUserId == appUserId && c.Id == id)
                .FirstOrDefaultAsync();

            if (contact == null)
            {
                return NotFound();
            }
            
            List<Counties> countiesList = Enum.GetValues(typeof(Counties)).Cast<Counties>().ToList();
            IEnumerable<string> counties = countiesList.Select(county => county.ToString().Replace("_", " "));

            ViewData["CountiesList"] = new SelectList(counties);
            ViewData["CategoryList"] = new MultiSelectList(await _addressBookService.GetUserCategoriesAsync(appUserId), "Id", "Name", await _addressBookService.GetContactCategoryIdsAsync(contact.Id));

            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,FirstName,LastName,BirthDate,Address1,Address2,City,County,PostCode,Email,PhoneNumber,Created,ImageData,ImageType,ImageFile")] Contact contact, List<int> CategoryList)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contact.Created = DateTime.SpecifyKind(contact.Created, DateTimeKind.Utc);
                    
                    if (contact.BirthDate != null)
                    {
                        contact.BirthDate = DateTime.SpecifyKind(contact.BirthDate.Value, DateTimeKind.Utc);
                    }

                    if (contact.ImageFile != null)
                    {
                        contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.ImageFile);
                        contact.ImageType = contact.ImageFile.ContentType;
                    }

                    _context.Update(contact);
                    await _context.SaveChangesAsync();

                    // save categories
                    List<Category> oldCategories = (await _addressBookService.GetContactCategoriesAsync(contact.Id)).ToList();
                    
                    // remove current categories
                    foreach (var category in oldCategories)
                    {
                        await _addressBookService.RemoveContactFromCategoryAsync(category.Id, contact.Id);
                    }
                    
                    // add the selected categories
                    foreach (int categoryId in CategoryList)
                    {
                        await _addressBookService.AddContactToCategoryAsync(categoryId, contact.Id);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", contact.AppUserId);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            string appUserId = _userManager.GetUserId(User);

            var contact = await _context.Contacts
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == appUserId);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string appUserId = _userManager.GetUserId(User);

            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == appUserId);
            
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
          return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
