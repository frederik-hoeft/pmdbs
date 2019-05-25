package com.rodaues.pmdbs_androidclient;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.widget.Toast;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.ArrayList;

public class DataBaseHelper extends SQLiteOpenHelper {

    private static String DB_PATH="";
    private static String DB_NAME="localdata.db";
    private static String DB_TBLDATA_NAME="Tbl_data";
    private static String DB_TBLDATA_COL_ID="D_id";
    private static String DB_TBLDATA_COL_HOST="D_host";
    private static String DB_TBLDATA_COL_URL="D_url";
    private static String DB_TBLDATA_COL_UNAME="D_uname";
    private static String DB_TBLDATA_COL_PASSW="D_password";
    private static String DB_TBLDATA_COL_EMAIL="D_email";
    private static String DB_TBLDATA_COL_NOTES="D_notes";
    private static String DB_TBLDATA_COL_DTIME="D_datetime";
    private static String DB_TBLDATA_COL_ICON="D_icon";
    private static String DB_TBLUSER_NAME_="Tbl_user";
    private static String DB_TBLUSER_COL_ID="U_id";
    private static String DB_TBLUSER_COL_PASSWORD="U_id";




    private SQLiteDatabase mDataBase;
    private Context mContext = null;
    private Cursor c = null;

    public DataBaseHelper(Context context) {
        super(context, DB_NAME, null, 1);

        DB_PATH = context.getApplicationInfo().dataDir+"/databases/";
        mContext = context;

    }

    //----------------------------------CHECK DATABASE
    @Override
    public synchronized void close() {
        if(mDataBase != null)
            mDataBase.close();
        super.close();
    }

    private boolean checkDataBase(){
        SQLiteDatabase tempDB = null;
        try{
            String path = DB_PATH+DB_NAME;
            tempDB = SQLiteDatabase.openDatabase(path,null,SQLiteDatabase.OPEN_READWRITE);
        } catch (Exception ex){
            msg(ex.toString());
        }
        if(tempDB != null)
            tempDB.close();
        return tempDB != null;
    }

    public void copyDataBase(){
        try{
            InputStream myInput = mContext.getAssets().open(DB_NAME);
            String outputFileName = DB_PATH+DB_NAME;
            OutputStream myOutput = new FileOutputStream(outputFileName);
            byte[] buffer = new byte[1024];
            int length;
            while((length=myInput.read(buffer))>0){
                myOutput.write(buffer,0,length);
            }
            myOutput.flush();
            myOutput.close();
            myInput.close();
        } catch(IOException e){
            msg(e.toString());
            e.printStackTrace();
        }
    }

    public void openDataBase(){
        String path = DB_PATH+DB_NAME;
        mDataBase = SQLiteDatabase.openDatabase(path,null,SQLiteDatabase.OPEN_READWRITE);
    }

    public void createDataBase(){
        boolean isDBExist = checkDataBase();
        if (!isDBExist) {

            this.getWritableDatabase();
            try{
                copyDataBase();
            }
            catch(Exception ex){
                msg("CREATING DATABASE FAILED");
            }
        }
    }
    //----------------------------------CHECK DATABASE


    public ArrayList<String> data_getIDs(){
        ArrayList<String> IDs = new ArrayList<String>();
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        c=db.rawQuery("select * from Tbl_data;",null);

        try{
            while(c.moveToNext()){
                IDs.add(String.valueOf(c.getInt(0)));
            }
        }
        catch(Exception e){
        }
        return IDs;
    }

    public Boolean checkPassword(String inputpw){
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        c=db.rawQuery("select * from Tbl_commonPasswords;",null);

        ArrayList<String> alist_commonPasswords = new ArrayList<>();

        try{
            while(c.moveToNext()){
                String new_pw = c.getString(1);
                alist_commonPasswords.add(new_pw);
            }
        }
        catch(Exception e){
            msg(e.toString());
        }

        String[] sarray_commonpasswords = new String[1000];
        alist_commonPasswords.toArray(sarray_commonpasswords);

        Boolean securepw = true;
        for(int i=0;i<1000;i++){
            String commonpw = sarray_commonpasswords[i];
            if(commonpw.trim().equals(inputpw)){
                securepw = false;
                break;
            }
        }

        return  securepw;
    }

    public ArrayList<ArrayList<String>> data_getAllData(){
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        c=db.rawQuery("select * from "+DB_TBLDATA_NAME+";",null);
        ArrayList<ArrayList<String>> data_outerlist = new ArrayList<>();

        try{
            while(c.moveToNext()){

                String host = c.getString(1);
                String url = c.getString(2);
                String uname = c.getString(3);
                String passw = c.getString(4);
                String email = c.getString(5);
                String notes = c.getString(6);


                ArrayList<String> subList = new ArrayList<String>();


                    subList.add(0,String.valueOf(c.getInt(0)));
                    subList.add(1,CryptoHelper.decrypt(host));
                    subList.add(2,CryptoHelper.decrypt(url));
                    subList.add(3,CryptoHelper.decrypt(uname));
                    subList.add(4,CryptoHelper.decrypt(passw));
                    subList.add(5,CryptoHelper.decrypt(email));
                    subList.add(6,CryptoHelper.decrypt(notes));
                    subList.add(7,c.getString(7));
                    subList.add(8,c.getString(8));
                    subList.add(9,c.getString(9));

                data_outerlist.add(subList);
            }
        }
        catch(Exception e){
            msg(e.toString());
        }

        return data_outerlist;
    }

    public int data_addrow(ArrayList<String> newData){

        Boolean isInserted = false;
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        long insertvalue = 0;

        try{
            String new_host = CryptoHelper.encrypt(newData.get(0));
            String new_url = CryptoHelper.encrypt(newData.get(1));
            String new_uname = CryptoHelper.encrypt(newData.get(2));
            String new_passw = CryptoHelper.encrypt(newData.get(3));
            String new_email = CryptoHelper.encrypt(newData.get(4));
            String new_notes = CryptoHelper.encrypt(newData.get(5));
            String new_dtime = newData.get(6);
            String new_B64bitmap = newData.get(7);

            ContentValues cV = new ContentValues();
            cV.put(DB_TBLDATA_COL_HOST,new_host);
            cV.put(DB_TBLDATA_COL_URL,new_url);
            cV.put(DB_TBLDATA_COL_UNAME,new_uname);
            cV.put(DB_TBLDATA_COL_PASSW,new_passw);
            cV.put(DB_TBLDATA_COL_EMAIL,new_email);
            cV.put(DB_TBLDATA_COL_NOTES,new_notes);
            cV.put(DB_TBLDATA_COL_DTIME,new_dtime);
            cV.put(DB_TBLDATA_COL_ICON,new_B64bitmap);

            insertvalue = db.insert(DB_TBLDATA_NAME,null,cV);

            isInserted = insertvalue>0;

            if(!isInserted) return -1;
        }
        catch(Exception e){
            msg(e.toString());
             return -1;
        }

        return 0;
    }
    public void user_setpassword(String hashedBCryptPW){
        ContentValues cV = new ContentValues();
    }

    public String data_updaterow(ArrayList<String> updatedData){

        SQLiteDatabase db = this.getWritableDatabase();
        ContentValues cV = new ContentValues();
        try{
            String upd_host = CryptoHelper.encrypt(updatedData.get(0));
            String upd_url = CryptoHelper.encrypt(updatedData.get(1));
            String upd_uname = CryptoHelper.encrypt(updatedData.get(2));
            String upd_passw = CryptoHelper.encrypt(updatedData.get(3));
            String upd_email = CryptoHelper.encrypt(updatedData.get(4));
            String upd_notes = CryptoHelper.encrypt(updatedData.get(5));
            String upd_dtime = updatedData.get(6);

            cV.put(DB_TBLDATA_COL_HOST,upd_host);
            cV.put(DB_TBLDATA_COL_URL,upd_url);
            cV.put(DB_TBLDATA_COL_UNAME,upd_uname);
            cV.put(DB_TBLDATA_COL_PASSW,upd_passw);
            cV.put(DB_TBLDATA_COL_EMAIL,upd_email);
            cV.put(DB_TBLDATA_COL_NOTES,upd_notes);
            cV.put(DB_TBLDATA_COL_DTIME,upd_dtime);

            db.update(DB_TBLDATA_NAME,cV,"D_id="+updatedData.get(7),null);

            return "DATA UPDATED - ACC: "+updatedData.get(0);
        }
        catch(Exception e){
            return "ERR: "+e.toString();
        }
    }

    public void data_deleterow(String ID){
        SQLiteDatabase db = this.getWritableDatabase();

        db.delete(DB_TBLDATA_NAME,DB_TBLDATA_COL_ID+"="+ID,null);
    }

    public void data_deleterows(ArrayList<String> IDs){
        SQLiteDatabase db = this.getWritableDatabase();

        for(int i=0;i<IDs.size();i++){

            String ID = IDs.get(i);
            db.delete(DB_TBLDATA_NAME,DB_TBLDATA_COL_ID+"="+ID,null);
        }
       // Toast.makeText(mContext,"DELETING SUCCESSFULL!", Toast.LENGTH_SHORT).show();
    }



    private void msg(String s){
        Toast.makeText(mContext,s,Toast.LENGTH_LONG).show();
    }
    @Override
    public void onCreate(SQLiteDatabase db) { }
    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) { }

}
