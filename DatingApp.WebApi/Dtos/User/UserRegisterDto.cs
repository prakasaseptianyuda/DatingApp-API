using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Dtos.User
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Username harus diisi")]
        [MinLength(3)]
        public string Username { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        [Required(ErrorMessage = "Password harus diisi")]
        [StringLength(8,MinimumLength = 4)]
        public string Password { get; set; }
    }
}
