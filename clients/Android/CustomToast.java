package com.example.rodaues.pmdbs_navdraw;

import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

public class CustomToast {
    public static void makeText(Context context,String text){

        LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE );
        View view = inflater.inflate( R.layout.custom_toast, null );

        TextView tv = (TextView) view.findViewById(R.id.text);
        tv.setText(text);

        Toast toast = new Toast(context.getApplicationContext());
        toast.setGravity(Gravity.CENTER_VERTICAL, 0, 0);
        toast.setDuration(Toast.LENGTH_SHORT);
        toast.setView(view);
        toast.show();
    }

    public static void invokeMakeText(final Context context, final String text){
        new Handler(Looper.getMainLooper()).post(new Runnable() {
            @Override
            public void run() {
               makeText(context,text);
            }
        });
    }
}
