namespace Wardrobe.Data.Entities
{
	public class OutfitItem
	{
		public int OutfitId { get; set; }

		public int ClothingItemId { get; set; }


		public Outfit Outfit { get; set; } = null!;

		public ClothingItem ClothingItem { get; set; } = null!;
	}
}