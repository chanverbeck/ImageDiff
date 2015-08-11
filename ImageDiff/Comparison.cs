using System;
using System.Windows.Media.Imaging;
using System.IO;

namespace ImageDiff
{
    public class Comparison
    {
        public Uri LeftImage { private set; get; }
        public string LeftName { private set; get; }
        public Uri RightImage { private set; get; }
        public string RightName { private set; get; }
        public BitmapSource Result { private set; get; }
        public string AverageAbsoluteDifference { private set; get; }
        public string AverageSquareDifference { private set; get; }

        public Comparison(string leftFile, string rightFile)
        {
            this.LeftImage = new Uri("file://" + leftFile);
            this.LeftName = Path.GetFileName(leftFile);
            this.RightImage = new Uri("file://" + rightFile);
            this.RightName = Path.GetFileName(rightFile);

            CompareImages(leftFile, rightFile);
        }

        private void CompareImages(string leftImageSource, string rightImageSource)
        {
            BitmapFrame leftFrame = GetBitmapFrame(leftImageSource);

            int leftHeight = leftFrame.PixelHeight;
            int leftWidth = leftFrame.PixelWidth;
            int leftBytesPerPixel = (leftFrame.Format.BitsPerPixel + 7) / 8;
            int leftStride = leftWidth * leftBytesPerPixel;

            byte[] leftBytes = new byte[leftHeight * leftStride];
            leftFrame.CopyPixels(leftBytes, leftStride, 0);

            BitmapFrame rightFrame = GetBitmapFrame(rightImageSource);

            int rightHeight = rightFrame.PixelHeight;
            int rightWidth = rightFrame.PixelWidth;
            int rightBytesPerPixel = (rightFrame.Format.BitsPerPixel + 7) / 8;
            int rightStride = rightWidth * rightBytesPerPixel;

            byte[] rightBytes = new byte[rightHeight * rightStride];
            rightFrame.CopyPixels(rightBytes, rightStride, 0);

            if (rightFrame.Format != leftFrame.Format)
            {
                throw new InvalidDataException("Images not the same format(" + leftFrame.Format.ToString() + " != " + rightFrame.Format.ToString());
            }

            int diffHeight = Math.Min(leftHeight, rightHeight);
            int diffWidth = Math.Min(leftWidth, rightWidth);
            int diffStride = diffWidth * leftBytesPerPixel;

            int pixelCount = diffHeight * diffStride;
            double totalAbsoluteDifference = 0;
            double totalSquareDifference = 0;

            int[] diffInts = new int[leftHeight * leftStride];
            int maxDiff = 0;
            for (int row = 0; row < diffHeight; ++row)
            {
                for (int col = 0; col < diffStride; ++col)
                {
                    int difference = rightBytes[col + row * rightStride] - leftBytes[col + row * leftStride];

                    totalAbsoluteDifference += Math.Abs(difference);
                    totalSquareDifference += difference * difference;

                    diffInts[col + row * diffStride] = difference;
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

            AverageAbsoluteDifference = (totalAbsoluteDifference / (double)pixelCount).ToString();
            AverageSquareDifference = (totalSquareDifference / (double)pixelCount).ToString();
            Result = BitmapSource.Create(diffWidth, diffHeight, leftFrame.DpiX, leftFrame.DpiY, leftFrame.Format, null, diffBytes, diffStride);
        }

        private static BitmapFrame GetBitmapFrame(string sourceFile)
        {
            BitmapDecoder d = BitmapDecoder.Create(new Uri(@"file://" + sourceFile), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);

            BitmapFrame frame = d.Frames[0];

            return frame;
        }


    }
}
