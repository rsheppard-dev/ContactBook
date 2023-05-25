namespace ContactBook.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Contact>? RecentContacts { get; set; }
        public List<Contact>? FavouriteContacts { get; set; }
        public List<Category>? RecentCategories { get; set; }
        public List<Category>? FavouriteCategories { get; set; }
    }
}