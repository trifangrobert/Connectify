using connectify.Data;
using connectify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// PASUL 4 - useri si roluri

namespace ConnectifyApp.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService
                <DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificam daca in baza de date exista cel putin un rol
                // insemnand ca a fost rulat codul 
                // De aceea facem return pentru a nu insera rolurile inca o data
                // Acesta metoda trebuie sa se execute o singura data 
                if (context.Roles.Any())
                {
                    return;   // DB has been seeded
                }

                // CREAREA ROLURILOR IN BD
                // daca nu contine roluri, acestea se vor crea
                context.Roles.AddRange(
                    
                    new IdentityRole { Id = "1dbe5869-efb2-40ff-ac1f-3694daaedef5", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole { Id = "1dbe5869-efb2-40ff-ac1f-3694daaedef6", Name = "Moderator", NormalizedName = "Moderator".ToUpper() },
                    new IdentityRole { Id = "1dbe5869-efb2-40ff-ac1f-3694daaedef7", Name = "User", NormalizedName = "User".ToUpper() }
                );

                // o noua instanta pe care o vom utiliza pentru crearea parolelor utilizatorilor
                // parolele sunt de tip hash
                var hasher = new PasswordHasher<ApplicationUser>();

                // CREAREA USERILOR IN BD
                // Se creeaza cate un user pentru fiecare rol
                context.Users.AddRange(
                    new ApplicationUser
                    {
                        Id = "894ac6c3-1d10-41c9-b16b-7bda4318b015", // primary key
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")
                    },
                    new ApplicationUser
                    {
                        Id = "894ac6c3-1d10-41c9-b16b-7bda4318b016", // primary key
                        UserName = "editor@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "EDITOR@TEST.COM",
                        Email = "editor@test.com",
                        NormalizedUserName = "EDITOR@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Editor1!")
                    },
                    new ApplicationUser
                    {
                        Id = "894ac6c3-1d10-41c9-b16b-7bda4318b017", // primary key
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    }
                );

                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "1dbe5869-efb2-40ff-ac1f-3694daaedef5",
                        UserId = "894ac6c3-1d10-41c9-b16b-7bda4318b015"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "1dbe5869-efb2-40ff-ac1f-3694daaedef6",
                        UserId = "894ac6c3-1d10-41c9-b16b-7bda4318b016"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "1dbe5869-efb2-40ff-ac1f-3694daaedef7",
                        UserId = "894ac6c3-1d10-41c9-b16b-7bda4318b017"
                    }
                );

                context.SaveChanges();

            }
        }
    }
}
