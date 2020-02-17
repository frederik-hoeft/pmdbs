package com.example.rodaues.pmdbs_navdraw;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Build;
import android.os.Bundle;
import androidx.appcompat.widget.Toolbar;
import androidx.biometric.BiometricManager;
import androidx.biometric.BiometricPrompt;
import androidx.core.content.ContextCompat;

import android.security.keystore.KeyGenParameterSpec;
import android.security.keystore.KeyProperties;
import android.telephony.mbms.MbmsErrors;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.Switch;

import java.io.IOException;
import java.nio.charset.Charset;
import java.security.InvalidAlgorithmParameterException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.NoSuchProviderException;
import java.security.UnrecoverableKeyException;
import java.security.cert.CertificateException;
import java.security.spec.AlgorithmParameterSpec;
import java.util.Arrays;
import java.util.concurrent.Executor;

import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;

public class Settings extends AppCompatActivity{

    Switch switch_bioauth;
    private Executor executor;
    private BiometricPrompt biometricPrompt;
    private BiometricPrompt.PromptInfo promptInfo;
    byte[] iv=null;
    DataBaseHelper db;



    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);




        getSupportActionBar().setTitle("");
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        getSupportActionBar().setDisplayShowHomeEnabled(true);

        db = DataBaseHelper.GetInstance();
        switch_bioauth = findViewById(R.id.settings_switch_bioauth);
        if(db.user_getBiometricInfo().get(0)!=null){
            switch_bioauth.setChecked(true);
        }


        BiometricManager biometricManager = BiometricManager.from(this);
        switch (biometricManager.canAuthenticate()) {
            case BiometricManager.BIOMETRIC_SUCCESS:
                Log.d("MY_APP_TAG", "App can authenticate using biometrics.");
                switch_bioauth.setEnabled(true);
                break;
            case BiometricManager.BIOMETRIC_ERROR_NO_HARDWARE:
                Log.e("MY_APP_TAG", "No biometric features available on this device.");
                switch_bioauth.setEnabled(false);
                break;
            case BiometricManager.BIOMETRIC_ERROR_HW_UNAVAILABLE:
                Log.e("MY_APP_TAG", "Biometric features are currently unavailable.");
                switch_bioauth.setEnabled(false);
                break;
            case BiometricManager.BIOMETRIC_ERROR_NONE_ENROLLED:
                Log.e("MY_APP_TAG", "The user hasn't associated " +
                        "any biometric credentials with their account.");
                switch_bioauth.setEnabled(false);
                break;
        }


        executor = ContextCompat.getMainExecutor(this);
        biometricPrompt = new BiometricPrompt(Settings.this,
                executor, new BiometricPrompt.AuthenticationCallback() {
            @Override
            public void onAuthenticationError(int errorCode,
                                              @NonNull CharSequence errString) {
                super.onAuthenticationError(errorCode, errString);

                switch_bioauth.setChecked(false);
            }

            @Override
            public void onAuthenticationSucceeded(
                    @NonNull BiometricPrompt.AuthenticationResult result) {
                super.onAuthenticationSucceeded(result);
                CustomToast.makeText(getApplicationContext(),"Biometric authentication activated!");
                try{

                    String unencryptedInfo = globalVARpool.hashedMasterPW;
                        byte[] encryptedInfo = result.getCryptoObject().getCipher().doFinal(
                                unencryptedInfo.getBytes("UTF-8"));

                    DataBaseHelper db = DataBaseHelper.GetInstance();
                    db.user_setBiometricInfo(encryptedInfo,iv);


                }catch(Exception e){
                    e.printStackTrace();
                }

            }

            @Override
            public void onAuthenticationFailed() {
                super.onAuthenticationFailed();

            }
        });

        promptInfo = new BiometricPrompt.PromptInfo.Builder()
                .setTitle("Biometric Login")
                .setSubtitle("Log in using your biometric credential")
                .setNegativeButtonText("ABORT")
                .build();


        switch_bioauth.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {


                try{

                    //genKEY
                    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                        try {
                            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                                generateSecretKey(new KeyGenParameterSpec.Builder(
                                        "PMDBS_KEY",
                                        KeyProperties.PURPOSE_ENCRYPT | KeyProperties.PURPOSE_DECRYPT)
                                        .setBlockModes(KeyProperties.BLOCK_MODE_CBC)
                                        .setEncryptionPaddings(KeyProperties.ENCRYPTION_PADDING_PKCS7)
                                        .setUserAuthenticationRequired(true)
                                        // Invalidate the keys if the user has registered a new biometric
                                        // credential, such as a new fingerprint. Can call this method only
                                        // on Android 7.0 (API level 24) or higher. The variable
                                        // "invalidatedByBiometricEnrollment" is true by default.
                                        .setInvalidatedByBiometricEnrollment(true)
                                        .build());
                            }
                        } catch (NoSuchProviderException e) {
                            e.printStackTrace();
                        } catch (NoSuchAlgorithmException e) {
                            e.printStackTrace();
                        } catch (InvalidAlgorithmParameterException e) {
                            e.printStackTrace();
                        }
                    }
                    //genKEY


                    Cipher cipher = getCipher();
                    SecretKey secretKey = getSecretKey();
                    cipher.init(Cipher.ENCRYPT_MODE, secretKey);

                    iv = cipher.getIV();
                    biometricPrompt.authenticate(promptInfo,
                            new BiometricPrompt.CryptoObject(cipher));

                }catch(Exception e){
                        e.printStackTrace();
                }
            }
        });

    }




    private void generateSecretKey(KeyGenParameterSpec keyGenParameterSpec) throws NoSuchProviderException, NoSuchAlgorithmException, InvalidAlgorithmParameterException {
        KeyGenerator keyGenerator = KeyGenerator.getInstance(
                KeyProperties.KEY_ALGORITHM_AES, "AndroidKeyStore");
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            keyGenerator.init(keyGenParameterSpec);
        }
        keyGenerator.generateKey();
    }

    private SecretKey getSecretKey() throws KeyStoreException, CertificateException, NoSuchAlgorithmException, IOException, UnrecoverableKeyException {
        KeyStore keyStore = KeyStore.getInstance("AndroidKeyStore");

        // Before the keystore can be accessed, it must be loaded.
        keyStore.load(null);
        return ((SecretKey)keyStore.getKey("PMDBS_KEY", null));
    }

    private Cipher getCipher() throws NoSuchPaddingException, NoSuchAlgorithmException {
        return Cipher.getInstance(KeyProperties.KEY_ALGORITHM_AES + "/"
                + KeyProperties.BLOCK_MODE_CBC + "/"
                + KeyProperties.ENCRYPTION_PADDING_PKCS7);
    }




}
