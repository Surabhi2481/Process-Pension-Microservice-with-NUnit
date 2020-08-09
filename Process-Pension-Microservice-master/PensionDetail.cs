using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProcessPension
{
    public class PensionDetail
    {
        public string name { get; set; }
        public double pensionAmount { get; set; }
        public string pan { get; set; }
        public string aadharNumber { get; set; }
        public DateTime dateOfBirth { get; set; }
        public PensionType pensionType { get; set; }
        public int bankType { get; set; }
        public int status { get; set; }
    }
}
