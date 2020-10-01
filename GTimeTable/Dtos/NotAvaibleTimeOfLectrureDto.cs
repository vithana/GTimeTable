using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTimeTable.Dtos
{
    class NotAvaibleTimeOfLectrureDto
    {
        public int id { get; set; }
        public string lecturer { get; set; }
        public string timeSlot { get; set; }
    }
}
