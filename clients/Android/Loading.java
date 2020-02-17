package com.example.rodaues.pmdbs_navdraw;


public class Loading {


    // --------------SINGLETON DESIGN PATTERN START
    private static Loading loading = null;

    private Loading() {
    }

    public static Loading GetInstance()
    {
        if(loading == null)
        {
            loading = new Loading();
        }
        return loading;
    }
    // --------------SINGLETON DESIGN PATTERN END


    public enum LoadingType{
        DEFAULT,LOGIN,REGISTER,PASSWORD_CHANGE,VERIFY_PASSWORD_CHANGE
    }
}
