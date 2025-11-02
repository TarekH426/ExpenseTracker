using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.DAL.Models.AuthModels
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresOn { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? RevokedOn { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;

        public bool IsActive => RevokedOn == null && !IsExpired;

        // Foreign key
        public string UserId { get; set; } = string.Empty;

        public virtual AppUser User { get; set; } = null!;
    }
}
