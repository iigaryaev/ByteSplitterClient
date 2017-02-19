using DataViewer.Model;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace DataViewer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool isDragging = false;

        private readonly int pageSize = 500;

        private readonly int scrollSize = 200;

        private readonly int displayWidth = 192;

        private Point anchorPoint = new Point();

        private BitmapPager pager;

        private List<Rectangle> frames = new List<Rectangle>();

        private Rectangle currentFrame;

        private void ResetFrames()
        {
            this.frames.ForEach(e => this.BackPlane.Children.Remove(e));
            this.frames.Clear();
            this.FramesGrid.Items.Clear();
            
            isDragging = false;
        }

        private void DrawVertiaclLine(double x)
        {
            var line1 = new Rectangle();
            line1.Stroke = Brushes.Green;

            line1.SetValue(Canvas.LeftProperty, x);
            line1.SetValue(Canvas.TopProperty, (double)1);

            line1.Width = 1;
            line1.Height = this.pageSize;
            
            line1.Visibility = Visibility.Visible;

            this.BackPlane.Children.Add(line1);
            this.frames.Add(line1);
        }

        private void DrawHorizontalLine(double y)
        {
            var line1 = new Rectangle();
            
            line1.Stroke = Brushes.Green;
            
            line1.SetValue(Canvas.LeftProperty, (double)1);
            line1.SetValue(Canvas.TopProperty, y);

            line1.Width = this.displayWidth;
            line1.Height = 1;

            line1.Visibility = Visibility.Visible;

            this.BackPlane.Children.Add(line1);
            this.frames.Add(line1);
        }

        private void SaveFrame()
        {
            isDragging = false;
            label_width.Content = "Width:";
            label_height.Content = "Length:";

            if(this.currentFrame.Width < 2)
            {
                this.DrawHorizontalLine(this.anchorPoint.Y);
                this.DrawHorizontalLine(this.anchorPoint.Y + this.currentFrame.Height);

                if (this.currentFrame.Width == 0)
                {
                    this.DrawVertiaclLine(this.anchorPoint.X);
                }
            }

            if (this.currentFrame.Height < 2)
            {
                this.DrawVertiaclLine(this.anchorPoint.X);
                this.DrawVertiaclLine(this.anchorPoint.X + this.currentFrame.Width);

                if (this.currentFrame.Height == 0)
                {
                    this.DrawHorizontalLine(this.anchorPoint.Y);
                }
            }

            this.FramesGrid.Items.Add(
                new { X = Math.Min(this.anchorPoint.X + 1, this.displayWidth),
                    Y = Math.Min(this.anchorPoint.Y + this.pager.CurrentPageStart, this.pager.CurrentPageStart + this.pageSize - 1),
                    Width = this.currentFrame.Width,
                    Length = this.currentFrame.Height });
            this.currentFrame = null;
        }

        private void BackPlane_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.pager == null)
                return;

            anchorPoint.X = e.GetPosition(BackPlane).X;
            anchorPoint.Y = e.GetPosition(BackPlane).Y;
            isDragging = true;

            this.currentFrame = new Rectangle();
            currentFrame.Stroke = Brushes.Orange;
            currentFrame.Fill = new SolidColorBrush(Color.FromArgb(77, 255, 255, 255));
            this.BackPlane.Children.Add(this.currentFrame);
            this.frames.Add(this.currentFrame);

            if (currentFrame.Visibility != Visibility.Visible)
                currentFrame.Visibility = Visibility.Visible;
        }

        private void BackPlane_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.pager == null)
                return;

            double x = e.GetPosition(BackPlane).X;
            double y = e.GetPosition(BackPlane).Y;

            label_x.Content = string.Format("X: {0:n0}", Math.Min(x + 1, this.displayWidth)); // some borders problem
            label_y.Content = string.Format("Y: {0:n0}", Math.Min(y + this.pager.CurrentPageStart, this.pager.CurrentPageStart + this.pageSize - 1)); // some borders problem

            if (isDragging)
            {
                currentFrame.SetValue(Canvas.LeftProperty, Math.Min(x, anchorPoint.X));
                currentFrame.SetValue(Canvas.TopProperty, Math.Min(y, anchorPoint.Y));

                var width = Math.Abs(x - anchorPoint.X);
                var height = Math.Abs(y - anchorPoint.Y);

                currentFrame.Width = width;
                currentFrame.Height = height;

                label_width.Content = string.Format("Width: {0:n0}", width);
                label_height.Content = string.Format("Length: {0:n0}", height);

                if(currentFrame.Visibility != Visibility.Visible)
                    currentFrame.Visibility = Visibility.Visible;
            }
        }

        private void BackPlane_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.currentFrame == null)
                return;
            SaveFrame();
        }

        private void BackPlane_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.currentFrame == null)
                return;
            SaveFrame();
        }

        private void DisplayBitmap(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                bitmap.Dispose();
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                this.label_pageStart.Content = string.Format("Page begins at {0}", this.pager.CurrentPageStart);

                this.ResetFrames();
                this.image.Source = bitmapimage;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            var bm = this.pager.GetPrev();
            this.DisplayBitmap(bm);
            if(this.pager.CurrentPage == 1)
            {
                this.Prev.IsEnabled = false;
            }

            if (!this.Next.IsEnabled)
            {
                this.Next.IsEnabled = true;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            var bm = this.pager.GetNext();
            this.DisplayBitmap(bm);

            if (!this.Prev.IsEnabled)
            {
                this.Prev.IsEnabled = true;
            }

            if(this.pager.CurrentPageStart + this.pager.ScrollSize >= this.pager.TotalRows)
            {
                this.Next.IsEnabled = false;
            }
        }

        private void InitFile(string filePath)
        {
            var ds = new DataSplitter();
            var data = ds.ReadFile(filePath);
            this.pager = new BitmapPager(data, this.pageSize, this.scrollSize);

            var bm = pager.GetCurrent();
            this.DisplayBitmap(bm);

            this.Prev.IsEnabled = false;
            this.Next.IsEnabled = true;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".000";
            dlg.Filter = "data Files (*.000)|*.000";

            var result = dlg.ShowDialog();
            
            if (result == true)
            {
                try
                {
                    this.InitFile(dlg.FileName);
                    label_fileName.Content = dlg.SafeFileName;
                }
                catch(Exception ex)
                {
                    var error = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
