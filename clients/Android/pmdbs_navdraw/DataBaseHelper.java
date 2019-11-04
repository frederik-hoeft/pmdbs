package com.example.rodaues.pmdbs_navdraw;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.ArrayList;
import java.util.List;

public class DataBaseHelper extends SQLiteOpenHelper {
    // --------------SINGLETON DESIGN PATTERN START
    private static DataBaseHelper dbhelper = null;

    private DataBaseHelper(Context mContext) {
        super(mContext, DB_NAME, null, 21);

        DB_PATH = mContext.getApplicationInfo().dataDir+"/databases/";
        this.mContext = mContext;
    }

    public static DataBaseHelper GetInstance()
    {
        return dbhelper;
    }

    public static void createInstance(Context context){

        dbhelper = new DataBaseHelper(context);
    }
    // --------------SINGLETON DESIGN PATTERN END
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
    private static String DB_TBLUSER_NAME="Tbl_user";
    private static String DB_TBLUSER_COL_ID="U_id";
    private static String DB_TBLUSER_COL_UNAME="U_username";
    private static String DB_TBLUSER_COL_NNAME="U_nickname";
    private static String DB_TBLUSER_COL_PASSWORD="U_password";
    private static String DB_TBLUSER_COL_WONLINE="U_wasOnline";
    private static String DB_TBLUSER_COL_FUSAGE="U_firstUsage";
    private static String DB_TBLUSER_COL_EMAIL="U_email";
    private static String DB_TBLUSER_COL_COOKIE="U_cookie";



    private SQLiteDatabase mDataBase;
    private Context mContext = null;
    private Cursor c = null;

    @Override
    public void onCreate(SQLiteDatabase db) {
        String query = "PRAGMA foreign_keys = off;BEGIN TRANSACTION;CREATE TABLE Tbl_data (    D_id       INTEGER PRIMARY KEY AUTOINCREMENT,    D_host     TEXT    NOT NULL,    D_url      TEXT,    D_uname    TEXT,    D_password TEXT    NOT NULL,    D_email    TEXT,    D_notes    TEXT,    D_hid      TEXT    DEFAULT EMPTY,    D_datetime TEXT    NOT NULL,    D_icon     TEXT);CREATE TABLE Tbl_delete (    DEL_id  INTEGER PRIMARY KEY AUTOINCREMENT,    DEL_hid TEXT);CREATE TABLE Tbl_settings (    S_id          INTEGER PRIMARY KEY AUTOINCREMENT,    S_server_ip   TEXT,    S_server_port TEXT,    S_auto_sync   INTEGER,    S_keep_alive  INTEGER);CREATE TABLE Tbl_user (    U_id         INTEGER PRIMARY KEY AUTOINCREMENT,    U_username   TEXT,    U_nickname   TEXT    DEFAULT User,    U_password   TEXT,    U_wasOnline  INTEGER DEFAULT (0),    U_firstUsage TEXT    NOT NULL,    U_email      TEXT,    U_cookie     TEXT,    U_privateKey TEXT,    U_publicKey  TEXT,    U_datetime   TEXT    DEFAULT (0) );COMMIT TRANSACTION;PRAGMA foreign_keys = on;";
        String[] queries = query.split(";");

        for(int i = 0;i<queries.length;i++){
            if(!queries[i].isEmpty())        db.execSQL(queries[i]+";");
        }

        System.out.println("ONCREATE!!!!!!!!!!!!!!!!");

    }
    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        db.execSQL("DROP TABLE IF EXISTS Tbl_data");
        db.execSQL("DROP TABLE IF EXISTS Tbl_user");
        db.execSQL("DROP TABLE IF EXISTS Tbl_delete");
        db.execSQL("DROP TABLE IF EXISTS Tbl_settings");

        onCreate(db);
    }
    //----------------------------------CHECK DATABASE


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
        /*
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
        }*/
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
        }finally {
            c.close();
            db.close();
        }
        return IDs;
    }


    public void updateDATASET(){
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
                String bitmap = c.getString(9);

                ArrayList<String> subList = new ArrayList<>();

                    subList.add(0,String.valueOf(c.getInt(0)));
                    subList.add(1,host.equals("\001")?"":CryptoHelper.AESDecrypt(host,globalVARpool.AESkey));
                    subList.add(2,url.equals("\001")?"":CryptoHelper.AESDecrypt(url,globalVARpool.AESkey));
                    subList.add(3,uname.equals("\001")?"":CryptoHelper.AESDecrypt(uname,globalVARpool.AESkey));
                    subList.add(4,passw.equals("\001")?"":CryptoHelper.AESDecrypt(passw,globalVARpool.AESkey));
                    subList.add(5,email.equals("\001")?"":CryptoHelper.AESDecrypt(email,globalVARpool.AESkey));
                    subList.add(6,notes.equals("\001")?"":CryptoHelper.AESDecrypt(notes,globalVARpool.AESkey));
                    subList.add(7,c.getString(7));
                    subList.add(8,c.getString(8));
                    subList.add(9,bitmap.equals("\001")?"":CryptoHelper.AESDecrypt(bitmap,globalVARpool.AESkey));

                data_outerlist.add(subList);
            }
        }
        catch(Exception e){
            msg(e.toString());
        }finally {
            c.close();
            db.close();
        }

        globalVARpool.dataSET = data_outerlist;
    }

    public void data_updateTBL(){


        deleteTblData();
        ArrayList<ArrayList<String>> dataset = globalVARpool.dataSET;

        for(int i=0;i<dataset.size();i++){
            ArrayList<String> aList_item = dataset.get(i);
            ArrayList<String> newData = new ArrayList<>();
            newData.add(aList_item.get(1));
            newData.add(aList_item.get(2));
            newData.add(aList_item.get(3));
            newData.add(aList_item.get(4));
            newData.add(aList_item.get(5));
            newData.add(aList_item.get(6));
            newData.add(aList_item.get(8));
            newData.add(aList_item.get(9));

            data_addrow(newData);
        }

        execSQL("UPDATE Tbl_data SET D_hid = \"EMPTY\";");
    }


    public String data_addrow(ArrayList<String> newData){

        Boolean isInserted = false;
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        long insertvalue = 0;
        String new_host="";
        String new_url="";
        String new_uname="";
        String new_passw="";
        String new_email="";
        String new_notes="";
        String new_dtime=newData.get(6);
        String new_B64bitmap="";

        try{
            new_host = newData.get(0).equals("")?"\001":CryptoHelper.AESEncrypt(newData.get(0),globalVARpool.AESkey);
            new_url = newData.get(1).equals("")?"\001":CryptoHelper.AESEncrypt(newData.get(1),globalVARpool.AESkey);
            new_uname = newData.get(2).equals("")?"\001":CryptoHelper.AESEncrypt(newData.get(2),globalVARpool.AESkey);
            new_passw = newData.get(3).equals("")?"\001":CryptoHelper.AESEncrypt(newData.get(3),globalVARpool.AESkey);
            new_email = newData.get(4).equals("")?"\001":CryptoHelper.AESEncrypt(newData.get(4),globalVARpool.AESkey);
            new_notes = newData.get(5).equals("")?"\001":CryptoHelper.AESEncrypt(newData.get(5),globalVARpool.AESkey);
            new_B64bitmap = newData.get(7).equals("")?"\001":CryptoHelper.AESEncrypt(newData.get(7),globalVARpool.AESkey);

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

            if(!isInserted) return "-1";
        }
        catch(Exception e){
                e.printStackTrace();
             return "-2";
        }finally {
            db.close();
        }
        updateDATASET();
        return String.valueOf(insertvalue);
    }

    public Integer user_setpassword(String hashedSCryptPW){
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        long insertvalue = 0;
        Integer rowValue = -1;
        ContentValues cV = new ContentValues();
        String inputPW = hashedSCryptPW;
        try{
            cV.put(DB_TBLUSER_COL_PASSWORD,inputPW);
           rowValue = db.update(DB_TBLUSER_NAME,cV,/*,DB_TBLUSER_COL_ID+"=1"*/"1=1",null);
        }catch(Exception e){
            msg(e.toString());
        }finally {
            db.close();
        }
        return rowValue;
    }

    public Integer user_setfUSAGE(String firstusage){
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        long insertvalue = 0;
        ContentValues cV = new ContentValues();

        Integer insertINT = -1;
        try{
            cV.put(DB_TBLUSER_COL_FUSAGE,firstusage);
            cV.put(DB_TBLUSER_COL_WONLINE,0);
            db.insert(DB_TBLUSER_NAME,null,cV);
        }catch(Exception e){
            msg(e.toString());
        }finally {

            db.close();
        }
        return insertINT;
    }

    public void user_setNNAME_EMAIL(String nickname, String email){
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        long insertvalue = 0;
        ContentValues cV = new ContentValues();
        try{
            cV.put(DB_TBLUSER_COL_NNAME,nickname);
            cV.put(DB_TBLUSER_COL_EMAIL,email);
            db.update(DB_TBLUSER_NAME,cV,DB_TBLUSER_COL_ID+"=1",null);
        }catch(Exception e){
            msg(e.toString());
        }finally {
            c.close();
            db.close();
        }
    }

    public boolean checkFirstUsage(){
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        c=db.rawQuery("select * from "+DB_TBLUSER_NAME+";",null);
        boolean firstUsage = false;

        try{
            while(c.moveToNext()){
                if(c.getString(5).equals("")) firstUsage=true;
            }
        }catch(Exception e){
            msg(e.toString());
        }finally {
            c.close();
            db.close();
        }

        return firstUsage;
    }

    public String user_getfUSAGE(){
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        c=db.rawQuery("select * from "+DB_TBLUSER_NAME+";",null);
        String fUSAGE = "";
        try{
            while(c.moveToNext()){
                fUSAGE = c.getString(5);
            }
        }catch(Exception e){
            msg(e.toString());
        }finally {
            c.close();
            db.close();
        }
        return fUSAGE;
    }

    public boolean user_wasOnline(){
        SQLiteDatabase db = this.getWritableDatabase();
        Boolean wasOnline = false;
        c = null;
        c=db.rawQuery("select U_wasOnline from "+DB_TBLUSER_NAME+";",null);
        while(c.moveToNext()){
            wasOnline =  c.getInt(0) == 1;
        }
        c.close();
        db.close();
        return wasOnline;
    }

    public String user_getMasterPW(){
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        c=db.rawQuery("select * from "+DB_TBLUSER_NAME+";",null);
        String masterPW = "";
        try{
            while(c.moveToNext()){
                masterPW = c.getString(3);
            }
        }catch(Exception e){
            msg(e.toString());
        }finally {
            c.close();
            db.close();
        }

        return masterPW;
    }
    public Integer user_getTableSize(){
        SQLiteDatabase db = this.getWritableDatabase();
        c = null;
        c=db.rawQuery("select * from "+DB_TBLUSER_NAME+";",null);
        ArrayList<Integer> aList = new ArrayList<>();
        try{
            while(c.moveToNext()){
                aList.add(c.getInt(0));
            }
        }catch(Exception e){
            msg(e.toString());
            System.out.println("getTableSizeERROR!!!!!!!!!!!!");
        }finally {
            c.close();
            db.close();
        }

        Integer lenght = aList.size();

        return lenght;
    }

    public String user_getCookie(){

        SQLiteDatabase db = this.getWritableDatabase();
        c=null;
        c=db.rawQuery("select "+DB_TBLUSER_COL_COOKIE+" from "+DB_TBLUSER_NAME + " Limit 1;",null);

        String cookie = "";
        try{
            while(c.moveToNext()){
                cookie = c.getString(0);
            }
        }catch(Exception e){}finally {
            c.close();
            db.close();
        }
        return cookie;
    }


    public void deletedatauser(){
        SQLiteDatabase db = this.getWritableDatabase();
        db.delete(DB_TBLDATA_NAME,"1=1",null);
        db.delete(DB_TBLUSER_NAME,"1=1",null);

        updateDATASET();
        db.close();
    }

    public void deleteTblData(){
        SQLiteDatabase db = this.getWritableDatabase();
        db.delete(DB_TBLDATA_NAME,"1=1",null);
        db.close();
    }


    public String data_updaterow(ArrayList<String> updatedData){
        SQLiteDatabase db = this.getWritableDatabase();
        ContentValues cV = new ContentValues();

        try{

            String upd_host = "";
            String upd_url = "";
            String upd_uname = "";
            String upd_passw = "";
            String upd_email = "";
            String upd_notes = "";
            String upd_dtime = updatedData.get(6);


            upd_host = updatedData.get(0).equals("")?"\001":CryptoHelper.AESEncrypt(updatedData.get(0),globalVARpool.AESkey);
            upd_url = updatedData.get(1).equals("")?"\001":CryptoHelper.AESEncrypt(updatedData.get(1),globalVARpool.AESkey);
            upd_uname = updatedData.get(2).equals("")?"\001":CryptoHelper.AESEncrypt(updatedData.get(2),globalVARpool.AESkey);
            upd_passw = updatedData.get(3).equals("")?"\001":CryptoHelper.AESEncrypt(updatedData.get(3),globalVARpool.AESkey);
            upd_email = updatedData.get(4).equals("")?"\001":CryptoHelper.AESEncrypt(updatedData.get(4),globalVARpool.AESkey);
            upd_notes = updatedData.get(5).equals("")?"\001":CryptoHelper.AESEncrypt(updatedData.get(5),globalVARpool.AESkey);
          /*  for(int i = 0;i<7;i++){
               String s=updatedData.get(i);
               if(s.equals("")){
                   switch(i){
                       case 0: upd_host = "\001"; break;
                       case 1: upd_url = "\001"; break;
                       case 2: upd_uname = "\001"; break;
                       case 3: upd_passw = "\001"; break;
                       case 4: upd_email = "\001"; break;
                       case 5: upd_notes = "\001"; break;
                   }
               }
               else{
                   switch(i){
                       case 0: upd_host = crypto.encrypt(updatedData.get(0)); break;
                       case 1: upd_url = crypto.encrypt(updatedData.get(1)); break;
                       case 2: upd_uname = crypto.encrypt(updatedData.get(2)); break;
                       case 3: upd_passw = crypto.encrypt(updatedData.get(3)); break;
                       case 4: upd_email = crypto.encrypt(updatedData.get(4)); break;
                       case 5: upd_notes = crypto.encrypt(updatedData.get(5)); break;
                   }
               }
           }*/

            cV.put(DB_TBLDATA_COL_HOST,upd_host);
            cV.put(DB_TBLDATA_COL_URL,upd_url);
            cV.put(DB_TBLDATA_COL_UNAME,upd_uname);
            cV.put(DB_TBLDATA_COL_PASSW,upd_passw);
            cV.put(DB_TBLDATA_COL_EMAIL,upd_email);
            cV.put(DB_TBLDATA_COL_NOTES,upd_notes);
            cV.put(DB_TBLDATA_COL_DTIME,upd_dtime);


            if(updatedData.size()>8){
                String upd_icon = CryptoHelper.AESEncrypt(updatedData.get(8),globalVARpool.AESkey);
                cV.put(DB_TBLDATA_COL_ICON,upd_icon);
            }
            db.update(DB_TBLDATA_NAME,cV,"D_id="+updatedData.get(7),null);

            updateDATASET();

            return "DATA UPDATED - ACC: "+updatedData.get(0);

        }
        catch(Exception e){
            return "ERR: "+e.toString();
        }
        finally {
            db.close();
        }


    }

    public void data_deleterow(String ID){
        SQLiteDatabase db = this.getWritableDatabase();

        db.delete(DB_TBLDATA_NAME,DB_TBLDATA_COL_ID+"="+ID,null);
        updateDATASET();
        db.close();
    }

    public void data_deleterows(ArrayList<String> IDs){
        SQLiteDatabase db = this.getWritableDatabase();

        for(int i=0;i<IDs.size();i++){

            String ID = IDs.get(i);
            db.delete(DB_TBLDATA_NAME,DB_TBLDATA_COL_ID+"="+ID,null);
        }
        updateDATASET();

        db.close();
    }

    private void msg(String s){
        System.out.println(s+"!!!!!!!!!!!!!!!!!!!!!");
    }

    public String execSQL(String query){
        SQLiteDatabase db = this.getWritableDatabase();

        c = null;
        c=db.rawQuery(query,null);
        String str_return = "";
        try{
            while(c.moveToNext()){
                str_return = c.getString(0);
            }
        }catch(Exception e){
            msg(e.toString());
        }
        c.close();
        db.close();
        return str_return;
    }

    public void modifyData(String query){
        SQLiteDatabase db = this.getWritableDatabase();
        db.rawQuery(query,null);
        db.close();
    }


    public List<String> getSingleEntryAsList(String query){
        SQLiteDatabase db = this.getWritableDatabase();

        c = null;
        c=db.rawQuery(query,null);
        List<String> list = new ArrayList<>();
        try{
            while(c.moveToNext()){
                for(int i=0;i<c.getColumnCount();i++){
                    list.add(c.getString(i));
                }
                break;
            }
        }catch(Exception e){
            msg(e.toString());
        }

        c.close();
        db.close();
        return list;


    }

    public List<String> str_list_execSQL(String query){
        SQLiteDatabase db = this.getWritableDatabase();

        c = null;
        c=db.rawQuery(query,null);
        List<String> list = new ArrayList<>();
        try{
            while(c.moveToNext()){
                list.add(c.getString(0));
            }
        }catch(Exception e){
            msg(e.toString());
        }

        c.close();
        db.close();
        return list;
    }

    public List<List<String>> getDataAs2DList(String query,int columns){
        SQLiteDatabase db = this.getWritableDatabase();

        c = null;
        c=db.rawQuery(query,null);

        List<List<String>> outerList = new ArrayList<>();
        try{
            while(c.moveToNext()){

                List<String> innerList = new ArrayList<>();
                for(int i=0;i<columns;i++){
                    innerList.add(c.getString(i));
                }
                outerList.add(innerList);

            }
        }catch(Exception e){
            msg(e.toString());
        }

        c.close();
        db.close();

        return outerList;
    }

    public static class Security{
        public static String SQLInjectionCheck(String unsafeString){
            return unsafeString.replace("\'","\'\'").replace("\"","\"\"");
        }

        public static String SQLInjectionCheckQuery(String[] unsafequery){
            int querylength = unsafequery.length;
            if(querylength==0 || querylength%2==0){
                throw new IllegalArgumentException("MUST BE AN ODD NUMBER OF ARGUMENTS!");
            }

            for(int i=1;i<querylength;i+=2){

                unsafequery[i] = SQLInjectionCheck(unsafequery[i]);
            }
            String query = "";
            for(int i = 0;i<querylength;i++){
                query += unsafequery[i];


            }
            return query;
        }
    }
}
