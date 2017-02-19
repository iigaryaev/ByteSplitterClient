using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DataViewer.Model
{
    public class BitmapPager
    {
        private List<int[]> data;

        private int displayCount;

        private int scrollSize;

        public BitmapPager(List<int[]> data, int displayCount, int scrollSize)
        {
            this.data = data;
            this.displayCount = displayCount;
            this.scrollSize = scrollSize;
            this.CurrentPage = 1;
        }

        public int CurrentPage { get; private set; }

        public int CurrentPageStart
        {
            get
            {
                return (this.CurrentPage - 1) * this.scrollSize + 1;
            }
        }

        public int TotalRows { get { return this.data.Count;  } }

        public int ScrollSize { get { return this.scrollSize; } }

        private Bitmap GetBitmap()
        {
            var bm = new Bitmap(data[0].Length, this.displayCount);

            int x = 0, y = 0;

            foreach (var row in data.GetRange(this.CurrentPageStart - 1, this.displayCount))
            {
                x = 0;

                foreach (var col in row)
                {
                    var color = 255 * col / 2047;
                    bm.SetPixel(x, y, Color.FromArgb(color, color, color));
                    ++x;
                }

                ++y;
            }

            return bm;
        }

        public Bitmap GetCurrent()
        {
            return this.GetBitmap();
        }

        public Bitmap GetNext()
        {
            if(this.CurrentPageStart + this.displayCount <= this.data.Count)
            {
                ++this.CurrentPage;
            }

            return this.GetBitmap();
        }

        public Bitmap GetPrev()
        {
            if(this.CurrentPage > 1)
            {
                --this.CurrentPage;
            }

            return this.GetBitmap();
        }
    }
}
