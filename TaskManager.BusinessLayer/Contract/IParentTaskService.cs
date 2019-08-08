using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.BusinessEntities;

namespace TaskManager.BusinessLayer
{
    public interface IParentTaskService
    {
        ParentTaskEntity GetParentTaskById(int taskId);
        IEnumerable<ParentTaskEntity> GetAllParentTasks();
        int CreateParentTask(ParentTaskEntity parentTaskEntity);
        bool UpdateParentTask(int parentTaskId, ParentTaskEntity parentTaskEntity);
        bool DeleteParentTask(int parentTaskId);
    }
}
