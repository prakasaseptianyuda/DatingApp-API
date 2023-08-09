using DatingApp.WebApi.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Entities
{
    public class AppUser: IdentityUser<int>
    {
        //public int UserId { get; set; }
        //[Required]
        //[StringLength(30)]
        //public string Username { get; set; }
        //public byte[] PasswordHash { get; set; }
        //public byte[] PasswordSalt { get; set; }
        public DateTime Birthdate { get; set; }
        public string KnownAs { get; set; }
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interest { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [StringLength(30)]
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        [StringLength(30)]
        public string UpdatedBy { get; set; }

        public ICollection<UserLike> LikedByUsers { get; set; }
        public ICollection<UserLike> LikedUsers { get; set; }

        public ICollection<Message> MeesagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
