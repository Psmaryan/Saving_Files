using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SavingFile.Models
{
    public class User : IdentityUser
    {
        public string Login { get; set; }
        public List<UserSaveFile> UserSaveFiles { get; set; }
        public User()
        {
            UserSaveFiles = new List<UserSaveFile>();
        }
        
    }

    //[Table("SaveFiles23")]
    public class SaveFile
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Choose file")]
        public byte[] File { get; set; }
        public List<UserSaveFile> UserSaveFiles { get; set; }

        public SaveFile()
        {
            UserSaveFiles = new List<UserSaveFile>();
        }
    }

    public class UserSaveFile
    {
        public UserSaveFile()
        {
            DateAdded = DateTime.Now;
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public User Users { get; set; }

        public int SaveFileId { get; set; }
        public SaveFile SaveFiles { get; set; }
        public DateTime DateAdded { get; set; }
    }
    
}
