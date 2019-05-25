package com.rodaues.pmdbs_androidclient;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Base64;
import java.io.ByteArrayOutputStream;

public class BitmapConverter {


    public Bitmap convert2Bitmap(String icon_base64){
        byte[] icon_bytes = icon_base64.getBytes();
        byte[] decodedString = Base64.decode(icon_bytes, Base64.DEFAULT);
        Bitmap icon = BitmapFactory.decodeByteArray(decodedString, 0, decodedString.length);

        return icon;
    }

    public String convert2Base64(Bitmap b){
        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        b.compress(Bitmap.CompressFormat.PNG, 100, byteArrayOutputStream);
        byte[] byteArray = byteArrayOutputStream .toByteArray();
        String encoded = Base64.encodeToString(byteArray, Base64.DEFAULT);
        String icon_base64 = encoded;

        return icon_base64;
    }
}
