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
        public Task AddContactToCategoryAsync(int categoryId, int contactId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Category>> GetContactCategoriesAsync(int contactId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId)
        {
            List<Category> categories = new List<Category>();

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

        public Task<bool> IsContactInCategory(int categoryId, int contactId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveContactFromCategoryAsync(int categoryId, int contactId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Contact> SearchForContacts(string searchString, int userId)
        {
            throw new NotImplementedException();
        }
    }
}