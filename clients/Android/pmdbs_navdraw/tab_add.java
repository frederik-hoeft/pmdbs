package com.example.rodaues.pmdbs_navdraw;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v4.app.Fragment;
import android.support.v4.content.res.ResourcesCompat;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;
import android.webkit.URLUtil;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ScrollView;
import android.widget.Spinner;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Scanner;

public class tab_add extends Fragment {
    View rootView;
    Context context;
    private char[] passwordSpecialCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', '-', '$', '%', '&', '/', '(', ')', '=', '?', '{', '[', ']', '}', '\\', '+', '*', '#', ',', '.', '<', '>', '|', '@', '!', '~', ';', ':', '"' };
    private char[] passwordCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
    private Random rng = new Random();

    private Spinner spinner_pw;
    private Button btn_generatepw;
    private CheckBox cb_specialChars;
    private Integer pw_length = 4;
    private EditText et_pw, et_hostname, et_url, et_username, et_email, et_notes;
    private DataBaseHelper dbhelper;
    private FloatingActionButton fab_addaccount;
    private ScrollView sv_dataentry;


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
        et_pw           =   (EditText)  rootView.findViewById(R.id.et_password);
        sv_dataentry    =   (ScrollView)rootView.findViewById(R.id.sv_dataentry);

        //--------------------------------------------------------------SPINNER
        spinner_pw = (Spinner) rootView.findViewById(R.id.spinner_pwlength);
        initSpinner();
        //--------------------------------------------------------------SPINNER

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

        return rootView;
    }


    private void addaccount(){

        if(et_hostname.length()>0 && et_pw.length()>0){
            new ast_addacc().execute("");
        }
        else {
            CustomToast.makeText(context,"MORE INFORMATION IS REQUIRED!");
        }

    }

    private void initSpinner(){
        List<String> list_pwlength = new ArrayList<String>();
        list_pwlength.add("4 CHARS");
        list_pwlength.add("8 CHARS");
        list_pwlength.add("12 CHARS");
        list_pwlength.add("16 CHARS");
        list_pwlength.add("32 CHARS");
        list_pwlength.add("64 CHARS");
        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(getContext(),R.layout.spinnerlayout,list_pwlength);
        arrayAdapter.setDropDownViewResource(R.layout.spinnerlayout);
        spinner_pw.setAdapter(arrayAdapter);
        spinner_pw.setSelection(0);
        spinner_pw.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                spinner_pw.setSelection(position);
                switch(position){
                    case 0: pw_length=4;    break;
                    case 1: pw_length=8;    break;
                    case 2: pw_length=12;   break;
                    case 3: pw_length=16;   break;
                    case 4: pw_length=32;   break;
                    case 5: pw_length=64;   break;
                }
            }
            @Override
            public void onNothingSelected(AdapterView<?> parent) {

            }
        });
    }
    private void resetFormular(){

        sv_dataentry.fullScroll(ScrollView.FOCUS_UP);
        cb_specialChars.setChecked(true);
        spinner_pw.setSelection(0);
        et_hostname.setText("");
        et_url.setText("");
        et_username.setText("");
        et_email.setText("");
        et_pw.setText("");
        et_notes.setText("");
    }

    private void generate_pw(){
        btn_generatepw.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                try{
                    et_pw.setText(get_generated_pw(cb_specialChars.isChecked(),pw_length));
                }
                catch(Exception e){
                    CustomToast.makeText(context,"ERROR!");
                }
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
            String password     = et_pw.getText().toString();
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

            if (url.length()>0 && !url.startsWith("http://") && !url.startsWith("https://"))
                url = "https://" + url;



            if(url.length()>0 && URLUtil.isValidUrl(url) && ((Main)getActivity()).networkCON()){

                String bitmap_base64="";

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

                newData.add(bitmap_base64);

            }
            else{

                newData.add(BitmapConverter.genIcon(hostname,context));

            }


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
