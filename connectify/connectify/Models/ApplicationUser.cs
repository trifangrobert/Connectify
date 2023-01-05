using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace connectify.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<Post>? Posts { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public bool? Visibility { get; set; }

        public ICollection<Group>? Groups { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

        //[NotMapped]
        public virtual ICollection<ApplicationUser>? Friends { get; set; }

    }
}