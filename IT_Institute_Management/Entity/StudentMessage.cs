﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IT_Institute_Management.Entity
{
    public class StudentMessage
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

     
        [ForeignKey("Student")]
        public string? StudentNIC { get; set; }

       
        public Student? Student { get; set; }
    }
}
