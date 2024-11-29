﻿using IT_Institute_Management.DTO.RequestDTO;
using IT_Institute_Management.DTO.ResponseDTO;
using IT_Institute_Management.Entity;
using IT_Institute_Management.IRepositories;
using IT_Institute_Management.IServices;
using SendGrid.Helpers.Errors.Model;

namespace IT_Institute_Management.Services
{
    public class StudentMessageService : IStudentMessageService
    {
        private readonly IStudentMessageRepository _repository;
        private readonly IStudentRepository _studentRepository;

        public StudentMessageService(IStudentMessageRepository studentMessageRepository, IStudentRepository studentRepository)
        {
            _repository = studentMessageRepository;
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<StudentMessageResponseDto>> GetAllMessagesAsync()
        {
            var messages = await _repository.GetAllAsync();
            if (messages == null)
            {
                throw new NotFoundException("Student messages not found");
            }
            var response = messages.Select(message => new StudentMessageResponseDto
            {
                Id = message.Id,
                Message = message.Message,
                Date = message.Date,
                StudentNIC = message.StudentNIC,
                Student = new StudentResponseDto
                {
                    NIC = message.Student.NIC,
                    FirstName = message.Student.FirstName,
                    LastName = message.Student.LastName,
                    Email = message.Student.Email,
                    Phone = message.Student.Phone,
                    IsLocked = message.Student.IsLocked,
                    FailedLoginAttempts = message.Student.FailedLoginAttempts,
                    ImagePath = message.Student.ImagePath,
                    Address = new AddressResponseDto
                    {
                        AddressLine1 = message.Student.Address.AddressLine1,
                        AddressLine2 = message.Student.Address.AddressLine2,
                        City = message.Student.Address.City,
                        State = message.Student.Address.State,
                        PostalCode = message.Student.Address.PostalCode,
                        Country = message.Student.Address.Country
                    }
                }
            });

            return response;
        }

        public async Task<IEnumerable<StudentMessageResponseDto>> GetMessagesByStudentNICAsync(string studentNIC)
        {
            var messages = await _repository.GetByStudentNICAsync(studentNIC);

            if (messages == null)
            {
                throw new NotFoundException("Student messages not found");
            }
            return messages.Select(message => new StudentMessageResponseDto
            {
                Id = message.Id,
                Message = message.Message,
                Date = message.Date,
                StudentNIC = message.StudentNIC,
                Student = new StudentResponseDto
                {
                    NIC = message.Student.NIC,
                    FirstName = message.Student.FirstName,
                    LastName = message.Student.LastName,
                    Email = message.Student.Email,
                    Phone = message.Student.Phone,
                    IsLocked = message.Student.IsLocked,
                    FailedLoginAttempts = message.Student.FailedLoginAttempts,
                    ImagePath = message.Student.ImagePath,
                    Address = new AddressResponseDto
                    {
                        AddressLine1 = message.Student.Address.AddressLine1,
                        AddressLine2 = message.Student.Address.AddressLine2,
                        City = message.Student.Address.City,
                        State = message.Student.Address.State,
                        PostalCode = message.Student.Address.PostalCode,
                        Country = message.Student.Address.Country
                    }
                }
            });
        }

        public async Task<StudentMessageResponseDto> AddMessageAsync(StudentMessageRequestDto requestDto)
        {
            try
            {
                // Map request DTO to entity
                var studentMessage = new StudentMessage
                {
                    Id = Guid.NewGuid(),
                    Message = requestDto.Message,
                    Date = requestDto.Date,
                    StudentNIC = requestDto.StudentNIC
                };

                // Add the student message asynchronously
                var message = await _repository.AddAsync(studentMessage);

                // Save changes asynchronously
                await _repository.SaveAsync();

                // Return the response DTO
                return new StudentMessageResponseDto
                {
                    Id = message.Id,
                    Message = message.Message,
                    Date = message.Date,
                    StudentNIC = message.StudentNIC
                };
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                // You should use a logging framework such as ILogger, here we are simply printing to the console for simplicity
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Throw a more descriptive exception to inform the caller
                throw new Exception("Error saving the message. Please try again later.", ex);
            }
        }
        public async Task<bool> DeleteMessageAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return await _repository.SaveAsync();
        }
    }
}
