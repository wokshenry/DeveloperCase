using System;
using System.Collections.Generic;

namespace DeveloperCase.Models
{
    public partial class TblErrorLog
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Module { get; set; }
        public int ErrorNumber { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
    }
}
