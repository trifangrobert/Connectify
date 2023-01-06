using Microsoft.EntityFrameworkCore;

namespace connectify.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserFriendId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser UserFriend { get; set; }
        public string Status { get; set; }
    }
}