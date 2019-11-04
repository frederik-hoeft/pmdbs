package com.example.rodaues.pmdbs_navdraw;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RelativeLayout;
import android.widget.TextView;

import java.util.List;
import java.util.concurrent.Callable;


public class tfa extends AppCompatActivity {
    Button btn_confirm;

    public static RelativeLayout rl_loading_tfa;
    TextView tv_tfa_title, tv_tfa_subtitle, tv_tfa_error;
    String str_subtitle=" An email containing a verification code has been sent to ";

    EditText et_tfa_code;
    PDTPClient pdtp;


    Loading.LoadingType loadingtype;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        setTheme(R.style.AppTheme_NoActionBar);
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tfa);

        globalVARpool.aca_tfa = this;

        rl_loading_tfa = findViewById(R.id.rlayout_tfa_progressbar);
        tv_tfa_title = findViewById(R.id.tv_tfa_title);
                tv_tfa_subtitle = findViewById(R.id.tv_tfa_subtitle);
        tv_tfa_error=findViewById(R.id.tv_tfa_error);
        et_tfa_code=findViewById(R.id.et_input2FAcode);

        btn_confirm = findViewById(R.id.btn_confirm);

        pdtp = PDTPClient.GetInstance();

        loadingtype = (Loading.LoadingType) getIntent().getExtras().get("loadingtype");
        switch(loadingtype){
            case REGISTER:
                tv_tfa_title.setText("Activate your account");
                tv_tfa_subtitle.setText("Please verify you email address."+str_subtitle+globalVARpool.email+".");
                break;
            case LOGIN:
                tv_tfa_title.setText("Confirm new device");
                tv_tfa_subtitle.setText("Looks like you're trying to login from a new device."+str_subtitle+globalVARpool.email+".");
                break;
        }

        btn_confirm.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(et_tfa_code.getText().toString().isEmpty() || et_tfa_code.getText().length()<6){
                    tv_tfa_error.setVisibility(View.VISIBLE);
                    tv_tfa_error.setText("ENTER 6 DIGIT CODE!");
                    return;
                }

                List<AutomatedTaskFramework.Task> scheduledTasks = AutomatedTaskFramework.Tasks.DeepCopy();
                AutomatedTaskFramework.Tasks.Clear();
                Runnable onTaskFailed = new Runnable() {
                    @Override
                    public void run() {
                        stopLoading_tfa();
                        //TODO DISPLAY ERROR MES
                    }
                };

                if(!pdtp.isConnected()){
                    //TODO DISPLAY ERRORMSG DIALOG!

                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.Contains, "DEVICE_AUTHORIZED", new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.Connect();
                        }
                    },onTaskFailed);
                }





                switch(loadingtype){
                    case REGISTER:

                        AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.Contains, "ACCOUNT_VERIFIED", new Runnable() {
                            @Override
                            public void run() {
                                NetworkAdapter.MethodProvider.ActivateAccount(et_tfa_code.getText().toString());
                            }
                        },onTaskFailed);

                        AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", new Runnable() {
                            @Override
                            public void run() {
                                NetworkAdapter.MethodProvider.Login();
                            }
                        },onTaskFailed);

                        Callable<Boolean> syncfinishedwhen = new Callable<Boolean>(){
                            @Override
                            public Boolean call() {
                                PDTPClient pdtp = PDTPClient.GetInstance();

                                return pdtp.isConnected();
                            }
                        };
                        AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.Interactive, new Runnable() {
                            @Override
                            public void run() {
                                NetworkAdapter.MethodProvider.Sync();
                            }
                        },syncfinishedwhen,new Runnable() {
                            @Override
                            public void run() {
                                startActivity(new Intent(tfa.this,registeronline.class));
                                finish();
                            }});

                        AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.FireAndForget, new Runnable() {
                            @Override
                            public void run() {
                                startActivity(new Intent(tfa.this,Main.class));
                                finish();
                            }
                        });
                        break;
                    case LOGIN:

                        AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.Contains, "LOGIN_SUCCESSFUL", new Runnable() {
                            @Override
                            public void run() {
                                NetworkAdapter.MethodProvider.ConfirmNewDevice(et_tfa_code.getText().toString());
                            }
                        },onTaskFailed);

                        for(int i=1;i<scheduledTasks.size();i++){
                            AutomatedTaskFramework.Tasks.Schedule(scheduledTasks.get(i));
                        }
                        break;
                }
                try {
                    AutomatedTaskFramework.Tasks.Finalize();
                    AutomatedTaskFramework.Tasks.Execute();
                } catch (Exception e) {
                    e.printStackTrace();
                }
                startLoading_tfa();

            }
        });


        et_tfa_code.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

                if(tv_tfa_error.getVisibility()==View.VISIBLE) tv_tfa_error.setVisibility(View.INVISIBLE);
            }

            @Override
            public void afterTextChanged(Editable s) {

            }
        });

    }




    public static void startLoading_tfa(){
        rl_loading_tfa.setVisibility(View.VISIBLE);

    }
    public static void stopLoading_tfa(){
        rl_loading_tfa.setVisibility(View.GONE);
    }
    public void onBackPressed(){

    }



}
