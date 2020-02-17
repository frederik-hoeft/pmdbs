package com.example.rodaues.pmdbs_navdraw;

import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.provider.MediaStore;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

import androidx.fragment.app.Fragment;
import androidx.core.content.res.ResourcesCompat;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;
import android.webkit.URLUtil;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.ScrollView;
import android.widget.TextView;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.net.URL;
import java.util.ArrayList;
import java.util.Random;
import java.util.Scanner;

import static android.app.Activity.RESULT_OK;

public class tab_add extends Fragment {
    View rootView;
    Context context;
    private char[] passwordSpecialCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', '-', '$', '%', '&', '/', '(', ')', '=', '?', '{', '[', ']', '}', '\\', '+', '*', '#', ',', '.', '<', '>', '|', '@', '!', '~', ';', ':', '"' };
    private char[] passwordCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
    private Random rng = new Random();

    private Button btn_generatepw;
    private CheckBox cb_specialChars;
    private Integer pw_length = 16;
    private EditText et_password, et_hostname, et_url, et_username, et_email, et_notes;
    private DataBaseHelper dbhelper;
    private FloatingActionButton fab_addaccount;
    private ScrollView sv_dataentry;
    private EditText et_pw_length;
    private TextView tv_password_title;


    private ImageView iv_selectedimage;

    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        rootView = inflater.inflate(R.layout.tab_add, container, false);
        context = rootView.getContext();



        btn_generatepw  =   (Button)    rootView.findViewById(R.id.btn_generate);
        cb_specialChars =   (CheckBox)  rootView.findViewById(R.id.cb_specialchars);
        et_hostname     =   (EditText)  rootView.findViewById(R.id.et_hostname);
        et_url          =   (EditText)  rootView.findViewById(R.id.et_url);
        et_username     =   (EditText)  rootView.findViewById(R.id.et_username);
        et_email        =   (EditText)  rootView.findViewById(R.id.et_email);
        et_notes        =   (EditText)  rootView.findViewById(R.id.et_notes);
        et_password        =   (EditText)  rootView.findViewById(R.id.et_password);
        sv_dataentry    =   (ScrollView)rootView.findViewById(R.id.sv_dataentry);
        iv_selectedimage =  (ImageView) rootView.findViewById(R.id.iv_selectedimage);
        et_pw_length = (EditText) rootView.findViewById(R.id.et_pw_length);
        tv_password_title = (TextView) rootView.findViewById(R.id.tv_password_title);

        //--------------------------------------------------------------FAB_ADDACCOUNT
        fab_addaccount = (FloatingActionButton)rootView.findViewById(R.id.fab_addaccount);
        fab_addaccount.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                addaccount();
            }
        });
        //--------------------------------------------------------------FAB_ADDACCOUNT

        //--------------------------------------------------------------DBHELPER_INIT
        dbhelper = DataBaseHelper.GetInstance();
        //--------------------------------------------------------------DBHELPER_INIT


        generate_pw();
        cb_specialChars.setTypeface(ResourcesCompat.getFont(context, R.font.hind_light));


        iv_selectedimage.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                selectImage();
            }
        });


        return rootView;
    }


    private void addaccount(){

        if(et_hostname.length()>0 && et_password.length()>0){
            new ast_addacc().execute("");
        }
        else {
            CustomToast.makeText(context,"MORE INFORMATION IS REQUIRED!");
        }



    }


    private void selectImage(){

        Intent pickPhoto = new Intent(Intent.ACTION_PICK,
                android.provider.MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
        startActivityForResult(pickPhoto , 1);//one can be replaced with any action code

    }

    public void onActivityResult(int requestCode, int resultCode, Intent imageReturnedIntent) {
        super.onActivityResult(requestCode, resultCode, imageReturnedIntent);
        switch(requestCode) {
            case 1:
                if(resultCode == RESULT_OK){
                    Uri selectedImage = imageReturnedIntent.getData();

                    String err_msg="";
                    Bitmap outputBM = null;

                    try {
                        Bitmap  inputBM = MediaStore.Images.Media.getBitmap(context.getContentResolver(), selectedImage);
                        if (inputBM.getWidth() >= inputBM.getHeight()){

                            outputBM = Bitmap.createBitmap(
                                    inputBM,
                                    inputBM.getWidth()/2 - inputBM.getHeight()/2,
                                    0,
                                    inputBM.getHeight(),
                                    inputBM.getHeight()
                            );
                        }
                        else{

                            outputBM = Bitmap.createBitmap(
                                    inputBM,
                                    0,
                                    inputBM.getHeight()/2 - inputBM.getWidth()/2,
                                    inputBM.getWidth(),
                                    inputBM.getWidth()
                            );
                        }

                        //------ IMAGE CROPPED TO SQUARE



                        //------ CHECK SIZE -> min 40px max 500px
                        if(outputBM.getWidth()>=40){

                            if(outputBM.getWidth()>500){

                                outputBM = Bitmap.createScaledBitmap(outputBM,500,500,true);
                            }

                            globalVARpool.selBITMAP = outputBM;
                        }

                        if(outputBM!=null) iv_selectedimage.setImageBitmap(outputBM);

                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }
                break;

        }
    }


    private void resetFormular(){

        sv_dataentry.fullScroll(ScrollView.FOCUS_UP);
        cb_specialChars.setChecked(true);
        et_hostname.setText("");
        et_url.setText("");
        et_username.setText("");
        et_email.setText("");
        et_password.setText("");
        et_notes.setText("");
    }


    private void generate_pw(){

        et_pw_length.setOnKeyListener(new View.OnKeyListener() {
            @Override
            public boolean onKey(View v, int keyCode, KeyEvent event) {

                if ((event.getAction() == KeyEvent.ACTION_DOWN) &&
                        (keyCode == KeyEvent.KEYCODE_0)){
                    if(et_pw_length.getText().length()<1) return true;
                }
                if ((event.getAction() == KeyEvent.ACTION_DOWN) &&
                        (keyCode == KeyEvent.KEYCODE_ENTER)){

                    if(et_pw_length.getText().length()>0){
                        closekeyboard();
                        btn_generatepw.performClick();
                    }
                    else{
                        closekeyboard();
                    }

                    return true;
                }
                return false;
            }
        });

        btn_generatepw.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                if(et_pw_length.getText().length()>0){
                    pw_length = Integer.valueOf(et_pw_length.getText().toString());

                    try{
                        et_password.setText(get_generated_pw(cb_specialChars.isChecked(),pw_length));
                    }
                    catch(Exception e){
                        CustomToast.makeText(context,"ERROR!");
                    }
                }
                else CustomToast.makeText(context,"MORE INFORMATION IS REQUIRED!");

                /*
                if(!dbhelper.checkPassword(et_username.getText().toString())){
                    msg("INSECURE!");
                }*/
            }
        });
    }
    private String get_generated_pw(boolean specialCharactersEnabled, int pw_length){
        char[] characterSet;
        StringBuilder randomizedPassword = new StringBuilder();
        if(specialCharactersEnabled){
            characterSet = passwordSpecialCharacters;
        }
        else{
            characterSet = passwordCharacters;
        }
        for (int i = 0; i < pw_length;i++){

            randomizedPassword.append(characterSet[rng.nextInt(characterSet.length-1)]);
        }
        return randomizedPassword.toString();
    }
    private void closekeyboard(){
        View view = getActivity().getCurrentFocus();
        if(view != null){
            InputMethodManager imm = (InputMethodManager)getActivity().getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(view.getWindowToken(),0);
        }
    }

    private class ast_addacc extends AsyncTask<String,String,String> {

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            closekeyboard();

            ((Main)getActivity()).startLoadingScreen();
        }


        @Override
        protected String doInBackground(String... string) {

            ArrayList<String> newData = new ArrayList<>();

            String hostname     = et_hostname.getText().toString();
            String url          = et_url.getText().toString().toLowerCase();
            String username     = et_username.getText().toString();
            String email        = et_email.getText().toString();
            String password     = et_password.getText().toString();
            String notes        = et_notes.getText().toString();


            long unixTime = System.currentTimeMillis() / 1000L;
            String date         = String.valueOf(unixTime);

            newData.add(hostname);
            newData.add(url);
            newData.add(username);
            newData.add(password);
            newData.add(email);
            newData.add(notes);
            newData.add(date);

            String bitmap_base64="";

            if(globalVARpool.selBITMAP!=null){


                bitmap_base64 = BitmapConverter.convert2Base64(globalVARpool.selBITMAP);

            }else{

                if (url.length()>0 && !url.startsWith("http://") && !url.startsWith("https://"))
                    url = "https://" + url;




                if(url.length()>0 && URLUtil.isValidUrl(url) && ((Main)getActivity()).networkCON()){



                    String rawURL = "https://i.olsh.me/allicons.json?url="+url;
                    String resultTXT="NO RESULT!";

                    InputStream in = null;
                    try {
                        in = new URL(rawURL).openConnection().getInputStream();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }

                    if(in!=null){
                        Scanner s = new Scanner(in).useDelimiter("\\A");
                        resultTXT = s.hasNext() ? s.next() : "";

                        JSONObject obj = null;
                        try {
                            obj = new JSONObject(resultTXT);
                   /*   msg(String.valueOf(obj.getJSONArray("icons").getJSONObject(0).getInt("width"))+" - "+
                        obj.getJSONArray("icons").length());
                   msg(obj.getJSONArray("icons").getJSONObject(0).getString("url"));*/

                        } catch (Exception e) {
                            System.out.println(e.toString());
                        }

                        Bitmap favICON = null;
                        try {
                            if(obj.getJSONArray("icons").length()>0 && obj.getJSONArray("icons").getJSONObject(0).getInt("width")>=64){
                                String pngURL = obj.getJSONArray("icons").getJSONObject(0).getString("url");

                                try {
                                    InputStream in_png = new URL(pngURL).openStream();
                                    favICON = BitmapFactory.decodeStream(in_png);
                                } catch (Exception e) {
                                    Log.e("Error", e.getMessage());
                                    e.printStackTrace();
                                }
                            }
                        } catch (JSONException e) {
                            e.printStackTrace();
                        }

                        if(favICON != null && favICON.getWidth()<64) {
                            favICON = null;
                        }

                        if(favICON!=null){

                            bitmap_base64=BitmapConverter.convert2Base64(favICON);
                        }
                        else{

                            bitmap_base64=BitmapConverter.genIcon(et_hostname.getText().toString(),context);
                        }

                    }
                    else{
                        bitmap_base64=BitmapConverter.genIcon(et_hostname.getText().toString(),context);
                    }


                }
                else{

                    bitmap_base64=BitmapConverter.genIcon(hostname,context);

                    System.out.println("ICON generated by hostname.");
                }
            }


            newData.add(bitmap_base64);

            String lastentry = dbhelper.data_addrow(newData);
            /* Integer lastentry = dbhelper.data_addrow(newData);*/
                   /* if(lastentry >= 0){
                        msg("last entry = "+lastentry.toString());
                    }
                     else msg("ERROR WHILE ADDING DATA!");*/

            return "";
        }

        @Override
        protected void onPostExecute(String s) {
            super.onPostExecute(s);
            ((Main)getActivity()).stopLoadingScreen();

            globalVARpool.selBITMAP=null;

            new Handler(Looper.getMainLooper()).post(new Runnable() {
                @Override
                public void run() {
                    resetFormular();
                   // msg("DATA ADDED!");
                }
            });

           // ((Main)getActivity()).onNavigationItemSelected(Main.navigationView.getMenu().getItem(1));
            Main.navigationView.getMenu().getItem(1).setChecked(true);
            ((Main)getActivity()).getSupportFragmentManager().beginTransaction().replace(R.id.contentmain,new tab_saveddata()).commit();



        }

    }
}
