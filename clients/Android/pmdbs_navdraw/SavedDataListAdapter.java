package com.example.rodaues.pmdbs_navdraw;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.os.Build;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
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
        ImageView circlecrop = view.findViewById(R.id.circle_crop);

        Bitmap inputBitmap = currentSavedData.getIcon();

        try{
            int p1 = inputBitmap.getPixel(0,0);
            int p2 = inputBitmap.getPixel(inputBitmap.getWidth()-1,0);
            int p3 = inputBitmap.getPixel(0,inputBitmap.getHeight()-1);
            int p4 = inputBitmap.getPixel(inputBitmap.getWidth()-1,inputBitmap.getHeight()-1);


            if(p1==p2 && p1==p3 && p1==p4 && p1!= Color.TRANSPARENT)   {
                circlecrop.setVisibility(View.VISIBLE);
            }
        }catch(Exception e){

        }


        if(inputBitmap!=null){
            Toast.makeText(getContext(),"input!=null",Toast.LENGTH_SHORT);
            iv_item_icon.setImageBitmap(inputBitmap);
        }
        else {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
                iv_item_icon.setImageDrawable(getContext().getDrawable(R.drawable.ic_person_black_24dp));
            }
        }



        return view;
    }

}
