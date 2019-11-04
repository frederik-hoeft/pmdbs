package com.example.rodaues.pmdbs_navdraw;

public class SyncThreadHelper implements Runnable{
    // --------------SINGLETON DESIGN PATTERN START
    private static SyncThreadHelper sync = null;

    private SyncThreadHelper() {}

    private static String[] params = null;

    public static SyncThreadHelper GetInstance(String[] inputparams)
    {
        params = inputparams;
        if(sync == null)
        {
            sync = new SyncThreadHelper();
        }
        return sync;
    }
    // --------------SINGLETON DESIGN PATTERN END
    public void run()
    {
        Sync.initialize(params);
    }
}
