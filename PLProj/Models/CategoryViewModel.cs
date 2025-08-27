using PLProj.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALProject.Models
{
    public class CategoryViewModel 
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

       #region Mapping
        public static explicit operator CategoryViewModel(Category model)
        {
            return new CategoryViewModel
            {
                Id = model.Id,
                Name = model.Name,
            };
        }

        public static explicit operator Category(CategoryViewModel viewModel)
        {
            return new Category
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
               
            };
        }
        #endregion
    }

}

