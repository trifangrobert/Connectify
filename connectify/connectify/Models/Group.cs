using System.ComponentModel.DataAnnotations;
namespace connectify.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [MinLength(4, ErrorMessage = "Numele trebuie sa aiba mai mult de 4 caractere")]
        [StringLength(30, ErrorMessage = "Numele nu poate avea mai mult de 30 de caractere")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descrierea grupului este obligatorie")]
        public string? Description { get; set; }

        public string? OwnerId { get; set; }
        public virtual ICollection<ApplicationUser>? Users { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
    }
}
