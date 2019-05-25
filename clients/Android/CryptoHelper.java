package com.rodaues.pmdbs_androidclient;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.spec.AlgorithmParameterSpec;
import java.util.Arrays;

import android.util.Base64;

import javax.crypto.BadPaddingException;
import javax.crypto.Cipher;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;


public class CryptoHelper {

    private static final String characterEncoding = "UTF-8";
    private static final String cipherTransformation = "AES/CBC/PKCS5Padding";
    private static final String aesEncryptionAlgorithm = "AES";
    private static final String key = globalVARpool.AESkey;
    private static byte[] ivBytes = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
    private static byte[] keyBytes;


    public static String encrypt(String messtring)
            throws NoSuchAlgorithmException,
            NoSuchPaddingException,
            InvalidKeyException,
            InvalidAlgorithmParameterException,
            IllegalBlockSizeException,
            BadPaddingException, IOException {

        byte[] mes = messtring.getBytes();




        AlgorithmParameterSpec ivSpec = new IvParameterSpec(ivBytes);
        SecretKeySpec newKey = new SecretKeySpec(hashedKEYbytes(key), "AES");
        Cipher cipher = null;
        cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        cipher.init(Cipher.ENCRYPT_MODE, newKey, ivSpec);

        byte[] destination = new byte[ivBytes.length + mes.length];
        System.arraycopy(ivBytes, 0, destination, 0, ivBytes.length);
        System.arraycopy(mes, 0, destination, ivBytes.length, mes.length);

        return Base64.encodeToString(cipher.doFinal(destination),Base64.DEFAULT);
    }

    public static String decrypt(String stringIn)
            throws NoSuchAlgorithmException,
            NoSuchPaddingException,
            InvalidKeyException,
            InvalidAlgorithmParameterException,IOException{

        byte[] bytes = Base64.decode(stringIn, Base64.DEFAULT);
        keyBytes = key.getBytes("UTF-8");

        MessageDigest md = MessageDigest.getInstance("SHA-256");
        md.update(keyBytes);
        keyBytes = md.digest();


        byte[] ivB = Arrays.copyOfRange(bytes, 0, 16);
        byte[] codB = Arrays.copyOfRange(bytes, 16, bytes.length);


        AlgorithmParameterSpec ivSpec = new IvParameterSpec(ivB);
        SecretKeySpec newKey = new SecretKeySpec(hashedKEYbytes(key), "AES");
        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        cipher.init(Cipher.DECRYPT_MODE, newKey, ivSpec);

        try{
            return new String(cipher.doFinal(codB),"UTF-8");
        }
        catch(Exception e){
            return e.toString();
        }

    }

    public static String hashedKEYstr(String inputKEY) throws UnsupportedEncodingException, NoSuchAlgorithmException {
        byte[] keyBytes;
        keyBytes = inputKEY.getBytes("UTF-8");
        MessageDigest md = MessageDigest.getInstance("SHA-256");
        md.update(keyBytes);
        keyBytes = md.digest();

        final char[] hexArray = "0123456789abcdef".toCharArray();

        char[] hexChars = new char[keyBytes.length * 2];
        for ( int j = 0; j < keyBytes.length; j++ ) {
            int v = keyBytes[j] & 0xFF;
            hexChars[j * 2] = hexArray[v >>> 4];
            hexChars[j * 2 + 1] = hexArray[v & 0x0F];
        }
        return new String(hexChars);

       // return hashedKey;
    }
    public static byte[] hashedKEYbytes(String inputKEY) throws UnsupportedEncodingException, NoSuchAlgorithmException {
        byte[] keyBytes;
        keyBytes = inputKEY.getBytes("UTF-8");
        MessageDigest md = MessageDigest.getInstance("SHA-256");
        md.update(keyBytes);
        keyBytes = md.digest();

        return keyBytes;
    }
}