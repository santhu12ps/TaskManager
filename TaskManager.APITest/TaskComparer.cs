using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = TaskManager.DataLayer.Task;

namespace TaskManager.APITest
{
    public class TaskComparer : IComparer, IComparer<Task>
    {
        public int Compare(object expected, object actual)
        {
            var lhs = expected as Task;
            var rhs = actual as Task;
            if (lhs == null || rhs == null) throw new InvalidOperationException();
            return Compare(lhs, rhs);
        }

        public int Compare(Task expected, Task actual)
        {
            int temp;
            return (temp = expected.Task_ID.CompareTo(actual.Task_ID)) != 0 ?
                    temp : expected.TaskName.CompareTo(actual.TaskName);
        }
    }
}
