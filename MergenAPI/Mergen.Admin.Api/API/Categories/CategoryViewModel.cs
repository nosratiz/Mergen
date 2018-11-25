﻿using System.Collections.Generic;
using System.Linq;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Entities;

namespace Mergen.Admin.Api.API.Categories
{
    public class CategoryViewModel : EntityViewModel
    {
        public string Title { get; set; }
        public string StatusId { get; set; }
        public string IconFileId { get; set; }
        public string CoverImageFileId { get; set; }
        public string Description { get; set; }

        public static CategoryViewModel Map(Category category)
        {
            return new CategoryViewModel
            {
                Id = category.Id.ToString(),
                Title = category.Title,
                Description = category.Description,
                StatusId = category.StatusId.ToString(),
                CoverImageFileId = category.CoverImageFileId,
                IconFileId = category.IconFileId
            };
        }

        public static IEnumerable<CategoryViewModel> MapAll(IEnumerable<Category> categories)
        {
            return categories.Select(Map);
        }
    }
}