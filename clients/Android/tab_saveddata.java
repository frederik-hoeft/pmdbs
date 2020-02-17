package com.example.rodaues.pmdbs_navdraw;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;

import com.google.android.material.floatingactionbutton.FloatingActionButton;

import androidx.fragment.app.Fragment;
import androidx.core.content.ContextCompat;
import android.view.ContextThemeWrapper;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.TextView;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.TimeZone;


public class tab_saveddata extends Fragment {
    View rootView;
    Context context;

    private ListView lv_data;
    private DataBaseHelper dbhelper;
    private FloatingActionButton fab_sync, fab_deleteitems;
    public static TextView tv_noData;
    private ArrayList<Integer> selItems;


    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        rootView = inflater.inflate(R.layout.tab_saveddata, container, false);
        context = rootView.getContext();

        globalVARpool.fragment_tab_saveddata=this;

        lv_data = (ListView) rootView.findViewById(R.id.lv_data);
        lv_data.setVisibility(View.VISIBLE);
        globalVARpool.lv_data = lv_data;

        tv_noData = (TextView) rootView.findViewById(R.id.tv_nodata);

        selItems = new ArrayList<>();
        globalVARpool.selItems = selItems;

      //  lv_data.smoothScrollToPosition(lv_data.geti);



        fab_deleteitems = (FloatingActionButton) rootView.findViewById(R.id.fab_deleteitems);

        globalVARpool.fab_deleteitems = fab_deleteitems;
        globalVARpool.fab_sync = fab_sync;
        dbhelper = DataBaseHelper.GetInstance();


        lv_data.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                if(selItems.size()<1){
                    Intent i = new Intent(getActivity(),ItemData.class);
                    i.putExtra("position",position);
                    startActivity(i);

                    //globalVARpool.ITEMselected=true;
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


        fab_sync = rootView.findViewById(R.id.fab_sync);
        fab_sync.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {



                if(!globalVARpool.wasOnline){
                    Intent i = new Intent(getActivity(),registeronline.class);
                    startActivity(i);
                    return;
                }
                //TODO START SYNC ANIMATION
                Runnable onTaskFailed = new Runnable() {
                    @Override
                    public void run() {
                        fab_sync.show();
                        CustomToast.makeText(context, "Syncing failed.");
                        //TODO ERROR MSG
                    }
                };
                AutomatedTaskFramework.Tasks.Clear();
                PDTPClient client = PDTPClient.GetInstance();

                System.out.println("isconnected: "+client.isConnected());
                System.out.println("isloggedin: "+client.isLogged_in());

                if(!client.isConnected()){
                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.Contains, "DEVICE_AUTHORIZED", new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.Connect();
                        }
                    },onTaskFailed);
                }
                if(!client.isLogged_in()){
                    AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", new Runnable() {
                        @Override
                        public void run() {
                            NetworkAdapter.MethodProvider.Login();
                        }
                    },onTaskFailed);

                }
                AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.Contains, "FETCH_SYNC", new Runnable() {
                    @Override
                    public void run() {
                        NetworkAdapter.MethodProvider.Sync();
                    }
                },onTaskFailed);
                try {
                    AutomatedTaskFramework.Tasks.Execute();
                } catch (Exception e) {
                    e.printStackTrace();
                }

                fab_sync.hide();

            }
        });


        fab_sync.show();

        fab_deleteitems.hide();
        fab_deleteitems.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                showDeleteDialog();
            }
        });

        refreshListviewData();
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


    public static void refreshListviewData(){
        DataBaseHelper db = DataBaseHelper.GetInstance();
        db.updateDATASET();
        ArrayList<ArrayList<String>> allData = globalVARpool.dataSET;


        List<SavedDataListItem> dataSource = new ArrayList<>();
        for (int i = 0; i < allData.size(); i++) {
            ArrayList<String> subList = allData.get(i);


            String addit_text;

            if(subList.get(3).length()>0) addit_text=subList.get(3);
            else if(subList.get(5).length()>0) addit_text=subList.get(5);
            else{
                Date date = new Date(Integer.valueOf(subList.get(8)) * 1000L);
                SimpleDateFormat sdf = new SimpleDateFormat("MM/dd/yyyy");
                sdf.setTimeZone(TimeZone.getTimeZone("GMT +1"));
                addit_text = "last change: "+sdf.format(date);
            }

            String pw_grade = "X";

            Password.Result pw_result = Password.Security.SimpleCheck(subList.get(4));

            pw_grade = pw_result.Grade();

            String icon_base64 = subList.get(9);
            if(icon_base64.length()>0){
                BitmapConverter bg = new BitmapConverter();
                Bitmap icon_bitmap = bg.convert2Bitmap(icon_base64);
                dataSource.add(new SavedDataListItem(subList.get(1), addit_text,icon_bitmap,pw_grade));
            }
            else{
                dataSource.add(new SavedDataListItem(subList.get(1), addit_text,null,pw_grade));
            }

        }



        //CHECK FOR EXISTING DATA
        if((new SavedDataListAdapter(globalVARpool.fragment_tab_saveddata.getActivity().getApplicationContext(), dataSource)).getCount()>0){
            globalVARpool.lv_data.setAdapter(new SavedDataListAdapter(globalVARpool.fragment_tab_saveddata.getActivity().getApplicationContext          (), dataSource));
            globalVARpool.lv_data.setVisibility(View.VISIBLE);
            tv_noData.setVisibility(View.INVISIBLE);
        }
        else{
            globalVARpool.lv_data.setVisibility(View.INVISIBLE);
            tv_noData.setVisibility(View.VISIBLE);
        }

        globalVARpool.fab_deleteitems.hide();
        globalVARpool.selItems.clear();

        closekeyboard();

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
        AlertDialog.Builder builder = new AlertDialog.Builder(new ContextThemeWrapper(getContext(), R.style.MyDialogTheme));
        builder.setTitle("Delete data?").setMessage(selItems.size()+" item(s) selected").setPositiveButton("Delete data", dialogClickListener)
                .setNegativeButton("Abort", dialogClickListener).show();
    }
    private static void closekeyboard(){
        View view = globalVARpool.fragment_tab_saveddata.getActivity().getCurrentFocus();
        if(view != null){
            InputMethodManager imm = (InputMethodManager)globalVARpool.fragment_tab_saveddata.getActivity().getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(view.getWindowToken(),0);
        }
    }


}
