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

            InitializeImage(ImageLeft, leftImageSource);
            InitializeImage(ImageRight, rightImageSource);
        }

        private void InitializeImage(Image i, string path)
        {
            BitmapImage left = new BitmapImage();
            left.BeginInit();
            left.UriSource = new Uri("file://" + path);
            left.EndInit();

            i.Source = left;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BitmapFrame leftFrame = GetBitmapFrame(leftImageSource);

            int leftHeight = leftFrame.PixelHeight;
            int leftWidth = leftFrame.PixelWidth;
            int leftBytesPerPixel = (leftFrame.Format.BitsPerPixel + 7) / 8;
            int leftStride = leftWidth * leftBytesPerPixel;
            MessageBox.Show("Width x Height: " + leftWidth.ToString() + "x" + leftHeight.ToString() + " Format: " + leftFrame.Format.ToString());
            
            byte[] leftBytes = new byte[leftHeight * leftStride];
            leftFrame.CopyPixels(leftBytes, leftStride, 0);

            BitmapFrame rightFrame = GetBitmapFrame(rightImageSource);

            int rightHeight = rightFrame.PixelHeight;
            int rightWidth = rightFrame.PixelWidth;
            int rightBytesPerPixel = (rightFrame.Format.BitsPerPixel + 7) / 8;
            int rightStride = rightWidth * rightBytesPerPixel;
            MessageBox.Show("Width x Height: " + rightWidth.ToString() + "x" + rightHeight.ToString() + " Format: " + rightFrame.Format.ToString());

            byte[] rightBytes = new byte[leftHeight * leftStride];
            rightFrame.CopyPixels(rightBytes, leftStride, 0);

            int[] diffInts = new int[leftHeight * leftStride];
            int maxDiff = 0;
            for (int i = 0; i < diffInts.Length; ++i)
            {
                diffInts[i] = rightBytes[i] - leftBytes[i];
                if (diffInts[i] > maxDiff)
                {
                    maxDiff = diffInts[i];
                }
                else if (diffInts[i] < -maxDiff)
                {
                    maxDiff = (short) -diffInts[i];
                }
            }
            MessageBox.Show("MaxDiff was " + maxDiff.ToString(), "Max Diff");

            byte[] diffBytes = new byte[leftHeight * leftStride];
            for (int i = 0; i < diffInts.Length; ++i)
            {
                diffBytes[i] = (byte)((float)diffInts[i] * 127.0 / (float)maxDiff + 128);
            }

            ImageDiff.Source = BitmapSource.Create(leftWidth, leftHeight, leftFrame.DpiX, leftFrame.DpiY, leftFrame.Format, null, diffBytes, leftStride);
        }

        private BitmapFrame GetBitmapFrame(string sourceFile)
        {
            BitmapDecoder d = BitmapDecoder.Create(new Uri(@"file://" + sourceFile), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);

            BitmapFrame frame = d.Frames[0];

            return frame;
        }
    }
}
