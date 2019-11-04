package com.example.rodaues.pmdbs_navdraw;

import android.app.ActivityManager;
import android.content.Context;
import android.content.Intent;
import android.os.Handler;
import android.os.Looper;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RelativeLayout;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.List;
import java.util.concurrent.Callable;

public class loginonline extends AppCompatActivity {

    Button btn_onlinelogin;
    EditText et_online_ip, et_online_port, et_online_muname, et_online_password;
    public static RelativeLayout rl_loading_onlinelogin;
    DataBaseHelper db = DataBaseHelper.GetInstance();
    private Context context;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        setTheme(R.style.AppTheme_NoActionBar);

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_loginonline);

        context = this;

        globalVARpool.aca_loginonline = this;

        btn_onlinelogin = findViewById(R.id.btn_onlinelogin);
        et_online_ip = findViewById(R.id.et_onlinelogin_ip);
        et_online_port = findViewById(R.id.et_onlinelogin_port);
        et_online_muname = findViewById(R.id.et_onlinelogin_masteruname);
        et_online_password = findViewById(R.id.et_onlinelogin_password);

        rl_loading_onlinelogin = findViewById(R.id.rlayout_onlinelogin_progressbar);


        btn_onlinelogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String ip = et_online_ip.getText().toString();
                String str_port = et_online_port.getText().toString();
                int int_port = 0;
                final String masteruname = et_online_muname.getText().toString();
                String password = et_online_password.getText().toString();
                Boolean isIP = false;

                globalVARpool.username = masteruname;


                if(ip.isEmpty()){
                    CustomToast.makeText(context, "Please enter an IP-address.");
                    return;
                }
                if(str_port.isEmpty()){
                    CustomToast.makeText(context, "Please enter a port.");
                    return;
                }
                int_port = Integer.parseInt(str_port);
                if(1>int_port || int_port>65536){
                    CustomToast.makeText(context, "Please enter a valid port number.");
                    return;
                }
                if(masteruname.isEmpty() || password.isEmpty()){
                    CustomToast.makeText(context, "Please enter your login information.");
                    return;
                }

                PDTPClient pdtp = PDTPClient.GetInstance();

                if(ip.matches("^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")){
                    isIP=true;
                    pdtp.setRemoteAddress(ip);
                }
                else{
                    try {
                        pdtp.setRemoteAddress(InetAddress.getByName(ip).getHostAddress());
                    } catch (UnknownHostException e) {
                        e.printStackTrace();
                    }
                }

                pdtp.setRemotePort(int_port);
                pdtp.setDebugging(true);

                globalVARpool.plainMasterPW = password;

                startLoading_onlinelogin();

                globalVARpool.loadingType = globalVARpool.LoadingType.loginonline;

                try {
                    final String pw_hash= CryptoHelper.SHA256(password);
                    globalVARpool.onlinePassword=CryptoHelper.SHA256(pw_hash.substring(0,31));

                    AutomatedTaskFramework.Tasks.Clear();


                    Runnable onTaskFailed = new Runnable() {
                        @Override
                        public void run() {
                            new Handler(Looper.getMainLooper()).post(new Runnable() {
                                @Override
                                public void run() {
                                    stopLoading_onlinelogin();

                                }
                            });
                        }
                    };

                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.Contains, "DEVICE_AUTHORIZED", new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.Connect();
                        }
                    },onTaskFailed);

                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.Login();
                        }
                    },onTaskFailed);

                    Runnable onTFAFailed = new Runnable() {
                        @Override
                        public void run() {
                            ActivityManager am = (ActivityManager) globalVARpool.application.getSystemService(ACTIVITY_SERVICE);
                            List<ActivityManager.RunningTaskInfo> taskInfo = am.getRunningTasks(1);

                                switch(taskInfo.get(0).topActivity.getShortClassName()){
                                    case ".tfa":
                                        Intent intent = new Intent(globalVARpool.applicationcontext,loginonline.class);
                                        intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                                        globalVARpool.application.startActivity(intent);
                                        globalVARpool.aca_tfa.finish();
                                        break;
                                    case ".loginonline":
                                        new Handler(Looper.getMainLooper()).post(new Runnable() {
                                            @Override
                                            public void run() {
                                                stopLoading_onlinelogin();

                                            }
                                        });
                                        break;
                                }
                        }
                    };
                    Callable<Boolean> syncfinishedwhen = new Callable<Boolean>(){
                        @Override
                        public Boolean call() {
                            PDTPClient pdtp = PDTPClient.GetInstance();

                            return pdtp.isConnected();
                        }
                    };

                    Runnable initializeDB = new Runnable() {
                        @Override
                        public void run() {

                            long unixTime = System.currentTimeMillis() / 1000L;
                            String firstUsage = String.valueOf(unixTime);
                            String ScryptedPW = CryptoHelper.getScryptString(pw_hash,firstUsage);

                            db.modifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"UPDATE Tbl_user SET U_password =\"",pw_hash,"\",U_wasOnline = 1, U_firstUsage = \"",firstUsage,"\",U_username =\"",masteruname,"\",U_cookie=\"",globalVARpool.cookie,"\",U_datetime = 0;"}));
                            globalVARpool.database_initialized=true;
                            globalVARpool.wasOnline=true;
                        }
                    };

                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.Interactive, initializeDB, new Callable<Boolean>() {
                        @Override
                        public Boolean call(){
                            return globalVARpool.database_initialized;
                        }
                    },onTFAFailed);

                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.In, "AD_OUTDATED|AD_UPTODATE", new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.GetAccountDetails();
                        }
                    },onTFAFailed);

                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.Interactive, new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.Sync();
                        }
                    },syncfinishedwhen,onTFAFailed);


                    Runnable finalize_login = new Runnable() {
                        @Override
                        public void run() {
                            startActivity(new Intent(globalVARpool.applicationcontext,Main.class));
                            finish();
                        }
                    };

                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.FireAndForget,finalize_login);

                    AutomatedTaskFramework.Tasks.Execute();

                    DataBaseHelper dbhelper = DataBaseHelper.GetInstance();
                    dbhelper.execSQL(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"UPDATE Tbl_settings SET S_server_ip = \"",ip,"\",S_server_port = \"",str_port,"\";"}));


                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        });
    }

    public static void startLoading_onlinelogin(){
        rl_loading_onlinelogin.setVisibility(View.VISIBLE);
    }
    public static void stopLoading_onlinelogin(){
        rl_loading_onlinelogin.setVisibility(View.GONE);
    }

}
