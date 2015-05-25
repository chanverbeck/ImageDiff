using System;
using System.Collections.Generic;
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

namespace ImageDiff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string leftImageSource = @"C:\Users\Chan\Documents\Visual Studio 2013\Projects\ImageDiff\ImageDiff\TestImages\20150511_114433_03.jpg";
        private string rightImageSource = @"C:\Users\Chan\Documents\Visual Studio 2013\Projects\ImageDiff\ImageDiff\TestImages\20150511_114433_04.jpg";

        public MainWindow()
        {
            InitializeComponent();

            InitializeImage(ImageLeft, TextLeft, leftImageSource);
            InitializeImage(ImageRight, TextRight, rightImageSource);
            CompareImages();
        }

        private void InitializeImage(Image i, TextBlock block, string path)
        {
            BitmapImage left = new BitmapImage();
            left.BeginInit();
            left.UriSource = new Uri("file://" + path);
            left.EndInit();

            i.Source = left;

            block.Text = System.IO.Path.GetFileName(path);
        }

        private void Compare_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Left File";
            bool? result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                leftImageSource = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            openFileDialog.Title = "Right File";
            result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                rightImageSource = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            InitializeImage(ImageLeft, TextLeft, leftImageSource);
            InitializeImage(ImageRight, TextRight, rightImageSource);
            CompareImages();
        }

        private void CompareImages()
        {
            BitmapFrame leftFrame = GetBitmapFrame(leftImageSource);

            int leftHeight = leftFrame.PixelHeight;
            int leftWidth = leftFrame.PixelWidth;
            int leftBytesPerPixel = (leftFrame.Format.BitsPerPixel + 7) / 8;
            int leftStride = leftWidth * leftBytesPerPixel;
            //MessageBox.Show("Width x Height: " + leftWidth.ToString() + "x" + leftHeight.ToString() + " Format: " + leftFrame.Format.ToString());

            byte[] leftBytes = new byte[leftHeight * leftStride];
            leftFrame.CopyPixels(leftBytes, leftStride, 0);

            BitmapFrame rightFrame = GetBitmapFrame(rightImageSource);

            int rightHeight = rightFrame.PixelHeight;
            int rightWidth = rightFrame.PixelWidth;
            int rightBytesPerPixel = (rightFrame.Format.BitsPerPixel + 7) / 8;
            int rightStride = rightWidth * rightBytesPerPixel;
            //MessageBox.Show("Width x Height: " + rightWidth.ToString() + "x" + rightHeight.ToString() + " Format: " + rightFrame.Format.ToString());

            byte[] rightBytes = new byte[rightHeight * rightStride];
            rightFrame.CopyPixels(rightBytes, rightStride, 0);

            if (rightFrame.Format != leftFrame.Format)
            {
                MessageBox.Show("Images not the same format (" + leftFrame.Format.ToString() + "!=" + rightFrame.Format.ToString());
                return;
            }

            int diffHeight = Math.Min(leftHeight, rightHeight);
            int diffWidth = Math.Min(leftWidth, rightWidth);
            int diffStride = diffWidth * leftBytesPerPixel;

            int[] diffInts = new int[leftHeight * leftStride];
            int maxDiff = 0;
            for (int row = 0; row < diffHeight; ++row)
            {
                for (int col = 0; col < diffStride; ++col)
                {
                    diffInts[col + row * diffStride] = rightBytes[col + row*rightStride] - leftBytes[col + row*leftStride];
                    if (diffInts[col + row * diffStride] > maxDiff)
                    {
                        maxDiff = diffInts[col + row * diffStride];
                    }
                    else if (diffInts[col + row * diffStride] < -maxDiff)
                    {
                        maxDiff = (short)-diffInts[col + row * diffStride];
                    }
                }
            }
            //MessageBox.Show("MaxDiff was " + maxDiff.ToString(), "Max Diff");

            byte[] diffBytes = new byte[leftHeight * leftStride];
            double scale = 1;
            if (maxDiff != 0)
            {
                scale = 127.0 / (double)maxDiff;
            }
            for (int i = 0; i < diffInts.Length; ++i)
            {
                diffBytes[i] = (byte)((double)diffInts[i] * scale + 128.0);
            }

            ImageDiff.Source = BitmapSource.Create(diffWidth, diffHeight, leftFrame.DpiX, leftFrame.DpiY, leftFrame.Format, null, diffBytes, diffStride);
        }

        private BitmapFrame GetBitmapFrame(string sourceFile)
        {
            BitmapDecoder d = BitmapDecoder.Create(new Uri(@"file://" + sourceFile), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);

            BitmapFrame frame = d.Frames[0];

            return frame;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown(0);
        }
    }
}
