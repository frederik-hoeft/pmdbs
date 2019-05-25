package com.rodaues.pmdbs_androidclient;

import android.support.v4.app.Fragment;
import android.content.Context;

import android.graphics.Bitmap;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.view.LayoutInflater;
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
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.concurrent.ExecutionException;

public class tab1_addacc extends Fragment {

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
    ArrayList<String> newData;

    View rootView;
    LoadingScreen loadScr;
    Context context;

    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        rootView = inflater.inflate(R.layout.tab1_addacc, container, false);
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
        dbhelper = new DataBaseHelper(context);
        dbhelper.createDataBase();
        //--------------------------------------------------------------DBHELPER_INIT

        newData = new ArrayList<>();

        generate_pw();

        return rootView;
    }
    private void addaccount(){

        if(et_hostname.length()>0 && et_pw.length()>0){

                    String hostname     = et_hostname.getText().toString();
                    String url          = "https://"+et_url.getText().toString();
                    String username     = et_username.getText().toString();
                    String email        = et_email.getText().toString();
                    String password     = et_pw.getText().toString();
                    String notes        = et_notes.getText().toString();

                   /* if(dbhelper.checkPassword(password)) msg("SECURE PASSWORD!");
                    else msg("UNSECURE PASSWORD!");*/

                    long unixTime = System.currentTimeMillis() / 1000L;
                    String date         = String.valueOf(unixTime);

                    newData.add(hostname);
                    newData.add(url);
                    newData.add(username);
                    newData.add(password);
                    newData.add(email);
                    newData.add(notes);
                    newData.add(date);

                    if(url.length()>8 && URLUtil.isValidUrl(url)){

                        BitmapConverter bg = new BitmapConverter();

                        Bitmap b_output=null;
                        String bitmap_base64="";

                        BitmapFromURL BFU = new BitmapFromURL();

                        try {
                            b_output = BFU.execute(url).get();
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        } catch (ExecutionException e) {
                            e.printStackTrace();
                        }

                        if(b_output!=null){

                            bitmap_base64=bg.convert2Base64(b_output);
                        }
                        else{

                            //generate icon
                        }


                        newData.add(bitmap_base64);
                        Integer lastentry = dbhelper.data_addrow(newData);

                        resetFormular();
                    }
                    else{

                        newData.add("");
                        Integer lastentry = dbhelper.data_addrow(newData);
                        resetFormular();
                    }


                   /* Integer lastentry = dbhelper.data_addrow(newData);*/
                   /* if(lastentry >= 0){
                        msg("last entry = "+lastentry.toString());
                    }
                     else msg("ERROR WHILE ADDING DATA!");*/

        }
        else {
            msg("MORE INFORMATION IS REQUIRED!");
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
        msg("DATA ADDED!");


        sv_dataentry.fullScroll(ScrollView.FOCUS_UP);
        cb_specialChars.setChecked(true);
        spinner_pw.setSelection(0);
        et_hostname.setText("");
        et_url.setText("");
        et_username.setText("");
        et_email.setText("");
        et_pw.setText("");
        et_notes.setText("");
        closekeyboard();
        newData.clear();
    }
    private void msg(String s){

        Snackbar.make(getActivity().findViewById(android.R.id.content),s, Snackbar.LENGTH_SHORT)
                .setAction("Action", null)
                .setActionTextColor(getResources().getColor(R.color.colorAccent)).show();

    }
    private void generate_pw(){
        btn_generatepw.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                try{
                    et_pw.setText(get_generated_pw(cb_specialChars.isChecked(),pw_length));

                }
                catch(Exception e){
                    msg("ERROR!");
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

}


