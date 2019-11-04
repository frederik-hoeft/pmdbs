package com.example.rodaues.pmdbs_navdraw;

import android.os.Build;
import android.os.StrictMode;
import android.support.v7.app.AppCompatActivity;

import java.util.List;

public class HelperMethods
{
    public static String GetOS()
    {
        //TODO return JSON-OSdata
        String system = Build.MODEL;
        return system;
    }

    public static void changeLocalMasterPW(String plainMasterPW){

        DataBaseHelper dbhelper = DataBaseHelper.GetInstance();
        String newhashedPW = "";
        try {
            newhashedPW = CryptoHelper.SHA256(plainMasterPW);
        } catch (Exception e) {
            e.printStackTrace();
        }

        try{
            globalVARpool.onlinePassword=CryptoHelper.SHA256(newhashedPW.substring(0,31));
            globalVARpool.AESkey = CryptoHelper.SHA256(newhashedPW.substring(32,63));
        }catch(Exception e){
            e.printStackTrace();
        }

        dbhelper.data_updateTBL();

        System.out.println("!!!!!!!!!!!!!!!! changeLocalMPW/aeskey -> "+globalVARpool.AESkey);

        String newScryptedPW = CryptoHelper.getScryptString(newhashedPW,globalVARpool.firstUsage);
        dbhelper.user_setpassword(newScryptedPW);

    }

    public static void setup(AppCompatActivity aca){
        globalVARpool.applicationcontext = aca.getApplicationContext();
        globalVARpool.application = aca.getApplication();

        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
        StrictMode.setThreadPolicy(policy);

        DataBaseHelper db = DataBaseHelper.GetInstance();

        List<String> user_settings = db.getSingleEntryAsList("SELECT * FROM Tbl_user LIMIT 1;");

        if(user_settings.size()==0) {
           // db.modifyData("INSERT INTO Tbl_user(U_wasOnline,U_firstUsage) VALUES(0,\"0\");");
            db.user_setfUSAGE("0");
            return;
        }

        globalVARpool.username = user_settings.get(1);
        globalVARpool.nickname = user_settings.get(2);
        globalVARpool.wasOnline = user_settings.get(4).equals("1");
        globalVARpool.firstUsage = user_settings.get(5);
        globalVARpool.email = user_settings.get(6);
        globalVARpool.cookie = user_settings.get(7);

        if(globalVARpool.wasOnline){
            List<String> temp_server = db.getSingleEntryAsList("SELECT * FROM Tbl_settings LIMIT 1;");

            System.out.println(temp_server.toString());
            PDTPClient pdtpClient = PDTPClient.GetInstance();
            pdtpClient.setRemoteAddress(temp_server.get(1));
            pdtpClient.setRemotePort(Integer.parseInt(temp_server.get(2)));

        }

    }
}