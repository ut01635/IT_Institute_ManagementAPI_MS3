﻿using IT_Institute_Management.Entity;

namespace IT_Institute_Management.IRepositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<Payment> GetPaymentByIdAsync(Guid id);
    }
}
