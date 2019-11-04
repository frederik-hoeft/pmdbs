package com.example.rodaues.pmdbs_navdraw;

import android.animation.ArgbEvaluator;
import android.animation.ValueAnimator;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.os.Vibrator;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.RelativeLayout;

import java.io.File;
import java.io.UnsupportedEncodingException;
import java.security.NoSuchAlgorithmException;
import java.util.concurrent.Callable;

public class login extends AppCompatActivity {

    EditText et_pw, et_masterpw1, et_masterpw2;
    ImageView greenhook;
    RelativeLayout rl_firstLogin, rl_secondLogin;
    Button btn_login, btn_register, btn_gotoREGISTER, btn_gotoONLINELOGIN, btn_resetaccount;

    private Context context;

    DataBaseHelper db;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        setTheme(R.style.AppTheme_NoActionBar);
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                WindowManager.LayoutParams.FLAG_FULLSCREEN);
        setContentView(R.layout.activity_login);

        context = this;

        DataBaseHelper.createInstance(this);

        HelperMethods.setup(this);
        db = DataBaseHelper.GetInstance();

        AutomatedTaskFramework.Tasks.SetBlockingTaskFailedAction(new Runnable() {
            @Override
            public void run() {
                AutomatedTaskFramework.Tasks.Finalize();
                AutomatedTaskFramework.Tasks.Clear();
                PDTPClient pdtp = PDTPClient.GetInstance();
                if(pdtp.isLogged_in()){
                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.FireAndForget, new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.Logout();
                        }
                    });
                }
                if(pdtp.isConnected()){
                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.FireAndForget, new Runnable() {
                        @Override
                        public void run() {
                            try {
                                NetworkAdapter.MethodProvider.Disconnect();
                            } catch (Exception e) {
                                e.printStackTrace();
                            }
                        }
                    });
                }
                AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.FireAndForget, new Runnable() {
                    @Override
                    public void run() {
                    System.out.println("ERR -> AutomatedTaskFramework failed through failed condition");
                    }
                });
                AutomatedTaskFramework.Tasks.Execute();
            }
        });

     //   db.createDataBase();
      //  db.copyDataBase();

      //  File f = new File(getApplicationContext().getApplicationInfo().dataDir+"/databases/"+"localdata.db");
/*
        if(f.exists() && !f.isDirectory()) msg("DATABASE FILE EXISTS!");
        else msg("DATABASE FILE DOES NOT EXIST");
*/


        //Toast.makeText(getApplicationContext(),db.special_execSQL("SELECT name FROM sqlite_master WHERE type='table';"),Toast.LENGTH_LONG).show();
        //db.copyDataBase();

        /*    try {
            // clearing app data
            String packageName = getApplicationContext().getPackageName();
            Runtime runtime = Runtime.getRuntime();
            runtime.exec("pm clear "+packageName);

        } catch (Exception e) {
            e.printStackTrace();
        }*/



        //INIT LAYOUT
        greenhook = (ImageView)findViewById(R.id.iv_greenhook);
        et_pw = (EditText) findViewById(R.id.et_login_pw);
        et_masterpw1 = (EditText) findViewById(R.id.et_login_masterpw1);
        et_masterpw2 = (EditText) findViewById(R.id.et_login_masterpw2);
        rl_firstLogin = (RelativeLayout) findViewById(R.id.rl_firstlogin);
        rl_secondLogin = (RelativeLayout) findViewById(R.id.rl_secondlogin);
        btn_login = (Button) findViewById(R.id.btn_login);
        btn_register = (Button) findViewById(R.id.btn_register);
        //INIT LAYOUT
        btn_gotoREGISTER = (Button) findViewById(R.id.btn_login2register);

        btn_gotoONLINELOGIN = findViewById(R.id.btn_go2onlinelogin);
        btn_resetaccount = findViewById(R.id.btn_resetaccount);



        rl_secondLogin.setVisibility(View.VISIBLE);
        rl_firstLogin.setVisibility(View.GONE);


        if(db.user_getfUSAGE().equals("0")){
            rl_secondLogin.setVisibility(View.GONE);
            rl_firstLogin.setVisibility(View.VISIBLE);
           // msg("Welcome to PMDBS!");
        }


        btn_gotoREGISTER.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v) {
                rl_secondLogin.setVisibility(View.GONE);
                rl_firstLogin.setVisibility(View.VISIBLE);
            }
        });

        btn_gotoONLINELOGIN.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                startActivity(new Intent(login.this, loginonline.class));
                finish();
            }
        });

/*
        System.out.println("System-Output: "+db.str_list_execSQL("SELECT * FROM Tbl_data;").toString());
        */


        //----------------------------FIRST LOGIN--------------------START
        btn_register.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {


                Boolean safePW = true;

                if(et_masterpw1.getText().toString().equals(et_masterpw2.getText().toString())){

                    if(safePW){

                        Password.Result result = Password.Security.Check(et_masterpw1.getText().toString());
                        CustomToast.makeText(getApplicationContext(),String.valueOf(result.IsCompromised()));
                        db.deletedatauser();

                        String masterPW = et_masterpw1.getText().toString();
                        String hashedMasterPW = "";
                        try {
                            hashedMasterPW = CryptoHelper.SHA256(masterPW);
                        } catch (Exception e) {
                            e.printStackTrace();
                        }

                        try{
                            globalVARpool.onlinePassword=CryptoHelper.SHA256(hashedMasterPW.substring(0,31));
                            globalVARpool.AESkey = CryptoHelper.SHA256(hashedMasterPW.substring(32,63));
                        }
                        catch(Exception e){
                            e.printStackTrace();
                        }


                        long unixTime = System.currentTimeMillis() / 1000L;
                        String firstUsage = String.valueOf(unixTime);
                        db.user_setfUSAGE(firstUsage);
                        String ScryptedPW = CryptoHelper.getScryptString(hashedMasterPW,firstUsage);

                        db.user_setpassword(ScryptedPW);


                        startActivity(new Intent(login.this, Main.class));
                        finish();
                    }
                }
                else CustomToast.makeText(context,"NOT EQUAL!");



            }
        });

        //----------------------------FIRST LOGIN--------------------END

        //----------------------------SECOND LOGIN--------------------START

        btn_login.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {


                String firstUSAGE = db.user_getfUSAGE();



                String entryPW = et_pw.getText().toString();
                String currentMasterPW = db.user_getMasterPW();
                String hashedEntryPW = "";
                try {
                    hashedEntryPW = CryptoHelper.SHA256(entryPW);
                } catch (Exception e) {
                    e.printStackTrace();
                }


                if(currentMasterPW!=null){
                    if(currentMasterPW.equals(CryptoHelper.getScryptString(hashedEntryPW,firstUSAGE))){

                        hideKB();
                        rl_secondLogin.setVisibility(View.GONE);
                        greenhook.setVisibility(View.VISIBLE);
                        try{
                            globalVARpool.onlinePassword=CryptoHelper.SHA256(hashedEntryPW.substring(0,31));
                            globalVARpool.AESkey = CryptoHelper.SHA256(hashedEntryPW.substring(32,63));
                            System.out.println("!!!!!!!!!!!!!!!! Sec_Login/aeskey -> "+globalVARpool.AESkey);
                        }
                        catch(Exception e){
                            e.printStackTrace();
                        }

                        //HelperMethods.changeLocalMasterPW(entryPW);
                        startActivity(new Intent(login.this, Main.class));
                        finish();
                    }
                    else{

                        Animation shake = AnimationUtils.loadAnimation(context, R.anim.shake);
                        et_pw.startAnimation(shake);
                        Vibrator vibrate= (Vibrator)getSystemService(Context.VIBRATOR_SERVICE) ;
                        vibrate.vibrate(500);
                    }
                }


            }
        });

        //----------------------------SECOND LOGIN--------------------END


        et_masterpw1.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                Password.Result result = Password.Security.SimpleCheck(et_masterpw1.getText().toString());
                int score = result.Score();
                Drawable drawable = et_masterpw1.getBackground();


                if(score<110)                       drawable.setColorFilter(Color.rgb(196,0,0), PorterDuff.Mode.SRC_ATOP);
                else if(score>=110 && score<125)    drawable.setColorFilter(Color.rgb(227, 121, 0), PorterDuff.Mode.SRC_ATOP);
                else if(score>=125 && score<140)    drawable.setColorFilter(Color.rgb(245, 184, 0), PorterDuff.Mode.SRC_ATOP);
                else if(score>=140 && score<155)    drawable.setColorFilter(Color.rgb(255, 255, 0), PorterDuff.Mode.SRC_ATOP);
                else if(score>=155 && score<170)    drawable.setColorFilter(Color.rgb(195, 255, 0), PorterDuff.Mode.SRC_ATOP);
                else if(score>=170 && score<185)    drawable.setColorFilter(Color.rgb(162, 255, 0), PorterDuff.Mode.SRC_ATOP);
                else if(score>=185 && score<1200)   drawable.setColorFilter(Color.rgb(123, 255, 0), PorterDuff.Mode.SRC_ATOP);
                else                                drawable.setColorFilter(Color.rgb(0, 255, 34), PorterDuff.Mode.SRC_ATOP);

                et_masterpw1.setBackground(drawable);
            }

            @Override
            public void afterTextChanged(Editable s) {


            }
        });


        btn_resetaccount.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

            }
        });

    }

    private void hideKB(){
        View view = this.getCurrentFocus();
        if (view != null) {
            InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
        }
    }

    @Override
    public void onBackPressed() {
        finish();
    }
}
