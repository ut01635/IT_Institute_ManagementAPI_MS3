﻿namespace IT_Institute_Management.EmailSerivice
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}