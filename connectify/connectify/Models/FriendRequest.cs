using Microsoft.EntityFrameworkCore;

namespace connectify.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Friend { get; set; }
        public string Status { get; set; }
    }
}