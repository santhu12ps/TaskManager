using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DataLayer
{
    public class TaskDetails
    {
        public int Task_ID { get; set; }
        public int Parent_ID { get; set; }
        public string Parent_Task { get; set; }
        public string TaskName { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public string TaskStartDate { get; set; }
        public string TaskEndDate { get; set; }
        public int Priority { get; set; }
    }
}
