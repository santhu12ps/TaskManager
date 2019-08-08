
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TaskManager.BusinessEntities;
using TaskManager.DataLayer;
using TaskManager.DataLayer.UnitOfWork;

namespace TaskManager.BusinessLayer
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>  
        /// Public constructor.  
        /// </summary>  
        public TaskService()
        {
            _unitOfWork = new UnitOfWork();
        }
        /// <summary>  
        /// Creates a task  
        /// </summary>  
        /// <param name="TaskEntity"></param>  
        /// <returns></returns>
        public int CreateTask(TaskEntity taskEntity)
        {
            using (var scope = new TransactionScope())
            {
                var task = new DataLayer.Task
                {
                    Parent_ID = taskEntity.Parent_ID,
                    TaskName = taskEntity.TaskName,
                    Start_Date = taskEntity.Start_Date,
                    End_Date = taskEntity.End_Date,
                    Priority = taskEntity.Priority
                };
                _unitOfWork.TaskRepository.Insert(task);
                _unitOfWork.Save();
                scope.Complete();
                return task.Task_ID;
            }
        }
        /// <summary>  
        /// Updates a task  
        /// </summary>  
        /// <param name="taskId"></param>  
        /// <param name="taskEntity"></param>  
        /// <returns></returns>
        public bool UpdateTask(int taskId, TaskEntity taskEntity)
        {
            var success = false;
            if (taskEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var task = _unitOfWork.TaskRepository.GetByID(taskId);
                    if (task != null)
                    {
                        task.TaskName = taskEntity.TaskName;
                        task.Start_Date = taskEntity.Start_Date;
                        task.End_Date = taskEntity.End_Date;
                        task.Priority = taskEntity.Priority;

                        _unitOfWork.TaskRepository.Update(task);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }
        /// <summary>  
        /// Deletes a particular task  
        /// </summary>  
        /// <param name="taskId"></param>  
        /// <returns></returns>
        public bool DeleteTask(int taskId)
        {
            var success = false;
            if (taskId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var task = _unitOfWork.TaskRepository.GetByID(taskId);
                    if (task != null)
                    {
                        _unitOfWork.TaskRepository.Delete(task);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }
        /// <summary>  
        /// Fetches all the tasks.  
        /// </summary>  
        /// <returns></returns>
        public IEnumerable<TaskEntity> GetAllTasks()
        {
            var tasks = _unitOfWork.TaskRepository.GetAll().ToList();
            if (tasks != null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DataLayer.Task, TaskEntity>();
                });
                IMapper mapper = config.CreateMapper();
                var taskModel = mapper.Map<List<DataLayer.Task>, List<TaskEntity>>(tasks);
                return taskModel;
            }
            return null;
        }
        /// <summary>  
        /// Fetches task details by id  
        /// </summary>  
        /// <param name="taskId"></param>  
        /// <returns></returns>
        public TaskEntity GetTaskById(int taskId)
        {
            var task = _unitOfWork.TaskRepository.GetByID(taskId);
            if (task != null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DataLayer.Task, TaskEntity>();
                });
                IMapper mapper = config.CreateMapper();
                var taskModel = mapper.Map<DataLayer.Task, TaskEntity>(task);
                return taskModel;
            }
            return null;
        }
        /// <summary>
        /// Fetch All Parent Task and Task Details
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TaskDetailsEntity> GetAllTaskDetails()
        {
            var tasks = _unitOfWork.TaskSearchRepository.GetAll().ToList();
            if (tasks.Any())
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<view_TaskDetails, TaskDetailsEntity>();
                });
                IMapper mapper = config.CreateMapper();
                var tasksModel = mapper.Map<List<view_TaskDetails>, List<TaskDetailsEntity>>(tasks);
                return tasksModel;
            }
            return null;
        }

    }
}
