using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Hosting;
using TaskManager.BusinessEntities;
using TaskManager.BusinessLayer;
using TaskManager.DataLayer;
using TaskManager.DataLayer.GenericRepository;
using TaskManager.DataLayer.UnitOfWork;
using TaskManager.WEBAPI.Controllers;
using Task = TaskManager.DataLayer.Task;

namespace TaskManager.APITest
{
    [TestFixture]
    public class TaskControllerTest
    {
        #region Variables  

        private ITaskService _taskService;
        private IParentTaskService _parentTaskService;
        private IUnitOfWork _unitOfWork;
        private List<Task> _tasks;
        private GenericRepository<Task> _taskRepository;
        private TaskManagerDBEntities _dbEntities;
        private HttpClient _client;
        private HttpResponseMessage _response;

        private const string serviceBaseURL = "http://172.18.4.96/api/task/";

        #endregion


        #region Setup  
        ///<summary>  
        /// Re-initializes test.  
        ///</summary>  
        [SetUp]
        public void ReInitializeTest()
        {
            _client = new HttpClient { BaseAddress = new Uri(serviceBaseURL) };
        }

        #endregion

        private GenericRepository<Task> SetUpTaskRepository()
        {

            // Initialise repository  
            var mockRepo = new Mock<GenericRepository<Task>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior  
            mockRepo.Setup(p => p.GetAll()).Returns(_tasks);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, Task>(
                    id => _tasks.Find(p => p.Task_ID.Equals(id))));


            mockRepo.Setup(p => p.Insert((It.IsAny<Task>())))
                .Callback(new Action<Task>(newTask =>
                {
                    _tasks.Add(newTask);
                }));


            mockRepo.Setup(p => p.Update(It.IsAny<Task>()))
                .Callback(new Action<Task>(tsk =>
                {
                    var oldTask = _tasks.Find(a => a.Task_ID == tsk.Task_ID);
                    oldTask = tsk;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Task>()))
                .Callback(new Action<Task>(tsk =>
                {
                    var tasksToRemove =
                        _tasks.Find(a => a.Task_ID == tsk.Task_ID);

                    if (tasksToRemove != null)
                        _tasks.Remove(tasksToRemove);
                }));

            // Return mock implementation object  
            return mockRepo.Object;
        }

        [TearDown]
        public void DisposeTest()
        {
            if (_response != null) _response.Dispose();
            if (_client != null) _client.Dispose();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _tasks = SetUpTasks();
            _dbEntities = new Mock<TaskManagerDBEntities>().Object;
            _taskRepository = SetUpTaskRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.TaskRepository).Returns(_taskRepository);
            _unitOfWork = unitOfWork.Object;
            _taskService = new TaskService();
            _parentTaskService = new ParentTaskService();
            _client = new HttpClient
            {
                BaseAddress = new Uri(serviceBaseURL)
            };
        }
        private static List<Task> SetUpTasks()
        {
            var tasks = TestHelper.GetAllTasks();
            return tasks;
        }

        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _tasks = null;
        }

        [Test]
        public void CreateTaskTest()
        {

            var taskController = new TaskController()
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(serviceBaseURL + "Create")
                }
            };
            taskController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var newTask = new TaskEntity()
            {
                Task_ID = 1,
                Parent_ID = 1,
                TaskName = "Task-1",
                Parent_Task = "ParentTask-1",
                Start_Date = Convert.ToDateTime("2019-08-20"),
                End_Date = Convert.ToDateTime("2019-08-30"),
                Priority = 5
            };
            int parentTask = _parentTaskService.CreateParentTask(new ParentTaskEntity() { Parent_Task = newTask.Parent_Task });
            taskController.Post(newTask);

            _response = taskController.Get();
            var responseResultSearch = JsonConvert.DeserializeObject<List<view_TaskDetails>>(_response.Content.ReadAsStringAsync().Result);
            var taskList =
                responseResultSearch.Select(
                    taskEntity =>
                    new Task
                    {
                        Task_ID = taskEntity.Task_ID,
                        Parent_ID = taskEntity.Parent_ID,
                        TaskName = taskEntity.TaskName,
                        Start_Date = taskEntity.Start_Date,
                        End_Date = taskEntity.End_Date,
                        Priority = taskEntity.Priority
                    }).ToList();
            var addedtask = new Task()
            {
                Task_ID = newTask.Task_ID,
                Parent_ID = newTask.Parent_ID,
                TaskName = newTask.TaskName,
                Start_Date = newTask.Start_Date,
                End_Date = newTask.End_Date,
                Priority = newTask.Priority
            };
            AssertObjects.PropertyValuesAreEquals(addedtask, taskList.Last());
        }
        [Test]
        public void UpdateTaskTest()
        {
            var taskController = new TaskController()
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(serviceBaseURL + "put/1")
                }
            };
            taskController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var firstTask = _tasks.First();
            firstTask.TaskName = "Task1_1";
            var updatedTask = new TaskEntity()
            {
                Task_ID = firstTask.Task_ID,
                Parent_ID = firstTask.Parent_ID,
                TaskName = firstTask.TaskName,
                Start_Date = firstTask.Start_Date,
                End_Date = firstTask.End_Date,
                Priority = firstTask.Priority
            };
            taskController.Put(firstTask.Task_ID, updatedTask);
            Assert.That(firstTask.Task_ID, Is.EqualTo(1)); // hasn't changed
        }

        [Test]
        public void GetAllTasksTest()
        {
            var taskController = new TaskController()
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(serviceBaseURL)
                }
            };
            taskController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            _response = taskController.Get();
            var results = JsonConvert.DeserializeObject<List<view_TaskDetails>>(_response.Content.ReadAsStringAsync().Result);
            var taskList =
                results.Select(
                    taskEntity =>
                    new Task
                    {
                        Task_ID = taskEntity.Task_ID,
                        Parent_ID = taskEntity.Parent_ID,
                        TaskName = taskEntity.TaskName,
                        Start_Date = taskEntity.Start_Date,
                        End_Date = taskEntity.End_Date,
                        Priority = taskEntity.Priority
                    }).ToList();
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(taskList.Any(), true);
            //var comparer = new TaskComparer();
            //CollectionAssert.AreEqual(taskList.OrderBy(tsk => tsk, comparer), _tasks.OrderBy(tsk => tsk, comparer), comparer);
        }

        [Test]
        public void GetTaskByIdTest()
        {
            var taskController = new TaskController()
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(serviceBaseURL + "get/4")
                }
            };
            taskController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            _response = taskController.Get(4);
            var responseResult = JsonConvert.DeserializeObject<Task>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
        }
        [Test]
        public void DeleteTaskTest()
        {
            var taskController = new TaskController()
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(serviceBaseURL + "delete")
                }
            };
            taskController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var lastTask = _tasks.Last();
            int maxID = _tasks.Max(a => a.Task_ID); // Before removal

            // Remove last user
            taskController.Delete(lastTask.Task_ID);

            _response = taskController.Get();
            var responseResultSearch = JsonConvert.DeserializeObject<List<view_TaskDetails>>(_response.Content.ReadAsStringAsync().Result);
            var taskList =
                responseResultSearch.Select(
                    taskEntity =>
                    new Task
                    {
                        Task_ID = taskEntity.Task_ID,
                        Parent_ID = taskEntity.Parent_ID,
                        TaskName = taskEntity.TaskName,
                        Start_Date = taskEntity.Start_Date,
                        End_Date = taskEntity.End_Date,
                        Priority = taskEntity.Priority
                    }).ToList();

            Assert.That(maxID > 0);
        }

        #region Integration Test

        /// <summary>
        /// Get all tasks test
        /// </summary>
        [Test]
        public void GetAllTasksIntegrationTest()
        {
            #region To be written inside Setup method specifically for integration tests
            var client = new HttpClient { BaseAddress = new Uri(serviceBaseURL) };
            MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            #endregion
            _response = client.GetAsync(serviceBaseURL).Result;
            var responseResult =
                JsonConvert.DeserializeObject<List<view_TaskDetails>>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseResult.Any(), true);
        }

        #endregion
    }
}
