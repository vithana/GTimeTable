using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTimeTable.Dtos
{
    class NotAvailableTimeOfSessionDto
    {
        public int id { get; set; }
        public int session { get; set; }
        public string timeSlot { get; set; }
    }
}
