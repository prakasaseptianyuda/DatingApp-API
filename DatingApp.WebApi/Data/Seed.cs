using DatingApp.WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.User.AnyAsync())
            {
                return;
            }

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<User>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.Username = user.Username.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("P@ssword"));
                user.PasswordSalt = hmac.Key;

                context.User.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}
