using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;



public static class MCGExtensions
{
    public static string PathCombine(string[] pathArray)
    {
        switch (pathArray.Length)
        {
            case 0:
                return "";
            case 1:
                return pathArray[0];
            case 2:

                return Path.Combine(pathArray[0], pathArray[1]);
            default:
                string result = pathArray[0];
                for (int i = 1; i < pathArray.Length; i++)
                {
                    result = Path.Combine(result, pathArray[i]);
                }
                return result;

        }
    }

    public static T Read<T>(this BinaryReader binaryReader)
    {
        byte[] bytes = binaryReader.ReadBytes(Marshal.SizeOf(typeof(T)));
        GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T structure = (T)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(T));
        gcHandle.Free();

        return structure;
    }

    public static object Read(this BinaryReader binaryReader, Type type)
    {
        byte[] bytes = binaryReader.ReadBytes(Marshal.SizeOf(type));
        GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        object structure = Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), type);
        gcHandle.Free();

        return structure;
    }

    public static T Read<T>(this byte[] bytes)
    {
        GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T structure = (T)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(T));
        gcHandle.Free();

        return structure;
    }

    public static object Read(this byte[] bytes, Type type)
    {
        GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        object structure = Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), type);
        gcHandle.Free();

        return structure;
    }

    // Inspired from several Stack Overflow discussions and an implementation by David Walker at http://coding.grax.com/2011/11/initialize-array-to-value-in-c-very.html
    public static void Fill<T>(this T[] destinationArray, params T[] value)
    {
        if (destinationArray == null)
        {
            throw new ArgumentNullException("destinationArray");
        }

        if (value.Length > destinationArray.Length)
        {
            throw new ArgumentException("Length of value array must not be more than length of destination");
        }

        // set the initial array value
        Array.Copy(value, destinationArray, value.Length);

        int copyLength, nextCopyLength;

        for (copyLength = value.Length; (nextCopyLength = copyLength << 1) < destinationArray.Length; copyLength = nextCopyLength)
        {
            Array.Copy(destinationArray, 0, destinationArray, copyLength, copyLength);
        }

        Array.Copy(destinationArray, 0, destinationArray, copyLength, destinationArray.Length - copyLength);
    }

    public static void Fill<T>(this T[] destinationArray, T value,int startPosition, int length)
    {
        if (destinationArray == null)
        {
            throw new ArgumentNullException("destinationArray");
        }

        if (length > destinationArray.Length)
        {
            throw new ArgumentException("Length of value array must not be more than length of destination");
        }

        throw new NotImplementedException("NOT WORKING");

        // set the initial array value
        ///Array.Copy(value, destinationArray, length);

        destinationArray[startPosition] = value;

        int copyLength, nextCopyLength;

        for (int i = 1; i < length; i++)
        {
            Array.Copy(destinationArray, startPosition, destinationArray, startPosition+i, i);
        }

        //for (copyLength = 1; (nextCopyLength = copyLength << 1) < startPosition+length; copyLength = nextCopyLength)
        //{
        //    Array.Copy(destinationArray, startPosition, destinationArray, copyLength, copyLength);
        //}

        //Array.Copy(destinationArray, startPosition, destinationArray, copyLength, destinationArray.Length - copyLength);
    }
}

