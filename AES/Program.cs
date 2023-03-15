﻿using System;
using System.Text;

namespace AES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var key = new byte[]
            {
                116, 249, 36, 5, 201, 243, 254, 74, 91, 59, 160, 208, 35, 75, 51, 209,
245, 197, 110, 94, 44, 252, 165, 253, 213, 12, 27, 173, 77, 230, 112, 36,
212, 228, 157, 41, 241, 136, 106, 100, 53, 106, 69, 172, 243, 46, 218, 235,
60, 147, 130, 140, 22, 190, 108, 31, 220, 240, 61, 108, 174, 157, 236, 49,
165, 75, 157, 57, 108, 145, 146, 90, 25, 117, 101, 204, 59, 198, 37, 39,
51, 238, 86, 213, 201, 154, 57, 88, 88, 110, 15, 149, 28, 166, 155, 27,
111, 48, 79, 109, 44, 111, 232, 164, 35, 248, 57, 182, 113, 132, 159, 154,
226, 127, 36, 143, 134, 32, 30, 80, 202, 170, 246, 145, 170, 18, 101, 57,
77, 223, 23, 205, 112, 159, 68, 197, 229, 55, 135, 129, 20, 7, 83, 133,
85, 246, 102, 63, 49, 198, 159, 25, 12, 156, 137, 115, 176, 113, 200, 96,
246, 64, 143, 182, 247, 174, 186, 223, 165, 39, 105, 54, 154, 3, 136, 185,
249, 240, 98, 107, 209, 224, 130, 112, 22, 65, 237, 59, 94, 154, 246, 5,
22, 177, 199, 57, 198, 86, 138, 93, 163, 132, 168, 186, 87, 27, 171, 201,
219, 8, 238, 59, 7, 103, 67, 191, 100, 248, 6, 120, 149, 72, 172, 174,
191, 79, 110, 144, 206, 195, 220, 157, 184, 12, 244, 122, 194, 138, 50, 229,
146, 82, 18, 202, 161, 83, 221, 160, 148, 24, 176, 229, 199, 107, 106, 42
            };
            var aes = new AES(key);
            var codingString = "Hello this world";
            var convertingBytes = Encoding.Default.GetBytes(codingString);
            foreach(var item in convertingBytes)
                Console.Write(item + " ");
            Console.WriteLine();
            var encodingArray = aes.Encode(convertingBytes);
            foreach (var item in encodingArray)
                Console.Write(item + " ");
            Console.WriteLine();
            var decodingArray = aes.Decode(encodingArray);
            foreach (var item in decodingArray)
                Console.Write(item + " ");
        }
    }
}