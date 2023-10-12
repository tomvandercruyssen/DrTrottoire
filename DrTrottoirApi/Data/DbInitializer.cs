using System.Drawing.Text;
using DrTrottoirApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task = DrTrottoirApi.Entities.Task;

namespace DrTrottoirApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            using var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            using var context =
                new DrTrottoirDbContext(serviceProvider.GetRequiredService<DbContextOptions<DrTrottoirDbContext>>());

            var random = new Random();
            var companies = new Company[12];
            var rounds = new Round[12];
            var garbageTypes = new GarbageType[6];
            var garbageCollections = new GarbageCollection[12];

            var date1 = new DateTime(2023, 5, 22, 9, 30, 0);
            var date2 = new DateTime(2023, 5, 23, 11, 30, 0);
            var date3 = new DateTime(2023, 5, 24, 13, 30, 0);

            if (!context.WorkAreas.Any())
            {
                var workAreas = new[]
                {
                    new WorkArea() { Id = Guid.NewGuid(), City = "Gent" },
                    new WorkArea() { Id = Guid.NewGuid(), City = "Antwerpen" },
                    new WorkArea() { Id = Guid.NewGuid(), City = "Brussel" }
                };
                context.WorkAreas.AddRange(workAreas);
                context.SaveChanges();
            }

            if (!context.Roles.Any())
            {
                _ = roleManager.CreateAsync(new Role { Name = Roles.Student.ToString(), Description = "Student" })
                    .Result;
                _ = roleManager.CreateAsync(new Role
                    { Name = Roles.SuperStudent.ToString(), Description = "SuperStudent" }).Result;
                _ = roleManager.CreateAsync(new Role { Name = Roles.Syndic.ToString(), Description = "Syndic" }).Result;
                _ = roleManager.CreateAsync(new Role { Name = Roles.Admin.ToString(), Description = "Admin" }).Result;
            }

            if (!context.Users.Any())
            {
                var workarea = context.WorkAreas.First(x => x.City == "Gent");
                var admin = new User() { UserName = "Laurens", FirstName = "Laurens", TelephoneNumber = "0482905432", LastName = "Admin", Email = "laurens@odisee.be", WorkAreaId = workarea.Id };
                _ = userManager.CreateAsync(admin, "_Azerty123").Result;
                _ = userManager.AddToRoleAsync(admin, Roles.Admin.ToString()).Result;
                var superStudent = new User() { UserName = "Jonas", FirstName = "Jonas", TelephoneNumber = "0486905432", LastName = "SuperStudent", Email = "jonas@odisee.be", WorkAreaId = workarea.Id };
                _ = userManager.CreateAsync(superStudent, "_Azerty123").Result;
                _ = userManager.AddToRoleAsync(superStudent, Roles.SuperStudent.ToString()).Result;
                var student = new User() { UserName = "Dirk", FirstName = "Dirk", TelephoneNumber = "0486905433", LastName = "Student", Email = "dirk@odisee.be", WorkAreaId = workarea.Id };
                _ = userManager.CreateAsync(student, "_Azerty123").Result;
                _ = userManager.AddToRoleAsync(student, Roles.Student.ToString()).Result;
                student = new User() { UserName = "Casper", FirstName = "Casper", TelephoneNumber = "0486945391", LastName = "Student", Email = "casper@odisee.be", WorkAreaId = workarea.Id };
                _ = userManager.CreateAsync(student, "_Azerty123").Result;
                _ = userManager.AddToRoleAsync(student, Roles.Student.ToString()).Result;
                student = new User() { UserName = "Waud", FirstName = "Waud", TelephoneNumber = "0586942391", LastName = "Student", Email = "waud@odisee.be", WorkAreaId = workarea.Id };
                _ = userManager.CreateAsync(student, "_Azerty123").Result;
                _ = userManager.AddToRoleAsync(student, Roles.Student.ToString()).Result;
            }

            if (!context.Companies.Any())
            {
                var syndics = new Syndic[]
                {
                    new() { FirstName = "Tom", LastName = "Syndic", TelephoneNumber = "0486976435" },
                    new() { FirstName = "Dries", LastName = "Syndic", TelephoneNumber = "0486976439" },
                    new() { FirstName = "Clark", LastName = "Syndic", TelephoneNumber = "0486976469" },
                    new() { FirstName = "Ann", LastName = "Syndic", TelephoneNumber = "0426976469" },
                    new() { FirstName = "Elise", LastName = "Syndic", TelephoneNumber = "0486376469" },
                    new() { FirstName = "Jef", LastName = "Syndic", TelephoneNumber = "0486376469" },
                    new() { FirstName = "Olivia", LastName = "Syndic", TelephoneNumber = "0436976469" },
                    new() { FirstName = "Peter", LastName = "Syndic", TelephoneNumber = "0486974469" },
                    new() { FirstName = "Femke", LastName = "Syndic", TelephoneNumber = "0486976969" },
                    new() { FirstName = "Elias", LastName = "Syndic", TelephoneNumber = "0486976419" },
                    new() { FirstName = "Thomas", LastName = "Syndic", TelephoneNumber = "0489876419" },
                    new() { FirstName = "Fien", LastName = "Syndic", TelephoneNumber = "0189876419" },
                };

                context.Syndics.AddRange(syndics);
                context.SaveChanges();

                companies = new Company[]
                {
                    // Station
                    new() { Name = "Wellington", IdKbo = "0884.824.496", Address = "Fortlaan 27-42", Syndic = syndics[0] },
                    new() { Name = "Alberts House", IdKbo = "0824.824.496", Address = "Floraliënlaan 11", Syndic = syndics[1] },
                    new() { Name = "Memlinc", IdKbo = "0884.821.496", Address = "Oostendestraat 2-34", Syndic = syndics[2] },
                    new() { Name = "Poort van Gent", IdKbo = "0884.834.496", Address = "St-Denijslaan 3-5", Syndic = syndics[3] },
                    new() { Name = "Atlantis", IdKbo = "0884.124.496", Address = "Kortrijsesteenweg 544-552", Syndic = syndics[4] },
                    new() { Name = "Galinago", IdKbo = "0884.824.446", Address = "Poelsnepstraat 3-15", Syndic = syndics[5] },
                    new() { Name = "De Sterre", IdKbo = "0884.826.496", Address = "Kortrijksesteenweg 768-786", Syndic = syndics[6] },
                    new() { Name = "Greenpark", IdKbo = "0880.824.496", Address = "Pacificatielaan 41-87", Syndic = syndics[7] },
                    // Haven
                    new() { Name = "Kobenhavn", IdKbo = "0834.814.496", Address = "Koperhagenstraat 2-12", Syndic = syndics[8] },
                    new() { Name = "Blaisantpark", IdKbo = "0884.820.406", Address = "Oosteeklostraat 15", Syndic = syndics[9] },
                    // Zuid
                    new() { Name = "Frontenac", IdKbo = "0884.833.496", Address = "Hubert Frère-Orbanlaan 130-143", Syndic = syndics[10] },
                    new() { Name = "Park View", IdKbo = "0774.824.486", Address = "Hubert Frère-Orbanlaan 150", Syndic = syndics[11] },
                };

                context.Companies.AddRange(companies);
                context.SaveChanges();
            }

            if (!context.Rounds.Any())
            {
                var dirk = context.Users.First(x => x.FirstName == "Dirk");
                var waud = context.Users.First(x => x.FirstName == "Waud");
                var casper = context.Users.First(x => x.FirstName == "Casper");

                rounds = new Round[]
                {
                    new() { Name = "Station", StartTime = date1.AddMinutes(-30).ToUniversalTime(), UserId = dirk.Id },
                    new() { Name = "Haven", StartTime = date2.AddMinutes(-30).ToUniversalTime(), UserId = waud.Id },
                    new() { Name = "Zuid", StartTime = date3.AddMinutes(-30).ToUniversalTime(), UserId = casper.Id }
                };
                
                context.Rounds.AddRange(rounds);
                context.SaveChanges();
            }

            if (!context.Tasks.Any())
            {
                var company = context.Companies.First(x => x.Name == "Wellington");

                var tasks = new List<Task>()
                {
                    // Station
                    new() { Company = companies[0], Round = rounds[0], OrderNumber = 1 },
                    new() { Company = companies[1], Round = rounds[0], OrderNumber = 2 },
                    new() { Company = companies[2], Round = rounds[0], OrderNumber = 3 },
                    new() { Company = companies[3], Round = rounds[0], OrderNumber = 4 },
                    new() { Company = companies[4], Round = rounds[0], OrderNumber = 5 },
                    new() { Company = companies[5], Round = rounds[0], OrderNumber = 6 },
                    new() { Company = companies[6], Round = rounds[0], OrderNumber = 7 },
                    new() { Company = companies[7], Round = rounds[0], OrderNumber = 8 },
                    // Haven
                    new() { Company = companies[8], Round = rounds[1], OrderNumber = 1 },
                    new() { Company = companies[9], Round = rounds[1], OrderNumber = 2 },
                    // Zuid
                    new() { Company = companies[10], Round = rounds[2], OrderNumber = 1 },
                    new() { Company = companies[11], Round = rounds[2], OrderNumber = 2 },
                };
                context.Tasks.AddRange(tasks);
                context.SaveChanges();
            }

            if (!context.GarbageTypes.Any())
            {
                garbageTypes = new GarbageType[]
                {
                    new() { Name = "REST" },
                    new() { Name = "GLAS" },
                    new() { Name = "PAPIER" },
                    new() { Name = "PMD" },
                    new() { Name = "GFT" },
                };

                context.GarbageTypes.AddRange(garbageTypes);
                context.SaveChanges();
            }

            if (!context.GarbageCollections.Any())
            {
                garbageCollections = new GarbageCollection[]
                {
                    new() { CollectionTime = date1.ToUniversalTime(),  HasToBeBroughtOutside = true },
                    new() { CollectionTime = date1.AddMinutes(30).ToUniversalTime(), HasToBeBroughtOutside = true },
                    new() { CollectionTime = date1.AddMinutes(60).ToUniversalTime(), HasToBeBroughtOutside = true },
                    new() { CollectionTime = date1.AddMinutes(60).ToUniversalTime(), HasToBeBroughtOutside = false },
                    new() { CollectionTime = date1.AddMinutes(90).ToUniversalTime(), HasToBeBroughtOutside = true },
                    new() { CollectionTime = date1.AddMinutes(120).ToUniversalTime(), HasToBeBroughtOutside = true },
                    new() { CollectionTime = date1.AddMinutes(120).ToUniversalTime(), HasToBeBroughtOutside = false },
                    new() { CollectionTime = date1.AddMinutes(150).ToUniversalTime(), HasToBeBroughtOutside = true },
                    new() { CollectionTime = date1.AddMinutes(180).ToUniversalTime(), HasToBeBroughtOutside = true },
                    new() { CollectionTime = date1.AddMinutes(210).ToUniversalTime(), HasToBeBroughtOutside = false },

                    new() { CollectionTime = date2.ToUniversalTime(), HasToBeBroughtOutside = false },
                    new() { CollectionTime = date2.AddMinutes(30).ToUniversalTime(), HasToBeBroughtOutside = true },

                    new() { CollectionTime = date3.ToUniversalTime(), HasToBeBroughtOutside = true },
                    new() { CollectionTime = date3.AddMinutes(30).ToUniversalTime(), HasToBeBroughtOutside = true },
                    new() { CollectionTime = date3.AddMinutes(30).ToUniversalTime(), HasToBeBroughtOutside = false },
                };
                context.GarbageCollections.AddRange(garbageCollections);
                context.SaveChanges();
            }

            if (!context.GarbageCollectionGarbageTypes.Any())
            {
                var garbageCollectionGarbageTypes = new GarbageCollectionGarbageType[]
                {
                    new() { GarbageCollection = garbageCollections[0], GarbageType = garbageTypes[0] },
                    new() { GarbageCollection = garbageCollections[1], GarbageType = garbageTypes[1] },
                    new() { GarbageCollection = garbageCollections[2], GarbageType = garbageTypes[2] },
                    new() { GarbageCollection = garbageCollections[3], GarbageType = garbageTypes[3] },
                    new() { GarbageCollection = garbageCollections[4], GarbageType = garbageTypes[4] },
                    new() { GarbageCollection = garbageCollections[5], GarbageType = garbageTypes[0] },
                    new() { GarbageCollection = garbageCollections[6], GarbageType = garbageTypes[1] },
                    new() { GarbageCollection = garbageCollections[7], GarbageType = garbageTypes[2] },
                    new() { GarbageCollection = garbageCollections[8], GarbageType = garbageTypes[3] },
                    new() { GarbageCollection = garbageCollections[9], GarbageType = garbageTypes[4] },
                    new() { GarbageCollection = garbageCollections[10], GarbageType = garbageTypes[0] },
                    new() { GarbageCollection = garbageCollections[11], GarbageType = garbageTypes[1] },
                    new() { GarbageCollection = garbageCollections[12], GarbageType = garbageTypes[2] },
                    new() { GarbageCollection = garbageCollections[13], GarbageType = garbageTypes[3] },
                    new() { GarbageCollection = garbageCollections[14], GarbageType = garbageTypes[4] }
                };

                context.GarbageCollectionGarbageTypes.AddRange(garbageCollectionGarbageTypes);
                context.SaveChanges();
            }

            if (!context.CompanyGarbageCollections.Any())
            {
                var companyGarbageCollections = new CompanyGarbageCollection[]
                {
                    new() { Company = companies[0], GarbageCollection = garbageCollections[0] },
                    new() { Company = companies[1], GarbageCollection = garbageCollections[1] },
                    new() { Company = companies[2], GarbageCollection = garbageCollections[2] },
                    new() { Company = companies[2], GarbageCollection = garbageCollections[3] },
                    new() { Company = companies[3], GarbageCollection = garbageCollections[4] },
                    new() { Company = companies[4], GarbageCollection = garbageCollections[5] },
                    new() { Company = companies[4], GarbageCollection = garbageCollections[6] },
                    new() { Company = companies[5], GarbageCollection = garbageCollections[7] },
                    new() { Company = companies[6], GarbageCollection = garbageCollections[8] },
                    new() { Company = companies[7], GarbageCollection = garbageCollections[9] },
                    new() { Company = companies[8], GarbageCollection = garbageCollections[10] },
                    new() { Company = companies[9], GarbageCollection = garbageCollections[11] },
                    new() { Company = companies[10], GarbageCollection = garbageCollections[12] },
                    new() { Company = companies[11], GarbageCollection = garbageCollections[13] },
                    new() { Company = companies[11], GarbageCollection = garbageCollections[14] }
                };
                context.CompanyGarbageCollections.AddRange(companyGarbageCollections);
                context.SaveChanges();
            }

            context.SaveChangesAsync();
        }
    }

}

