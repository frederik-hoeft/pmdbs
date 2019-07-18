package pmdbs;

public class PDTPClientThreadHelper implements Runnable
{
	// --------------SINGLETON DESIGN PATTERN START
	private static PDTPClientThreadHelper client = null;
	
	private PDTPClientThreadHelper() {}
	
	public static PDTPClientThreadHelper GetInstance() 
	{
		if(client == null) 
		{
			client = new PDTPClientThreadHelper();
		}
	    return client;
	}
	// --------------SINGLETON DESIGN PATTERN END
    public void run()
    {
        PDTPClient client = PDTPClient.GetInstance();
		try {
			client.Connect();
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

    }
}
