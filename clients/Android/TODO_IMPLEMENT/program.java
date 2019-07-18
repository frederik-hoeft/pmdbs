package pmdbs;

import java.io.File;
import java.io.FileInputStream;
import java.util.concurrent.Callable;

public class program 
{
	public static void main(String[] args) throws Exception 
	{
		try 
		{
			File file = new File("cookie.txt");
			FileInputStream fis = new FileInputStream(file);
			byte[] data = new byte[(int) file.length()];
			fis.read(data);
			fis.close();

			GlobalVarPool.cookie = new String(data, "UTF-8");
		}
		catch (Exception e) {}
		AutomatedTaskFramework.Tasks.Clear();
		AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.SearchCondition.Contains, "DEVICE_AUTHORIZED", new Callable<Object>() { public Integer call() { NetworkAdapter.MethodProvider.Connect(); return 0; }});
		AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.SearchCondition.Match, null, new Callable<Object>() { public Integer call() throws Exception { NetworkAdapter.MethodProvider.Disconnect(); return 0; }});
		AutomatedTaskFramework.Tasks.Execute();
	}
}
