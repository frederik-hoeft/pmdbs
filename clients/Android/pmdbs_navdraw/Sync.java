package com.example.rodaues.pmdbs_navdraw;

import android.os.Handler;
import android.os.Looper;
import android.provider.ContactsContract;

import java.sql.DatabaseMetaData;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.Callable;

public class Sync {



    public static void initialize(String[] params){

        DataBaseHelper dbhelper = DataBaseHelper.GetInstance();
        Boolean refresh = false;
        String remoteHeader = params[0];
        String deletedItem = params[1];

        if(!deletedItem.equals("deleted%eq![]!")){
            String cleanedDeletedItem = deletedItem.replace("deleted%eq![('","").replace("')]!","");
            String[] deletedItems = cleanedDeletedItem.split("'\\),\\('");


            List<String> hids = dbhelper.str_list_execSQL("SELECT D_hid FROM Tbl_data");
            for(int i=0;i<deletedItems.length;i++){
                if(hids.contains(deletedItems[i]) && !deletedItems[i].isEmpty()){
                    dbhelper.execSQL(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"DELETE FROM Tbl_data WHERE D_hid = \"",deletedItems[i],"\";"}));
                    refresh = true;
                }
            }
        }


        List<List<String>> localHeaders = dbhelper.getDataAs2DList("SELECT D_hid, D_datetime, D_id FROM Tbl_data;",3);
        List<String> accountsToGet = new ArrayList<>();
        List<String> accountsToUpdate = new ArrayList<>();
        List<String> accountsToDelete = new ArrayList<>();

        if(!remoteHeader.equals("headers%eq![]!")){
            String cleanedRemoteHeader = remoteHeader.replace("headers%eq![('","").replace("')]!","");
            String[] splittedRemoteHeader = cleanedRemoteHeader.split("'\\),\\('");

            List<String[]> remoteHeaders = new ArrayList<>();
            for(int i=0;i<splittedRemoteHeader.length;i++){
                remoteHeaders.add(splittedRemoteHeader[i].split("','"));
            }

            List<String[]> temp_remoteHeaders = new ArrayList<>();

            for(int i=0;i<remoteHeaders.size();i++){
                temp_remoteHeaders.add(remoteHeaders.get(i));
            }

            for(int i=0;i<temp_remoteHeaders.size();i++){
                String remoteHid = temp_remoteHeaders.get(i)[0];
                int remoteTimeStamp = Integer.parseInt(temp_remoteHeaders.get(i)[1]);

                List<List<String>> temp_localHeaders = new ArrayList<>();

                for(int j=0;j<localHeaders.size();j++){
                    temp_localHeaders.add(localHeaders.get(j));
                }

                for (int j=0;j<temp_localHeaders.size();j++){
                    String localHid = temp_localHeaders.get(j).get(0);
                    int localTimeStamp = Integer.parseInt(temp_localHeaders.get(j).get(1));
                    if(remoteHid.equals(localHid)){
                        if(remoteTimeStamp>localTimeStamp){
                            accountsToGet.add(remoteHid);
                        }
                        else if(remoteTimeStamp != localTimeStamp){
                            accountsToUpdate.add(localHeaders.get(j).get(2));
                        }
                        localHeaders.remove(temp_localHeaders.get(j));
                        remoteHeaders.remove(temp_remoteHeaders.get(i));
                    }

                }
            }
            for(int i=0;i<remoteHeaders.size();i++){
                String hid = remoteHeaders.get(i)[0];
                List<String> checkExists = dbhelper.getSingleEntryAsList(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"SELECT EXISTS (SELECT 1 FROM Tbl_delete WHERE DEL_hid = \"",hid,"\" LIMIT 1);"}));

                Boolean isDeleted = Integer.parseInt(checkExists.get(0))==1;
                if(isDeleted){
                    accountsToDelete.add(hid);
                }
                else{
                    accountsToGet.add(hid);
                }
            }

        }


        globalVARpool.countedPackets=0;
        globalVARpool.expectedPacketCount=accountsToUpdate.size()+accountsToGet.size()+localHeaders.size()+(accountsToDelete.size()>0?1:0);

        globalVARpool.countSyncPackets=globalVARpool.expectedPacketCount>0;

        if(accountsToDelete.size()>0){

            NetworkAdapter.MethodProvider.Delete(accountsToDelete);
        }

        for(int i=0;i<localHeaders.size();i++){
            String id = localHeaders.get(i).get(2);
            List<String> account = dbhelper.getSingleEntryAsList(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"SELECT D_id, D_host, D_url, D_uname, D_password, D_email, D_notes, D_icon, D_hid, D_datetime FROM Tbl_data WHERE D_id = ",id," LIMIT 1;"}));
            NetworkAdapter.MethodProvider.Insert(account);
        }
        for(int i=0;i<accountsToUpdate.size();i++){

            List<String> account = dbhelper.getSingleEntryAsList(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"SELECT D_id, D_host, D_url, D_uname, D_password, D_email, D_notes, D_icon, D_hid, D_datetime FROM Tbl_data WHERE D_id = ",accountsToUpdate.get(i)," LIMIT 1;"}));
            NetworkAdapter.MethodProvider.Update(account);
        }

        if(accountsToGet.size()>0){
            NetworkAdapter.MethodProvider.Select(accountsToGet);
        }
        if(globalVARpool.expectedPacketCount==0){
            globalVARpool.countSyncPackets=false;
            List<AutomatedTaskFramework.Task> scheduledTasks = AutomatedTaskFramework.Tasks.DeepCopy();
            AutomatedTaskFramework.Tasks.Clear();
            AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.NetworkTask, AutomatedTaskFramework.SearchCondition.In, "LOGGED_OUT|NOT_LOGGED_IN", new Runnable() {
                @Override
                public void run() {
                    NetworkAdapter.MethodProvider.Logout();
                }
            });

            AutomatedTaskFramework.TaskFactory.Create(AutomatedTaskFramework.TaskType.FireAndForget, new Runnable() {
                @Override
                public void run() {
                    try {
                        NetworkAdapter.MethodProvider.Disconnect();
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            });

            for(int i=1;i<scheduledTasks.size();i++){
                AutomatedTaskFramework.Tasks.Schedule(scheduledTasks.get(i));
            }

            AutomatedTaskFramework.Tasks.Execute();

            if(refresh){
                dbhelper.updateDATASET();

                //TODO STOP SYNC ANIMATION!!!
                new Handler(Looper.getMainLooper()).post(new Runnable() {
                    @Override
                    public void run() {
                        //TODO try catch
                        tab_saveddata.refreshListviewData();
                        globalVARpool.fab_sync.show();
                    }
                });
            }
        }


    }

    public static void finish(){
        DataBaseHelper db = DataBaseHelper.GetInstance();

        for(int i=0;i<globalVARpool.selectedAccounts.size();i++){
            String[] account = new String[] { "host%eq", "url%eq", "uname%eq", "password%eq", "email%eq", "notes%eq", "icon%eq", "hid%eq", "datetime%eq" };
            String[] values = new String[] { null, null, null, null, null, null, null, null, null };
            String[] accountParts = globalVARpool.selectedAccounts.get(i).split(";");


            try{
                for(int j=0;j<accountParts.length;j++){
                    if(accountParts[j].equals("mode%eq!SELECT!"))continue;

                    for(int k = 0; k<account.length;k++){
                        if(accountParts[j].contains(account[k])){
                            values[k] = DataBaseHelper.Security.SQLInjectionCheck(accountParts[j].split("!")[1]);
                        }
                    }
                }
            }catch (IndexOutOfBoundsException e){
                e.printStackTrace();
            }

            if(Arrays.asList(values).contains(null)){
                System.out.println("NULL-VALUE in SYNC-VALUES !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                continue;
            }

            for(int j=0;j<account.length;j++){
                account[j] =  DataBaseHelper.Security.SQLInjectionCheck(account[j].replace("%eq",""));
            }
            String checkHIDexists=db.execSQL("SELECT EXISTS(SELECT 1 FROM Tbl_data WHERE D_hid = \""+values[7]+"\");");

            if(Integer.valueOf(checkHIDexists)==1){
                String query = "UPDATE Tbl_data SET ";
                Boolean isFirstValue = true;
                try{
                    for(int j=0;j<account.length;j++){
                        if(isFirstValue){
                            query+="D_"+account[j]+"=\""+values[j]+"\"";
                            isFirstValue=false;
                        }
                        else{
                            query+=",D_"+account[j]+"=\""+values[j]+"\"";
                        }
                    }
                }catch(IndexOutOfBoundsException e){
                    e.printStackTrace();
                }
                query+=" WHERE D_hid = \""+values[7]+"\";";
                db.modifyData(query);
            }
            else{
                String query = "INSERT INTO Tbl_data (";
                Boolean isFirst = true;
                try
                {
                    for (int j = 0; j < account.length; j++)
                    {
                        if (isFirst)
                        {
                            query += "D_" + account[j];
                            isFirst = false;
                        }
                        else
                        {
                            query += ", D_" + account[j];
                        }
                    }
                }
                catch (IndexOutOfBoundsException e)
                {
                    e.printStackTrace();
                }
                query += ") VALUES (";
                isFirst = true;
                try
                {
                    for (int j = 0; j < values.length; j++)
                    {
                        if (isFirst)
                        {
                            query += "\"" + values[j] + "\"";
                            isFirst = false;
                        }
                        else
                        {
                            query += ", \"" + values[j] + "\"";
                        }
                    }
                }
                catch (IndexOutOfBoundsException e)
                {

                    e.printStackTrace();
                }
                query += ");";
                db.modifyData(query);
            }
        }

        globalVARpool.selectedAccounts.clear();
        reloadData();

    }
    public static void setHid(String[] params){

        String local_id = "";
        String hid = "";
        for(int i=0; i<params.length;i++){
            if(params[i].contains("local_id")){
                local_id = params[i].split("!")[1];
            }
            else if(params[i].contains("hashed_id")){
                hid = params[i].split("!")[1];
            }
        }
        if(Arrays.asList(new String[]{local_id,hid}).contains("")){
            System.out.println("MISSING PARAMETER IN setHID()");
            return;
        }

        DataBaseHelper db = DataBaseHelper.GetInstance();
        db.modifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"UPDATE Tbl_data SET D_hid = \"",hid,"\" WHERE D_id = ",local_id,";"}));

    }
    public static void reloadData(){

        DataBaseHelper db = DataBaseHelper.GetInstance();
        db.updateDATASET();

        new Handler(Looper.getMainLooper()).post(new Runnable() {
            @Override
            public void run() {
                //TODO try catch
                tab_saveddata.refreshListviewData();
                if(globalVARpool.fab_sync!=null){
                    globalVARpool.fab_sync.show();
                }
            }
        });
    }
}
