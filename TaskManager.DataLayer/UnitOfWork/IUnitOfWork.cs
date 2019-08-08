using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.DataLayer;
using TaskManager.DataLayer.GenericRepository;

namespace TaskManager.DataLayer.UnitOfWork
{
    public interface IUnitOfWork
    {

        GenericRepository<Task> TaskRepository
        {
            get;
        }
        GenericRepository<ParentTask> ParentTaskRepository
        {
            get;
        }
        GenericRepository<view_TaskDetails> TaskSearchRepository
        {
            get;
        }
        void Save();
    }
}
