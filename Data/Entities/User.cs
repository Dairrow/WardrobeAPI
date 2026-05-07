using Wardrobe.Data.Entities;
using System.Collections.Generic;
using Wardrobe.Data.Common;

namespace Wardrobe.Data.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;


        public int RoleId { get; set; }


        public Role Role { get; set; } = null!;


        public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();

        public ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
    }
}