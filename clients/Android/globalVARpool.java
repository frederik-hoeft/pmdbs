package com.example.rodaues.pmdbs_navdraw;

import android.app.Application;
import android.content.Context;
import android.graphics.Bitmap;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import androidx.fragment.app.Fragment;
import androidx.appcompat.app.AppCompatActivity;

import android.widget.ListView;

import java.util.ArrayList;
import java.util.List;

public class globalVARpool {
    public static String AESkey="";
    public static ArrayList<ArrayList<String>> dataSET = new ArrayList<>();
    public static Boolean ITEMselected = false;
    public static String username = "testuser";
    public static String cookie = null;
    public static String CLIENT_VERSION = "0.7-1.19";
    public static String hashedMasterPW = "hashedMasterPW";
    public static String onlinePassword = "password";
    public static String email = "mail@example.com";
    public static String nickname = "User";
    public static Boolean wasOnline = false;
    public static Context applicationcontext=null;
    public static Application application = null;
    public static AppCompatActivity aca_tfa = null;
    public static AppCompatActivity aca_main = null;
    public static AppCompatActivity aca_registeronline = null;
    public static AppCompatActivity aca_loginonline = null;
    public static Fragment fragment_tab_saveddata = null;
    public static String plainMasterPW = "";
    public static String firstUsage = "";

    public static int countedPackets = 0;
    public static int expectedPacketCount = 0;
    public static int HID_ThreadCounter = 0;

    public static Boolean countSyncPackets = false;

    public static Boolean database_initialized = false;

    public static List<String> selectedAccounts = new ArrayList<>();


    public static FloatingActionButton fab_deleteitems = null;
    public static FloatingActionButton fab_sync = null;
    public static ArrayList<Integer> selItems = null;
    public static ListView lv_data = null;

    public static Bitmap selBITMAP = null;



    public enum LoadingType{
        loginonline,
        none
    }
    public static LoadingType loadingType = LoadingType.none;
}


