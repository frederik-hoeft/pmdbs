package com.rodaues.pmdbs_androidclient;

import android.content.Context;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.View;
import android.view.animation.Animation;
import android.view.animation.LinearInterpolator;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;

import java.io.UnsupportedEncodingException;
import java.security.NoSuchAlgorithmException;
import java.security.Security;

public class login extends AppCompatActivity {

    EditText et_pw1, et_masterpw1, et_masterpw2;
    ImageView greenhook;
    RelativeLayout rl_firstLogin;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        setTheme(R.style.AppTheme_NoActionBar);
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        //INIT LAYOUT
        greenhook = (ImageView)findViewById(R.id.iv_greenhook);
        et_pw1 = (EditText) findViewById(R.id.et_login_pw);
        et_masterpw1 = (EditText) findViewById(R.id.et_login_masterpw1);
        et_masterpw2 = (EditText) findViewById(R.id.et_login_masterpw2);
        rl_firstLogin = (RelativeLayout) findViewById(R.id.rl_firstlogin);
        //INIT LAYOUT



        //FIRST LOGIN ->
            et_pw1.setVisibility(View.GONE);
            rl_firstLogin.setVisibility(View.VISIBLE);
        //SECOND LOGIN ->
            et_pw1.setVisibility(View.VISIBLE);
            rl_firstLogin.setVisibility(View.GONE);



        et_pw1.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                String entry = et_pw1.getText().toString();
                if(entry.equals("1234")){
                    hideKB();
                    et_pw1.animate().alpha(0.0f);
                    et_pw1.setVisibility(View.GONE);

                    greenhook.setVisibility(View.VISIBLE);

                    startActivity(new Intent(login.this, Main.class));
                    finish();
                }
            }

            @Override
            public void afterTextChanged(Editable s) {

            }
        });

    }



    private void setNEWUser(){





    }


    private void hideKB(){
        View view = this.getCurrentFocus();
        if (view != null) {
            InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
        }
    }


}
