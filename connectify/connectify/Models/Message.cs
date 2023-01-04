using System.ComponentModel.DataAnnotations;
namespace connectify.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Content { get; set; }
        public DateTime Date { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public Group? Group { get; set; }
        public int GroupId { get; set; }
    }
}
