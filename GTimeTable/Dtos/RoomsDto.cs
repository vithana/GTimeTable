using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTimeTable.Dtos
{
    class RoomsDto
    {
        public int id { get; set; }
        public int capacity { get; set; }
        public string roomId { get; set; }
        public string roomType { get; set; }
        public string building { get; set; }
    }
}
