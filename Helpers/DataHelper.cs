using ContactBook.Data;
using ContactBook.Enums;
using ContactBook.Models;
using ContactBook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Helpers
{
    public static class DataHelper
    {
        private static string? demoUserId;
        private static int category1Id;
        private static int category2Id;
        private static int category3Id;
        public static async Task ManageDataAsync(IHost host)
        {
            using var svcScope = host.Services.CreateScope();
            var svcProvider = svcScope.ServiceProvider;

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<AppUser>>();
            var addressBookSvc = svcProvider.GetRequiredService<IAddressBookService>();

            await dbContextSvc.Database.MigrateAsync();

            // seed data
            await SeedDemoUserAsync(userManagerSvc);
            await SeedDemoCategoriesAsync(dbContextSvc);
            await SeedDemoContactsAsync(dbContextSvc, addressBookSvc);
        }

        public static async Task SeedDemoUserAsync(UserManager<AppUser> userManager)
        {
            // seed demo user
            var demoUser = new AppUser
            {
                UserName = "demo@contactbook.co.uk",
                Email = "demo@contactbook.co.uk",
                FirstName = "Demo",
                LastName = "User",
                EmailConfirmed = true,
            };

            try
            {
                AppUser? existingUser = await userManager.FindByEmailAsync(demoUser.Email);
                    
                if (existingUser is null)
                {
                    var result = await userManager.CreateAsync(demoUser, "DemoPa$5word");

                    if (result.Succeeded)
                    {
                        demoUserId = demoUser.Id;
                    }
                    else
                    {
                        Console.WriteLine("Failed to create the demo user.");
                    }
                }
                else
                {
                    demoUserId = existingUser.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDemoCategoriesAsync(ApplicationDbContext context)
        {
            // convert image files to byte arrays
            byte[] familyImage = await File.ReadAllBytesAsync("wwwroot/images/demo/family.jpeg");
            byte[] pokerNightImage = await File.ReadAllBytesAsync("wwwroot/images/demo/poker-night.jpeg");
            byte[] footballTeamImage = await File.ReadAllBytesAsync("wwwroot/images/demo/football-team.jpeg");

            try
            {
                IList<Category> categories = new List<Category>() {
                    new Category()
                    {
                        Name = "Family",
                        Favourite = true,
                        ImageData = familyImage,
                        ImageType = "image/jpeg",
                        AppUserId = demoUserId
                    },
                    new Category()
                    {
                        Name = "Poker Night",
                        LastContact = DateTime.Now,
                        ImageData = pokerNightImage,
                        ImageType = "image/jpeg",
                        AppUserId = demoUserId
                    },
                    new Category()
                    {
                        Name = "Football Team",
                        Favourite = true,
                        ImageData = footballTeamImage,
                        ImageType = "image/jpeg",
                        AppUserId = demoUserId
                    }
                };

                List<string?> dbCategories = context.Categories.Select(c => c.Name).ToList();

                await context.Categories.AddRangeAsync(categories.Where(c => !dbCategories.Contains(c.Name)));
                await context.SaveChangesAsync();

                // Get a list of IDs from the categories list
                List<int> categoryIds = categories.Select(c => c.Id).ToList();

                // Set category1Id, category2Id, and category3Id based on the newly added categories
                category1Id = categoryIds[0];
                category2Id = categoryIds[1];
                category3Id = categoryIds[2];
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Categories.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");     
                throw;
            }
        }

        public static async Task SeedDemoContactsAsync(ApplicationDbContext context, IAddressBookService addressBookService)
        {
            // convert image files to byte arrays
            byte[] brandonImage = await File.ReadAllBytesAsync("wwwroot/images/demo/brandon-walsh.jpeg");
            byte[] brookImage = await File.ReadAllBytesAsync("wwwroot/images/demo/brook-mitchell.jpeg");
            byte[] maggieImage = await File.ReadAllBytesAsync("wwwroot/images/demo/maggie-bennett.jpeg");
            byte[] michaelImage = await File.ReadAllBytesAsync("wwwroot/images/demo/michael-clarke.jpeg");
            byte[] nicholasImage = await File.ReadAllBytesAsync("wwwroot/images/demo/nicholas-horn.jpeg");
            byte[] samImage = await File.ReadAllBytesAsync("wwwroot/images/demo/sam-kelly.jpeg");
            byte[] susanImage = await File.ReadAllBytesAsync("wwwroot/images/demo/susan-richards.jpeg");

            try
            {
                IList<Contact> contacts = new List<Contact>() {
                    new Contact()
                    {
                        FirstName = "Brandon",
                        LastName = "Walsh",
                        BirthDate = new DateTime(1991, 07, 25),
                        Email = "bwalsh@contactbook.co.uk",
                        Address1 = "196 Brownhill Ave",
                        City = "Burnley",
                        County = Counties.Lancashire,
                        PostCode = "BB10 4QH",
                        PhoneNumber = "01282702879",
                        ImageData = brandonImage,
                        ImageType = "image/jpeg",
                        Favourite = true,
                        LastContact = DateTime.Now.AddDays(-7),
                        AppUserId = demoUserId
                    },
                    new Contact()
                    {
                        FirstName = "Maggie",
                        LastName = "Bennett",
                        BirthDate = new DateTime(1994, 09, 05),
                        Email = "mbennett@contactbook.co.uk",
                        Address1 = "19 Fisheracre",
                        City = "Arbroath",
                        County = Counties.Angus,
                        PostCode = "DD11 1LE",
                        PhoneNumber = "01241879054",
                        ImageData = maggieImage,
                        ImageType = "image/jpeg",
                        Favourite = true,
                        LastContact = DateTime.Now.AddDays(-2),
                        AppUserId = demoUserId
                    },
                    new Contact()
                    {
                        FirstName = "Michael",
                        LastName = "Clarke",
                        BirthDate = new DateTime(1991, 10, 21),
                        Email = "mclarke@contactbook.co.uk",
                        Address1 = "28 New St",
                        City = "Chelmsford",
                        County = Counties.Essex,
                        PostCode = "CM1 1NT",
                        PhoneNumber = "01245354342",
                        ImageData = michaelImage,
                        ImageType = "image/jpeg",
                        Favourite = true,
                        LastContact = DateTime.Now.AddDays(-3),
                        AppUserId = demoUserId
                    },
                    new Contact()
                    {
                        FirstName = "Nicholas",
                        LastName = "Horn",
                        BirthDate = new DateTime(1997, 02, 01),
                        Email = "nhorn@contactbook.co.uk",
                        Address1 = "89 High St",
                        City = "Sidcup",
                        County = Counties.Greater_London,
                        PostCode = "DA14 6DJ",
                        PhoneNumber = "02032484037",
                        ImageData = nicholasImage,
                        ImageType = "image/jpeg",
                        AppUserId = demoUserId
                    },
                    new Contact()
                    {
                        FirstName = "Sam",
                        LastName = "Kelly",
                        BirthDate = new DateTime(1990, 11, 21),
                        Email = "skelly@contactbook.co.uk",
                        Address1 = "9 Estcourt St",
                        City = "Devizes",
                        County = Counties.Wiltshire,
                        PostCode = "SN10 1LQ",
                        PhoneNumber = "01380720665",
                        ImageData = samImage,
                        ImageType = "image/jpeg",
                        AppUserId = demoUserId
                    },
                    new Contact()
                    {
                        FirstName = "Brook",
                        LastName = "Mitchell",
                        BirthDate = new DateTime(1997, 08, 11),
                        Email = "bmitchell@contactbook.co.uk",
                        Address1 = "178 Eastern Esplanade",
                        City = "Southend on Sea",
                        County = Counties.Essex,
                        PostCode = "SS1 3AA",
                        PhoneNumber = "01702587917",
                        ImageData = brookImage,
                        ImageType = "image/jpeg",
                        Favourite = true,
                        LastContact = DateTime.Now.AddDays(-1),
                        AppUserId = demoUserId
                    },
                    new Contact()
                    {
                        FirstName = "Susan",
                        LastName = "Richards",
                        BirthDate = new DateTime(1997, 12, 11),
                        Email = "srichards@contactbook.co.uk",
                        Address1 = "20 Chapel Brow",
                        City = "Leyland",
                        County = Counties.Lancashire,
                        PostCode = "PR25 3NE",
                        PhoneNumber = "01772433281",
                        ImageData = susanImage,
                        ImageType = "image/jpeg",
                        AppUserId = demoUserId
                    },
                };

                List<string?> dbContacts = context.Contacts.Select(c => c.Email).ToList();

                await context.Contacts.AddRangeAsync(contacts.Where(c => !dbContacts.Contains(c.Email)));
                await context.SaveChangesAsync();

                // Get a list of IDs from the contacts list
                List<int> contactIds = contacts.Select(c => c.Id).ToList();

                int contact1Id = contactIds[0];
                int contact2Id = contactIds[1];
                int contact3Id = contactIds[2];
                int contact4Id = contactIds[3];
                int contact5Id = contactIds[4];
                int contact6Id = contactIds[5];
                int contact7Id = contactIds[6];

                // add contacts to family category
                await addressBookService.AddContactToCategoryAsync(category1Id, contact1Id);
                await addressBookService.AddContactToCategoryAsync(category1Id, contact6Id);
                await addressBookService.AddContactToCategoryAsync(category1Id, contact7Id);

                // add contacts to poker night cateogry
                await addressBookService.AddContactToCategoryAsync(category2Id, contact2Id);
                await addressBookService.AddContactToCategoryAsync(category2Id, contact3Id);
                await addressBookService.AddContactToCategoryAsync(category2Id, contact7Id);

                // add contacts to football team cateogry
                await addressBookService.AddContactToCategoryAsync(category3Id, contact3Id);
                await addressBookService.AddContactToCategoryAsync(category3Id, contact1Id);
                await addressBookService.AddContactToCategoryAsync(category3Id, contact4Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Contacts.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");     
                throw;
            }
        }
    }
}