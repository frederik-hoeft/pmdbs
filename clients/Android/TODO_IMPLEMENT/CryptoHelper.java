package pmdbs;

import java.io.IOException;
import java.io.Reader;
import java.io.StringReader;
import java.io.StringWriter;
import java.io.UnsupportedEncodingException;

import org.spongycastle.jce.provider.BouncyCastleProvider;
import org.spongycastle.util.io.pem.PemObject;
import org.spongycastle.util.io.pem.PemReader;
import org.spongycastle.util.io.pem.PemWriter;

import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.KeyFactory;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.SecureRandom;
import java.security.Security;
import java.security.spec.AlgorithmParameterSpec;
import java.security.spec.InvalidKeySpecException;
import java.security.spec.X509EncodedKeySpec;
import java.util.Arrays;
import java.util.Base64;
import java.util.Random;

import javax.crypto.BadPaddingException;
import javax.crypto.Cipher;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

public class CryptoHelper 
{
	static 
	{
		Security.addProvider(new BouncyCastleProvider());
	}
	
	// --------------SINGLETON DESIGN PATTERN START
	private static CryptoHelper cryptoHelper = null;
	
	private CryptoHelper() {}
	
	public static CryptoHelper GetInstance() 
	{
		if(cryptoHelper == null) 
		{
			cryptoHelper = new CryptoHelper();
		}
	    return cryptoHelper;
	}
	// --------------SINGLETON DESIGN PATTERN END
	
	public static String SHA1Hash(String plaintext) throws IOException, Exception 
	{
		byte[] bytes = plaintext.getBytes("UTF-8");
		MessageDigest md = null;
        md = MessageDigest.getInstance("SHA-1");
		return byteArrayToHexString(md.digest(bytes));
	}
	
	private static String byteArrayToHexString(byte[] b) {
		String result = "";
		for (int i=0; i < b.length; i++) 
		{
			result += Integer.toString( ( b[i] & 0xff ) + 0x100, 16).substring( 1 );
		}
		return result;
	}
	
	public static String[] GenerateRSAKeys() throws NoSuchAlgorithmException, IOException
	{
		// Get an instance of the RSA key generator
        KeyPairGenerator keyPairGenerator = KeyPairGenerator.getInstance("RSA");
        keyPairGenerator.initialize(4096);
        
        // Generate the KeyPair
        KeyPair keyPair = keyPairGenerator.generateKeyPair();
        
        // Get the public and private key
        PublicKey publicKey = keyPair.getPublic();
        PrivateKey privateKey = keyPair.getPrivate();
        PDTPClient client = PDTPClient.GetInstance();
        client.setPrivateKey(privateKey);
        StringWriter writer = new StringWriter();
        PemWriter pemWriter = new PemWriter(writer);
        pemWriter.writeObject(new PemObject("PUBLIC KEY", publicKey.getEncoded()));
        pemWriter.flush();
        pemWriter.close();
        String publicKeyPEM = writer.toString();
        writer = new StringWriter();
        pemWriter = new PemWriter(writer);
        pemWriter.writeObject(new PemObject("PRIVATE KEY", privateKey.getEncoded()));
        pemWriter.flush();
        pemWriter.close();
        String privateKeyPEM = writer.toString();
        writer.close();
		return new String[] {privateKeyPEM, publicKeyPEM};
	} 

    public static String SHA256(String text) throws UnsupportedEncodingException, NoSuchAlgorithmException
    {
        byte[] keyBytes;
        keyBytes = text.getBytes("UTF-8");
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
    }
    
    public static String SHA256HashBase64(String text) throws UnsupportedEncodingException, NoSuchAlgorithmException
    {
    	byte[] keyBytes;
        keyBytes = text.getBytes("UTF-8");
        MessageDigest md = MessageDigest.getInstance("SHA-256");
        md.update(keyBytes);
        keyBytes = md.digest();
        
        return Base64.getEncoder().encodeToString(keyBytes);
    }
    
    public static String RSAEncrypt(String text) throws IOException, InvalidKeySpecException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, IllegalBlockSizeException, BadPaddingException
    {
    	PDTPClient client = PDTPClient.GetInstance();
        Reader reader = new StringReader(client.getRemoteRSAKey()); // or from file etc.
        PemReader pemReader = new PemReader(reader);
        PemObject pemObject = pemReader.readPemObject();
        pemReader.close();

        PublicKey key = KeyFactory.getInstance("RSA").generatePublic(new X509EncodedKeySpec(pemObject.getContent()));
        //Get Cipher Instance RSA With ECB Mode and OAEPWITHSHA-512ANDMGF1PADDING Padding
        Cipher cipher = Cipher.getInstance("RSA/ECB/OAEPPadding");
        
        //Initialize Cipher for ENCRYPT_MODE
        cipher.init(Cipher.ENCRYPT_MODE, key);
        
        //Perform Encryption
        byte[] cipherText = cipher.doFinal(text.getBytes("UTF-8")) ;

        return Base64.getEncoder().encodeToString(cipherText);
    }
    
    public static String RSADecrypt(String text) throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, IllegalBlockSizeException, BadPaddingException
    {
    	//Get Cipher Instance RSA With ECB Mode and OAEPWITHSHA-512ANDMGF1PADDING Padding
        Cipher cipher = Cipher.getInstance("RSA/ECB/OAEPPadding");
        
        PDTPClient client = PDTPClient.GetInstance();
        PrivateKey privateKey = client.getPrivateKey();
        //Initialize Cipher for DECRYPT_MODE
        cipher.init(Cipher.DECRYPT_MODE, privateKey);
        byte[] cipherTextArray = Base64.getDecoder().decode(text);
        //Perform Decryption
        byte[] decryptedTextArray = cipher.doFinal(cipherTextArray);
        
        return new String(decryptedTextArray);
    }
    
    public static String RandomString(int length) throws UnsupportedEncodingException, NoSuchAlgorithmException
    {
    	String symbols = "ABCDEFGJKLMNPRSTUVWXYZ0123456789";
    	Random random = new SecureRandom();
    	char[] buf;
    	if (length < 1)
    	{
    		throw new IllegalArgumentException("length < 1: " + length);
    	}
        buf = new char[length];
        for (int idx = 0; idx < buf.length; ++idx) 
        {
        	buf[idx] = symbols.charAt(random.nextInt(symbols.length()));
        }
        return SHA256(new String(buf));
    }
    
    public static String AESEncrypt(String plainText, String password) throws UnsupportedEncodingException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException 
    {
        byte[] bytes = plainText.getBytes("UTF-8");
        byte[] ivBytes = new byte[16];
        SecureRandom.getInstanceStrong().nextBytes(ivBytes);
        AlgorithmParameterSpec ivSpec = new IvParameterSpec(ivBytes);
        
        byte[] keyBytes = password.getBytes("UTF-8");
        MessageDigest md = MessageDigest.getInstance("SHA-256");
        md.update(keyBytes);
        keyBytes = md.digest();
        
        SecretKeySpec newKey = new SecretKeySpec(keyBytes, "AES");
        Cipher cipher = null;
        cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        cipher.init(Cipher.ENCRYPT_MODE, newKey, ivSpec);

        byte[] cipherBytes = cipher.doFinal(bytes);
        
        byte[] finalBytes = new byte[ivBytes.length + cipherBytes.length];
        System.arraycopy(ivBytes, 0, finalBytes, 0, ivBytes.length);
        System.arraycopy(cipherBytes, 0, finalBytes, ivBytes.length, cipherBytes.length);

        return Base64.getEncoder().encodeToString(finalBytes);
    }

    public static String AESDecrypt(String cipherText, String password) throws UnsupportedEncodingException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, InvalidAlgorithmParameterException
    {
        byte[] bytes = Base64.getDecoder().decode(cipherText);
        byte[] keyBytes = password.getBytes("UTF-8");

        MessageDigest md = MessageDigest.getInstance("SHA-256");
        md.update(keyBytes);
        keyBytes = md.digest();

        byte[] ivB = Arrays.copyOfRange(bytes, 0, 16);
        byte[] codB = Arrays.copyOfRange(bytes, 16, bytes.length);

        AlgorithmParameterSpec ivSpec = new IvParameterSpec(ivB);
        SecretKeySpec newKey = new SecretKeySpec(keyBytes, "AES");
        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        cipher.init(Cipher.DECRYPT_MODE, newKey, ivSpec);

        try
        {
            return new String(cipher.doFinal(codB),"UTF-8");
        }
        catch(Exception e)
        {
            return e.toString();
        }
    }
    
    public static String CalculateHMAC(String hmacKey, String message) throws Exception
	{
    	String k1 = hmacKey.substring(0, 32);
    	String k2 = hmacKey.substring(32, 64);
	    return SHA256HashBase64(k2 + SHA256HashBase64(k1 + message));
	}
	
	public static boolean VerifyHMAC(String hmacKey, String fullMessage) throws Exception
	{
	    String hmac = fullMessage.substring(fullMessage.length() - 44, fullMessage.length());
	    String message = fullMessage.substring(0, fullMessage.length() - 44);
	    String actualHmac = CalculateHMAC(hmacKey, message);
	    if (hmac.equals(actualHmac))
	    {
	        return true;
	    }
	    else
	    {
	        return false;
	    }
	}
}

