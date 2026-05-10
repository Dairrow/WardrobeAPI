using Wardrobe.Data.Entities;
using System.Collections.Generic;
using Wardrobe.Data.Common;

namespace Wardrobe.Data.Entities
{
	public class Outfit : BaseEntity
	{
		public string Name { get; set; } = null!;


		public int UserId { get; set; }


		public User User { get; set; } = null!;


		public ICollection<OutfitItem> OutfitItems { get; set; } = new List<OutfitItem>();
	}
}