﻿using System.ComponentModel.DataAnnotations;

namespace IT_Institute_Management.Entity
{

    public class Admin
    {


        [Key]
        [Required(ErrorMessage = "NIC is required.")]
        [RegularExpression(@"^(?!.*[^0-9VXZ]).{9}$|^(?!.*[^0-9]).{12}$",
            ErrorMessage = "NIC must be either 9 digits followed by V/Z/X or 12 digits.")]
        public string NIC { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and contain at least 1 uppercase letter, 1 lowercase letter, and 1 special character.")]
        public string Password { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
