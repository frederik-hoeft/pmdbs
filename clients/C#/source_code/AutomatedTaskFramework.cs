using System;
using System.Collections.Generic;
using System.Linq;

namespace pmdbs
{
    /// <summary>
    /// Allows for functions or methods to be scheduled and executed upon completion of the previous task in a work queue.
    /// </summary>
    public static class AutomatedTaskFramework
    {
        /// <summary>
        /// Keeps the tasks up to date by checking if their Failed / Finish conditions are met in a specific data set. Automatically starts the next task in queue if the previous task finished.
        /// </summary>
        /// <param name="data">The data set to check in for finish / failed conditions.</param>
        public static void DoNetworkTasks(string data)
        {
            if (Tasks.Available())
            {
                Task currentTask = Tasks.GetCurrent();
                if (currentTask.FailedCondition.Split('|').Where(failedCondition => data.Contains(failedCondition)).Count() == 0 && !currentTask.IsFailed() && !currentTask.IsTerminated)
                {
                    if (currentTask.SearchCondition == SearchCondition.Match)
                    {
                        if (data.Equals(currentTask.FinishedCondition))
                        {
                            currentTask.Delete();

                            if (Tasks.Available())
                            {
                                Tasks.GetCurrent().Run();
                            }
                        }
                    }
                    else if (currentTask.SearchCondition == SearchCondition.In)
                    {
                        if (currentTask.FinishedCondition.Split('|').Where(taskCondition => data.Contains(taskCondition)).Count() != 0)
                        {
                            currentTask.Delete();

                            if (Tasks.Available())
                            {
                                Tasks.GetCurrent().Run();
                            }
                        }
                    }
                    else
                    {
                        if (data.Contains(currentTask.FinishedCondition))
                        {
                            currentTask.Delete();

                            if (Tasks.Available())
                            {
                                Tasks.GetCurrent().Run();
                            }
                        }
                    }
                }
                else
                {
                    if (currentTask.BlockingState == FailedTaskBlocking.Block)
                    {
                        currentTask.TaskFailedAction?.Invoke();
                        Tasks.BlockingTaskFailedAction();
                    }
                    else
                    {
                        currentTask.Delete();

                        if (Tasks.Available())
                        {
                            Tasks.GetCurrent().Run();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Tasks maintains a list of all currently active tasks and provides basic task management such as scheduling, executing and cancelling.
        /// </summary>
        public sealed class Tasks
        {
            private static Action _blockingTaskFailedAction = new Action(delegate () { });
            private static readonly List<Task> taskList = new List<Task>();
            /// <summary>
            /// Gets the next scheduled task
            /// </summary>
            /// <returns>Task object</returns>
            public static Task GetCurrent()
            {
                return taskList[0];
            }

            /// <summary>
            /// The code to be executed when a failed task blocks the queue.
            /// </summary>
            public static Action BlockingTaskFailedAction
            {
                get { return _blockingTaskFailedAction; }
                set { _blockingTaskFailedAction = value; }
            }
            /// <summary>
            /// Gets the next scheduled task or NULL if no task is scheduled
            /// </summary>
            /// <returns>Task object or NULL</returns>
            public static Task GetCurrentOrDefault()
            {
                if (taskList.Count > 0)
                {
                    return taskList[0];
                }
                else
                {
                    return null;
                }
            }
            /// <summary>
            /// Checks whether any tasks are scheduled
            /// </summary>
            /// <returns></returns>
            public static bool Available()
            {
                return taskList.Count > 0 ? true : false;
            }
            /// <summary>
            /// Executes the next scheduled task
            /// </summary>
            public static void Execute()
            {
                new System.Threading.Thread(delegate () 
                {
                    GetCurrent().Run();
                }).Start();
            }
            /// <summary>
            /// Schedules a new task to be executed by the ATS
            /// </summary>
            /// <param name="task"></param>
            public static void Schedule(Task task)
            {
                taskList.Add(task);
            }
            /// <summary>
            /// Cancels a specific task from the schedule
            /// </summary>
            /// <param name="task">The task to be cancelled</param>
            public static void Remove(Task task)
            {
                taskList.Remove(task);
            }
            /// <summary>
            /// Cancels all scheduled tasks
            /// </summary>
            public static void Clear()
            {
                taskList.Clear();
            }
            /// <summary>
            /// Gets all currently scheduled tasks
            /// </summary>
            /// <returns>List of all scheduleds tasks</returns>
            public static List<Task> GetAll()
            {
                return taskList;
            }
            /// <summary>
            /// Cancels the current task
            /// </summary>
            /// <returns>Returns true if the task has been cancelled successfully</returns>
            public static bool RemoveCurrent()
            {
                try
                {
                    taskList.RemoveAt(0);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// Creates a deep copy of all scheduled tasks.
            /// </summary>
            /// <returns>Returns a deep copy of all scheduled tasks.</returns>
            public static List<Task> DeepCopy()
            {
                return taskList.ConvertAll(task => task.Copy());
            }

            /// <summary>
            /// Schedules multiple tasks at once.
            /// </summary>
            /// <param name="tasks">The list of tasks to be appended to the queue.</param>
            public static void ScheduleRange(List<Task> tasks)
            {
                taskList.AddRange(tasks);
            }
        }
        /// <summary>
        /// The task object allows scheduling actions/functions/methods to be executed.
        /// </summary>
        public partial class Task
        {
            private bool _isTerminated = false;
            private readonly Action _taskFailedAction = new Action(delegate () { });
            private readonly Action _automatedAction = new Action(delegate () { });
            private readonly string _automatedTaskCondition = string.Empty;
            private readonly string _failedCondition = "SIG_TASK_FAILED";
            private readonly SearchCondition _searchCondition = SearchCondition.Match;
            private readonly FailedTaskBlocking _failedTaskBlocking = FailedTaskBlocking.Block;
            private readonly TaskType _taskType = TaskType.NetworkTask;
            private readonly Func<bool> _funcFinishedCondition = () => { return true; };
            private readonly Func<bool> _funcFailedCondition = () => { return false; };

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="TaskType"></param>
            public Task(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
            }

            private Task(Task task)
            {
                _taskFailedAction = task._taskFailedAction;
                _automatedAction = task._automatedAction;
                _automatedTaskCondition = task._automatedTaskCondition;
                _failedCondition = task._failedCondition;
                _searchCondition = task._searchCondition;
                _failedTaskBlocking = task._failedTaskBlocking;
                _taskType = task._taskType;
                _funcFinishedCondition = task._funcFinishedCondition;
                _funcFailedCondition = task._funcFailedCondition;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="FailedTaskBlocking"></param>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _failedTaskBlocking = FailedTaskBlocking;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FailedCondition"></param>
            /// <param name="TaskType"></param>
            public Task(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _failedCondition = FailedCondition;
            }
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="FailedTaskBlocking"></param>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FailedCondition"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _failedCondition = FailedCondition;
                _failedTaskBlocking = FailedTaskBlocking;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFailedCondition"></param>
            public Task(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Func<bool> FuncFailedCondition)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _funcFailedCondition = FuncFailedCondition;
            }
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="FailedTaskBlocking"></param>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFailedCondition"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Func<bool> FuncFailedCondition)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _funcFailedCondition = FuncFailedCondition;
                _failedTaskBlocking = FailedTaskBlocking;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="TaskAction"></param>
            public Task(TaskType TaskType, Action TaskAction)
            {
                if (TaskType != TaskType.FireAndForget)
                {
                    throw new ArgumentNullException("Missing arguments to call back for task success status. Please use TaskType.FireAndForget to ignore the result of the task.");
                }
                _taskType = TaskType;
                _automatedAction = TaskAction;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFinishedCondition"></param>
            public Task(TaskType TaskType, Action TaskAction, Func<bool> FuncFinishedCondition)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _funcFinishedCondition = FuncFinishedCondition;
            }
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="FailedTaskBlocking"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFinishedCondition"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Action TaskAction, Func<bool> FuncFinishedCondition)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _funcFinishedCondition = FuncFinishedCondition;
                _failedTaskBlocking = FailedTaskBlocking;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFinishedCondition"></param>
            /// <param name="FuncFailedCondition"></param>
            public Task(TaskType TaskType, Action TaskAction, Func<bool> FuncFinishedCondition, Func<bool> FuncFailedCondition)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _funcFinishedCondition = FuncFinishedCondition;
                _funcFailedCondition = FuncFailedCondition;
            }
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFinishedCondition"></param>
            /// <param name="FuncFailedCondition"></param>
            /// <param name="FailedTaskBlocking"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Action TaskAction, Func<bool> FuncFinishedCondition, Func<bool> FuncFailedCondition)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _funcFinishedCondition = FuncFinishedCondition;
                _funcFailedCondition = FuncFailedCondition;
                _failedTaskBlocking = FailedTaskBlocking;
            }



            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="TaskType"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _taskFailedAction = TaskFailedAction;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="FailedTaskBlocking"></param>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _failedTaskBlocking = FailedTaskBlocking;
                _taskFailedAction = TaskFailedAction;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FailedCondition"></param>
            /// <param name="TaskType"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _failedCondition = FailedCondition;
                _taskFailedAction = TaskFailedAction;
            }
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="FailedTaskBlocking"></param>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FailedCondition"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _failedCondition = FailedCondition;
                _failedTaskBlocking = FailedTaskBlocking;
                _taskFailedAction = TaskFailedAction;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFailedCondition"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Func<bool> FuncFailedCondition, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _funcFailedCondition = FuncFailedCondition;
                _taskFailedAction = TaskFailedAction;
            }
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="FailedTaskBlocking"></param>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFailedCondition"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Func<bool> FuncFailedCondition, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _funcFailedCondition = FuncFailedCondition;
                _failedTaskBlocking = FailedTaskBlocking;
                _taskFailedAction = TaskFailedAction;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFinishedCondition"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, Action TaskAction, Func<bool> FuncFinishedCondition, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _funcFinishedCondition = FuncFinishedCondition;
                _taskFailedAction = TaskFailedAction;
            }
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="FailedTaskBlocking"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFinishedCondition"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Action TaskAction, Func<bool> FuncFinishedCondition, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _funcFinishedCondition = FuncFinishedCondition;
                _failedTaskBlocking = FailedTaskBlocking;
                _taskFailedAction = TaskFailedAction;
            }

            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFinishedCondition"></param>
            /// <param name="FuncFailedCondition"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, Action TaskAction, Func<bool> FuncFinishedCondition, Func<bool> FuncFailedCondition, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _funcFinishedCondition = FuncFinishedCondition;
                _funcFailedCondition = FuncFailedCondition;
                _taskFailedAction = TaskFailedAction;
            }
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="TaskType"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FuncFinishedCondition"></param>
            /// <param name="FuncFailedCondition"></param>
            /// <param name="FailedTaskBlocking"></param>
            /// <param name="TaskFailedAction"></param>
            public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Action TaskAction, Func<bool> FuncFinishedCondition, Func<bool> FuncFailedCondition, Action TaskFailedAction)
            {
                _taskType = TaskType;
                _automatedAction = TaskAction;
                _funcFinishedCondition = FuncFinishedCondition;
                _funcFailedCondition = FuncFailedCondition;
                _failedTaskBlocking = FailedTaskBlocking;
                _taskFailedAction = TaskFailedAction;
            }

            /// <summary>
            /// Defines what to do upon task failure.
            /// </summary>
            public FailedTaskBlocking BlockingState
            {
                get { return _failedTaskBlocking; }
            }

            /// <summary>
            /// Terminates the current task by invoking it's failed condition.
            /// </summary>
            public void Terminate()
            {
                _isTerminated = true;
                if (_taskType == TaskType.NetworkTask)
                {
                    if (_failedTaskBlocking == FailedTaskBlocking.Block)
                    {
                        _taskFailedAction?.Invoke();
                        Tasks.BlockingTaskFailedAction();
                    }
                    else
                    {
                        Delete();

                        if (Tasks.Available())
                        {
                            Tasks.GetCurrent().Run();
                        }
                    }
                }
            }

            /// <summary>
            /// Checks wether the task has been terminated.
            /// </summary>
            public bool IsTerminated
            {
                get { return _isTerminated; }
            }

            /// <summary>
            /// The function or method that is linked to the task.
            /// </summary>
            public Action TaskAction
            {
                get { return _automatedAction; }
            }
            /// <summary>
            /// The action to be executed when the task is terminated or fails.
            /// </summary>
            public Action TaskFailedAction
            {
                get { return _taskFailedAction; }
            }
            /// <summary>
            /// The SearchCondition that is used to check for the FinishedCondition in the provided data set.
            /// </summary>
            public SearchCondition SearchCondition
            {
                get { return _searchCondition; }
            }
            /// <summary>
            /// The condition that has to be met to consider the task completed.
            /// </summary>
            public string FinishedCondition
            {
                get { return _automatedTaskCondition; }
            }
            /// <summary>
            /// The condition that has to be met to consider the task failed.
            /// </summary>
            public string FailedCondition
            {
                get { return _failedCondition; }
            }

            /// <summary>
            /// Gets the TaskType of the task.
            /// </summary>
            public TaskType TaskType
            {
                get { return _taskType; }
            }

            /// <summary>
            /// Checks if the task is finished. Will always be true if the TaskType is not Interactive.
            /// </summary>
            /// <returns></returns>
            public bool IsFinished()
            {
                return _funcFinishedCondition();
            }

            /// <summary>
            /// Checks if the task is failed.
            /// </summary>
            /// <returns></returns>
            public bool IsFailed()
            {
                return _funcFailedCondition();
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction)
            {
                Task task = new Task(TaskType, SearchCondition, FinishedCondition, TaskAction);
                Tasks.Schedule(task);
                return task;
            }
            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction);
                Tasks.Schedule(task);
                return task;
            }
            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FailedCondition">The condition that has to be met to consider the task failed.</param>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition)
            {
                Task task = new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, FailedCondition);
                Tasks.Schedule(task);
                return task;
            }
            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FailedCondition">The condition that has to be met to consider the task failed.</param>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, FailedCondition);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, Action TaskAction)
            {
                Task task = new Task(TaskType, TaskAction);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFinishedCondition">The expression to be checked to consider the task finished.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, Action TaskAction, Func<bool> FuncFinishedCondition)
            {
                Task task = new Task(TaskType, TaskAction, FuncFinishedCondition);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFinishedCondition">The expression to be checked to consider the task finished.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Action TaskAction, Func<bool> FuncFinishedCondition)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, TaskAction, FuncFinishedCondition);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFinishedCondition">The expression to be checked to consider the task finished.</param>
            /// <param name="FuncFailedCondition">The expression to be checked to consider the task failed.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, Action TaskAction, Func<bool> FuncFinishedCondition, Func<bool> FuncFailedCondition)
            {
                Task task = new Task(TaskType, TaskAction, FuncFinishedCondition, FuncFailedCondition);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFinishedCondition">The expression to be checked to consider the task finished.</param>
            /// <param name="FuncFailedCondition">The expression to be checked to consider the task failed.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Action TaskAction, Func<bool> FuncFinishedCondition, Func<bool> FuncFailedCondition)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, TaskAction, FuncFinishedCondition, FuncFailedCondition);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFailedCondition">The expression to be checked to consider the task failed.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Func<bool> FuncFailedCondition)
            {
                Task task = new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, FuncFailedCondition);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFailedCondition">The expression to be checked to consider the task failed.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Func<bool> FuncFailedCondition)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, FuncFailedCondition);
                Tasks.Schedule(task);
                return task;
            }





            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Action TaskFailedAction)
            {
                Task task = new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, TaskFailedAction);
                Tasks.Schedule(task);
                return task;
            }
            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="TaskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Action TaskFailedAction)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, TaskFailedAction);
                Tasks.Schedule(task);
                return task;
            }
            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FailedCondition">The condition that has to be met to consider the task failed.</param>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition, Action TaskFailedAction)
            {
                Task task = new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, FailedCondition, TaskFailedAction);
                Tasks.Schedule(task);
                return task;
            }
            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FailedCondition">The condition that has to be met to consider the task failed.</param>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <param name="TaskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition, Action TaskFailedAction)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, FailedCondition, TaskFailedAction);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="taskType">The TaskType of the task.</param>
            /// <param name="taskAction">The function or method that is linked to the task.</param>
            /// <param name="funcFinishedCondition">The expression to be checked to consider the task finished.</param>
            /// <param name="taskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType taskType, Action taskAction, Func<bool> funcFinishedCondition, Action taskFailedAction)
            {
                Task task = new Task(taskType, taskAction, funcFinishedCondition, taskFailedAction);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFinishedCondition">The expression to be checked to consider the task finished.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <param name="TaskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Action TaskAction, Func<bool> FuncFinishedCondition, Action TaskFailedAction)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, TaskAction, FuncFinishedCondition, TaskFailedAction);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFinishedCondition">The expression to be checked to consider the task finished.</param>
            /// <param name="FuncFailedCondition">The expression to be checked to consider the task failed.</param>
            /// <param name="TaskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, Action TaskAction, Func<bool> FuncFinishedCondition, Func<bool> FuncFailedCondition, Action TaskFailedAction)
            {
                Task task = new Task(TaskType, TaskAction, FuncFinishedCondition, FuncFailedCondition, TaskFailedAction);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFinishedCondition">The expression to be checked to consider the task finished.</param>
            /// <param name="FuncFailedCondition">The expression to be checked to consider the task failed.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <param name="TaskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Action TaskAction, Func<bool> FuncFinishedCondition, Func<bool> FuncFailedCondition, Action TaskFailedAction)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, TaskAction, FuncFinishedCondition, FuncFailedCondition, TaskFailedAction);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFailedCondition">The expression to be checked to consider the task failed.</param>
            /// <param name="TaskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Func<bool> FuncFailedCondition, Action TaskFailedAction)
            {
                Task task = new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, FuncFailedCondition, TaskFailedAction);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="TaskType">The TaskType of the task.</param>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FuncFailedCondition">The expression to be checked to consider the task failed.</param>
            /// <param name="FailedTaskBlocking">The blocking state of the task.</param>
            /// <param name="TaskFailedAction">The action to be executed when the task is terminated or fails.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, Func<bool> FuncFailedCondition, Action TaskFailedAction)
            {
                Task task = new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, FuncFailedCondition, TaskFailedAction);
                Tasks.Schedule(task);
                return task;
            }

            /// <summary>
            /// Cancels this task.
            /// </summary>
            public void Delete()
            {
                Tasks.Remove(this);
            }

            /// <summary>
            /// Creates a deep copy of the task.
            /// </summary>
            /// <returns>Returns the deep copy.</returns>
            public Task Copy()
            {
                return new Task(this);
            }

            /// <summary>
            /// Executes the method or function that is linked to the task.
            /// </summary>
            public void Run()
            {
                switch (_taskType)
                {
                    case TaskType.FireAndForget:
                        _automatedAction();
                        Delete();
                        if (Tasks.Available())
                        {
                            Tasks.GetCurrent().Run();
                        }
                        break;

                    case TaskType.Interactive:
                        new System.Threading.Thread(delegate ()
                        {
                            while (!IsFinished() && !IsFailed() && !IsTerminated)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                            if (IsFailed() || IsTerminated)
                            {
                                if (BlockingState == FailedTaskBlocking.Block)
                                {
                                    TaskFailedAction?.Invoke();
                                    Tasks.BlockingTaskFailedAction();
                                    return;
                                }
                            }
                            Delete();
                            if (Tasks.Available())
                            {
                                Tasks.GetCurrent().Run();
                            }
                        }).Start();
                        try
                        {
                            _automatedAction();
                        }
                        catch
                        {
                            Terminate();
                        }
                        break;

                    default:
                        try
                        {
                            _automatedAction();
                        }
                        catch
                        {
                            Terminate();
                        }
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Defines how conditions should be searched for in the data set.
    /// </summary>
    public enum SearchCondition
    {
        /// <summary>
        /// The data set has to match the provided string exactly.
        /// </summary>
        Match = 1,
        /// <summary>
        /// The data set has to contain the provided string.
        /// </summary>
        Contains = 2,
        /// <summary>
        /// The data set has to contain one or more of the provided search terms seperated by the | character.
        /// </summary>
        In = 3
    }

    /// <summary>
    /// Defines how the system should interact with the scheduled task.
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// Executes the task and immediately continues with the next one.
        /// </summary>
        FireAndForget = 1,
        /// <summary>
        /// Check success of task by going through the network output.
        /// </summary>
        NetworkTask = 2,
        /// <summary>
        /// Ceck success of task by checking a custom condition.
        /// </summary>
        Interactive = 3
    }
    /// <summary>
    /// Defines how the system should act when a task fails.
    /// </summary>
    public enum FailedTaskBlocking
    {
        /// <summary>
        /// Task will block the task queue upon failing.
        /// </summary>
        Block = 1,
        /// <summary>
        /// The task failed state will be ignored and the next task will be executed.
        /// </summary>
        Ignore = 2
    }
}
