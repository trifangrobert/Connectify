using System.ComponentModel.DataAnnotations;

namespace connectify.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [MinLength(4, ErrorMessage = "Titlul trebuie sa aiba mai mult de 4 caractere")]
        [StringLength(30, ErrorMessage = "Titlul nu poate avea mai mult de 30 de caractere")]
        
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Continutul postarii este obligatoriu")]
        public string Content { get; set; }
        public string? UserId { get; set; }
        public DateTime Date { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

    }
    
}
