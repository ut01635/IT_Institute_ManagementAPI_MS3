﻿using IT_Institute_Management.Database;
using IT_Institute_Management.Entity;
using IT_Institute_Management.IRepositories;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace IT_Institute_Management.Repositories
{
    public class StudentMessageRepository : IStudentMessageRepository
    {
        private readonly InstituteDbContext _context;

        public StudentMessageRepository(InstituteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentMessage>> GetAllAsync()
        {
            var data = await _context.StudentMessages
                                 .Include(sm => sm.Student)
                                 .ThenInclude(s => s.Address)
                                 .ToListAsync();

            if (data != null)
            {
                return data;
            }
            else
            {
                throw new NotFoundException("Student messages not found");
            }
        }

        public async Task<IEnumerable<StudentMessage>> GetByStudentNICAsync(string studentNIC)
        {
            var data =  await _context.StudentMessages
                                 .Where(sm => sm.StudentNIC == studentNIC)
                                 .Include(sm => sm.Student)
                                 .ThenInclude(s => s.Address)
                                 .ToListAsync();

            if (data != null)
            {
                return data;
            }
            else
            {
                throw new NotFoundException("Student messages not found");
            }
        }

        public async Task<StudentMessage> AddAsync(StudentMessage studentMessage)
        {
            var data = await _context.StudentMessages.AddAsync(studentMessage);
            await _context.SaveChangesAsync();

            return data.Entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var message = await _context.StudentMessages.FindAsync(id);
            if (message != null)
            {
                _context.StudentMessages.Remove(message);
            }
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}