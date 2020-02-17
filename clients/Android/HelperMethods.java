package com.example.rodaues.pmdbs_navdraw;

import android.app.ActivityManager;
import android.content.Context;
import android.os.Build;
import android.os.Environment;
import android.os.StatFs;
import android.os.StrictMode;
import android.provider.Settings;
import androidx.appcompat.app.AppCompatActivity;

import org.json.JSONException;
import org.json.JSONObject;

import java.lang.reflect.Field;
import java.util.List;

public class HelperMethods
{
    public static String GetOS()
    {
        JSONObject OS_json = new JSONObject();
        try {

            OS_json.put("Name","("+Build.MANUFACTURER+") "+ Build.BRAND.substring(0,1).toUpperCase() + Build.BRAND.substring(1)+" "+Build.MODEL);
            OS_json.put("Architecture",System.getProperty("os.arch"));
            OS_json.put("Edition","Android "+Build.VERSION.RELEASE);

             StringBuilder builder = new StringBuilder();

        Field[] fields = Build.VERSION_CODES.class.getFields();
        for (Field field : fields) {
            String fieldName = field.getName();
            int fieldValue = -1;

            try {
                fieldValue = field.getInt(new Object());
            } catch (IllegalArgumentException e) {
                e.printStackTrace();
            } catch (IllegalAccessException e) {
                e.printStackTrace();
            } catch (NullPointerException e) {
                e.printStackTrace();
            }

            if (fieldValue == Build.VERSION.SDK_INT) {
                builder.append(fieldName);
                break;
            }
        }

            OS_json.put("ServicePack",  builder.toString());
            OS_json.put("Version",Build.VERSION.INCREMENTAL);
            OS_json.put("UserName",Build.VERSION.SDK);
            OS_json.put("DeviceName",Settings.Secure.getString( globalVARpool.applicationcontext.getContentResolver(), "bluetooth_name")); StatFs stat = new StatFs(Environment.getExternalStorageDirectory().getPath());
            long storagesize = (long)stat.getBlockSize()*(long)stat.getBlockCount();
            OS_json.put("Processor",String.valueOf(storagesize));
            ActivityManager.MemoryInfo memINFO = new ActivityManager.MemoryInfo();

            ((ActivityManager)globalVARpool.applicationcontext.getSystemService(Context.ACTIVITY_SERVICE)).getMemoryInfo(memINFO);
            
            OS_json.put("PhysicalMemory",String.valueOf(memINFO.totalMem));



        } catch (JSONException e) {
            e.printStackTrace();
        }
        

        String output_json = OS_json.toString();

        return output_json.replace("\"","§").replace("\'","§");
    }

    public static void changeLocalMasterPW(String plainMasterPW){

        DataBaseHelper dbhelper = DataBaseHelper.GetInstance();
        String newhashedPW = "";
        try {
            newhashedPW = CryptoHelper.SHA256(plainMasterPW);
        } catch (Exception e) {
            e.printStackTrace();
        }

        try{ globalVARpool.onlinePassword=CryptoHelper.SHA256(newhashedPW.substring(0,32));
            globalVARpool.AESkey = CryptoHelper.SHA256(newhashedPW.substring(32,64));
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