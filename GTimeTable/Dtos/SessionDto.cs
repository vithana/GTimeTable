using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTimeTable.Dtos
{
    class SessionDto
    {
        public int id { get; set; }
        public List<string> lecturers { get; set; }
        public string subject_name { get; set; }
        public string subject_code { get; set; }
        public string tag { get; set; }
        public string groupId { get; set; }
        public int count { get; set; }
        public string duration { get; set; }
    }
}
