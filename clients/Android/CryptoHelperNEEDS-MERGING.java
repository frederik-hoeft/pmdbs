package pmdbs;

import java.io.IOException;
import java.security.MessageDigest;

public class CryptoHelper 
{
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
}
