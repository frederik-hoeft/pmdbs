package com.example.rodaues.pmdbs_navdraw;

import android.content.Context;
import android.os.Bundle;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

import androidx.fragment.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;

public class tab_myaccount extends Fragment {
    View rootView;
    Context context;

    DataBaseHelper dbhelper;
    EditText et_accmasteruname, et_accnickname, et_currentmasterPW, et_newmasterPW1, et_newmasterPW2;
    FloatingActionButton fab_savechanges;

    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        rootView = inflater.inflate(R.layout.tab_myaccount, container, false);
        context = rootView.getContext();

        dbhelper = DataBaseHelper.GetInstance();

        et_accmasteruname = (EditText) rootView.findViewById(R.id.et_accmasterusername);
        et_accnickname = (EditText) rootView.findViewById(R.id.et_accnickname);
        et_currentmasterPW = (EditText) rootView.findViewById(R.id.et_currentmasterpw);
        et_newmasterPW1 = (EditText) rootView.findViewById(R.id.et_newmasterpw1);
        et_newmasterPW2 = (EditText) rootView.findViewById(R.id.et_newmasterpw2);

        fab_savechanges = (FloatingActionButton) rootView.findViewById(R.id.fab_savechanges);

        fab_savechanges.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String entryPW = et_currentmasterPW.getText().toString();
                String currentMasterPW = dbhelper.user_getMasterPW();
                String firstUSAGE = dbhelper.user_getfUSAGE();
                String hashedEntryPW = "";
                Boolean safePW = true;

                try {
                    hashedEntryPW = CryptoHelper.SHA256(entryPW);
                } catch (Exception e) {
                    e.printStackTrace();
                }


                if(currentMasterPW.equals(CryptoHelper.getScryptString(hashedEntryPW,firstUSAGE))){
                    if(et_newmasterPW1.getText().toString().equals(et_newmasterPW2.getText().toString())){


                        if(safePW){ //Abfrage sicheres Passwort

                            HelperMethods.changeLocalMasterPW(et_newmasterPW1.getText().toString());



                            CustomToast.makeText(context,"MASTERPASSWORD CHANGED!");

                            //msg(hashedEntryPW.substring(0,9)+" "+newhashedPW.substring(0,9));
                            et_currentmasterPW.setText("");
                            et_newmasterPW1.setText("");
                            et_newmasterPW2.setText("");
                            et_currentmasterPW.requestFocus();
                        }

                    }
                    else CustomToast.makeText(context,"NEW PASSWORD NOT EQUAL!");
                }
                else CustomToast.makeText(context,"CURRENT MASTERPASSWORD WRONG!");

            }
        });

        return rootView;
    }




}
