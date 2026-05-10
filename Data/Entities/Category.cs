using Wardrobe.Data.Entities;
using System.Collections.Generic;
using Wardrobe.Data.Common;

namespace Wardrobe.Data.Entities
{
	public class Category : BaseEntity
	{
		public string Name { get; set; } = null!;


		public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
	}
}