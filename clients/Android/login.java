package com.example.rodaues.pmdbs_navdraw;

import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.graphics.drawable.Drawable;
import android.os.Build;
import android.os.Bundle;
import android.os.Vibrator;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.biometric.BiometricPrompt;
import androidx.core.content.ContextCompat;

import android.security.keystore.KeyGenParameterSpec;
import android.security.keystore.KeyProperties;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Base64;
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
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.security.InvalidAlgorithmParameterException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.NoSuchProviderException;
import java.security.UnrecoverableEntryException;
import java.security.UnrecoverableKeyException;
import java.security.cert.CertificateException;
import java.util.Objects;
import java.util.concurrent.Executor;

import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;


public class login extends AppCompatActivity {

    EditText et_pw, et_masterpw1, et_masterpw2;
    ImageView greenhook;
    RelativeLayout rl_firstLogin, rl_secondLogin;
    Button btn_login, btn_register, btn_gotoREGISTER, btn_gotoONLINELOGIN, btn_resetaccount;

    private Context context;
    private TextView tv_test_grade;

    private Executor executor;
    private BiometricPrompt biometricPrompt;
    private BiometricPrompt.PromptInfo promptInfo;

    DataBaseHelper db;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        setTheme(R.style.AppTheme_NoActionBar);
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                WindowManager.LayoutParams.FLAG_FULLSCREEN);
        setContentView(R.layout.activity_login);


        //--------------------- SETUP -----------------------
        context = this;
        globalVARpool.applicationcontext=getApplicationContext();

        DataBaseHelper.createInstance(this);

        HelperMethods.setup(this);
        db = DataBaseHelper.GetInstance();
        //--------------------- SETUP -----------------------



        //--------------------- AUTOMATED TASK FRAMEWORK -----------------------
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
        //--------------------- AUTOMATED TASK FRAMEWORK -----------------------


        //INIT LAYOUT
        greenhook = (ImageView)findViewById(R.id.iv_greenhook);
        et_pw = (EditText) findViewById(R.id.et_login_pw);
        et_masterpw1 = (EditText) findViewById(R.id.et_login_masterpw1);
        et_masterpw2 = (EditText) findViewById(R.id.et_login_masterpw2);
        rl_firstLogin = (RelativeLayout) findViewById(R.id.rl_firstlogin);
        rl_secondLogin = (RelativeLayout) findViewById(R.id.rl_secondlogin);
        btn_login = (Button) findViewById(R.id.btn_login);
        btn_register = (Button) findViewById(R.id.btn_register);
        btn_gotoREGISTER = (Button) findViewById(R.id.btn_login2register);
        btn_gotoONLINELOGIN = findViewById(R.id.btn_go2onlinelogin);
        btn_resetaccount = findViewById(R.id.btn_resetaccount);
        tv_test_grade=findViewById(R.id.tv_test_grade);
        //INIT LAYOUT




       db.printDB();


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
                        //CustomToast.makeText(getApplicationContext(),String.valueOf(result.IsCompromised()));
                        db.deletedatauser();

                        String masterPW = et_masterpw1.getText().toString();
                        String hashedMasterPW = "";
                        try {
                            hashedMasterPW = CryptoHelper.SHA256(masterPW);
                            globalVARpool.hashedMasterPW=hashedMasterPW;

                        } catch (Exception e) {
                            e.printStackTrace();
                        }

                        try{
                            globalVARpool.onlinePassword=CryptoHelper.SHA256(hashedMasterPW.substring(0,32));
                            globalVARpool.AESkey = CryptoHelper.SHA256(hashedMasterPW.substring(32,64));
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



        //TEXTWATCHER FOR GETTING SCORE OF MASTER PASSWORD (REGISTER)
        et_masterpw1.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                Password.Result result = Password.Security.SimpleCheck(et_masterpw1.getText().toString());
                int score = result.Score();
                String str_score = result.Grade();
                Drawable drawable = et_masterpw1.getBackground();




                int grade_color = 0;


                if(score<110)             {
                    grade_color = Color.rgb(196,0,0);


                }
                else if(score>=110 && score<125)   {
                    grade_color = Color.rgb(227, 121, 0);

                }
                else if(score>=125 && score<140)   {
                    grade_color = Color.rgb(245, 184, 0);

                }
                else if(score>=140 && score<155)  {
                    grade_color = Color.rgb(255, 255, 0);

                }
                else if(score>=155 && score<170)   {
                    grade_color = Color.rgb(195, 255, 0);

                }
                else if(score>=170 && score<185)   {
                    grade_color = Color.rgb(162, 255, 0);

                }
                else if(score>=185 && score<1200)  {
                    grade_color = Color.rgb(123, 255, 0);
                }
                else                          {
                    drawable.setColorFilter(Color.rgb(0, 255, 34), PorterDuff.Mode.SRC_ATOP);
                }
                drawable.setColorFilter(grade_color, PorterDuff.Mode.SRC_ATOP);
                et_masterpw1.setBackground(drawable);
                tv_test_grade.setTextColor(grade_color);
                tv_test_grade.setText(str_score);

            }

            @Override
            public void afterTextChanged(Editable s) {


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

                        login_successful(hashedEntryPW);
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


        //----------------------------BIOMETRIC LOGIN--------------------START
        if(db.user_getBiometricInfo().get(0)!=null){
            executor = ContextCompat.getMainExecutor(this);
            biometricPrompt = new BiometricPrompt(this,
                    executor, new BiometricPrompt.AuthenticationCallback() {
                @Override
                public void onAuthenticationError(int errorCode,
                                                  @NonNull CharSequence errString) {
                    super.onAuthenticationError(errorCode, errString);

                    System.out.println("ERROR: "+errString);

                }

                @Override
                public void onAuthenticationSucceeded(
                        @NonNull BiometricPrompt.AuthenticationResult result) {
                    super.onAuthenticationSucceeded(result);
                    System.out.println("AUTHENTICATED");

                    try{
                        byte[] decryptedInfo = result.getCryptoObject().getCipher().doFinal(db.user_getBiometricInfo().get(0));

                        String masterPW_HASH = new String(decryptedInfo, "UTF-8");



                        String out_msg = db.user_getMasterPW() +"\n"+CryptoHelper.getScryptString(masterPW_HASH,db.user_getfUSAGE())+"\n"+masterPW_HASH;
                        System.out.println(out_msg);

                        if(db.user_getMasterPW().equals(CryptoHelper.getScryptString(masterPW_HASH,db.user_getfUSAGE()))){

                            //CustomToast.makeText(getApplicationContext(),"SUCCESS");
                            login_successful(masterPW_HASH);
                        }


                    }catch(Exception e){
                        e.printStackTrace();
                    }
                }

                @Override
                public void onAuthenticationFailed() {
                    super.onAuthenticationFailed();
                    System.out.println("ERROR: AUTHENTICATION FAILED");


                }
            });

            promptInfo = new BiometricPrompt.PromptInfo.Builder()
                    .setTitle("Biometric Login")
                    .setSubtitle("Log in using your biometric credential")
                    .setNegativeButtonText("Use Master Password")
                    .build();


        }else{
            System.out.println("BIOMETRICINFO is NULL");
        }
            //-------show biometric prompt
            try{
                Cipher cipher = getCipher();
                SecretKey secretKey = getSecretKey();
                cipher.init(Cipher.DECRYPT_MODE, secretKey,new IvParameterSpec(db.user_getBiometricInfo().get(1)));
                biometricPrompt.authenticate(promptInfo,
                        new BiometricPrompt.CryptoObject(cipher));
            }catch(Exception e){
                e.printStackTrace();
            }
            //-------show biometric prompt


        //----------------------------BIOMETRIC LOGIN--------------------END






        //----------------END ON CREATE!!!!
    }


    private void login_successful(String hashed_masterPW){

        hideKB();
        rl_secondLogin.setVisibility(View.GONE);
        greenhook.setVisibility(View.VISIBLE);
        try{
            globalVARpool.hashedMasterPW=hashed_masterPW;
            globalVARpool.onlinePassword=CryptoHelper.SHA256(hashed_masterPW.substring(0,32));
            globalVARpool.AESkey = CryptoHelper.SHA256(hashed_masterPW.substring(32,64));
        }
        catch(Exception e){
            e.printStackTrace();
        }

        startActivity(new Intent(login.this, Main.class));
        finish();


    }

    private void hideKB(){
        View view = this.getCurrentFocus();
        if (view != null) {
            InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
        }
    }

    private SecretKey getSecretKey() throws KeyStoreException, CertificateException, NoSuchAlgorithmException, IOException, UnrecoverableKeyException {
        KeyStore keyStore = KeyStore.getInstance("AndroidKeyStore");

        // Before the keystore can be accessed, it must be loaded.
        keyStore.load(null);
        return ((SecretKey)keyStore.getKey("PMDBS_KEY", null));
    }

    private Cipher getCipher() throws NoSuchPaddingException, NoSuchAlgorithmException {
        return Cipher.getInstance(KeyProperties.KEY_ALGORITHM_AES + "/"
                + KeyProperties.BLOCK_MODE_CBC + "/"
                + KeyProperties.ENCRYPTION_PADDING_PKCS7);
    }

    @Override
    public void onBackPressed() {
        finish();
    }
}
