package com.example.rodaues.pmdbs_navdraw;

import android.graphics.Bitmap;

public class SavedDataListItem {
    private String host;
    private String addit_text;
    private Bitmap icon;


    public SavedDataListItem(final String host, final String date, final Bitmap icon) {
        this.host = host;
        this.addit_text = date;
        this.icon = icon;

    }

    public String getHost() {
        return host;
    }

    public Bitmap getIcon(){
        return icon;
    }

    public String getAddit_text() {
        return addit_text;
    }

}
