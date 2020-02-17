package com.example.rodaues.pmdbs_navdraw;

import java.util.ArrayList;
import java.util.List;

public class ByteExtension
{
    // --------------SINGLETON DESIGN PATTERN START
    private static ByteExtension byteExtension = null;

    private ByteExtension() {}

    public static ByteExtension GetInstance()
    {
        if(byteExtension == null)
        {
            byteExtension = new ByteExtension();
        }
        return byteExtension;
    }
    // --------------SINGLETON DESIGN PATTERN END

    public static byte[] ByteListToBytes(List<Byte> byteList)
    {
        byte[] bytes = new byte[byteList.size()];
        for (int i = 0; i < byteList.size(); i++)
        {
            bytes[i] = byteList.get(i).byteValue();
        }
        return bytes;
    }

    public static Byte[] ByteListToByteObjects(List<Byte> byteList)
    {
        Byte[] bytes = new Byte[byteList.size()];
        for (int i = 0; i < byteList.size(); i++)
        {
            bytes[i] = byteList.get(i);
        }
        return bytes;
    }

    public static List<Byte> ByteArrayToList(byte[] bytes)
    {
        List<Byte> byteList = new ArrayList<Byte>();
        for (int i = 0; i < bytes.length; i++)
        {
            byteList.add(bytes[i]);
        }
        return byteList;
    }

    public static List<Byte> ByteArrayToList(Byte[] bytes)
    {
        List<Byte> byteList = new ArrayList<Byte>();
        for (int i = 0; i < bytes.length; i++)
        {
            byteList.add(bytes[i]);
        }
        return byteList;
    }

    public static byte[] RemoveFromByteArray(byte[] bytes, byte byteToRemove)
    {
        List<Byte> cleanedBytes = new ArrayList<Byte>();
        for (int i = 0; i < bytes.length; i++)
        {
            if (bytes[i] != byteToRemove)
            {
                cleanedBytes.add(bytes[i]);
            }
        }
        return ByteListToBytes(cleanedBytes);
    }

    public static boolean ByteArrayContains(byte[] bytes, byte byteToCheckFor)
    {
        for (int i = 0; i < bytes.length; i++)
        {
            if (bytes[i] == byteToCheckFor)
            {
                return true;
            }
        }
        return false;
    }

    public static boolean ByteArrayContains(Byte[] bytes, byte byteToCheckFor)
    {
        for (int i = 0; i < bytes.length; i++)
        {
            if (bytes[i].byteValue() == byteToCheckFor)
            {
                return true;
            }
        }
        return false;
    }

    public static List<Byte[]> SplitByteArray(byte[] bytes, byte byteToSplitAt)
    {
        List<Byte[]> parts = new ArrayList<Byte[]>();
        int index = 0;
        Byte[] part;
        for (int i = 0; i < bytes.length; i++)
        {
            if (bytes[i] == byteToSplitAt)
            {
                part = new Byte[i - index];
                for (int j = index; j < i; j++)
                {
                    part[j - index] = bytes[j];
                }
                parts.add(part);
                index = i + 1;
            }
        }
        part = new Byte[bytes.length - index];
        for (int j = index; j < bytes.length; j++)
        {
            part[j - index] = bytes[j];
        }
        parts.add(part);
        return parts;
    }

    public static List<Byte[]> DeepCopyByteList(List<Byte[]> bytes)
    {
        List<Byte[]> copiedBytes = new ArrayList<Byte[]>();
        for (int i = 0; i < bytes.size(); i++)
        {
            Byte[] source = bytes.get(i);
            Byte[] destination = new Byte[source.length];
            System.arraycopy(source, 0, destination, 0, source.length);
            copiedBytes.add(destination);
        }
        return copiedBytes;
    }

    public static byte[] ByteObjectsToBytes(Byte[] byteObjects)
    {
        byte[] bytes = new byte[byteObjects.length];
        for (int i = 0; i < byteObjects.length; i++)
        {
            bytes[i] = byteObjects[i].byteValue();
        }
        return bytes;
    }

    public static Byte[] BytesToByteObjects(byte[] bytes)
    {
        Byte[] byteObjects = new Byte[bytes.length];
        for (int i = 0; i < bytes.length; i++)
        {
            byteObjects[i] = bytes[i];
        }
        return byteObjects;
    }

    public static int ByteListCountBytes(List<Byte> byteList, byte byteToCount)
    {
        byte[] bytes = ByteListToBytes(byteList);
        int count = 0;
        for (int i = 0; i < bytes.length; i++)
        {
            if (bytes[i] == byteToCount)
            {
                count++;
            }
        }
        return count;
    }
}