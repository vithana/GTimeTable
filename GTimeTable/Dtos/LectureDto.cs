using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTimeTable.Dtos
{
    public class LectureDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string emp_id { get; set; }
        public string faculty { get; set; }
        public string dept { get; set; }
        public string center { get; set; }
        public string building { get; set; }
        public int lvl { get; set; }
        public string rank { get; set; }

       
    }
}
