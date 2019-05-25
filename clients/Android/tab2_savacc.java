package com.rodaues.pmdbs_androidclient;

import android.app.AlertDialog;
import android.support.v4.app.Fragment;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.DialogInterface;
import android.graphics.Bitmap;
import android.os.Bundle;

import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v4.content.ContextCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.ScrollView;
import android.widget.Spinner;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Random;
import java.util.TimeZone;

import static java.lang.Integer.parseInt;

public class tab2_savacc extends Fragment {
    private ListView lv_data;
    private LinearLayout layout_data;
    private ImageButton btn_backtolist, btn_dataupdate, btn_datadelete, btn_copypw;
    private Button btn_generatepwdata;
    private Spinner spinner_pwdata;
    private CheckBox cb_specialCharspwdata;
    private Integer pw_lengthdata = 4;
    private Integer item_position;
    private DataBaseHelper dbhelper;
    private EditText et_datahostname, et_datausername, et_dataemail, et_datapassword, et_dataurl, et_datanotes;
    private FloatingActionButton fab_refreshlist, fab_deleteitems;
    private ScrollView sv_data;
    private char[] passwordSpecialCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', '-', '$', '%', '&', '/', '(', ')', '=', '?', '{', '[', ']', '}', '\\', '+', '*', '#', ',', '.', '<', '>', '|', '@', '!', '~', ';', ':', '"' };
    private char[] passwordCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
    private Random rng = new Random();
    private ArrayList<Integer> selItems;
    private Boolean datachanges = false;
    private String current_hostname ="",current_url ="",current_username ="",current_email ="",current_notes ="",current_password ="";
    private ImageView iv_selitem;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.tab2_savacc, container, false);
        final Context context = rootView.getContext();

        lv_data = (ListView) rootView.findViewById(R.id.lv_data);
        lv_data.setVisibility(View.VISIBLE);

        selItems = new ArrayList<>();

        layout_data = (LinearLayout) rootView.findViewById(R.id.layout_showdata);


        iv_selitem = (ImageView)rootView.findViewById(R.id.iv_icon_iteminfo);
        et_datahostname = (EditText) rootView.findViewById(R.id.et_datahostname);
        et_dataurl      = (EditText) rootView.findViewById(R.id.et_dataurl);
        et_datausername = (EditText) rootView.findViewById(R.id.et_datausername);
        et_dataemail    = (EditText) rootView.findViewById(R.id.et_dataemail);
        et_datapassword = (EditText) rootView.findViewById(R.id.et_datapassword);
        et_datanotes    = (EditText) rootView.findViewById(R.id.et_datanotes);
        cb_specialCharspwdata =   (CheckBox)  rootView.findViewById(R.id.cb_specialcharspwdata);
        btn_generatepwdata = (Button) rootView.findViewById(R.id.btn_generatepwdata);
        sv_data = (ScrollView) rootView.findViewById(R.id.sv_showdata);
        btn_backtolist = (ImageButton) rootView.findViewById(R.id.btn_backtolist);
        fab_refreshlist = (FloatingActionButton) rootView.findViewById(R.id.fab_updatelist);
        btn_datadelete = rootView.findViewById(R.id.btn_deleteitem);
        fab_deleteitems = (FloatingActionButton) rootView.findViewById(R.id.fab_deleteitems);
        btn_copypw = (ImageButton) rootView.findViewById(R.id.btn_copypw);
        spinner_pwdata = (Spinner) rootView.findViewById(R.id.spinner_pwlengthpwdata);

        //--------------------------------------------------------------SPINNER
        List<String> list_pwlength = new ArrayList<String>();
        list_pwlength.add("4 CHARS");
        list_pwlength.add("8 CHARS");
        list_pwlength.add("12 CHARS");
        list_pwlength.add("16 CHARS");
        list_pwlength.add("32 CHARS");
        list_pwlength.add("64 CHARS");
        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(context,R.layout.spinnerlayout,list_pwlength);
        arrayAdapter.setDropDownViewResource(R.layout.spinnerlayout);
        spinner_pwdata.setAdapter(arrayAdapter);
        spinner_pwdata.setSelection(0);
        spinner_pwdata.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                spinner_pwdata.setSelection(position);
                switch(position){
                    case 0: pw_lengthdata=4;    break;
                    case 1: pw_lengthdata=8;    break;
                    case 2: pw_lengthdata=12;   break;
                    case 3: pw_lengthdata=16;   break;
                    case 4: pw_lengthdata=32;   break;
                    case 5: pw_lengthdata=64;   break;
                }
            }
            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
        //--------------------------------------------------------------SPINNER


        layout_data.setVisibility(View.GONE);
        doAnimShowList();
        dbhelper = new DataBaseHelper(context);


        lv_data.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                if(selItems.size()<1){
                    doAnimShowItemData();
                    fillItemData(position);
                    item_position = position;
                    fab_refreshlist.hide();
                    fab_deleteitems.hide();
                    selItems.clear();
                    sv_data.fullScroll(ScrollView.FOCUS_UP);
                }
                else{
                    ItemSelection(view,position);
                }
            }
        });

        lv_data.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> adapterView, View view, int i, long l) {
                ItemSelection(view,i);
                return true;
            }
        });



        btn_backtolist.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(et_datahostname.length()>0 && et_datapassword.length()>0){
                    updaterow();
                    refreshListviewData();
                    fab_refreshlist.show();
                    doAnimShowList();
                }
                else msg("MORE INFORMATION IS REQUIRED!");
            }
        });

        fab_refreshlist.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                refreshListviewData();

            }
        });
        fab_refreshlist.show();

        fab_deleteitems.hide();
        fab_deleteitems.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                showDeleteDialog();
            }
        });

        btn_copypw.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(et_datapassword.length()>0) copyText2Clipboard(et_datapassword.getText().toString());
            }
        });

        btn_datadelete.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                deleterow();
                fab_refreshlist.show();
                doAnimShowList();
                refreshListviewData();
            }
        });

        refreshListviewData();
        generate_pw();

        return rootView;
    }

    private void ItemSelection(View view, int i){
        View view_item = view;
        Boolean itemisselected = false;
        int selItemPos = -1;
        for(int p=0;p<selItems.size();p++){
            if(selItems.get(p)==i){
                itemisselected = true;
                selItemPos = p;
                break;
            }
        }
        if(itemisselected){
            selItems.remove(selItemPos);
            view_item.setBackground(ContextCompat.getDrawable(getContext(),R.drawable.listitem_background));
        }
        else {
            selItems.add(i);
            view_item.setBackground(ContextCompat.getDrawable(getContext(),R.drawable.seleclistitem_background));
        }

        if(selItems.size()>0){
            fab_deleteitems.show();
        }
        else fab_deleteitems.hide();
    }
    private void fillItemData(int position){

        ArrayList<ArrayList<String>> allData = (ArrayList<ArrayList<String>>) dbhelper.data_getAllData();
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
        btn_generatepwdata.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                et_datapassword.setText(get_generated_pw(cb_specialCharspwdata.isChecked(),pw_lengthdata));
            }
        });
    }
    private void deleterow() {

        ArrayList<String> IDs = dbhelper.data_getIDs();
        String row_id = IDs.get(item_position);

        dbhelper.data_deleterow(row_id);
        //msg("DATA DELETED");


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

        String updatedsuccessfully = dbhelper.data_updaterow(updatedData);
       // msg(updatedsuccessfully);
    }
    private void refreshListviewData(){
        ArrayList<ArrayList<String>> allData = (ArrayList<ArrayList<String>>) dbhelper.data_getAllData();

        List<SavedDataListItem> dataSource = new ArrayList<>();
        for (int i = 0; i < allData.size(); i++) {
            ArrayList<String> subList = allData.get(i);

          /*  Date date = new Date(Integer.valueOf(subList.get(8)) * 1000L);
            SimpleDateFormat sdf = new SimpleDateFormat("MM/dd/yyyy");
            sdf.setTimeZone(TimeZone.getTimeZone("GMT +1"));
            String date = sdf.format(date);*/

            String addit_text;

            if(subList.get(3).length()>0) addit_text=subList.get(3);
            else if(subList.get(5).length()>0) addit_text=subList.get(5);
            else{
                Date date = new Date(Integer.valueOf(subList.get(8)) * 1000L);
                SimpleDateFormat sdf = new SimpleDateFormat("MM/dd/yyyy");
                sdf.setTimeZone(TimeZone.getTimeZone("GMT +1"));
                addit_text = "last change: "+sdf.format(date);
            }

            String icon_base64 = subList.get(9);
            if(icon_base64.length()>0){
                BitmapConverter bg = new BitmapConverter();
                Bitmap icon_bitmap = bg.convert2Bitmap(icon_base64);
                dataSource.add(new SavedDataListItem(subList.get(1), addit_text,icon_bitmap));
            }
            else{
                dataSource.add(new SavedDataListItem(subList.get(1), addit_text,null));
            }





        }
        lv_data.setAdapter(new SavedDataListAdapter(getActivity().getApplicationContext(), dataSource));
        fab_deleteitems.hide();
        selItems.clear();
        closekeyboard();

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
    private void msg(String s){
        Snackbar.make(getActivity().findViewById(R.id.main_content),s, Snackbar.LENGTH_SHORT)
                .setAction("Action", null).show();
    }
    private void deleteSelectedItems(){
        ArrayList<String> IDs = dbhelper.data_getIDs();
        ArrayList<String> row_IDs = new ArrayList<>();
        for(int i=0;i<selItems.size();i++){

            String row_id = IDs.get(selItems.get(i));
            row_IDs.add(row_id);

        }
        dbhelper.data_deleterows(row_IDs);
        selItems.clear();
        fab_refreshlist.show();
        refreshListviewData();
    }
    private void showDeleteDialog(){

        DialogInterface.OnClickListener dialogClickListener = new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                switch (which){
                    case DialogInterface.BUTTON_POSITIVE:
                        deleteSelectedItems();
                        break;
                    case DialogInterface.BUTTON_NEGATIVE:
                        break;
                }
            }
        };
        AlertDialog.Builder builder = new AlertDialog.Builder(getContext());
        builder.setMessage(selItems.size()+" ITEM(S) SELECTED").setPositiveButton("DELETE DATA", dialogClickListener)
                .setNegativeButton("ABORT", dialogClickListener).show();
    }
    private void doAnimShowList(){

        lv_data.setVisibility(View.VISIBLE);
        lv_data.animate().alpha(1.0f).setDuration(500);
        layout_data.animate().alpha(0.0f).setDuration(500);
        layout_data.setVisibility(View.GONE);

    }
    private void doAnimShowItemData(){

        layout_data.setVisibility(View.VISIBLE);
        layout_data.animate().alpha(1.0f).setDuration(500);

        lv_data.animate().alpha(0.0f).setDuration(500);
        lv_data.setVisibility(View.GONE);
    }
    private void closekeyboard(){
        View view = getActivity().getCurrentFocus();
        if(view != null){
            InputMethodManager imm = (InputMethodManager)getActivity().getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(view.getWindowToken(),0);
        }
    }
    private void copyText2Clipboard(String text){
        ClipboardManager clipboard = (ClipboardManager) getContext().getSystemService(Context.CLIPBOARD_SERVICE);
        ClipData clip = ClipData.newPlainText("", text);
        clipboard.setPrimaryClip(clip);
        msg("PASSWORD COPIED TO CLIPBOARD");
    }
}