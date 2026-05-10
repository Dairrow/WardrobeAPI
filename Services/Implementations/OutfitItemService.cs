using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class OutfitItemService : IOutfitItemService
{
	private readonly IOutfitItemRepository _outfitItemRepository;
	private readonly IOutfitRepository _outfitRepository;
	private readonly IClothingItemRepository _clothingItemRepository;

	public OutfitItemService(
		IOutfitItemRepository outfitItemRepository,
		IOutfitRepository outfitRepository,
		IClothingItemRepository clothingItemRepository)
	{
		_outfitItemRepository = outfitItemRepository;
		_outfitRepository = outfitRepository;
		_clothingItemRepository = clothingItemRepository;
	}

	public async Task<IEnumerable<OutfitItem>> GetByOutfitIdAsync(int outfitId, int userId)
	{
		var outfit = await _outfitRepository.GetByIdWithDetailsAsync(outfitId, userId);
		if (outfit == null)
		{
			throw new UnauthorizedAccessException("Outfit not found or access denied");
		}

		return await _outfitItemRepository.GetByOutfitIdAsync(outfitId, userId);
	}

	public async Task<OutfitItem> AddAsync(int outfitId, int clothingItemId, int userId)
	{
		var outfit = await _outfitRepository.GetByIdWithDetailsAsync(outfitId, userId);
		if (outfit == null)
		{
			throw new ArgumentException("Outfit not found or access denied");
		}

		var clothingItem = await _clothingItemRepository.GetByIdWithDetailsAsync(clothingItemId, userId);
		if (clothingItem == null)
		{
			throw new ArgumentException("Clothing item not found or access denied");
		}

		if (await _outfitItemRepository.ExistsAsync(outfitId, clothingItemId))
		{
			throw new InvalidOperationException("This clothing item is already in the outfit");
		}

		var outfitItem = new OutfitItem
		{
			OutfitId = outfitId,
			ClothingItemId = clothingItemId
		};

		await _outfitItemRepository.AddAsync(outfitItem);
		return outfitItem;
	}

	public async Task DeleteAsync(int outfitId, int clothingItemId, int userId)
	{
		var outfit = await _outfitRepository.GetByIdWithDetailsAsync(outfitId, userId);
		if (outfit == null)
		{
			throw new ArgumentException("Outfit not found or access denied");
		}

		await _outfitItemRepository.DeleteAsync(outfitId, clothingItemId, userId);
	}
}