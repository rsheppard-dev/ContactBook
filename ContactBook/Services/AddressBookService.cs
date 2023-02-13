using ContactBook.Data;
using ContactBook.Models;
using ContactBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Exception = System.Exception;

namespace ContactBook.Services;

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
            // check to see if category already exists
            if (!await IsContactInCategory(categoryId, contactId))
            {
                Contact? contact = await _context.Contacts.FindAsync(contactId);
                Category? category = await _context.Categories.FindAsync((categoryId));

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

    public async Task<bool> IsContactInCategory(int categoryId, int contactId)
    {
        Contact? contact = await _context.Contacts.FindAsync(contactId);

        return await _context.Categories
            .Include(cat => cat.Contacts)
            .Where(cat => cat.Id == categoryId && cat.Contacts.Contains(contact))
            .AnyAsync();
    }

    public async Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId)
    {
        List<Category> categories = new List<Category>();

        try
        {
            categories = await _context.Categories
                .Where(cat => cat.AppUserId == userId)
                .OrderBy(cat => cat.Name)
                .ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }

        return categories;
    }

    public async Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId)
    {
        try
        {
            var contact = await _context.Contacts.Include(c => c.Categories)
                .FirstOrDefaultAsync(c => c.Id == contactId);

            List<int> categoryIds = contact.Categories.Select(c => c.Id).ToList();

            return categoryIds;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICollection<Category>> GetContactCategoriesAsync(int contactId)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveContactFromCategoryAsync(int categoryId, int contactId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Contact> SearchForContacts(string searchString, string userId)
    {
        throw new NotImplementedException();
    }
}