using FF;
using System;
using System.Text;

namespace AES
{
    public class AES
    {
        private FiniteField field = new FiniteField(2, 8,new int[] {1,0,0,0,1,1,1,0,1}); //x^8+x^4+x^3+x^2+1
        private byte[] _key;
        private int _convertingNumber = 16;
        // must be the square of some number
        private int _fragmentationSize = 16;
        public AES(byte[] key)
        {
            CreateKey(key);
        }
        private void CreateKey(byte[] key)
        {
            if (key.Length != 256) throw new ArgumentException();
            _key = key;
        }
        public byte[] Encode(byte[] msg)
        {
            if(msg.Length % _fragmentationSize != 0) throw new ArgumentException();
            var result = new List<byte>();
            for(var i = 0; i < msg.Length; i+= _fragmentationSize)
            {
                byte[,] msgBlock = CreateMsgBlock(i,msg);
                byte[,] keyBlock = CreateKeyBlock(i);
                for(var j = 0;j < _convertingNumber;j++)
                {
                    msgBlock = GetInverse(msgBlock);
                    msgBlock = GetXOR(msgBlock,keyBlock);
                    msgBlock = ShiftRows(msgBlock);
                    msgBlock = ShiftColomns(msgBlock);
                }
                byte[] msg2 = msgBlock.Cast<byte>().ToArray();
                //result += Encoding.Default.GetString(msg2, 0, msg2.Length);
                result.AddRange(msg2);
            }
            return result.ToArray();
        }
        private byte[,] CreateMsgBlock(int startIndex, byte[] msg)
        {
            var matrixSize = (int)Math.Sqrt(_fragmentationSize);
            var block = new byte[matrixSize, matrixSize];
            for (var i = 0; i < matrixSize; i++)
                for(var j = 0;j < matrixSize; j++)
                    block[i, j] = msg[startIndex++];
            return block;
        }
        private byte[,] CreateKeyBlock(int startIndex)
        {
            startIndex %= _key.Length;
            var matrixSize = (int)Math.Sqrt(_fragmentationSize);
            var block = new byte[matrixSize, matrixSize];
            for (var i = 0; i < matrixSize; i++)
                for (var j = 0; j < matrixSize; j++)
                    block[i, j] = _key[startIndex++];
            return block;
        }
        private byte[,] GetInverse(byte[,] msgBlock)
        {
            for (var i = 0; i < msgBlock.GetLength(0); i++)
                for (var j = 0; j < msgBlock.GetLength(1); j++)
                    if (msgBlock[i, j] != 0)
                        msgBlock[i, j] = field.GetFiniteFieldElement(new byte[] { msgBlock[i, j], 0, 0, 0 }).GetInverse().ConvertToByte()[0];
            return msgBlock;
        }
        private byte[,] GetXOR(byte[,] msgBlock, byte[,] keyBlock)
        {
            for (var i = 0; i < msgBlock.GetLength(0); i++)
                for (var j = 0; j < msgBlock.GetLength(1); j++)
                    if (msgBlock[i, j] != 0)
                        msgBlock[i, j] = (byte)(msgBlock[i, j] ^ keyBlock[i, j]);
            return msgBlock;
        }
        private byte[,] ShiftRows(byte[,] msgBlock)
        {
            byte[,] tempArray = new byte[msgBlock.GetLength(0), msgBlock.GetLength(0)];

            for (var i = 0; i < tempArray.GetLength(0); i++)
            {
                for (var j = 0; j < tempArray.GetLength(0); j++)
                {
                    int newColumnIndex = (i + 2) % msgBlock.GetLength(0);
                    tempArray[newColumnIndex, j] = msgBlock[i, j];
                }
            }
            return tempArray;
        }
        private byte[,] ShiftColomns(byte[,] msgBlock)
        {
            byte[,] tempArray = new byte[msgBlock.GetLength(0), msgBlock.GetLength(0)];
            for (int i = 0; i < tempArray.GetLength(0); i++)
            {
                for (int j = 0; j < tempArray.GetLength(0); j++)
                {
                    int newColumnIndex = (j + 2) % msgBlock.GetLength(0);
                    tempArray[i, newColumnIndex] = msgBlock[i, j];
                }
            }
            return tempArray;
        }
        public byte[] Decode(byte[] msg)
        {
            if (msg.Length % 16 != 0) throw new ArgumentException();
            var result = new List<byte>();
            for (var i = 0; i < msg.Length; i += _fragmentationSize)
            {
                byte[,] msgBlock = CreateMsgBlock(i, msg);
                byte[,] keyBlock = CreateKeyBlock(i);
                for (var j = 0; j < _convertingNumber; j++)
                {
                    msgBlock = ShiftColomns(msgBlock);
                    msgBlock = ShiftRows(msgBlock);
                    msgBlock = GetXOR(msgBlock, keyBlock);
                    msgBlock = GetInverse(msgBlock);
                }
                byte[] msg2 = msgBlock.Cast<byte>().ToArray();
                result.AddRange(msg2);
            }
            return result.ToArray();
        }
    }
}
