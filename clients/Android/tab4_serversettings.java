package com.rodaues.pmdbs_androidclient;

import android.support.v4.app.Fragment;
import android.content.Context;
import android.os.Bundle;
import android.support.design.widget.Snackbar;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import java.security.MessageDigest;

public class tab4_serversettings extends Fragment {

    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.tab4_serversettings, container, false);
        Context context = rootView.getContext();

        TextView tv_test = (TextView) rootView.findViewById(R.id.tab4_tv_test);



        return rootView;
    }
}