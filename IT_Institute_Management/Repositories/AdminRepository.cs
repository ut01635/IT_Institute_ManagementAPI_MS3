﻿using IT_Institute_Management.Database;
using IT_Institute_Management.Entity;
using IT_Institute_Management.IRepositories;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;

namespace IT_Institute_Management.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly InstituteDbContext _instituteDbContext;
        public AdminRepository(InstituteDbContext context) { _instituteDbContext = context; }
        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _instituteDbContext.Admins.ToListAsync();
        }
        public async Task<Admin> GetByIdAsync(string nic)
        {
            return await _instituteDbContext.Admins.FindAsync(nic);
        }
        public async Task AddAsync(Admin admin)
        {
            
            var user = new User()
            {
                NIC = admin.NIC,
                Password = admin.Password,
                Role = Role.Admin,
            };
            await _instituteDbContext.Users.AddAsync(user);
            await _instituteDbContext.SaveChangesAsync(); // Ensure the User entity is saved first

            // Now set the UserId on the Admin object
            admin.UserId = user.Id;

            await _instituteDbContext.Admins.AddAsync(admin);
            await _instituteDbContext.SaveChangesAsync();
        }


        public async Task UpdateAsync(Admin admin)
        {
            _instituteDbContext.Admins.Update(admin);
            var user = new User()
            {
                NIC = admin.NIC,
                Password = admin.Password,
                Role = Role.Admin,
            };
            _instituteDbContext.Users.Update(user);
            await _instituteDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(string nic)
        {
            try
            {
                // Find the admin and user by NIC
                var admin = await _instituteDbContext.Admins
                    .FirstOrDefaultAsync(a => a.NIC == nic);

                var user = await _instituteDbContext.Users
                    .FirstOrDefaultAsync(u => u.NIC == nic);

                if (admin == null || user == null)
                {
                    throw new KeyNotFoundException($"Admin or User with NIC {nic} not found.");
                }

                // Remove Admin and User from the DbContext
                _instituteDbContext.Admins.Remove(admin);
                _instituteDbContext.Users.Remove(user);

                // Save the changes to the database
                await _instituteDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception and provide detailed error message
                throw new ApplicationException($"An error occurred while deleting the admin with NIC {nic}: {ex.Message}", ex);
            }
        }




    }
}
