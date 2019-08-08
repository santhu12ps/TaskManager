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
    public class ParentTaskService : IParentTaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParentTaskService()
        {
            _unitOfWork = new UnitOfWork();
        }
        public int CreateParentTask(ParentTaskEntity taskEntity)
        {
            using (var scope = new TransactionScope())
            {
                var task = new ParentTask
                {
                    Parent_ID = taskEntity.Parent_ID,
                    Parent_Task = taskEntity.Parent_Task
                };
                _unitOfWork.ParentTaskRepository.Insert(task);
                _unitOfWork.Save();
                scope.Complete();
                return task.Parent_ID;
            }
        }

        public bool DeleteParentTask(int parentTaskId)
        {
            var success = false;
            if (parentTaskId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var parentTask = _unitOfWork.ParentTaskRepository.GetByID(parentTaskId);
                    if (parentTask != null)
                    {
                        _unitOfWork.ParentTaskRepository.Delete(parentTask);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        public IEnumerable<ParentTaskEntity> GetAllParentTasks()
        {
            var tasks = _unitOfWork.ParentTaskRepository.GetAll().ToList();
            if (tasks != null)
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<ParentTask, ParentTaskEntity>();
                });
                IMapper mapper = config.CreateMapper();
                var taskModel = mapper.Map<List<ParentTask>, List<ParentTaskEntity>>(tasks);
                return taskModel;
            }
            return null;
        }

        public ParentTaskEntity GetParentTaskById(int parentTaskId)
        {
            var parentTask = _unitOfWork.ParentTaskRepository.GetByID(parentTaskId);
            if (parentTask != null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ParentTask, ParentTaskEntity>();
                });
                IMapper mapper = config.CreateMapper();
                var taskModel = mapper.Map<ParentTask, ParentTaskEntity>(parentTask);
                return taskModel;
            }
            return null;
        }

        public bool UpdateParentTask(int parentTaskId, ParentTaskEntity taskEntity)
        {
            throw new NotImplementedException();
        }
    }
}
