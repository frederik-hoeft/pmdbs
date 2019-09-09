
package com.example.administrator.grwrandomselector;


import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;



public class DataBaseHelper extends SQLiteOpenHelper {
    public static final String DATABASE_NAME = "items.db";
    public static final String DATABASE_TABLE_NAME = "item_table";
    public static final String DATABASE_COL_ITEM_ID = "item_id";
    public static final String DATABASE_COL_ITEM_NAME = "item_name";
    public static final String DATABASE_COL_ITEM_GROUP = "item_group";


    public DataBaseHelper(Context context) {
        super(context, DATABASE_NAME, null, 1);
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
        db.execSQL("create table " + DATABASE_TABLE_NAME + " ("+DATABASE_COL_ITEM_ID+" INTEGER PRIMARY KEY AUTOINCREMENT,"+DATABASE_COL_ITEM_NAME+" TEXT,"+DATABASE_COL_ITEM_GROUP+" TEXT)");
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int i, int i1) {
        db.execSQL("DROP TABLE IF EXISTS "+DATABASE_TABLE_NAME);
        onCreate(db);
    }

    public boolean insertData(String PrimName, String PrimGroup) {
        SQLiteDatabase db = this.getWritableDatabase();
        ContentValues contentValues = new ContentValues();
        contentValues.put(DATABASE_COL_ITEM_NAME,PrimName);
        contentValues.put(DATABASE_COL_ITEM_GROUP,PrimGroup);
        long result = db.insert(DATABASE_TABLE_NAME,null, contentValues);
        if(result == -1)
            return false;
        else
            return true;
    }

    public Cursor getAllData(String searchEntry) {
        SQLiteDatabase db = this.getWritableDatabase();
        Cursor allResult;
        if (searchEntry.equals("")){
            allResult = db.rawQuery("select * from " + DATABASE_TABLE_NAME, null);
        }else{
            allResult = db.rawQuery("select * from " + DATABASE_TABLE_NAME + " where " + DATABASE_COL_ITEM_ID + " like \"%" + searchEntry + "%\" or " + DATABASE_COL_ITEM_NAME + " like \"%" + searchEntry + "%\" or " + DATABASE_COL_ITEM_GROUP + " like \"%" + searchEntry + "%\"", null);
        }
        return allResult;
    }

    public Cursor getRndData(String selGroup, String grpContains) {
        SQLiteDatabase db = this.getWritableDatabase();
        Cursor rndResult;
        if (selGroup.equals("") && grpContains.equals("")) {
            rndResult = db.rawQuery("select " + DATABASE_COL_ITEM_NAME + " from " + DATABASE_TABLE_NAME + " order by random() limit 1", null);
            return rndResult;
        }else if (!selGroup.equals("") && grpContains.equals("")){
            rndResult = db.rawQuery("select " + DATABASE_COL_ITEM_NAME + " from " + DATABASE_TABLE_NAME + " where " + DATABASE_COL_ITEM_GROUP + " = \"" + selGroup + "\" order by random() limit 1", null);
            return rndResult;
        }else{
            rndResult = db.rawQuery("select " + DATABASE_COL_ITEM_NAME + " from " + DATABASE_TABLE_NAME + " where " + DATABASE_COL_ITEM_GROUP + " = \"" + selGroup + "\" or " + DATABASE_COL_ITEM_GROUP + " like \"%" + grpContains + "%\" order by random() limit 1", null);
            return rndResult;
        }
    }

    public void delSingleData(String singleDelete){
        SQLiteDatabase db = this.getWritableDatabase();
        db.execSQL("delete from " + DATABASE_TABLE_NAME + " where " + DATABASE_COL_ITEM_NAME + " = \"" + singleDelete + "\"");
    }

    public void delGroupData(String groupDelete){
        SQLiteDatabase db = this.getWritableDatabase();
        db.execSQL("delete from " + DATABASE_TABLE_NAME + " where " + DATABASE_COL_ITEM_GROUP + " = \"" + groupDelete + "\"");
    }

    public Cursor checkGroup(String groupCheck){
        SQLiteDatabase db = this.getWritableDatabase();
        Cursor checkResult = db.rawQuery("select " + DATABASE_COL_ITEM_NAME + " from " + DATABASE_TABLE_NAME + " where " + DATABASE_COL_ITEM_GROUP + " = \"" + groupCheck + "\" limit 1", null);
        return checkResult;
    }

    public Cursor checkSingle(String singleCheck, String groupCheck){
        SQLiteDatabase db = this.getWritableDatabase();
        Cursor checkResult;
        if (groupCheck.equals("")){
            checkResult = db.rawQuery("select " + DATABASE_COL_ITEM_NAME + " from " + DATABASE_TABLE_NAME + " where " + DATABASE_COL_ITEM_NAME + " = \"" + singleCheck + "\" limit 1", null);
        }else {
            checkResult = db.rawQuery("select " + DATABASE_COL_ITEM_NAME + " from " + DATABASE_TABLE_NAME + " where " + DATABASE_COL_ITEM_NAME + " = \"" + singleCheck + "\" and " + DATABASE_COL_ITEM_GROUP + " = \"" + groupCheck + "\" limit 1", null);
        }
        return checkResult;
    }
}
