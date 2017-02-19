using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteSplitterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = "C:\\Users\\Ilya\\Downloads\\DataExCh\\DataExCh.000";

            var rowLength = 240;
            var currentRow = 0;
            var chunkSize = 10;

            var ds = new DataSplitter(chunkSize);

            var data = new List<int[]>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var row = new byte[rowLength];
                while(true)
                {
                    var readed = fs.Read(row, 0, rowLength);
                    if(readed <= 0)
                    {
                        break;
                    }

                    var result = ds.Split(row);
                    data.Add(result);
                    ++currentRow;
                }
            }
        }
    }
}
