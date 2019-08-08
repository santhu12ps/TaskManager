using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskManager.BusinessEntities;
using TaskManager.Logger;
using TaskManager.BusinessLayer;
using System.Web.Http.Cors;

namespace TaskManager.WEBAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class TaskController : ApiController
    {
        private readonly ITaskService _taskService;
        private readonly ILoggerService _loggerService;
        private readonly IParentTaskService _parentTaskService;
        /// <summary>
        /// Public Constructor
        /// </summary>
        public TaskController()
        {
            _taskService = new TaskService();
            _loggerService = new LoggerService();
            _parentTaskService = new ParentTaskService();
        }
        // GET: api/Task
        public HttpResponseMessage Get()
        {
            try
            {
                _loggerService.LogInfo("InfoCode: API Info - Message :" + "Controller Name : Task - Method Name : GetAllTasks - Description : Method Begin", LoggerConstants.Info.APIInfo);
                var tasks = _taskService.GetAllTaskDetails();
                if (tasks != null)
                {
                    var taskEntities = tasks as List<TaskDetailsEntity> ?? tasks.ToList();
                    if (taskEntities.Any())
                        return Request.CreateResponse(HttpStatusCode.OK, taskEntities);
                }
            }
            catch (Exception exception)
            {
                _loggerService.LogException(exception, LoggerConstants.Info.APIInfo);
                //throw new Exception(exception.Message);
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Tasks not found");
        }

        // GET: api/Task/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                _loggerService.LogInfo("InfoCode: API Info - Message :" + "Controller Name : Task - Method Name : GetTaskById - Description : Method Begin", LoggerConstants.Info.APIInfo);
                var task = _taskService.GetTaskById(id);
                if (task != null)
                    return Request.CreateResponse(HttpStatusCode.OK, task);
            }
            catch (Exception exception)
            {
                _loggerService.LogException(exception, LoggerConstants.Info.APIInfo);
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No task found for this id");
        }

        // POST: api/Task
        public int Post([FromBody]TaskEntity taskEntity)
        {
            try
            {
                _loggerService.LogInfo("InfoCode: API Info - Message :" + "Controller Name : Task - Method Name : Post - Description : Method Begin", LoggerConstants.Info.APIInfo);
                int parentTaskId = _parentTaskService.CreateParentTask(new ParentTaskEntity() { Parent_Task = taskEntity.Parent_Task });
                if (parentTaskId > 0)
                {
                    taskEntity.Parent_ID = parentTaskId;
                    int taskId = _taskService.CreateTask(taskEntity);
                    return taskId;
                }
            }
            catch (Exception exception)
            {
                _loggerService.LogException(exception, LoggerConstants.Info.APIInfo);
            }
            return 0;
        }

        // PUT: api/Task/5
        public bool Put(int id, [FromBody]TaskEntity taskEntity)
        {
            try
            {
                if (id > 0)
                {
                    _loggerService.LogInfo("InfoCode: API Info - Message :" + "Controller Name : Task - Method Name : Put - Description : Method Begin", LoggerConstants.Info.APIInfo);
                    bool status = _taskService.UpdateTask(id, taskEntity);
                    return status;
                }
            }
            catch (Exception exception)
            {
                _loggerService.LogException(exception, LoggerConstants.Info.APIInfo);
            }
            return false;
        }

        // DELETE: api/Task/5
        public bool Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    _loggerService.LogInfo("InfoCode: API Info - Message :" + "Controller Name : Task - Method Name : Delete - Description : Method Begin", LoggerConstants.Info.APIInfo);
                    bool status = _taskService.DeleteTask(id);
                    //_parentTaskService.DeleteParentTask(parentTaskId);
                    return status;
                }
            }
            catch (Exception exception)
            {
                _loggerService.LogException(exception, LoggerConstants.Info.APIInfo);
            }
            return false;
        }
    }
}
