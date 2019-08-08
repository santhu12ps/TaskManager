using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = TaskManager.DataLayer.Task;

namespace TaskManager.APITest
{
    public class TestHelper
    {
        public static List<Task> GetAllTasks()
        {
            var tasks = new List<Task>
                {
                new Task()
                {
                    Task_ID = 1,
                    Parent_ID = 1,
                    TaskName = "Task-1",
                    Start_Date = Convert.ToDateTime("2018-01-20"),
                    End_Date = Convert.ToDateTime("2018-01-30"),
                    Priority = 5
                },
                new Task()
                {
                    Task_ID = 2,
                    Parent_ID = 2,
                    TaskName = "Task-2",
                    Start_Date = Convert.ToDateTime("2018-02-10"),
                    End_Date = Convert.ToDateTime("2018-02-20"),
                    Priority = 10
                }
            };
            return tasks;
        }
    }
}
