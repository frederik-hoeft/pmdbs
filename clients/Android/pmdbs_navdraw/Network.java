package com.example.rodaues.pmdbs_navdraw;

public class Network
{
    private Network() {}

    public static void Send(String data)
    {
        PDTPClient client = PDTPClient.GetInstance();
        try
        {
            if (client.isDebugging())
            {
                System.out.println("SENDING: U" + data);
            }
            client.getServerInput().writeBytes("\1U" + data + "\4");
        }
        catch (Exception e)
        {
            if (!client.isThreadKilled())
            {
                e.printStackTrace();
            }
        }
    }

    public static void SendEncrypted(String data)
    {
        PDTPClient client = PDTPClient.GetInstance();
        try
        {
            String encryptedData = CryptoHelper.AESEncrypt(data, client.getAESKey());
            String hmac = CryptoHelper.CalculateHMAC(client.getHMACKey(), encryptedData);
            if (client.isDebugging())
            {
               /* System.out.println("SENDING: E" + data);
                System.out.println("SENDINGE: E" + encryptedData);
                System.out.println("CALCULATED HMAC: " + hmac);*/
            }


            String v = ("\1" + "E" + encryptedData + hmac + "\4").replace("\r","").replace("\n","");
            System.out.println("SENDING: E"+data);

            client.getServerInput().writeBytes(v);
        }
        catch (Exception e)
        {
            if (!client.isThreadKilled())
            {
                e.printStackTrace();
            }
        }
    }


}