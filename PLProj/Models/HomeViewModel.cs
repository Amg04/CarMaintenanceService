using DALProject.Models;
using System.Collections.Generic;

namespace PLProj.Models
{
    public class HomeViewModel
    {
        public List<ServiceViewModel> Services { get; set; }
        public List<TechnicianViewModel> Technicians { get; set; }
    }
}
