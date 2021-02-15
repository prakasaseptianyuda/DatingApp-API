using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Dtos.User
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Username harus diisi")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password harus diisi")]
        public string Password { get; set; }
    }
}
