namespace DatingApp.WebApi.Dtos.User
{
    public class PhotoDto
    {
        public int UserId { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}