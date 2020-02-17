package com.example.rodaues.pmdbs_navdraw;

import android.content.Intent;
import android.os.Handler;
import android.os.Looper;

import java.util.List;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.spec.InvalidKeySpecException;
import java.util.ArrayList;
import java.util.concurrent.Callable;

public class PDTPClient
{
    // --------------SINGLETON DESIGN PATTERN START
    private static PDTPClient client = null;

    public boolean isLogged_in() {
        return logged_in;
    }

    private PDTPClient() {
        this.RemoteAddress = "th3fr3d.ddns.net";
        this.RemotePort = 4447;
    }

    public static PDTPClient GetInstance()
    {
        if(client == null)
        {
            client = new PDTPClient();
        }
        return client;
    }
    // --------------SINGLETON DESIGN PATTERN END
    private boolean logged_in =false;
    private boolean Connected = false;
    private boolean Debugging = true;
    private boolean ThreadKilled = false;
    private int ErrorCode = -1;
    private int RemotePort = -1;
    private String RemoteAddress = "NONE";
    private PrivateKey privateKey = null;

    private Socket Socket = null;
    private String HMACKey = "NONE";
    private String Nonce = "NONE";
    private String AESKey = "NONE";
    private String RSAPublicKey = "NONE";
    private String RSAPrivateKey = "NONE";
    private String RemoteRSAKey = "NONE";
    private DataOutputStream ServerInput = null;
    private InputStream ServerOutput = null;

    public boolean isConnected() {
        return Connected;
    }

    public String getRemoteAddress() {
        return RemoteAddress;
    }

    public void setRemoteAddress(String remoteAddress) {
        RemoteAddress = remoteAddress;
    }

    public int getRemotePort() {
        return RemotePort;
    }

    public void setRemotePort(int remotePort) {
        RemotePort = remotePort;
    }

    public int getErrorCode() {
        return ErrorCode;
    }

    public Socket getSocket() {
        return Socket;
    }

    public String getHMACKey() {
        return HMACKey;
    }

    public void setHMACKey(String hMACKey) {
        HMACKey = hMACKey;
    }

    public String getRSAPublicKey() {
        return RSAPublicKey;
    }

    public void setRSAPublicKey(String rSAPublicKey) {
        RSAPublicKey = rSAPublicKey;
    }

    public String getRSAPrivateKey() {
        return RSAPrivateKey;
    }

    public void setRSAPrivateKey(String rSAPrivateKey) {
        RSAPrivateKey = rSAPrivateKey;
    }

    public String getNonce() {
        return Nonce;
    }

    public String getAESKey() {
        return AESKey;
    }

    public void setRemoteRSAKey(String remoteRSAKey) {
        RemoteRSAKey = remoteRSAKey;
    }

    public void Connect() throws NoSuchAlgorithmException, IOException
    {
        this.ErrorCode = -1;
        String[] keypair = CryptoHelper.GenerateRSAKeys();
        this.RSAPrivateKey = keypair[0];
        this.RSAPublicKey = keypair[1];
        this.Socket = new Socket(RemoteAddress, RemotePort);
        this.Socket.setReuseAddress(true);
        this.ServerInput = new DataOutputStream(Socket.getOutputStream());
        this.ServerOutput = Socket.getInputStream();
        this.Connected = true;
        System.out.println("CONNECTED");
        System.out.println("Sending Client Hello ...");
        this.ServerInput.writeBytes("\1UINIPEM\4");
        try
        {
            // INITIALIZE 32 KB RECEIVE BUFFER FOR INCOMING DATA
            int bufferSize = 32768;
            List<Byte> buffer = new ArrayList<Byte>();
            this.Socket.setReceiveBufferSize(bufferSize);
            byte[] data = new byte[bufferSize];
            while(true)
            {
                boolean receiving = true;
                List<Byte[]> dataPackets = new ArrayList<Byte[]>();
                while (receiving)

                {
                    int connectionDropped = ServerOutput.read( data, 0, data.length );
                    if (connectionDropped == -1)
                    {
                        this.ThreadKilled = true;
                        throw new SocketException("RST");
                    }
                    data = ByteExtension.RemoveFromByteArray(data, (byte)0);
                    if (ByteExtension.ByteArrayContains(data, (byte)4) && buffer.size() == 0)
                    {
                        List<Byte[]> rawDataPackets = ByteExtension.SplitByteArray(data, (byte)4);
                        Byte[] lastDataPacket = rawDataPackets.get(rawDataPackets.size() - 1);
                        List<Byte[]> tempRawDataPackets = ByteExtension.DeepCopyByteList(rawDataPackets);
                        tempRawDataPackets.remove(tempRawDataPackets.size() - 1);
                        dataPackets = ByteExtension.DeepCopyByteList(tempRawDataPackets);
                        if (lastDataPacket.length != 0 && ByteExtension.ByteArrayContains(lastDataPacket, (byte)0))
                        {
                            buffer.addAll(ByteExtension.ByteArrayToList(lastDataPacket));
                        }
                        receiving = false;
                    }
                    else if (ByteExtension.ByteArrayContains(data, (byte)4) && buffer.size() != 0)
                    {
                        List<Byte[]> rawDataPackets = ByteExtension.SplitByteArray(data, (byte)4);
                        List<Byte> firstPacket = new ArrayList<Byte>();
                        firstPacket.addAll(buffer);
                        firstPacket.addAll(ByteExtension.ByteArrayToList(rawDataPackets.get(0)));
                        rawDataPackets.set(0, ByteExtension.ByteListToByteObjects(firstPacket));
                        buffer = new ArrayList<Byte>();
                        Byte[] lastDataPacket = rawDataPackets.get(rawDataPackets.size() - 1);
                        List<Byte[]> tempRawDataPackets = ByteExtension.DeepCopyByteList(rawDataPackets);
                        tempRawDataPackets.remove(tempRawDataPackets.size() - 1);
                        dataPackets = ByteExtension.DeepCopyByteList(tempRawDataPackets);
                        if (lastDataPacket.length != 0 && ByteExtension.ByteArrayContains(lastDataPacket, (byte)0))
                        {
                            buffer.addAll(ByteExtension.ByteArrayToList(lastDataPacket));
                        }
                        receiving = false;
                    }
                    else
                    {
                        buffer.addAll(ByteExtension.ByteArrayToList(data));
                    }
                    data = new byte[bufferSize];
                }

                for (int i = 0; i < dataPackets.size(); i++)
                {
                    List<Byte> dataPacket = ByteExtension.ByteArrayToList(dataPackets.get(i));

                    if (!dataPacket.contains((byte)1))
                    {
                        System.out.println("[ERRNO 02] ISOH: Received invalid packet from server.");
                        continue;
                    }
                    else if (ByteExtension.ByteListCountBytes(dataPacket, (byte)1) == 1)
                    {
                        dataPacket.subList(0, dataPacket.indexOf((byte)1)).clear();
                    }
                    else
                    {
                        System.out.println("[ERRNO 02] ISOH: Received invalid packet from server.");
                        continue;
                    }
                    dataPacket.remove(0);
                    String dataString = new String(ByteExtension.ByteListToBytes(dataPacket), StandardCharsets.UTF_8);
                    char packetSpecifier = dataString.charAt(0);
                    switch(packetSpecifier)
                    {
                        case 'U':
                        {
                            String packetID = dataString.substring(1, 4);
                            switch (packetID)
                            {
                                case "FIN":
                                {
                                    try
                                    {
                                        this.Socket.shutdownInput();
                                        this.Socket.shutdownOutput();
                                        this.Socket.close();
                                        this.ErrorCode = 0;
                                    }
                                    finally {
                                        this.Connected = false;
                                        this.logged_in = false;
                                    }
                                    if (this.Debugging)
                                    {
                                        System.out.println("Disconnected");
                                        if(dataString.contains("message")){
                                            try{
                                                System.out.println("REASON: "+dataString.split("!")[1]);

                                            }catch(Exception e){
                                                e.printStackTrace();
                                            }
                                        }
                                    }

                                    return;
                                }
                                case "KEY":
                                {
                                    System.out.println("Received Server Hello ...");
                                    String packetSID = dataString.substring(4, 7);
                                    if (packetSID.equals("PEM"))
                                    {
                                        this.RemoteRSAKey = dataString.substring(7).split("!")[1];
                                    }
                                    else if(packetSID.equals("CRT")){
                                        Boolean successful = CryptoHelper.GetPublicKeyFromCertificate(dataString.substring(7).split("!")[1]);

                                        if(!successful){

                                            //TODO msg->ERROR INVALID FORMAT (CERTIFICATE)
                                            System.out.println("CERTIFICATE->INVALID FORMAT");
                                        }
                                    }
                                    else
                                    {
                                        System.out.println("RSA Key Format not supported.");
                                        return;
                                    }
                                    this.Nonce = CryptoHelper.RandomString(256);
                                    String encNonce = CryptoHelper.RSAEncrypt(this.Nonce);
                                    String message = "CKEformat%eq!PEM!;key%eq!" + this.RSAPublicKey + "!;nonce%eq!" + encNonce + "!;";
                                    System.out.println("Client Key Exchange ...");
                                    this.ServerInput.writeBytes("\1K" + message + "\4");
                                    break;
                                }
                                default:
                                {
                                    break;
                                }
                            }
                            break;
                        }
                        case 'K':
                        {
                            String decrypted = CryptoHelper.RSADecrypt(dataString.substring(1));
                            String packetID = decrypted.substring(0, 3);
                            if (packetID.equals("SKE"))
                            {
                                System.out.println("Symmetric Keys Exchange ...");
                                String key = "";
                                String providedNonce = "";
                                String[] parts = decrypted.substring(3).split(";");
                                for( int j = 0; j < parts.length; j++)
                                {
                                    if (parts[j].contains("key"))
                                    {
                                        key = parts[j].split("!")[1];
                                    }
                                    else if (parts[j].contains("nonce"))
                                    {
                                        providedNonce = parts[j].split("!")[1].replace("\0", "");
                                    }
                                }
                                if (this.Debugging)
                                {
                                    System.out.println("Provided nonce: " + providedNonce);
                                    System.out.println("Real nonce: " + this.Nonce);
                                }
                                if (!providedNonce.equals(this.Nonce))
                                {
                                    return;
                                }
                                this.AESKey = key;

                                this.HMACKey = CryptoHelper.SHA256(this.AESKey + this.Nonce);
                                if (this.Debugging)
                                {
                                    System.out.println("AES KEY: " + this.AESKey);
                                    System.out.println("NONCE: " + this.Nonce);
                                    System.out.println("HMAC KEY: " + this.HMACKey);
                                }
                                this.Nonce = CryptoHelper.RandomString(256);
                                System.out.println("Acknowledging ...");
                                Network.SendEncrypted("KEXACKnonce%eq!" + this.Nonce + "!;");
                            }
                            break;
                        }
                        case 'E':
                        {
                            if (!CryptoHelper.VerifyHMAC(this.HMACKey, dataString.substring(1)))
                            {
                                System.out.println("[ERRNO 31] IMAC: Received an invalid HMAC checksum.");
                            }
                            else if (this.Debugging)
                            {
                                System.out.println("HMAC OK!");
                            }
                            String decryptedData = CryptoHelper.AESDecrypt(dataString.substring(1, dataString.length() - 44), this.AESKey);
                            if (this.Debugging)
                            {
                                System.out.println("SERVER: " + decryptedData);
                            }
                            String packetID = decryptedData.substring(0, 3);
                            String packetSID = decryptedData.substring(3, 6);
                            AutomatedTaskFramework.DoNetworkTasks(decryptedData);
                            switch (packetID)
                            {
                                case "KEX":
                                {
                                    if (!packetSID.equals("ACK") || !decryptedData.substring(6).split("!")[1].equals(this.Nonce))
                                    {
                                        System.out.println("Invalid nonce");
                                        return;
                                    }
                                    else
                                    {
                                        System.out.println("Key Exchange finished.");
                                        if (globalVARpool.cookie == null)
                                        {
                                            NetworkAdapter.MethodProvider.GetCookie();
                                        }
                                        else
                                        {
                                            NetworkAdapter.MethodProvider.CheckCookie();
                                        }
                                        break;
                                    }
                                }
                                case "DTA":
                                {
                                    switch(packetSID)
                                    {
                                        case "CKI":
                                        {
                                            globalVARpool.cookie = decryptedData.substring(6).split("!")[1];
                                            DataBaseHelper dbhelper = DataBaseHelper.GetInstance();
                                            dbhelper.execSQL("UPDATE Tbl_user SET U_cookie = \""+ globalVARpool.cookie+ "\";");
                                            NetworkAdapter.MethodProvider.Authorize();
                                            break;
                                        }
                                        case "RET":
                                            try{
                                                String content = decryptedData.substring(6);
                                                String mode = "";
                                                String[] splitted_content = content.split(";");
                                                for(int j=0;j<splitted_content.length;j++){
                                                    if(splitted_content[j].contains("mode")){
                                                        mode = splitted_content[j];
                                                    }
                                                }
                                                if(mode.isEmpty()){
                                                    break;
                                                }
                                                switch(mode.split("!")[1]){
                                                    case "FETCH_SYNC":
                                                        String remoteHeader = "";
                                                        String deletedItem = "";
                                                        for(int j=0;j<splitted_content.length;j++){
                                                            if(splitted_content[j].contains("headers")){
                                                                remoteHeader=splitted_content[j];
                                                            }
                                                            else if(splitted_content[j].contains("deleted")){
                                                                deletedItem=splitted_content[j];
                                                            }
                                                        }
                                                        SyncThreadHelper synchelper = SyncThreadHelper.GetInstance(new String[]{remoteHeader,deletedItem});
                                                        Thread t = new Thread(synchelper);
                                                        t.start();
                                                        break;
                                                    case "INSERT":
                                                        final String[] params = content.split(";");

                                                        new Thread(){
                                                            public void run(){
                                                                Sync.setHid(params);
                                                            }
                                                        }.start();

                                                        globalVARpool.HID_ThreadCounter++;

                                                        if(globalVARpool.countSyncPackets){
                                                            globalVARpool.countedPackets++;
                                                        }
                                                        break;
                                                    case "DELETING_COMPLETE":
                                                    case "UDPATE":
                                                        if(globalVARpool.countSyncPackets) globalVARpool.countedPackets++;
                                                        break;
                                                    case "SELECT":
                                                        globalVARpool.selectedAccounts.add(content);
                                                        if(globalVARpool.countSyncPackets) globalVARpool.countedPackets++;
                                                        break;

                                                    default:
                                                        break;
                                                }
                                                if(globalVARpool.expectedPacketCount == globalVARpool.countedPackets && globalVARpool.countSyncPackets){
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

                                                    for(int j=1;j<scheduledTasks.size();j++){
                                                        AutomatedTaskFramework.Tasks.Schedule(scheduledTasks.get(j));
                                                    }

                                                    AutomatedTaskFramework.Tasks.Execute();

                                                    new Thread(){
                                                        public void run(){
                                                            Sync.finish();
                                                        }
                                                    }.start();
                                                }
                                            }catch(Exception e){
                                                AutomatedTaskFramework.Task task = AutomatedTaskFramework.Tasks.GetCurrentOrDefault();
                                                if(task!=null){
                                                    task.Terminate();
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                }
                                case "INF":
                                {
                                    switch (packetSID)
                                    {
                                        case "RET":
                                        {
                                            switch (decryptedData.split("!")[1])
                                            {
                                                case "COOKIE_DOES_EXIST":
                                                {
                                                    NetworkAdapter.MethodProvider.Authorize();
                                                    break;
                                                }
                                                case "COOKIE_DOES_NOT_EXIST":
                                                {
                                                    NetworkAdapter.MethodProvider.GetCookie();
                                                    break;
                                                }
                                                case "SEND_VERIFICATION_ACTIVATE_ACCOUNT":
                                                {
                                                     Intent intent = new Intent(globalVARpool.applicationcontext,tfa.class);
                                                     intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                                                     intent.putExtra("loadingtype",Loading.LoadingType.REGISTER);
                                                     globalVARpool.application.startActivity(intent);
                                                     new Handler(Looper.getMainLooper()).post(new Runnable() {
                                                        @Override
                                                        public void run() {
                                                            globalVARpool.aca_registeronline.finish();
                                                        }
                                                     });
                                                     break;
                                                }
                                                case "LOGIN_SUCCESSFUL":
                                                    if(!globalVARpool.wasOnline && globalVARpool.loadingType==globalVARpool.LoadingType.none){
                                                        new Thread(){
                                                            public void run(){
                                                                DataBaseHelper db = DataBaseHelper.GetInstance();
                                                                db.execSQL(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"UPDATE Tbl_user SET U_wasOnline = 1, U_username = \"",globalVARpool.username,"\", U_email = \"",globalVARpool.email,"\";"}));
                                                                db.execSQL(DataBaseHelper.Security.SQLInjectionCheckQuery(new String[]{"INSERT INTO Tbl_settings ( S_server_ip, S_server_port) VALUES( \"",client.getRemoteAddress(),"\", \"",String.valueOf(client.getRemotePort()),"\");"}));
                                                                globalVARpool.wasOnline = true;
                                                            }
                                                        }.start();
                                                        CustomToast.invokeMakeText(globalVARpool.applicationcontext, "Login successful!");
                                                    }
                                                    logged_in=true;
                                                    break;
                                                case "SEND_VERIFICATION_NEW_DEVICE":
                                                    Intent intent = new Intent(globalVARpool.applicationcontext,tfa.class);
                                                    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                                                    intent.putExtra("loadingtype",Loading.LoadingType.LOGIN);
                                                    globalVARpool.application.startActivity(intent);
                                                    new Handler(Looper.getMainLooper()).post(new Runnable() {
                                                        @Override
                                                        public void run() {
                                                            globalVARpool.aca_loginonline.finish();
                                                        }
                                                    });
                                                    break;
                                            }
                                            break;
                                        }

                                        case "ERR":
                                            String err_no = "";
                                            String err_id = "";
                                            String err_msg = "";

                                            String[] sa_decryptedData = decryptedData.split(";");
                                            try{
                                                for(String s:sa_decryptedData){
                                                    if(s.contains("errno")){
                                                        err_no = s.split("!")[1];
                                                    }
                                                    else if(s.contains("code")){
                                                        err_id = s.split("!")[1];
                                                    }
                                                    else if(s.contains("message")){
                                                        err_msg = s.split("!")[1];
                                                    }

                                                }

                                            }catch(IndexOutOfBoundsException e){
                                                err_no="-1";
                                                err_id="UNKN";
                                                err_msg="UNKNOWN ERROR";
                                            }
                                            AutomatedTaskFramework.Task task = AutomatedTaskFramework.Tasks.GetCurrentOrDefault();

                                            switch(err_id){
                                                case "UEXT":
                                                    client.setErrorCode(-2);
                                                    //TODO MSG-> THIS USERNAME IS ALREADY IN USE!!!!!!!!!!!!
                                                    break;
                                                case "MAIL":
                                                    if(err_msg.equals("EMAIL_ALREADY_IN_USE")){
                                                        client.setErrorCode(-2);
                                                        //TODO MSG!!!
                                                    }
                                                    break;
                                                case "UDNE":
                                                    client.setErrorCode(-2);
                                                    if(task!=null){
                                                        task.Terminate();
                                                    }
                                                    //TODO MSG-> THIS USERNAME DOES NOT EXIST!!!!!!!!!
                                                    break;
                                                case "CRED":
                                                    client.setErrorCode(-2);
                                                    if(task!=null){
                                                        task.Terminate();
                                                    }
                                                    //TODO MSG-> INVALID CREDENTIALS
                                                    break;
                                                case "I2FA":
                                                    client.setErrorCode(1);
                                                    //TODO MSG -> INVALID 2FA-Code
                                                    break;
                                                case "E2FA":
                                                    client.setErrorCode(-2);
                                                    //TODO MSG-> EXPIRED 2FA-Code
                                                    break;
                                                    default:
                                                        System.out.println("[ERR-NO "+err_no+"] "+err_id);
                                                        if(task!=null){
                                                            task.Terminate();
                                                        }
                                                        break;
                                            }
                                            break;
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        catch (SocketException e) {
            if (e.getMessage().equalsIgnoreCase("RST")) {
                System.out.println("DISCONNECTED RST");
            }
            else {
                e.printStackTrace();
            }
            // TODO Auto-generated catch block
        } catch (InvalidKeyException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (InvalidKeySpecException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (NoSuchPaddingException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (IllegalBlockSizeException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (BadPaddingException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (InvalidAlgorithmParameterException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (Exception e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        finally {
            if(isConnected()) this.Connected = false;
            if(isLogged_in()) this.logged_in = false;
        }
    }

    public boolean isDebugging() {
        return Debugging;
    }

    public void setDebugging(boolean debugging) {
        Debugging = debugging;
    }

    public String getRemoteRSAKey() {
        return RemoteRSAKey;
    }

    public PrivateKey getPrivateKey() {
        return privateKey;
    }

    public void setPrivateKey(PrivateKey privateKey) {
        this.privateKey = privateKey;
    }

    public DataOutputStream getServerInput() {
        return ServerInput;
    }

    public BufferedReader getServerOutput() {
        return null;// ServerOutput;
    }

    public boolean isThreadKilled() {
        return ThreadKilled;
    }

    public void setThreadKilled(boolean threadKilled) {
        ThreadKilled = threadKilled;
    }

    public void setErrorCode(int errorCode) {
        ErrorCode = errorCode;
    }
}