﻿using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Contact : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Adı"),Required(ErrorMessage ="{0} alanı boş geçilemez")]
        public string  Name { get; set; }
        [Display(Name = "Soyadı"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string  SurName { get; set; }
        public string?  Email { get; set; }
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
        [Display(Name = "Mesaj"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Message { get; set; }
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
