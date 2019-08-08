
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.BusinessEntities;

namespace TaskManager.BusinessLayer
{
    public interface ITaskService
    {
        int CreateTask(TaskEntity taskEntity);
        bool UpdateTask(int taskId, TaskEntity taskEntity);
        bool DeleteTask(int taskId);
        IEnumerable<TaskEntity> GetAllTasks();
        TaskEntity GetTaskById(int taskId);
        IEnumerable<TaskDetailsEntity> GetAllTaskDetails();
    }
}
