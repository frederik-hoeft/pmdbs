
package com.example.administrator.grwrandomselector;



import android.app.AlertDialog;
import android.content.DialogInterface;
import android.database.Cursor;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;
import android.app.AlertDialog.Builder;



public class MainActivity extends AppCompatActivity {
    DataBaseHelper PrimDB;
    EditText editName, editGroup, editSS;
    Button btn_addData, btn_allData, btn_delRData, btn_delGData, btn_rndData;
    TextView textResult;
    Switch ThisSwitchOfMine;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        PrimDB = new DataBaseHelper(this);

        editName = (EditText)findViewById(R.id.editTextName);
        editGroup = (EditText)findViewById(R.id.editTextGroup);
        editSS = (EditText)findViewById(R.id.editTextSS);
        textResult = (TextView)findViewById(R.id.textViewRndResult);
        btn_addData = (Button)findViewById(R.id.button_add);
        btn_allData = (Button)findViewById(R.id.button_all);
        btn_delRData = (Button)findViewById(R.id.button_del_r);
        btn_delGData = (Button)findViewById(R.id.button_del_g);
        btn_rndData = (Button)findViewById(R.id.button_rnd);
        ThisSwitchOfMine = (Switch)findViewById(R.id.switch1);
        AddData();
        SearchData();
        DelSingleData();
        DelGroupData();
        RndData();
    }

    public void AddData() {
        btn_addData.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View view) {
                        if(editName.getText().toString().equals("")){
                            Toast.makeText(MainActivity.this, "ERROR: Enter some name!", Toast.LENGTH_SHORT).show();
                        }else if(editGroup.getText().toString().equals("")){
                            Toast.makeText(MainActivity.this, "ERROR: Enter some group!", Toast.LENGTH_SHORT).show();
                        }
                        else{
                            Cursor checkResult = PrimDB.checkSingle(editName.getText().toString(), editGroup.getText().toString());
                            StringBuffer buffer = new StringBuffer();
                            while (checkResult.moveToNext()) {
                                buffer.append(checkResult.getString(0));
                            }
                            String result = buffer.toString();
                            if (result.equals("")){
                                Boolean isInserted = PrimDB.insertData(editName.getText().toString(),
                                        editGroup.getText().toString());
                                final EditText EditTextToChange = (EditText) findViewById(R.id.editTextName);
                                if (isInserted) {
                                    Toast.makeText(MainActivity.this, "Data successfully inserted!", Toast.LENGTH_SHORT).show();
                                    EditTextToChange.setText("");
                                } else {
                                    Toast.makeText(MainActivity.this, "ERROR: Data not inserted!", Toast.LENGTH_SHORT).show();
                                }
                            }else {
                                AddConfirm();
                            }
                        }
                }
                }
        );
    }

    public void AddConfirm(){
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setCancelable(true);
        builder.setTitle("Data redundancy!");
        builder.setMessage("An entry called \"" + editName.getText().toString() + "\" already exists in group \"" + editGroup.getText().toString() + "\"!\nDo you want to add it regardless?");
        builder.setPositiveButton("Yes",
                new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        Boolean isInserted = PrimDB.insertData(editName.getText().toString(),
                                editGroup.getText().toString());
                        final EditText EditTextToChange = (EditText) findViewById(R.id.editTextName);
                        if (isInserted) {
                            Toast.makeText(MainActivity.this, "Data successfully inserted!", Toast.LENGTH_SHORT).show();
                            EditTextToChange.setText("");
                        } else {
                            Toast.makeText(MainActivity.this, "ERROR: Data not inserted!", Toast.LENGTH_SHORT).show();
                        }
                    }
                });
        builder.setNegativeButton("No", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {

            }
        });

        AlertDialog dialog = builder.create();
        dialog.show();
    }

    public void SearchData() {
        btn_allData.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View view) {
                        boolean switchState = ThisSwitchOfMine.isChecked();
                        if(switchState) {
                            Cursor allResult = PrimDB.getAllData(editSS.getText().toString());
                            if (allResult.getCount() == 0) {
                                Toast.makeText(MainActivity.this, "ERROR: No data found!", Toast.LENGTH_SHORT).show();
                                return;
                            }
                            StringBuffer buffer = new StringBuffer();
                            while (allResult.moveToNext()) {
                                buffer.append("ID: " + allResult.getString(0) + "\n");
                                buffer.append("Entry Name: " + allResult.getString(1) + "\n");
                                buffer.append("Group Name: " + allResult.getString(2) + "\n\n");
                            }
                            showMessage("Results for \"" + editSS.getText().toString() + "\":", buffer.toString());
                        }else{
                            Toast.makeText(MainActivity.this, "ERROR: Search function not selected!", Toast.LENGTH_SHORT).show();
                        }
                    }
                }
        );
    }

    public void RndData() {
        btn_rndData.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View view) {
                        boolean switchState = ThisSwitchOfMine.isChecked();
                        if(!switchState) {
                            Cursor rndResult = PrimDB.getRndData(editGroup.getText().toString(), editSS.getText().toString());
                            final TextView EditTextToChange = (TextView) findViewById(R.id.textViewRndResult);
                            if (rndResult.getCount() == 0) {
                                Toast.makeText(MainActivity.this, "ERROR: No data found!", Toast.LENGTH_SHORT).show();
                                return;
                            }
                            StringBuffer buffer = new StringBuffer();
                            while (rndResult.moveToNext()) {
                                buffer.append(rndResult.getString(0));
                            }
                            String disp = buffer.toString();
                            EditTextToChange.setText(disp);
                        }else{
                            Toast.makeText(MainActivity.this, "ERROR: Selection function not selected!", Toast.LENGTH_SHORT).show();
                        }
                    }
                }
        );
    }

    public void DelSingleData() {
        btn_delRData.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View view) {
                        Cursor checkResult = PrimDB.checkSingle(editName.getText().toString(), editGroup.getText().toString());
                        StringBuffer buffer = new StringBuffer();
                        while (checkResult.moveToNext()) {
                            buffer.append(checkResult.getString(0));
                        }
                        String result = buffer.toString();
                        if(result.equals("")){
                            Toast.makeText(MainActivity.this, "ERROR: Entry not found!", Toast.LENGTH_SHORT).show();
                        }else {
                            DelSingleConfirm();
                        }
                    }
                }
        );
    }

    public void DelSingleConfirm(){
        final EditText EditTextToChange = (EditText) findViewById(R.id.editTextName);
        String title;
        String message;
        if (editGroup.getText().toString().equals("")){
            title = "DO YOU REALLY WANT TO DELETE ENTRY \"" + editName.getText().toString() + "\"?";
            message = "All entries called \"" + editName.getText().toString() + "\" will be deleted from all groups!\nThis action is irreversible!\n\nYou can specify the entry by entering it's group!";
        }else{
            title = "DO YOU REALLY WANT TO DELETE ENTRY \"" + editName.getText().toString() + "\"?";
            message = "All entries called \"" + editName.getText().toString() + "\" will be deleted from group \"" + editGroup.getText().toString() + "\"!\n\nThis action is irreversible!";
        }
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setCancelable(true);
        builder.setTitle(title);
        builder.setMessage(message);
        builder.setPositiveButton("Confirm",
                new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        PrimDB.delSingleData(editName.getText().toString());
                        Toast.makeText(MainActivity.this, "Object deleted!", Toast.LENGTH_SHORT).show();
                        EditTextToChange.setText("");
                    }
                });
        builder.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {

            }
        });

        AlertDialog dialog = builder.create();
        dialog.show();
    }

    public void DelGroupData(){
        btn_delGData.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View view) {
                        Cursor checkResult = PrimDB.checkGroup(editGroup.getText().toString());
                        StringBuffer buffer = new StringBuffer();
                        while (checkResult.moveToNext()) {
                            buffer.append(checkResult.getString(0));
                        }
                        String result = buffer.toString();
                        if(result.equals("")){
                            Toast.makeText(MainActivity.this, "ERROR: Group not found!", Toast.LENGTH_SHORT).show();
                        }else {
                            DelGroupConfirm();
                        }
                    }
                }
        );
    }

    public void DelGroupConfirm(){
        final EditText EditTextToChange = (EditText) findViewById(R.id.editTextGroup);
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setCancelable(true);
        builder.setTitle("DO YOU REALLY WANT TO DELETE GROUP \"" + editGroup.getText().toString() + "\"?");
        builder.setMessage("The whole group will be deleted! Including all of it's entries!\n\nThis action is irreversible!");
        builder.setPositiveButton("Confirm",
                new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        PrimDB.delGroupData(editGroup.getText().toString());
                        Toast.makeText(MainActivity.this, "Group deleted!", Toast.LENGTH_SHORT).show();
                        EditTextToChange.setText("");
                    }
                });
        builder.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {

            }
        });

        AlertDialog dialog = builder.create();
        dialog.show();
    }

    public void showMessage(String title, String message){
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setCancelable(true);
        builder.setTitle(title);
        builder.setMessage(message);
        builder.show();
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
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }
}
