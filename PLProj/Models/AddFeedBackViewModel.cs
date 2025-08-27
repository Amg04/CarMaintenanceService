using DALProject.Models;
using System.ComponentModel.DataAnnotations;

namespace PLProj.Models
{
    public class AddFeedBackViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FeedBack { get; set; }

        #region  Mapping
        public static explicit operator AddFeedBackViewModel(Ticket model)
        {
            return new AddFeedBackViewModel()
            {
                Id = model.Id,
                FeedBack = model.Feedback,
            };
        }
        #endregion
    }
}
