using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Wardrobe.Data.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}