package com.example.rodaues.pmdbs_navdraw;

import android.app.AlertDialog;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.Drawable;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.Uri;
import android.os.AsyncTask;
import android.provider.MediaStore;

import androidx.core.content.res.ResourcesCompat;
import androidx.appcompat.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.ContextThemeWrapper;
import android.view.KeyEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.webkit.URLUtil;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.ScrollView;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.lang.reflect.Method;
import java.net.URL;
import java.util.ArrayList;
import java.util.Random;
import java.util.Scanner;

public class ItemData extends AppCompatActivity {

    private LinearLayout layout_data;
    private DataBaseHelper dbhelper;

    private RelativeLayout progressbar;

    private ImageButton btn_backtolist, btn_dataupdate, btn_datadelete, btn_copypw, btn_openURL;
    private Button btn_generatepwdata;
    private CheckBox cb_specialCharspwdata;
    private Integer pw_lengthdata = 4;
    private EditText et_datahostname, et_datausername, et_dataemail, et_datapassword, et_dataurl, et_datanotes, et_data_pw_length;
    private ScrollView sv_data;
    private char[] passwordSpecialCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', '-', '$', '%', '&', '/', '(', ')', '=', '?', '{', '[', ']', '}', '\\', '+', '*', '#', ',', '.', '<', '>', '|', '@', '!', '~', ';', ':', '"' };
    private char[] passwordCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
    private Random rng = new Random();
    private ImageView iv_selitem;
    private String current_hostname ="",current_url ="",current_username ="",current_email ="",current_notes ="",current_password ="";
    private Integer item_position;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        setTheme(R.style.AppTheme_NoActionBar);
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                WindowManager.LayoutParams.FLAG_FULLSCREEN);
        setContentView(R.layout.activity_item_data);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_SECURE, WindowManager.LayoutParams.FLAG_SECURE);

        dbhelper = DataBaseHelper.GetInstance();

        iv_selitem = (ImageView)findViewById(R.id.iv_icon_iteminfo);
        et_datahostname = (EditText) findViewById(R.id.et_datahostname);
        et_dataurl      = (EditText) findViewById(R.id.et_dataurl);
        et_datausername = (EditText) findViewById(R.id.et_datausername);
        et_dataemail    = (EditText) findViewById(R.id.et_dataemail);
        et_datapassword = (EditText) findViewById(R.id.et_datapassword);
        et_datanotes    = (EditText) findViewById(R.id.et_datanotes);
        et_data_pw_length = (EditText) findViewById(R.id.itemdata_et_pw_length);
        cb_specialCharspwdata =   (CheckBox) findViewById(R.id.cb_specialcharspwdata);
        btn_generatepwdata = (Button) findViewById(R.id.btn_generatepwdata);
        sv_data = (ScrollView) findViewById(R.id.sv_showdata);
        btn_backtolist = (ImageButton) findViewById(R.id.btn_backtolist);
        btn_datadelete = findViewById(R.id.btn_deleteitem);
        btn_copypw = (ImageButton) findViewById(R.id.btn_copypw);
        layout_data = (LinearLayout) findViewById(R.id.layout_showdata);
        btn_openURL = findViewById(R.id.btn_openURL);
        progressbar = findViewById(R.id.rlayout_progressbar_itemdata);

        btn_openURL.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                String inputURL =  et_dataurl.getText().toString();
                if (!inputURL.startsWith("http://") && !inputURL.startsWith("https://") && !inputURL.equals(""))
                    inputURL = "http://" + inputURL;

                    Intent browserIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(inputURL));
                    startActivity(browserIntent);
            }
        });





        btn_datadelete.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DialogInterface.OnClickListener dialogClickListener = new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        switch (which){
                            case DialogInterface.BUTTON_POSITIVE:
                                deleterow();
                                break;
                            case DialogInterface.BUTTON_NEGATIVE:
                                break;
                        }
                    }
                };

                Drawable drawable = iv_selitem.getDrawable();
                AlertDialog.Builder builder = new AlertDialog.Builder(new ContextThemeWrapper(ItemData.this, R.style.MyDialogTheme));
                builder.setTitle("Delete item?").setMessage("Hostname: "+current_hostname).setPositiveButton("Yes", dialogClickListener)
                        .setNegativeButton("No", dialogClickListener).setIcon(drawable).show();



            }
        });


        //ITEM SELECTED
        sv_data.fullScroll(ScrollView.FOCUS_UP);


        //ITEM SELECTED


        btn_backtolist.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(et_datahostname.length()>0 && et_datapassword.length()>0){

                    if(!current_hostname.equals(et_datahostname.getText().toString()) ||
                            !current_url.equals(et_dataurl.getText().toString()) ||
                            !current_username.equals(et_datausername.getText().toString()) ||
                            !current_email.equals(et_dataemail.getText().toString()) ||
                            !current_notes.equals(et_datanotes.getText().toString()) ||
                            !current_password.equals(et_datapassword.getText().toString()) || globalVARpool.selBITMAP!=null){

                        DialogInterface.OnClickListener dialogClickListener = new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {
                                switch (which){
                                    case DialogInterface.BUTTON_POSITIVE:
                                        updaterow();

                                        globalVARpool.ITEMselected=false;
                                        break;
                                    case DialogInterface.BUTTON_NEGATIVE:

                                        finish();
                                        globalVARpool.ITEMselected=false;

                                        break;
                                }
                            }
                        };
                        AlertDialog.Builder builder = new AlertDialog.Builder(new ContextThemeWrapper(ItemData.this, R.style.MyDialogTheme));
                        builder.setTitle("Save changes?").setMessage("new hostname: "+et_datahostname.getText()).setPositiveButton("YES", dialogClickListener)
                                .setNegativeButton("NO", dialogClickListener).show();

                    }
                    else{
                        finish();
                        globalVARpool.ITEMselected=false;

                    }



                }
                else CustomToast.makeText(getApplicationContext(),"MORE INFORMATION IS REQUIRED!");
            }
        });



        btn_copypw.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(et_datapassword.length()>0) copyText2Clipboard(et_datapassword.getText().toString());
            }
        });


        generate_pw();


        Intent INPUTi = getIntent();
        item_position=INPUTi.getIntExtra("position",0);

        fillItemData(item_position);


       cb_specialCharspwdata.setTypeface(ResourcesCompat.getFont(getApplicationContext(), R.font.hind_light));


       iv_selitem.setOnClickListener(new View.OnClickListener() {
           @Override
           public void onClick(View v) {
                selectImage();
           }

       });

       globalVARpool.selBITMAP=null;


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
                        Bitmap  inputBM = MediaStore.Images.Media.getBitmap(getContentResolver(), selectedImage);
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



                        //------ CHECK SIZE -> min 40px
                        if(outputBM.getWidth()>=40){

                            if(outputBM.getWidth()>500){

                                outputBM = Bitmap.createScaledBitmap(outputBM,500,500,true);
                            }

                            globalVARpool.selBITMAP = outputBM;
                        }

                        if(outputBM!=null) iv_selitem.setImageBitmap(outputBM);

                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }
                break;
        }
    }

    private void fillItemData(int position){

        ArrayList<ArrayList<String>> allData = globalVARpool.dataSET;
        ArrayList<String> subList = allData.get(position);

        current_hostname = subList.get(1);
        current_url = subList.get(2);
        current_username = subList.get(3);
        current_password = subList.get(4);
        current_email = subList.get(5);
        current_notes = subList.get(6);

        et_datahostname.setText(current_hostname);
        et_dataurl.setText(current_url);
        et_datausername.setText(current_username);
        et_datapassword.setText(current_password);
        et_dataemail.setText(current_email);
        et_datanotes.setText(current_notes);

        String icon_base64 = subList.get(9);
        if(icon_base64.length()>0){
            BitmapConverter bg = new BitmapConverter();
            Bitmap icon_bitmap = bg.convert2Bitmap(icon_base64);
            iv_selitem.setImageBitmap(icon_bitmap);
        }
        else{
            iv_selitem.setImageResource(R.drawable.ic_person_black_24dp);
        }
    }

    private void generate_pw(){

        et_data_pw_length.setOnKeyListener(new View.OnKeyListener() {
            @Override
            public boolean onKey(View v, int keyCode, KeyEvent event) {

                if ((event.getAction() == KeyEvent.ACTION_DOWN) &&
                        (keyCode == KeyEvent.KEYCODE_0)){
                    if(et_data_pw_length.getText().length()<1) return true;
                }
                if ((event.getAction() == KeyEvent.ACTION_DOWN) &&
                        (keyCode == KeyEvent.KEYCODE_ENTER)){

                    if(et_data_pw_length.getText().length()>0){
                        closekeyboard();
                        btn_generatepwdata.performClick();
                    }
                    else{
                        closekeyboard();
                    }

                    return true;
                }
                return false;
            }
        });

        btn_generatepwdata.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(et_data_pw_length.getText().length()>0){
                    pw_lengthdata = Integer.valueOf(et_data_pw_length.getText().toString());

                    try{
                        et_datapassword.setText(get_generated_pw(cb_specialCharspwdata.isChecked(),pw_lengthdata));
                    }
                    catch(Exception e){
                        CustomToast.makeText(getApplicationContext(),"ERROR!");
                    }
                }
                else CustomToast.makeText(getApplicationContext(),"MORE INFORMATION IS REQUIRED!");

                et_datapassword.setText(get_generated_pw(cb_specialCharspwdata.isChecked(),pw_lengthdata));
            }
        });
    }

    private void deleterow() {

        ArrayList<String> IDs = dbhelper.data_getIDs();
        String row_id = IDs.get(item_position);
        dbhelper.data_deleterow(row_id);

        invokeREFRESHlist();
    }




    private void updaterow(){
        String hostname     = et_datahostname.getText().toString();
        String url          = et_dataurl.getText().toString();
        String username     = et_datausername.getText().toString();
        String email        = et_dataemail.getText().toString();
        String password     = et_datapassword.getText().toString();
        String notes        = et_datanotes.getText().toString();

        long unixTime = System.currentTimeMillis() / 1000L;
        String date         = String.valueOf(unixTime);

        ArrayList<String> updatedData = new ArrayList<>();
        updatedData.add(hostname);
        updatedData.add(url);
        updatedData.add(username);
        updatedData.add(password);
        updatedData.add(email);
        updatedData.add(notes);
        updatedData.add(date);


        ArrayList<String> IDs = dbhelper.data_getIDs();
        String row_id = IDs.get(item_position);
        updatedData.add(String.valueOf(row_id));

        if(globalVARpool.selBITMAP!=null){
            String bitmap_base64=BitmapConverter.convert2Base64(globalVARpool.selBITMAP);
            updatedData.add(bitmap_base64);
            dbhelper.data_updaterow(updatedData);
            invokeREFRESHlist();
        }
        else if(!url.equals(current_url)){

            ArrayList<Object> al = new ArrayList<>();
            al.add(updatedData);
            al.add(url);

            new ast_updacc_genicon().execute(al);
            //generate new icon
            //updatedData.add(changed_base64icon);

           // String updatedsuccessfully = dbhelper.data_updaterow(updatedData);
            // msg(updatedsuccessfully);

        }
        else{
            String updatedsuccessfully = dbhelper.data_updaterow(updatedData);
            // msg(updatedsuccessfully);

            invokeREFRESHlist();

        }



    }
    private void copyText2Clipboard(String text){
        ClipboardManager clipboard = (ClipboardManager) getSystemService(Context.CLIPBOARD_SERVICE);
        ClipData clip = ClipData.newPlainText("", text);
        clipboard.setPrimaryClip(clip);
        CustomToast.makeText(getApplicationContext(),"PASSWORD COPIED TO CLIPBOARD");
    }


    private String get_generated_pw(boolean specialCharactersEnabled,int pw_length) {
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

    private void invokeREFRESHlist(){

        try {
            Method method = tab_saveddata.class.getMethod("refreshListviewData");
            method.invoke(globalVARpool.fragment_tab_saveddata);
        } catch (Exception e) {
            e.printStackTrace();
        }
finish();
       // startActivity(new Intent(ItemData.this, Main.class));
    }



    @SuppressWarnings("unchecked")
    private class ast_updacc_genicon extends AsyncTask<ArrayList<Object>,String,String> {

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            closekeyboard();
            startLoadingScreen();
        }


        @Override
        protected String doInBackground(ArrayList<Object>... al) {

            ArrayList<?> al_input = al[0];
            ArrayList<String> updatedData = (ArrayList<String>)al_input.get(0);
            String url = (String)al_input.get(1);

            if (url.length()>0 && !url.startsWith("http://") && !url.startsWith("https://"))
                url = "http://" + url.toLowerCase();

            boolean connected = false;
            ConnectivityManager connectivityManager = (ConnectivityManager)getSystemService(Context.CONNECTIVITY_SERVICE);
            if(connectivityManager.getNetworkInfo(ConnectivityManager.TYPE_MOBILE).getState() == NetworkInfo.State.CONNECTED ||
                    connectivityManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI).getState() == NetworkInfo.State.CONNECTED) {
                //we are connected to a network
                connected = true;
            }
            else
                connected = false;

            if(url.length()>0 && URLUtil.isValidUrl(url) && connected){

                String bitmap_base64="";

                String rawURL = "https://i.olsh.me/allicons.json?url="+url;
                String resultTXT="NO RESULT!";

                InputStream in = null;
                try {
                    in = new URL(rawURL).openConnection().getInputStream();
                } catch (IOException e) {
                    e.printStackTrace();

                }

                if (in != null) {
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

                        bitmap_base64=BitmapConverter.genIcon(et_datahostname.getText().toString(),getApplicationContext());
                    }


                    updatedData.add(bitmap_base64);


                }
                else{
                    bitmap_base64=BitmapConverter.genIcon(et_datahostname.getText().toString(),getApplicationContext());
                    updatedData.add(bitmap_base64);

                }

            }
            else{

                updatedData.add(BitmapConverter.genIcon(updatedData.get(0),getApplicationContext()));

            }

            String updatedsuccessfully = dbhelper.data_updaterow(updatedData);
            // msg(updatedsuccessfully);

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
            stopLoadingScreen();

            invokeREFRESHlist();

           // finish();
           /* new Handler(Looper.getMainLooper()).post(new Runnable() {
                @Override
                public void run() {
                 //method in ItemData.java
                }
            });
*/

        }
    }


    private void startLoadingScreen(){
        progressbar.setVisibility(View.VISIBLE);
    }
    private void stopLoadingScreen(){
        progressbar.setVisibility(View.GONE);
    }

    @Override
    public void onBackPressed() {
        btn_backtolist.performClick();
    }

    @Override
    protected void onResume()
    {
        super.onResume();
        //overridePendingTransition(R.anim.slide_in_fromright, R.anim.slide_out_toleft);
    }

    private void closekeyboard(){
        View view = getCurrentFocus();
        if(view != null){
            InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(view.getWindowToken(),0);
        }
    }

}
