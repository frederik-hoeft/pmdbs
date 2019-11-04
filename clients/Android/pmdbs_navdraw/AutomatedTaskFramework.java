package com.example.rodaues.pmdbs_navdraw;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Callable;



/// <summary>
/// Allows for functions or methods to be scheduled and executed upon completion of the previous task in a work queue.
/// </summary>
public class AutomatedTaskFramework
{
    // --------------SINGLETON DESIGN PATTERN START
    private static AutomatedTaskFramework _atf = null;

    private AutomatedTaskFramework() {}

    public static AutomatedTaskFramework GetInstance()
    {
        if(_atf == null)
        {
            _atf = new AutomatedTaskFramework();
        }
        return _atf;
    }
    /// <summary>
    /// Defines how conditions should be searched for in the data set.
    /// </summary>
    public enum SearchCondition
    {
        /// <summary>
        /// The data set has to match the provided String exactly.
        /// </summary>
        Match,
        /// <summary>
        /// The data set has to contain the provided String.
        /// </summary>
        Contains,
        /// <summary>
        /// The data set has to contain one or more of the provided search terms seperated by the | character.
        /// </summary>
        In
    }

    /// <summary>
    /// Defines how the system should interact with the scheduled task.
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// Executes the task and immediately continues with the next one.
        /// </summary>
        FireAndForget,
        /// <summary>
        /// Check success of task by going through the network output.
        /// </summary>
        NetworkTask,
        /// <summary>
        /// Ceck success of task by checking a custom condition.
        /// </summary>
        Interactive
    }
    /// <summary>
    /// Defines how the system should act when a task fails.
    /// </summary>
    public enum FailedTaskBlocking
    {
        /// <summary>
        /// Task will block the task queue upon failing.
        /// </summary>
        Block,
        /// <summary>
        /// The task failed state will be ignored and the next task will be executed.
        /// </summary>
        Ignore
    }

    /// <summary>
    /// Keeps the tasks up to date by checking if their Failed / Finish conditions are met in a specific data set. Automatically starts the next task in queue if the previous task finished.
    /// </summary>
    /// <param name="data">The data set to check in for finish / failed conditions.</param>
    public static void DoNetworkTasks(String data)
    {
        if (Tasks.Available())
        {
            Task currentTask = Tasks.GetCurrent();
            int failedCount = 0;
            if (currentTask.GetFailedCondition().contains("|"))
            {
                String[] failedConditions = currentTask.GetFailedCondition().split("\\|");
                for (int i = 0; i < failedConditions.length; i++)
                {
                    if (data.contains(failedConditions[i]))
                    {
                        failedCount++;
                    }
                }
            }
            else
            {
                if (data.contains(currentTask.GetFailedCondition()))
                {
                    failedCount++;
                }
            }

            if (failedCount == 0 && !currentTask.IsTerminated())
            {
                if (currentTask.GetSearchCondition() == SearchCondition.Match)
                {
                    if (data.equals(currentTask.GetFinishedCondition()))
                    {
                        currentTask.Delete();

                        if (Tasks.Available())
                        {
                            Tasks.GetCurrent().Run();
                        }
                    }
                }
                else if (currentTask.GetSearchCondition() == SearchCondition.In)
                {
                    int finishCount = 0;
                    //TODO fix splitting
                    String[] finishedConditions = currentTask.GetFinishedCondition().split("\\|");
                    for (int i = 0; i < finishedConditions.length; i++)
                    {
                        if (data.contains(finishedConditions[i]))
                        {
                            finishCount++;
                        }
                    }
                    if (finishCount != 0)
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
                    if (data.contains(currentTask.GetFinishedCondition()))
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
                if (currentTask.GetBlockingState() == FailedTaskBlocking.Block)
                {
                    if (currentTask.GetTaskFailedAction() != null)
                    {
                        currentTask.GetTaskFailedAction().run();
                    }
                    Tasks.GetBlockingTaskFailedAction().run();
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
    public static class Tasks
    {
        private static Boolean executing = false;

        private static Runnable _blockingTaskFailedAction = null;
        private static final List<Task> taskList = new ArrayList<Task>();
        /// <summary>
        /// Gets the next scheduled task
        /// </summary>
        /// <returns>Task object</returns>
        public static Task GetCurrent()
        {
            return taskList.get(0);
        }

        public static void Finalize(){
            executing=false;
        }

        /// <summary>
        /// The code to be executed when a failed task blocks the queue.
        /// </summary>
        public static Runnable GetBlockingTaskFailedAction()
        {
            return _blockingTaskFailedAction;
        }

        public static void SetBlockingTaskFailedAction(Runnable value)
        {
            _blockingTaskFailedAction = value;
        }
        /// <summary>
        /// Gets the next scheduled task or NULL if no task is scheduled
        /// </summary>
        /// <returns>Task object or NULL</returns>
        public static Task GetCurrentOrDefault()
        {
            if (taskList.size() > 0)
            {
                return taskList.get(0);
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
        public static boolean Available()
        {
            boolean available = taskList.size()>0;
            if(!available){
                Finalize();
            }
            return available;
        }
        /// <summary>
        /// Executes the next scheduled task
        /// </summary>
        public static void Execute()
        {
            if(!executing){
                executing=true;
                new Thread(new Runnable() {
                    @Override public void run() {
                        Task task = GetCurrentOrDefault();
                        if(task!=null) {
                            task.Run();
                        }
                    }
                }).start();
            }
        }
        /// <summary>
        /// Schedules a new task to be executed by the ATS
        /// </summary>
        /// <param name="task"></param>
        public static void Schedule(Task task)
        {
            taskList.add(task);
        }
        /// <summary>
        /// Cancels a specific task from the schedule
        /// </summary>
        /// <param name="task">The task to be cancelled</param>
        public static void Remove(Task task)
        {
            taskList.remove(task);
        }
        /// <summary>
        /// Cancels all scheduled tasks
        /// </summary>
        public static void Clear()
        {
            taskList.clear();
        }
        /// <summary>
        /// Gets all currently scheduled tasks
        /// </summary>
        /// <returns>List of all scheduled tasks</returns>
        public static List<Task> GetAll()
        {
            return taskList;
        }
        /// <summary>
        /// Cancels the current task
        /// </summary>
        /// <returns>Returns true if the task has been cancelled successfully</returns>
        public static boolean RemoveCurrent()
        {
            try
            {
                taskList.remove(0);
                return true;
            }
            catch (Exception e)
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
            List<Task> copy = new ArrayList<Task>();
            for (int i = 0; i < taskList.size(); i++)
            {
                copy.add(taskList.get(i).Copy());
            }
            return copy;
        }

        /// <summary>
        /// Schedules multiple tasks at once.
        /// </summary>
        /// <param name="tasks">The list of tasks to be appended to the queue.</param>
        public static void ScheduleRange(List<Task> tasks)
        {
            taskList.addAll(tasks);
        }
    }

    public static class TaskFactory
    {
        private static AutomatedTaskFramework atf = AutomatedTaskFramework.GetInstance();
        /// <summary>
        /// Creates a new Task object.
        /// </summary>
        /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
        /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
        /// <param name="TaskAction">The function or method that is linked to the task.</param>
        /// <param name="TaskType">The TaskType of the task.</param>
        /// <returns>Returns the created Task object.</returns>
        public static Task Create(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction)
        {
            Task task = atf.new Task(TaskType, SearchCondition, FinishedCondition, TaskAction);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction);
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
        public static Task Create(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, String FailedCondition)
        {
            Task task = atf.new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, FailedCondition);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, String FailedCondition)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, FailedCondition);
            Tasks.Schedule(task);
            return task;
        }

        /// <summary>
        /// Creates a new Task object.
        /// </summary>
        /// <param name="TaskType">The TaskType of the task.</param>
        /// <param name="TaskAction">The function or method that is linked to the task.</param>
        /// <returns>Returns the created Task object.</returns>
        public static Task Create(TaskType TaskType, Runnable TaskAction)
        {
            Task task = atf.new Task(TaskType, TaskAction);
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
        public static Task Create(TaskType TaskType, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition)
        {
            Task task = atf.new Task(TaskType, TaskAction, FuncFinishedCondition);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, TaskAction, FuncFinishedCondition);
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
        public static Task Create(TaskType TaskType, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Callable<Boolean> FuncFailedCondition)
        {
            Task task = atf.new Task(TaskType, TaskAction, FuncFinishedCondition, FuncFailedCondition);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Callable<Boolean> FuncFailedCondition)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, TaskAction, FuncFinishedCondition, FuncFailedCondition);
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
        public static Task Create(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Callable<Boolean> FuncFailedCondition)
        {
            Task task = atf.new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, FuncFailedCondition);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Callable<Boolean> FuncFailedCondition)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, FuncFailedCondition);
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
        public static Task Create(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Runnable TaskFailedAction)
        {
            Task task = atf.new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, TaskFailedAction);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Runnable TaskFailedAction)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, TaskFailedAction);
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
        public static Task Create(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, String FailedCondition, Runnable TaskFailedAction)
        {
            Task task = atf.new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, FailedCondition, TaskFailedAction);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, String FailedCondition, Runnable TaskFailedAction)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, FailedCondition, TaskFailedAction);
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
        public static Task Create(TaskType taskType, Runnable taskAction, Callable<Boolean> funcFinishedCondition, Runnable taskFailedAction)
        {
            Task task = atf.new Task(taskType, taskAction, funcFinishedCondition, taskFailedAction);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Runnable TaskFailedAction)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, TaskAction, FuncFinishedCondition, TaskFailedAction);
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
        public static Task Create(TaskType TaskType, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Callable<Boolean> FuncFailedCondition, Runnable TaskFailedAction)
        {
            Task task = atf.new Task(TaskType, TaskAction, FuncFinishedCondition, FuncFailedCondition, TaskFailedAction);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Callable<Boolean> FuncFailedCondition, Runnable TaskFailedAction)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, TaskAction, FuncFinishedCondition, FuncFailedCondition, TaskFailedAction);
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
        public static Task Create(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Callable<Boolean> FuncFailedCondition, Runnable TaskFailedAction)
        {
            Task task = atf.new Task(TaskType, SearchCondition, FinishedCondition, TaskAction, FuncFailedCondition, TaskFailedAction);
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
        public static Task Create(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Callable<Boolean> FuncFailedCondition, Runnable TaskFailedAction)
        {
            Task task = atf.new Task(TaskType, FailedTaskBlocking, SearchCondition, FinishedCondition, TaskAction, FuncFailedCondition, TaskFailedAction);
            Tasks.Schedule(task);
            return task;
        }
    }

    /// <summary>
    /// The task object allows scheduling actions/functions/methods to be executed.
    /// </summary>
    public class Task
    {
        private boolean _isTerminated = false;
        private Runnable _automatedAction = null;
        private Runnable _taskFailedAction = null;
        private String _automatedTaskCondition = "";
        private String _failedCondition = "SIG_TASK_FAILED";
        private SearchCondition _searchCondition = AutomatedTaskFramework.SearchCondition.Match;
        private FailedTaskBlocking _failedTaskBlocking = FailedTaskBlocking.Block;
        private TaskType _taskType = AutomatedTaskFramework.TaskType.NetworkTask;
        private Callable<Boolean> _funcFinishedCondition = new Callable<Boolean>() {public Boolean call() { return true; };};
        private Callable<Boolean> _funcFailedCondition = new Callable<Boolean>() {public Boolean call() { return false; };};

        /// <summary>
        /// Task constructor
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <param name="FinishedCondition"></param>
        /// <param name="TaskAction"></param>
        /// <param name="TaskType"></param>
        public Task(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction)
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
        public Task(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, String FailedCondition)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, String FailedCondition)
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
        public Task(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Callable<Boolean> FuncFailedCondition)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Callable<Boolean> FuncFailedCondition)
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
        public Task(TaskType TaskType, Runnable TaskAction)
        {
            if (TaskType != AutomatedTaskFramework.TaskType.FireAndForget)
            {
                throw new IllegalArgumentException("Missing arguments to call back for task success status. Please use TaskType.FireAndForget to ignore the result of the task.");
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
        public Task(TaskType TaskType, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition)
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
        public Task(TaskType TaskType, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Callable<Boolean> FuncFailedCondition)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Callable<Boolean> FuncFailedCondition)
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
        public Task(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Runnable TaskFailedAction)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Runnable TaskFailedAction)
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
        public Task(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, String FailedCondition, Runnable TaskFailedAction)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, String FailedCondition, Runnable TaskFailedAction)
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
        public Task(TaskType TaskType, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Callable<Boolean> FuncFailedCondition, Runnable TaskFailedAction)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, SearchCondition SearchCondition, String FinishedCondition, Runnable TaskAction, Callable<Boolean> FuncFailedCondition, Runnable TaskFailedAction)
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
        public Task(TaskType TaskType, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Runnable TaskFailedAction)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Runnable TaskFailedAction)
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
        public Task(TaskType TaskType, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Callable<Boolean> FuncFailedCondition, Runnable TaskFailedAction)
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
        public Task(TaskType TaskType, FailedTaskBlocking FailedTaskBlocking, Runnable TaskAction, Callable<Boolean> FuncFinishedCondition, Callable<Boolean> FuncFailedCondition, Runnable TaskFailedAction)
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
        public FailedTaskBlocking GetBlockingState()
        {
            return _failedTaskBlocking;
        }

        /// <summary>
        /// Terminates the current task by invoking it's failed condition.
        /// </summary>
        public void Terminate()
        {
            System.out.println("Task terminated!!!!!!!!!!!!!!!!!!!!!!!");
            _isTerminated = true;
            if (_taskType == AutomatedTaskFramework.TaskType.NetworkTask)
            {
                if (_failedTaskBlocking == FailedTaskBlocking.Block)
                {
                    if (_taskFailedAction != null)
                    {
                        _taskFailedAction.run();
                    }
                    Tasks.GetBlockingTaskFailedAction().run();
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
        /// Checks whether the task has been terminated.
        /// </summary>
        public boolean IsTerminated()
        {
            return _isTerminated;
        }

        /// <summary>
        /// The function or method that is linked to the task.
        /// </summary>
        public Runnable GetTaskAction()
        {
            return _automatedAction;
        }
        /// <summary>
        /// The action to be executed when the task is terminated or fails.
        /// </summary>
        public Runnable GetTaskFailedAction()
        {
            return _taskFailedAction;
        }
        /// <summary>
        /// The SearchCondition that is used to check for the FinishedCondition in the provided data set.
        /// </summary>
        public SearchCondition GetSearchCondition()
        {
            return _searchCondition;
        }
        /// <summary>
        /// The condition that has to be met to consider the task completed.
        /// </summary>
        public String GetFinishedCondition()
        {
            return _automatedTaskCondition;
        }
        /// <summary>
        /// The condition that has to be met to consider the task failed.
        /// </summary>
        public String GetFailedCondition()
        {
            return _failedCondition;
        }

        /// <summary>
        /// Gets the TaskType of the task.
        /// </summary>
        public TaskType GetTaskType()
        {
            return _taskType;
        }

        /// <summary>
        /// Checks if the task is finished. Will always be true if the TaskType is not Interactive.
        /// </summary>
        /// <returns></returns>
        public boolean IsFinished()
        {
            try {
                return _funcFinishedCondition.call();
            } catch (Exception e) {
                e.printStackTrace();
                this.Terminate();
                return false;
            }
        }

        /// <summary>
        /// Checks if the task is failed.
        /// </summary>
        /// <returns></returns>
        public boolean IsFailed()
        {
            try {
                return _funcFailedCondition.call();
            } catch (Exception e) {
                e.printStackTrace();
                this.Terminate();
                return true;
            }
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
                case FireAndForget:
                    GetTaskAction().run();
                    Delete();
                    if (Tasks.Available())
                    {
                        Tasks.GetCurrent().Run();
                    }
                    break;

                case Interactive:
                    new Thread(new Runnable(){
                        @Override
                        public void run()
                        {
                            while (!IsFinished() && !IsFailed() && !IsTerminated())
                            {
                                try {
                                    Thread.sleep(100);
                                } catch (InterruptedException e) {
                                    e.printStackTrace();
                                    Terminate();
                                    break;
                                }
                            }
                            if (IsFailed() || IsTerminated())
                            {
                                if (GetBlockingState() == FailedTaskBlocking.Block)
                                {
                                    if (GetTaskFailedAction() != null) {
                                        GetTaskFailedAction().run();
                                    }
                                    Tasks.GetBlockingTaskFailedAction().run();
                                    return;
                                }
                            }
                            Delete();
                            if (Tasks.Available())
                            {
                                Tasks.GetCurrent().Run();
                            }
                        }

                    }).start();
                    try
                    {
                        GetTaskAction().run();
                    }
                    catch (Exception e)
                    {
                        Terminate();
                    }
                    break;

                default:
                    try
                    {
                        GetTaskAction().run();
                    }
                    catch (Exception e)
                    {
                        Terminate();
                    }
                    break;
            }
        }
    }
}
