using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessPension
{
    public class ProcessPensionInput
    {  
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Pan { get; set; }
        public string AadharNumber { get; set; }
        public PensionType PensionType { get; set; }   
    }
    public enum PensionType
    {
        Self=1,
        Family=2
    }
    public class ResultforProcessPensionInput
    {
        public string message { get; set; }
        public ProcessPensionInput result { get; set; }
    }
    public class ResultforValueCalculation
    {
        public string message { get; set; }
        public ValueforCalCulation result { get; set; }
    }
}
