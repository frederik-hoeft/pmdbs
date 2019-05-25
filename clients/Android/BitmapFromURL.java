package com.rodaues.pmdbs_androidclient;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.util.Log;

import java.io.IOException;
import java.io.InputStream;
import java.net.URL;
import java.util.Scanner;

public class BitmapFromURL extends AsyncTask<String, Void, Bitmap>{

        @Override
        protected Bitmap doInBackground(String... string) {

            String rawURL = "http://besticon-demo.her" +
                    "okuapp.com/allicons.json?url="+string[0];
            String resultTXT="NO RESULT!";
            try {
                InputStream in = new URL(rawURL).openConnection().getInputStream();
                Scanner s = new Scanner(in).useDelimiter("\\A");
                resultTXT = s.hasNext() ? s.next() : "";

            } catch (IOException e) {
                e.printStackTrace();
            }


            Bitmap favICON = null;
            if(resultTXT.length()>100){
                String pngURL = resultTXT.split("\"")[9];

                try {
                    InputStream in = new java.net.URL(pngURL).openStream();
                    favICON = BitmapFactory.decodeStream(in);
                } catch (Exception e) {
                    Log.e("Error", e.getMessage());
                    e.printStackTrace();
                }
            }

            return favICON;
        }
}
