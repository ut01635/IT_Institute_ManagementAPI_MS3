﻿using IT_Institute_Management.Database;
using IT_Institute_Management.IRepositories;

namespace IT_Institute_Management.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly InstituteDbContext _context;

        public NotificationRepository(InstituteDbContext context)
        {
            _context = context;
        }
    }
}
