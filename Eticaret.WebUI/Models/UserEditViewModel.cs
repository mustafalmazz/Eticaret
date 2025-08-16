using System.ComponentModel.DataAnnotations;

namespace Eticaret.WebUI.Models
{
    public class UserEditViewModel 
    {
        public int Id { get; set; }
        [Display(Name = "Adı"), Required(ErrorMessage = "{0} alanı zorunludur!")]
        public string Name { get; set; }
        [Display(Name = "Soyadı"), Required(ErrorMessage = "{0} alanı zorunludur!")]
        public string SurName { get; set; }
        [Required(ErrorMessage = "Email alanı zorunludur!")]
        public string Email { get; set; }
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
        [Display(Name = "Şifre"), Required(ErrorMessage = "{0} alanı zorunludur!")]
        public string Password { get; set; }
    }
}
