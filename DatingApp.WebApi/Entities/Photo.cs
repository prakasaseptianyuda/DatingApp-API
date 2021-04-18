using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.WebApi.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int PhotoId { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}