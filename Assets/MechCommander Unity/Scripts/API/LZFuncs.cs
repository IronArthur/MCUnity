
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MechCommanderUnity.API
{
    internal static class LZFuncs
    {
        static int HASH_CLEAR = 256;           //clear hash table command code
        static int HASH_EOF = 257;            //End Of Data command code
        static int HASH_FREE = 258;             //First Hash Table Chain Offset Value
        static int BASE_BITS = 9;
        static int MAX_BIT_INDEX = (1 << BASE_BITS);

        public struct HashStruct
        {
            public uint prev, back;
            public byte c;
        };


        //HashStruct LZOldChain = NULL;            //Old Chain Value Found
        //HashStruct LZChain = NULL;               //Current Chain Value Found 
        static uint LZMaxIndex = 0;               //Max index value in Hash Table
        static uint LZCodeMask = 0;
        static uint LZFreeIndex = 0;          //Current Free index into Hash Table
        //MemoryPtr LZSrcBufEnd = NULL;           //ptr to 3rd from last byte in src buffer
        //MemoryPtr LZOrigDOSBuf = NULL;      //original offset to start of src buffer
        //char LZHashBuffer = new char[16384];
        //char LZOldSuffix = 0;           //Current Suffix Value found

        static Dictionary<uint, HashStruct> dictionary;


        //[DllImport("API/LZFuncs", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern int LZDecomp([Out] byte[] outputBuffer, [In] byte[] compressedBuffer, uint compBufferLength);

        //[DllImport("API/LZFuncs", CharSet = CharSet.None, ExactSpelling = false)]
        //internal static extern int LZCompress([Out] byte[] compDataBuff, [In] byte[] uncompDataBuff, uint uncompDataSize);


        public static int LZDecomp(out byte[] outputBuffer, byte[] compressedBuffer, uint outputLength, uint compBufferLength)
        {
            
            BASE_BITS = 9;
            MAX_BIT_INDEX = (1 << BASE_BITS);

            //INIT
            //LZSrcBufEnd = compressedBuffer[compBufferLength - 3];

            LZCodeMask = (uint)MAX_BIT_INDEX - 1;
            LZMaxIndex = (uint)MAX_BIT_INDEX; //max index for 9 bits == 512
            LZFreeIndex = (uint)HASH_FREE;//set index to 258

            List<byte> uncompressed = new List<byte>();

            uint code, c, t, next_code = (uint)HASH_FREE;
            int bits = 8;
            HashStruct aux;

            clearDictionary();

            int shift = 0;
            int shiftInc = 1;

            for (int i = 0; i < compBufferLength; i++)
            {

                if (shift > 7)
                {
                    shift -= 8;
                    continue;
                }


                code = ReadCode(compressedBuffer, i, bits, shift);

                shift += shiftInc;



                if (code == HASH_EOF) break;
                if (code == HASH_CLEAR)
                {
                    bits = 8;
                    shiftInc = 1;
                    clearDictionary();
                    continue;
                }

                if (code >= LZFreeIndex)
                    throw new Exception("Bad sequence.");

                HashStruct next = new HashStruct();
                next.prev = c = code;

                if (!dictionary.ContainsKey(LZFreeIndex))
                {
                    dictionary.Add(LZFreeIndex, next);
                    while (c > 255)
                    {
                        t = dictionary[c].prev;
                        aux = dictionary[t];
                        aux.back = c;
                        dictionary[t] = aux;
                        c = t;
                    }
                    dictionary[LZFreeIndex] = next;
                }
                if (dictionary.ContainsKey(LZFreeIndex - 1))
                {
                    aux = dictionary[LZFreeIndex - 1];
                    aux.c = (byte)c;
                    dictionary[LZFreeIndex - 1] = aux;

                    LZFreeIndex++;
                }


                while (dictionary[c].back > 0)
                {
                    // write_out(d[c].c);
                    uncompressed.Add((byte)dictionary[c].c);
                    t = dictionary[c].back;
                    aux = dictionary[c];
                    aux.back = 0;
                    dictionary[c] = aux;
                    c = t;
                }
                uncompressed.Add((byte)dictionary[c].c);

                if (LZFreeIndex > LZMaxIndex)
                {
                    if (bits == 11)
                    {
                        continue;
                    } else
                    {
                        MAX_BIT_INDEX *= 2;
                        bits++;
                        LZCodeMask = (uint)MAX_BIT_INDEX - 1;
                        LZMaxIndex = (uint)MAX_BIT_INDEX;
                        shiftInc++;
                    }

                }
            }

            outputBuffer = new byte[outputLength];

            Array.Copy(uncompressed.ToArray(), outputBuffer, uncompressed.Count);
            //      outputBuffer = uncompressed.ToArray();
            return uncompressed.Count;
        }


        static uint ReadCode(byte[] compressedBuffer, int index, int bits, int shift = 0)
        {
            uint code = 0;

            if (index + bits <= compressedBuffer.Length)
            {
                code = BitConverter.ToUInt32(compressedBuffer.Skip(index).Take(bits).ToArray(), 0);
            } else
            {
                var array = compressedBuffer.Skip(index).Take(compressedBuffer.Length - index).ToArray();

                if (array.Length <= 2)
                {
                    return (uint)HASH_EOF;
                } else if (array.Length < 4)
                {
                    var arrayN = new byte[4];
                    Array.Clear(arrayN, 0, 4);

                    Array.Copy(array, 0, arrayN, 0, array.Length);
                    array = arrayN;

                }
                code = BitConverter.ToUInt32(array, 0);
            }



            code = code >> shift;

            code = code & LZCodeMask;

            return code;
        }

        private static void clearDictionary()
        {
            MAX_BIT_INDEX = (1 << BASE_BITS);
            LZCodeMask = (uint)MAX_BIT_INDEX - 1;
            LZMaxIndex = (uint)MAX_BIT_INDEX;

            LZFreeIndex = (uint)HASH_FREE;//set index to 258

            dictionary = new Dictionary<uint, HashStruct>();

            for (uint i = 0; i < 256; i++)
                dictionary.Add(i, new HashStruct { c = (byte)i, prev = 0, back = 0 });

            dictionary.Add(256, new HashStruct { c = (byte)0, prev = 0, back = 0 });
            dictionary.Add(257, new HashStruct { c = (byte)0, prev = 0, back = 0 });


        }
    
}
}
