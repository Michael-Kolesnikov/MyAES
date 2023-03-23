using FF;

namespace MyAES
{
    public class AES
    {
        private readonly FiniteField field = new(2, 8, new int[] { 1, 0, 0, 0, 1, 1, 1, 0, 1 }); // x^8+x^4+x^3+x^2+1
        private readonly byte[] key;
        private readonly int convertingNumber = 16;

        // must be the square of some number
        private readonly int fragmentationSize = 16;

        public AES(byte[] key)
        {
            if (key.Length != 256)
            {
                throw new ArgumentException("key size does not match the given parameters");
            }

            this.key = new byte[key.Length];
            Array.Copy(key, this.key, this.key.Length);
        }

        public byte[] Encode(byte[] msg)
        {
            if (msg.Length % this.fragmentationSize != 0)
            {
                throw new ArgumentException("message size does not match the given parameters");
            }

            var result = new List<byte>();
            for (var i = 0; i < msg.Length; i += this.fragmentationSize)
            {
                byte[,] msgBlock = CreateMsgBlock(i, msg);
                for (var j = 0; j < this.convertingNumber; j++)
                {
                    // each byte is converted to an element of the final field, then its reverse is taken
                    byte[,] keyBlock = CreateKeyBlock(j * this.fragmentationSize);
                    msgBlock = GetInverse(msgBlock);
                    msgBlock = GetXOR(msgBlock, keyBlock);
                    msgBlock = ShiftRows(msgBlock);
                    msgBlock = ShiftColumns(msgBlock);
                }

                byte[] msg2 = msgBlock.Cast<byte>().ToArray();
                result.AddRange(msg2);
            }

            return result.ToArray();
        }

        public byte[] Decode(byte[] msg)
        {
            if (msg.Length % this.fragmentationSize != 0)
            {
                throw new ArgumentException("msg length must divide by 16");
            }

            var result = new List<byte>();
            for (var i = 0; i < msg.Length; i += this.fragmentationSize)
            {
                byte[,] msgBlock = CreateMsgBlock(i, msg);
                for (var j = 0; j < this.convertingNumber; j++)
                {
                    byte[,] keyBlock = CreateKeyBlock(this.key.Length - this.fragmentationSize - (j * this.fragmentationSize));

                    msgBlock = ShiftColumns(msgBlock);
                    msgBlock = ShiftRows(msgBlock);
                    msgBlock = GetXOR(msgBlock, keyBlock);
                    msgBlock = GetInverse(msgBlock);
                }

                byte[] msg2 = msgBlock.Cast<byte>().ToArray();
                result.AddRange(msg2);
            }

            return result.ToArray();
        }

        private static byte[,] GetXOR(byte[,] msgBlock, byte[,] keyBlock)
        {
            for (var i = 0; i < msgBlock.GetLength(0); i++)
            {
                for (var j = 0; j < msgBlock.GetLength(1); j++)
                {
                    msgBlock[i, j] = (byte)(msgBlock[i, j] ^ keyBlock[i, j]);
                }
            }

            return msgBlock;
        }

        private static byte[,] ShiftRows(byte[,] msgBlock)
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

        private static byte[,] ShiftColumns(byte[,] msgBlock)
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

        private byte[,] GetInverse(byte[,] msgBlock)
        {
            for (var i = 0; i < msgBlock.GetLength(0); i++)
            {
                for (var j = 0; j < msgBlock.GetLength(1); j++)
                {
                    if (msgBlock[i, j] != 0)
                    {
                        msgBlock[i, j] = this.field.GetFiniteFieldElement(new byte[] { msgBlock[i, j], 0, 0, 0 }).GetInverse().ConvertToByte()[0];
                    }
                }
            }

            return msgBlock;
        }

        private byte[,] CreateMsgBlock(int startIndex, byte[] msg)
        {
            var matrixSize = (int)Math.Sqrt(this.fragmentationSize);
            var block = new byte[matrixSize, matrixSize];
            for (var i = 0; i < matrixSize; i++)
            {
                for (var j = 0; j < matrixSize; j++)
                {
                    block[i, j] = msg[startIndex++];
                }
            }

            return block;
        }

        private byte[,] CreateKeyBlock(int startIndex)
        {
            startIndex %= this.key.Length;
            var matrixSize = (int)Math.Sqrt(this.fragmentationSize);
            var block = new byte[matrixSize, matrixSize];
            for (var i = 0; i < matrixSize; i++)
            {
                for (var j = 0; j < matrixSize; j++)
                {
                    block[i, j] = this.key[startIndex++];
                }
            }

            return block;
        }
    }
}
