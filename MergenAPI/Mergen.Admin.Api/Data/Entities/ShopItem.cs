﻿namespace Mergen.Admin.Api.Data.Entities
{
	public class ShopItem
	{
		public string Title { get; set; }
		public int TypeId { get; set; }
		public int? AvatarCategoryId { get; set; }
		public int PriceTypeId { get; set; }
		public decimal Price { get; set; }
		public string ImageFileId { get; set; }
		public int? UnlockLevel { get; set; }
		public int? UnlockSky { get; set; }
		public int StatusId { get; set; }
	}
}