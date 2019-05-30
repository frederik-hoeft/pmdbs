using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    public static class AutomatedTaskFramework
    {
        public static void DoTasks(string data)
        {
            if (AutomatedTaskFramework.Tasks.Available())
            {
                AutomatedTaskFramework.Task currentTask = AutomatedTaskFramework.Tasks.GetCurrent();
                if (currentTask.FailedCondition.Split('|').Where(failedCondition => data.Contains(failedCondition)).Count() == 0)
                {
                    if (currentTask.SearchCondition == SearchCondition.Match)
                    {
                        if (data.Equals(currentTask.FinishedCondition))
                        {
                            currentTask.Delete();

                            if (AutomatedTaskFramework.Tasks.Available())
                            {
                                AutomatedTaskFramework.Tasks.GetCurrent().Run();
                            }
                        }
                    }
                    else if (currentTask.SearchCondition == SearchCondition.In)
                    {
                        if (currentTask.FinishedCondition.Split('|').Where(taskCondition => data.Contains(taskCondition)).Count() != 0)
                        {
                            currentTask.Delete();

                            if (AutomatedTaskFramework.Tasks.Available())
                            {
                                AutomatedTaskFramework.Tasks.GetCurrent().Run();
                            }
                        }
                    }
                    else
                    {
                        if (data.Contains(currentTask.FinishedCondition))
                        {
                            currentTask.Delete();

                            if (AutomatedTaskFramework.Tasks.Available())
                            {
                                AutomatedTaskFramework.Tasks.GetCurrent().Run();
                            }
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
            private static readonly List<AutomatedTaskFramework.Task> taskList = new List<Task>();
            /// <summary>
            /// Gets the next scheduled task
            /// </summary>
            /// <returns>Task object</returns>
            public static Task GetCurrent()
            {
                return taskList[0];
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
                GetCurrent().Run();
            }
            /// <summary>
            /// Schedules a new task to be executed by the ATS
            /// </summary>
            /// <param name="task"></param>
            public static void Add(Task task)
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
        }
        public partial class Task
        {
            private readonly Action _automatedAction = new Action(delegate { });
            private readonly string _automatedTask = string.Empty;
            private readonly string _automatedTaskCondition = string.Empty;
            private readonly string _failedCondition = "SIG_TASK_FAILED";
            private readonly SearchCondition _searchCondition = SearchCondition.Match;
            private Task(SearchCondition SearchCondition, string FinishedCondition, string Command)
            {
                _automatedTask = Command;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                Tasks.Add(this);
            }

            private Task(SearchCondition SearchCondition, string FinishedCondition, string Command, string FailedCondition)
            {
                _automatedTask = Command;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _failedCondition = FailedCondition;
                Tasks.Add(this);
            }

            private Task(SearchCondition SearchCondition, string FinishedCondition, Action TaskAction)
            {
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                Tasks.Add(this);
            }

            private Task(SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition)
            {
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _failedCondition = FailedCondition;
                Tasks.Add(this);
            }

            public Action TaskAction
            {
                get { return _automatedAction; }
            }

            public SearchCondition SearchCondition
            {
                get { return _searchCondition; }
            }

            public string Command
            {
                get { return _automatedTask; }
            }

            public string FinishedCondition
            {
                get { return _automatedTaskCondition; }
            }

            public string FailedCondition
            {
                get { return _failedCondition; }
            }

            public static Task Create(SearchCondition SearchCondition, string FinishedCondition, string Command)
            {
                return new Task(SearchCondition, FinishedCondition, Command);
            }

            public static Task Create(SearchCondition SearchCondition, string FinishedCondition, string Command, string FailedCondition)
            {
                return new Task(SearchCondition, FinishedCondition, Command, FailedCondition);
            }

            public static Task Create(SearchCondition SearchCondition, string FinishedCondition, Action TaskAction)
            {
                return new Task(SearchCondition, FinishedCondition, TaskAction);
            }

            public static Task Create(SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition)
            {
                return new Task(SearchCondition, FinishedCondition, TaskAction, FailedCondition);
            }

            public void Delete()
            {
                Tasks.Remove(this);
            }

            public void Run()
            {
                _automatedAction();
            }
        }
    }
}
