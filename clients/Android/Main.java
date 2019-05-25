package com.rodaues.pmdbs_androidclient;

import android.support.design.widget.TabLayout;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.ViewPager;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.RelativeLayout;

import java.lang.reflect.Method;

public class Main extends AppCompatActivity{

    private SectionsPagerAdapter mSectionsPagerAdapter;
    private ViewPager mViewPager;


    public static RelativeLayout rl_loading;
    public static Boolean loading=false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        setTheme(R.style.AppTheme_NoActionBar);
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        rl_loading = findViewById(R.id.rlayout_progressbar);

        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        // Create the adapter that will return a fragment for each of the three
        // primary sections of the activity.
        mSectionsPagerAdapter = new SectionsPagerAdapter(getSupportFragmentManager());

        // Set up the ViewPager with the sections adapter.
        mViewPager = (ViewPager) findViewById(R.id.container);
        mViewPager.setAdapter(mSectionsPagerAdapter);

        TabLayout tabLayout = (TabLayout) findViewById(R.id.tabs);

        mViewPager.addOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout));
        tabLayout.addOnTabSelectedListener(new TabLayout.ViewPagerOnTabSelectedListener(mViewPager));

        stopLoadingScreen();

    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.

        getMenuInflater().inflate(R.menu.menu_main, menu);

        return true;
    }
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        /*if (id == R.id.action_settings) {
            return true;
        }*/

        return super.onOptionsItemSelected(item);
    }
    public class SectionsPagerAdapter extends FragmentPagerAdapter {

        public SectionsPagerAdapter(FragmentManager fm) {
            super(fm);
        }

        @Override
        public Fragment getItem(int position) {
            Fragment fragment = null;
            switch (position){
                case 0:
                    fragment = new tab1_addacc();
                    break;
                case 1:
                    fragment = new tab2_savacc();
                    break;
                case 2:
                    fragment = new tab3_myacc();
                    break;
                case 3:
                    fragment = new tab4_serversettings();
                    break;
            }
            return fragment;
        }

        @Override
        public int getCount() {
            // Show 4 total pages.
            return 4;
        }
    }

    private void msg(String s){
        View parentLayout = findViewById(android.R.id.content);
        Snackbar.make(parentLayout,s, Snackbar.LENGTH_SHORT)
                .setAction("Action", null)
                .setActionTextColor(getResources().getColor(R.color.colorAccent)).show();

    }

    public static Method startLoadingScreen(){

       // rl_loading.animate().alpha(1.0f);
        //rl_loading.setClickable(true);

         rl_loading.setVisibility(View.VISIBLE);

         if(rl_loading.getVisibility()==View.VISIBLE)  loading=true;
         else{
             Log.e("","MOIN! Hat nicht geladen :(");
         }

        return null;
    }
    public static Method stopLoadingScreen(){
        //rl_loading.animate().alpha(0.0f);
        //rl_loading.setClickable(false);
       rl_loading.setVisibility(View.GONE);
        if(rl_loading.getVisibility()==View.VISIBLE)  loading=false;
        return null;
    }
}
