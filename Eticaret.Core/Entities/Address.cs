using System.ComponentModel.DataAnnotations;
namespace Eticaret.Core.Entities
{
    public class Address : IEntity
    {
        public int Id { get; set; }
        [Display(Name ="Adres Başlığı"),Required(ErrorMessage = "{0} Alanı Zorunludur!"), StringLength(50)]
        public string Title { get; set; }
        [Display(Name = "Şehir"), Required(ErrorMessage = "{0} Alanı Zorunludur!"), StringLength(50)]
        public string City { get; set; }
        [Display(Name = "İlçe"), Required(ErrorMessage = "{0} Alanı Zorunludur!"), StringLength(50)]
        public string District { get; set; }
        [Display(Name = "Açık Adres"), Required(ErrorMessage = "{0} Alanı Zorunludur!"),DataType(DataType.MultilineText)]
        public string OpenAddress { get; set; }
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }
        [Display(Name = "Fatura Adresi")]
        public bool IsBillingAddress { get; set; }
        [Display(Name = "Teslimat Adresi")]
        public bool IsDeliveryAddress { get; set; }
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [ScaffoldColumn(false)]
        public Guid? AddressGuid { get; set; } = Guid.NewGuid();
        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
