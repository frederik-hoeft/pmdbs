package com.example.rodaues.pmdbs_navdraw;

import android.graphics.Bitmap;

public class SavedDataListItem {
    private String host;
    private String addit_text;
    private Bitmap icon;
    private String pw_grade;


    public SavedDataListItem(final String host, final String addit_text, final Bitmap icon, final String pw_grade) {
        this.host = host;
        this.addit_text = addit_text;
        this.icon = icon;
        this.pw_grade = pw_grade;

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

    public String getPw_grade(){return pw_grade;}

}
