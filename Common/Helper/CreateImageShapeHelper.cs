using JeffTools.CDRTool.Common.Helpers;
using Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JeffTools.CDRTool.Common.Helper
{
    public class CreateImageShapeHelper
    {
        public string CreateImageShape(string path,int blocks, bool isGray, bool isRevert) {

            BitmapImage bitmap = new BitmapImage(new Uri(path, UriKind.Absolute));
            int width;
            int height;
            byte[] pixels;

            WriteableBitmap grayImage = RGB2Gray(bitmap);


            if (isGray)
            {
                width = grayImage.PixelWidth;
                height = grayImage.PixelHeight;
                pixels = new byte[width * height];
            }
            else
            {
                width = bitmap.PixelWidth;
                height = bitmap.PixelHeight;
                pixels = new byte[width * height * 4];
            }



            //固定分块的行列数
            int rows = blocks;
            int cols = blocks*width/height;

            float blockWidth = (float)width /(float) cols;
            float blockHeight = (float)height / (float)rows;


            SvgDocument SvgDoc=new SvgDocument()
            {
                Width = new SvgUnit(width),
                Height = new SvgUnit(height)
            };

            if (isGray)
            {
                int stride = width;
                
                grayImage.CopyPixels(pixels, stride, 0);
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        float startX = col * blockWidth;
                        float startY = row * blockHeight;

                        int sumGray = 0;
                        int pixelCount = 0;

                        for(int y = (int)startY; y<startY+blockHeight && y <height;y++)
                        {
                            for (int x = (int)startX; x<startX+blockWidth && x <width;x++)
                            {
                                sumGray += pixels[y*stride + x];
                                pixelCount++;
                            }
                        }
                        int averageGray = sumGray / pixelCount;

                        float maxRadius = Math.Min(blockWidth, blockHeight) / 2f;
                        float radius = (1-averageGray / 255f) * maxRadius;


                        if(isRevert)
                        {
                            radius = maxRadius*(averageGray/255f);
                        }

                        SvgCircle circle = new SvgCircle()
                        {
                            CenterX = new SvgUnit(startX + blockWidth / 2f),
                            CenterY = new SvgUnit(startY + blockHeight / 2f),
                            Radius = new SvgUnit(radius)
                        };

                        SvgDoc.Children.Add(circle);
                    }
                }
            }
            else
            {
                int stride = width * 4;
                bitmap.CopyPixels(pixels, stride, 0);

                for(int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        float startX = col * blockWidth;
                        float startY = row * blockHeight;
                        int sumR = 0, sumG = 0, sumB = 0;
                        int pixelCount = 0;
                        for (int y = (int)startY; y < startY + blockHeight && y < height; y++)
                        {
                            for (int x = (int)startX; x < startX + blockWidth && x < width; x++)
                            {
                                int pixelIndex = (y * stride) + (x * 4);
                                sumR += pixels[pixelIndex + 2];
                                sumG += pixels[pixelIndex + 1];
                                sumB += pixels[pixelIndex];
                                pixelCount++;
                            }
                        }
                        byte avgR = (byte)(sumR / pixelCount);
                        byte avgG = (byte)(sumG / pixelCount);
                        byte avgB = (byte)(sumB / pixelCount);
                        float radius = Math.Min(blockWidth, blockHeight) / 2f;
                        //float maxRadius = Math.Min(blockWidth, blockHeight) / 2f;
                        //float radius = (1 - ((avgR + avgG + avgB) / (3f * 255f))) * maxRadius;
                        //if (isRevert)
                        //{
                        //    radius = maxRadius * ((avgR + avgG + avgB) / (3f * 255f));
                        //}
                        SvgCircle circle = new SvgCircle()
                        {
                            CenterX = new SvgUnit(startX + blockWidth / 2f),
                            CenterY = new SvgUnit(startY + blockHeight / 2f),
                            Radius = new SvgUnit(radius),
                            Fill = new SvgColourServer(System.Drawing.Color.FromArgb(avgR, avgG, avgB))
                        };
                        SvgDoc.Children.Add(circle);
                    }
                }
            }
                ConfigHelper Config = new ConfigHelper();
            string tempFile = System.IO.Path.Combine(Config.GetTempPath(), $"shape{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.svg");
            SvgDoc.Write(tempFile);

            return tempFile;
        }

        private WriteableBitmap RGB2Gray(BitmapImage bitmap)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            WriteableBitmap grayBitmap = new WriteableBitmap(width, height, bitmap.DpiX, bitmap.DpiY, PixelFormats.Gray8, null);

            // Create a byte array to hold pixel data  
            byte[] pixels = new byte[width * height * 4];
            bitmap.CopyPixels(pixels, width * 4, 0);

            byte[] grayPixels = new byte[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width + x) * 4;
                    byte r = pixels[pixelIndex + 2];
                    byte g = pixels[pixelIndex + 1];
                    byte b = pixels[pixelIndex];
                    byte grayValue = (byte)((r + g + b) / 3);
                    grayPixels[y * width + x] = grayValue;
                }
            }

            grayBitmap.WritePixels(new Int32Rect(0, 0, width, height), grayPixels, width, 0);
            return grayBitmap;
        }
    }
}
