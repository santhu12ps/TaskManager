using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.BusinessEntities
{
    public class TaskDetailsEntity
    {
        public int Task_ID { get; set; }
        public string TaskName { get; set; }
        public System.DateTime Start_Date { get; set; }
        public System.DateTime End_Date { get; set; }
        public string TaskStartDate { get; set; }
        public string TaskEndDate { get; set; }
        public int Priority { get; set; }
        public int Parent_ID { get; set; }
        public string ParentTaskName { get; set; }
    }
}
