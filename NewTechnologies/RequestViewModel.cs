using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTechnologies
{
    public class RequestViewModel
    {
        public int PartnerId { get; set; }
        public string PartnerType { get; set; }
        public string PartnerName { get; set; }
        public string Director { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public decimal Cost { get; set; }
    }
}
