using Wardrobe.Data.Entities;
using System.Collections.Generic;
using Wardrobe.Data.Common;

namespace Wardrobe.Data.Entities
{
	public class Role : BaseEntity
	{
		public string Name { get; set; } = null!;


		public ICollection<User> Users { get; set; } = new List<User>();
	}
}