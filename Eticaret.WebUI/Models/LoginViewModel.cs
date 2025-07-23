using System.ComponentModel.DataAnnotations;

namespace Eticaret.WebUI.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Kullanıcı Adı"), Required(ErrorMessage = "{0} alanı zorunludur!")]
        public string Email { get; set; }
        [Display(Name = "Kullanıcı Adı"), Required(ErrorMessage = "{0} alanı zorunludur!"),DataType(DataType.Password)]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
}
