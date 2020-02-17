package com.example.rodaues.pmdbs_navdraw;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.util.Base64;

import java.io.ByteArrayOutputStream;
import java.util.regex.Pattern;

public class BitmapConverter {


    public static Bitmap convert2Bitmap(String icon_base64){
        byte[] icon_bytes = icon_base64.getBytes();
        byte[] decodedString = Base64.decode(icon_bytes, Base64.DEFAULT);
        Bitmap icon = BitmapFactory.decodeByteArray(decodedString, 0, decodedString.length);

        return icon;
    }

    public static String convert2Base64(Bitmap b){
        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        b.compress(Bitmap.CompressFormat.PNG, 100, byteArrayOutputStream);
        byte[] byteArray = byteArrayOutputStream .toByteArray();
        String encoded = Base64.encodeToString(byteArray, Base64.DEFAULT);
        String icon_base64 = encoded;

        return icon_base64;
    }

    public static String genIcon(String url, Context context){
        String inputURL = url;
        String initial = Character.toString(inputURL.charAt(0)).toLowerCase();
        Drawable draw;
        if(Pattern.matches("[a-zA-Z]",initial))
            draw = context.getResources().getDrawable(context.getResources().getIdentifier(initial,"drawable","com.example.rodaues.pmdbs_navdraw"));
        else draw = context.getResources().getDrawable(context.getResources().getIdentifier("qmark","drawable","com.example.rodaues.pmdbs_navdraw"));

        return convert2Base64(((BitmapDrawable) draw).getBitmap());
    }
}
