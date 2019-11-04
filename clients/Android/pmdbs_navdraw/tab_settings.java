package com.example.rodaues.pmdbs_navdraw;

import android.content.Context;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

public class tab_settings extends Fragment {
    View rootView;
    Context context;


    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        rootView = inflater.inflate(R.layout.tab_settings, container, false);
        context = rootView.getContext();
        return rootView;
    }
}
