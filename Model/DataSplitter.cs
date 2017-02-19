using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DataSplitter
    {
        private int leftover = 8;

        private int outIndex = 0;

        private int readingValue;

        private int fullness = 0;

        private int currentIndex = 0;

        private readonly int chunkSize = 10;

        private readonly int rowLength = 240;

        IEnumerator<byte> enumerator;

        public DataSplitter()
        {
        }

        private void Init()
        {
            this.leftover = 8;
            this.outIndex = 0;
            this.fullness = 0;
        }

        public List<int[]> ReadFile(string filePath)
        {
            var currentRow = 0;

            var data = new List<int[]>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var row = new byte[rowLength];
                while (true)
                {
                    var readed = fs.Read(row, 0, rowLength);
                    if (readed == 0)
                    {
                        break;
                    }

                    if(readed < rowLength)
                    {
                        throw new Exception("File corrupted. It can not be read as 240 length lines");
                    }

                    var result = this.Split(row);
                    data.Add(result);
                    ++currentRow;
                }
            }
            return data;
        }

        private bool MoveNext()
        {
            var notEmpty = enumerator.MoveNext();
            this.readingValue = enumerator.Current;
            this.leftover = 8;
            ++currentIndex;
            return notEmpty;
        }

        public int[] Split(byte[] data)
        {
            if(data.Length * 8 % chunkSize != 0)
            {
                throw new Exception("data can not be splitted");
            }

            this.Init();

            var outLength = data.Length * 8 / chunkSize;
            var outData = new int[outLength];

            int? subvalue1 = null, subvalue2 = null;

            this.enumerator = data.ToList().GetEnumerator();
            

            var notEmpty = this.MoveNext();

            while (notEmpty)
            {
                if (!subvalue1.HasValue)
                {
                    var bitsToTake = leftover;

                    var mask = 0xFF >> (8 - bitsToTake);

                    subvalue1 = (readingValue & mask) << (chunkSize - leftover);

                    fullness = bitsToTake;

                    leftover = leftover - bitsToTake;

                    if(leftover == 0)
                    {
                        notEmpty = this.MoveNext();

                        if(!notEmpty)
                        {
                            break;
                        }
                    }
                }

                if(!subvalue2.HasValue)
                {
                    var bitsToTake = chunkSize - fullness;
                    leftover = 8 - bitsToTake;
                    subvalue2 = readingValue & (0xFF << leftover);

                    if(!subvalue1.HasValue)
                    {
                        throw new Exception("subvalue1 undefined");
                    }

                    if (!subvalue2.HasValue)
                    {
                        throw new Exception("subvalue2 undefined");
                    }

                    outData[outIndex] = (subvalue1 + subvalue2).Value;
                    outIndex++;

                    subvalue1 = null;
                    subvalue2 = null;

                    if (leftover == 0)
                    {
                        notEmpty = this.MoveNext();
                    }
                }
            }

            return outData;
        }
    }
}
