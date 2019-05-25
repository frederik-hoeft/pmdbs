package com.rodaues.pmdbs_androidclient;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.design.widget.Snackbar;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import java.util.List;

public class SavedDataListAdapter extends ArrayAdapter<SavedDataListItem> {



    public SavedDataListAdapter(@NonNull Context context, List<SavedDataListItem> objects) {
        super(context, 0, objects);
        }

    @Override
    public View getView(final int position, @Nullable View convertView, @NonNull ViewGroup parent) {

        SavedDataListItem currentSavedData = getItem(position);

        View view = convertView;


        if(view == null){
            view = LayoutInflater.from(getContext()).inflate(R.layout.saveddatalist_listitem, parent, false);
        }

        TextView tv_host = (TextView) view.findViewById(R.id.tv_host);
        tv_host.setText(currentSavedData.getHost());
        ((TextView) view.findViewById(R.id.listitem_additionalfield)).setText(currentSavedData.getAddit_text());


        ImageView iv_item_icon = view.findViewById(R.id.iv_listviewitem_icon);

        Bitmap inputBitmap = currentSavedData.getIcon();

        if(inputBitmap!=null){
            Toast.makeText(getContext(),"input!=null",Toast.LENGTH_SHORT);
            iv_item_icon.setImageBitmap(inputBitmap);
        }
        else {
            iv_item_icon.setImageDrawable(getContext().getDrawable(R.drawable.ic_person_black_24dp));
        }



        return view;
    }

}
