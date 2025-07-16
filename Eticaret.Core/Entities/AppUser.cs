using System;
using System.Collections.Generic;
using System.Linq;
namespace Eticaret.Core.Entities
{
    public class AppUser : IEntity
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public int SurName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; } 
        public bool IsAdmin{ get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UserGuid { get; set; } = Guid.NewGuid(); 

    }
}
