package pmdbs;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

public class program 
{
	public static void main(String[] args) throws Exception 
	{
		PDTPClient client = PDTPClient.GetInstance();
		client.Connect();
	}
}
