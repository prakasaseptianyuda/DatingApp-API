using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Entities
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        [StringLength(30)]
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        [StringLength(30)]
        public string CreatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        [StringLength(30)]
        public string UpdatedBy { get; set; }
    }
}
