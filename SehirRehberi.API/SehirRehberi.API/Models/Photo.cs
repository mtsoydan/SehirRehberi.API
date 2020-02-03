using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SehirRehberi.API.Models
{
    public class Photo
    {
        public int ID { get; set; }
        public int CityID { get; set; }
        public DateTime DateAdded { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public string Url { get; set; }
        public string PublicID { get; set; }
        public City City { get; set; }
    }
}
