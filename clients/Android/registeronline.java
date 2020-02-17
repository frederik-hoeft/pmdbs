package com.example.rodaues.pmdbs_navdraw;

import android.content.Context;

import androidx.appcompat.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RelativeLayout;

import java.net.InetAddress;
import java.util.concurrent.Callable;
import java.util.regex.Pattern;

public class registeronline extends AppCompatActivity {


    Button btn_registeronline;
    EditText et_registeronline_ip, et_registeronline_port, et_registeronline_muname, et_registeronline_nickname, et_registeronline_email;
    PDTPClient pdtp;
    DataBaseHelper dbhelper;
    public static RelativeLayout rl_loading_registeronline;
    private Context context;

    private static final Pattern VALID_EMAIL_ADDRESS_REGEX =
            Pattern.compile("^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,6}$", Pattern.CASE_INSENSITIVE);
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        setTheme(R.style.AppTheme_NoActionBar);

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_registeronline);

        context = this;

        globalVARpool.aca_registeronline = this;
        pdtp = PDTPClient.GetInstance();
        dbhelper = DataBaseHelper.GetInstance();

        String onlinepw = "";
        if(getIntent().hasExtra("onlinepw")) onlinepw = getIntent().getStringExtra("onlinepw");
       // Toast.makeText(getApplicationContext(),onlinepw,Toast.LENGTH_LONG).show();


        btn_registeronline = findViewById(R.id.btn_onlineregister);
        et_registeronline_ip = findViewById(R.id.et_onlineregister_ip);
        et_registeronline_port = findViewById(R.id.et_onlineregister_port);
        et_registeronline_muname = findViewById(R.id.et_onlineregister_masteruname);
        et_registeronline_email = findViewById(R.id.et_onlineregister_email);
        et_registeronline_nickname = findViewById(R.id.et_onlineregister_nickname);

        rl_loading_registeronline = findViewById(R.id.rlayout_registeronline_progressbar);

        btn_registeronline.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                globalVARpool.application = getApplication();


                String ip = et_registeronline_ip.getText().toString();
                String port = et_registeronline_port.getText().toString();
                String muname = et_registeronline_muname.getText().toString();
                String email = et_registeronline_email.getText().toString();
                String nickname = et_registeronline_nickname.getText().toString();
                String onlinePassword = globalVARpool.onlinePassword;

                Boolean isIP = false;

                if(nickname.isEmpty()){
                    nickname="User";
                }
                if(ip.isEmpty()){
                    CustomToast.makeText(context, "Please enter an IP-address.");
                    return;

                }
                if(port.isEmpty()){
                    CustomToast.makeText(context, "Please enter a port.");
                     return;
                }
                if(muname.isEmpty()){
                    CustomToast.makeText(context, "You must set a masterusername.");
                    return;
                }
                String[] unameemailnickname= new String[]{muname,email,nickname};
                String[] forbiddenChars = new String[]{" ","'","\""};

                for (int i=0;i<unameemailnickname.length;i++) {
                    for(int j=0;j<forbiddenChars.length;j++){

                        if(unameemailnickname[i].contains(forbiddenChars[j])) {
                            switch(i) {
                                case 0:
                                    CustomToast.makeText(context, "There is at least one forbidden character in your masterusername!");
                                    break;
                                case 1:
                                    CustomToast.makeText(context, "There is at least one forbidden character in your email address!");
                                    break;
                                case 2:
                                    CustomToast.makeText(context, "There is at least one forbidden character in your nickname!");
                                    break;
                            }

                            return;
                        }

                    }
                }

                if(muname.contains("__")){
                    CustomToast.makeText(context, "Your masterusername must not contain two underscores!");
                    return;
                }

                if(email.isEmpty()){
                    //Toast.makeText(context, "Please enter an email address.",Toast.LENGTH_LONG).show();
                    CustomToast.makeText(context,"Please enter an email address.");
                    return;
                }


                if (!VALID_EMAIL_ADDRESS_REGEX.matcher(email).find())
                {
                    CustomToast.makeText(context, "Please enter a valid email address.");
                    return;
                }


                globalVARpool.nickname = nickname;
                globalVARpool.username = muname;
                globalVARpool.email = email;

                int int_port = Integer.parseInt(port);
                if(int_port <1 || int_port > 65536){
                    CustomToast.makeText(context, "Please enter a valid port number.");
                    return;
                }


                if(ip.matches("^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")){
                    isIP=true;
                    pdtp.setRemoteAddress(ip);
                }




                try{
                    if(!isIP){
                        pdtp.setRemoteAddress(InetAddress.getByName(ip).getHostAddress());
                    }

                    pdtp.setRemotePort(int_port);


                    Callable finishedCondition = new Callable() { public Boolean call() {
                       return pdtp.isLogged_in();
                    }};

                    startLoading_register();


                   /* Object[] parameters = new Object[]{finishedCondition,loadingtype,globalVARpool.aca_registeronline};

                    Object parameter = parameters;
                    LoadingThreadHelper loadingThreadHelper = LoadingThreadHelper.GetInstance(parameter);
                    Thread t = new Thread(loadingThreadHelper);
                    t.start();*/

                    AutomatedTaskFramework.Tasks.Clear();

                    Runnable onTaskFailed = new Runnable() {
                        @Override
                        public void run() {
                            stopLoading_register();
                            CustomToast.invokeMakeText(context, "Registering failed.");
                        }
                    };

                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.Contains, "DEVICE_AUTHORIZED", new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.Connect();
                        }
                    },onTaskFailed);

                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.Contains, "SEND_VERIFICATION_ACTIVATE_ACCOUNT", new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.Register();
                        }
                    },onTaskFailed);

                    AutomatedTaskFramework.Tasks.Execute();

                    dbhelper.execSQL(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"UPDATE Tbl_settings SET S_server_ip = \"",pdtp.getRemoteAddress(),"\",S_server_port = \"",String.valueOf(pdtp.getRemotePort()),"\";"}));



                }catch(Exception e){
                    e.printStackTrace();
                }


            }
        });

    }


    public static void startLoading_register(){
        rl_loading_registeronline.setVisibility(View.VISIBLE);

    }
    public static void stopLoading_register(){
        rl_loading_registeronline.setVisibility(View.GONE);
    }




}
