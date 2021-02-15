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
        [Required(ErrorMessage = "Password harus diisi")]
        [MinLength(3)]
        public string Password { get; set; }
    }
}
