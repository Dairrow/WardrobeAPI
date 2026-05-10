using Wardrobe.Data.Entities;
using System.Collections.Generic;
using Wardrobe.Data.Common;

namespace Wardrobe.Data.Entities
{
	public class ClothingItem : BaseEntity
	{
		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public string? Color { get; set; }

		public string? Size { get; set; }

		public string? Season { get; set; }

		public decimal Price { get; set; }

		public string? ImagePath { get; set; }


		public int CategoryId { get; set; }

		public int BrandId { get; set; }

		public int UserId { get; set; }


		public Category Category { get; set; } = null!;

		public Brand Brand { get; set; } = null!;

		public User User { get; set; } = null!;


		public ICollection<OutfitItem> OutfitItems { get; set; } = new List<OutfitItem>();
	}
}