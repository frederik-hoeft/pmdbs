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
        public static void DoTasks(string data)
        {
            if (Tasks.Available())
            {
                Task currentTask = Tasks.GetCurrent();
                if (currentTask.FailedCondition.Split('|').Where(failedCondition => data.Contains(failedCondition)).Count() == 0)
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
            }
        }
        /// <summary>
        /// Tasks maintains a list of all currently active tasks and provides basic task management such as scheduling, executing and cancelling.
        /// </summary>
        public sealed class Tasks
        {
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
        /// <summary>
        /// The task object allows scheduling a specific action/function/method to be executed.
        /// </summary>
        public partial class Task
        {
            private readonly Action _automatedAction = new Action(delegate { });
            private readonly string _automatedTaskCondition = string.Empty;
            private readonly string _failedCondition = "SIG_TASK_FAILED";
            private readonly SearchCondition _searchCondition = SearchCondition.Match;
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            public Task(SearchCondition SearchCondition, string FinishedCondition, Action TaskAction)
            {
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
            }
            /// <summary>
            /// Task constructor
            /// </summary>
            /// <param name="SearchCondition"></param>
            /// <param name="FinishedCondition"></param>
            /// <param name="TaskAction"></param>
            /// <param name="FailedCondition"></param>
            public Task(SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition)
            {
                _automatedAction = TaskAction;
                _automatedTaskCondition = FinishedCondition;
                _searchCondition = SearchCondition;
                _failedCondition = FailedCondition;
            }
            /// <summary>
            /// The function or method that is linked to the task.
            /// </summary>
            public Action TaskAction
            {
                get { return _automatedAction; }
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
            /// Creates a new Task object.
            /// </summary>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(SearchCondition SearchCondition, string FinishedCondition, Action TaskAction)
            {
                Task task = new Task(SearchCondition, FinishedCondition, TaskAction);
                Tasks.Add(task);
                return task;
            }
            /// <summary>
            /// Creates a new Task object.
            /// </summary>
            /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
            /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
            /// <param name="TaskAction">The function or method that is linked to the task.</param>
            /// <param name="FailedCondition">The condition that has to be met to consider the task failed.</param>
            /// <returns>Returns the created Task object.</returns>
            public static Task Create(SearchCondition SearchCondition, string FinishedCondition, Action TaskAction, string FailedCondition)
            {
                Task task = new Task(SearchCondition, FinishedCondition, TaskAction, FailedCondition);
                Tasks.Add(task);
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
            /// Executes the method or function that is linked to the task.
            /// </summary>
            public void Run()
            {
                _automatedAction();
            }
        }
    }
    /// <summary>
    /// Defines how conditions should be searched for in the data set.
    /// </summary>
    public enum SearchCondition
    {
        Match = 1,
        Contains = 2,
        In = 3
    }
}
