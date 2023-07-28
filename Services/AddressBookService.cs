using ContactBook.Services.Interfaces;
using ContactBook.Models;
using ContactBook.Data;
using ContactBook.Services;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Services
{
    public class AddressBookService : IAddressBookService
    {
        private readonly ApplicationDbContext _context;

        public AddressBookService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddContactToCategoryAsync(int categoryId, int contactId)
        {
            try
            {
                // check if category is in the contact already
                if (!await IsContactInCategory(categoryId, contactId))
                {
                    Contact? contact = await _context.Contacts.FindAsync(contactId);
                    Category? category = await _context.Categories.FindAsync(categoryId);

                    if (category != null && contact != null)
                    {
                        category.Contacts.Add(contact);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<Category>> GetContactCategoriesAsync(int contactId)
        {
            try
            {
                Contact? contact = await _context.Contacts
                    .Include(c => c.Categories)
                    .FirstOrDefaultAsync(c => c.Id == contactId);

                return contact!.Categories;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId)
        {
            try
            {
                var contact = await _context.Contacts
                    .Include(c => c.Categories)
                    .FirstOrDefaultAsync(c => c.Id == contactId);

                List<int> categoryIds = contact!.Categories
                    .Select(c => c.Id)
                    .ToList();

                return categoryIds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId)
        {
            List<Category> categories = new();

            try
            {
                categories = await _context.Categories
                    .Where(c => c.AppUserId == userId)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return categories;
        }

        public async Task<bool> IsContactInCategory(int categoryId, int contactId)
        {
            Contact? contact = await _context.Contacts.FindAsync(contactId);

            return await _context.Categories
                .Include(c => c.Contacts)
                .Where(c => c.Id == categoryId && c.Contacts.Contains(contact!))
                .AnyAsync();
        }

        public async Task RemoveContactFromCategoryAsync(int categoryId, int contactId)
        {
            try
            {
                if (await IsContactInCategory(categoryId, contactId))
                {
                    Contact? contact = await _context.Contacts.FindAsync(contactId);
                    Category? category = await _context.Categories.FindAsync(categoryId);

                    if (contact != null && category != null)
                    {
                        category.Contacts.Remove(contact);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Contact> SearchForContacts(string searchString, int userId)
        {
            throw new NotImplementedException();
        }
    }
}