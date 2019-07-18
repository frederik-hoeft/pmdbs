package pmdbs;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Callable;

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
    /// Keeps the tasks up to date by checking if their Failed / Finish conditions are met in a specific data set. Automatically starts the next task in queue if the previous task finished.
    /// </summary>
    /// <param name="data">The data set to check in for finish / failed conditions.</param>
    public static void DoTasks(String data) throws Exception
    {
        if (Tasks.Available())
        {
            Task currentTask = Tasks.GetCurrent();
            int failedCount = 0;
            if (currentTask.getFailedCondition().contains("|"))
            {
            	String[] failedConditions = currentTask.getFailedCondition().split("|");
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
            	if (data.contains(currentTask.getFailedCondition()))
            	{
            		failedCount++;
            	}
            }
            
            if (failedCount == 0)
            {
                if (currentTask.getSearchCondition() == SearchCondition.Match)
                {
                    if (data.equals(currentTask.getFinishedCondition()))
                    {
                        currentTask.Delete();

                        if (Tasks.Available())
                        {
                            Tasks.GetCurrent().Run();
                        }
                    }
                }
                else if (currentTask.getSearchCondition() == SearchCondition.In)
                {
                	int finishCount = 0;
                	String[] finishedConditions = currentTask.getFinishedCondition().split("|");
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
                    if (data.contains(currentTask.getFinishedCondition()))
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
	/// Defines how conditions should be searched for in the data set.
	/// </summary>
	public static enum SearchCondition
	{
	    /// <summary>
	    /// The data set has to match the provided string exactly.
	    /// </summary>
	    Match,
	    /// <summary>
	    /// The data set has to contain the provided string.
	    /// </summary>
	    Contains,
	    /// <summary>
	    /// The data set has to contain one or more of the provided search terms seperated by the | character.
	    /// </summary>
	    In
	}
	
	// --------------SINGLETON DESIGN PATTERN END
	/// <summary>
    /// Tasks maintains a list of all currently active tasks and provides basic task management such as scheduling, executing and cancelling.
    /// </summary>
    public static class Tasks
    {
        private static final List<Task> taskList = new ArrayList<Task>();
        /// <summary>
        /// Gets the next scheduled task
        /// </summary>
        /// <returns>Task object</returns>
        public static Task GetCurrent()
        {
            return taskList.get(0);
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
            return taskList.size() > 0;
        }
        /// <summary>
        /// Executes the next scheduled task
        /// </summary>
        public static void Execute() throws Exception
        {
            GetCurrent().Run();
        }
        /// <summary>
        /// Schedules a new task to be executed by the ATS
        /// </summary>
        /// <param name="task"></param>
        public static void Add(Task task)
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
        /// <returns>List of all scheduleds tasks</returns>
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
    }
    
    public static class TaskFactory
    {
    	/// <summary>
        /// Creates a new Task object.
        /// </summary>
        /// <param name="SearchCondition">The SearchCondition that is used to check for the FinishedCondition in the provided data set.</param>
        /// <param name="FinishedCondition">The condition that has to be met to consider the task completed.</param>
        /// <param name="TaskAction">The function or method that is linked to the task.</param>
        /// <returns>Returns the created Task object.</returns>
        public static Task Create(SearchCondition SearchCondition, String FinishedCondition, Callable<?> TaskAction)
        {
        	AutomatedTaskFramework atf = AutomatedTaskFramework.GetInstance();
            Task task = atf.new Task(SearchCondition, FinishedCondition, TaskAction);
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
        public static Task Create(SearchCondition SearchCondition, String FinishedCondition, Callable<?> TaskAction, String FailedCondition)
        {
        	AutomatedTaskFramework atf = AutomatedTaskFramework.GetInstance();
            Task task = atf.new Task(SearchCondition, FinishedCondition, TaskAction, FailedCondition);
            Tasks.Add(task);
            return task;
        }
    }
    
    /// <summary>
    /// The task object allows scheduling a specific action/function/method to be executed.
    /// </summary>
    public class Task
    {
        private Callable<?> _automatedAction = null;
        private String _automatedTaskCondition = "";
        private String _failedCondition = "SIG_TASK_FAILED";
        private SearchCondition _searchCondition = pmdbs.AutomatedTaskFramework.SearchCondition.Match;
        /// <summary>
        /// Task constructor
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <param name="FinishedCondition"></param>
        /// <param name="TaskAction"></param>
        public Task(SearchCondition SearchCondition, String FinishedCondition, Callable<?> TaskAction)
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
        public Task(SearchCondition SearchCondition, String FinishedCondition, Callable<?> TaskAction, String FailedCondition)
        {
            _automatedAction = TaskAction;
            _automatedTaskCondition = FinishedCondition;
            _searchCondition = SearchCondition;
            _failedCondition = FailedCondition;
        }
        /// <summary>
        /// The function or method that is linked to the task.
        /// </summary>
        public Callable<?> getTaskAction()
        {
            return _automatedAction;
        }
        
        /// <summary>
        /// The SearchCondition that is used to check for the FinishedCondition in the provided data set.
        /// </summary>
        public SearchCondition getSearchCondition()
        {
            return _searchCondition;
        }
        /// <summary>
        /// The condition that has to be met to consider the task completed.
        /// </summary>
        public String getFinishedCondition()
        {
            return _automatedTaskCondition;
        }
        /// <summary>
        /// The condition that has to be met to consider the task failed.
        /// </summary>
        public String getFailedCondition()
        {
            return _failedCondition;
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
        public void Run() throws Exception
        {
            _automatedAction.call();
        }
    }
}
