using System.ComponentModel.DataAnnotations;

namespace connectify.Models
{
    public class ApplicationUserGroup
    {
        [Key]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Key]
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}