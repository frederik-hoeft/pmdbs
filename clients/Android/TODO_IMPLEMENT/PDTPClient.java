package pmdbs;
import java.util.List;
import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.util.ArrayList;

public class PDTPClient
{
	// --------------SINGLETON DESIGN PATTERN START
	private static PDTPClient client = null;
	
	private PDTPClient() {
		this.RemoteAddress = "192.168.178.46";
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

	public void Connect() throws Exception
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
        	byte[] data = new byte[bufferSize];
			while(true)
			{
				boolean receiving = true;
				List<Byte[]> dataPackets = new ArrayList<Byte[]>();
				this.Socket.setReceiveBufferSize(bufferSize);
				/*
				 * THIS MAY BE THE SOLUTION!!!
				int offset = 0;
			    int wanted = buffer.length;

			    while( wanted > 0 )
			    {
			        final int len = istream.read( buffer, offset, wanted );     
			        if( len == -1 )
			        {
			            throw new java.io.EOFException( "Connection closed gracefully by peer" );
			        }
			        wanted -= len;
			        offset += len;
			    } 
				*/
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
	                                    this.Connected = false;
	                                    try
	                                    {
	                                        this.Socket.shutdownInput();
	                                        this.Socket.shutdownOutput();
	                                        this.Socket.close();
	                                        this.ErrorCode = 0;
	                                    }
	                                    finally { }
	                                    if (this.Debugging)
	                                    {
	                                        System.out.println("Disconnected");
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
	                                        this.Nonce = CryptoHelper.RandomString(256);
	                                        String encNonce = CryptoHelper.RSAEncrypt(this.Nonce);
	                                        String message = "CKEkey%eq!" + this.RSAPublicKey + "!;nonce%eq!" + encNonce + "!;";
	                                        System.out.println("Client Key Exchange ...");
	                                        this.ServerInput.writeBytes("\1K" + message + "\4");
	                                    }
	                                    else
	                                    {
	                                    	System.out.println("RSA Key Format not supported.");
	                                    }
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
		                    // TODO: AUTOMATED TASK MANAGEMENT (CHECK FOR COMPLETED TASKS AND START NEXT ONE IN QUEUE)
		                    // AutomatedTaskFramework.DoTasks(decryptedData);
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
		                                // if (this.Cookie.Equals(String.Empty))
		                                // {
		                                    // NetworkAdapter.MethodProvider.GetCookie();
		                                // }
		                                // else
		                                // {
		                                    // NetworkAdapter.MethodProvider.CheckCookie();
		                                // }
		                                break;
		                            }
		                        }
		                    }
		                    break;
		                }
					}
				}
			}
		} 
		catch (Exception e) {
			e.printStackTrace();
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
}
