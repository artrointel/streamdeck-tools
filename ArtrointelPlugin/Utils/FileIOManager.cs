﻿using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using BarRaider.SdTools;

namespace ArtrointelPlugin.Utils
{
    internal class FileIOManager
    {
        // For resource directories
        public const string RES_DIR = "Res";
        public const string IMAGE_DIR = "Images";
        public const string BASE_IMAGE_NAME = "baseImage.png";

        private static Image mFallbackImage;

        public static string GetFallbackImagePath()
        {
            var Sep = Path.DirectorySeparatorChar;
            return RES_DIR + Sep + IMAGE_DIR + Sep + BASE_IMAGE_NAME;
        }

        public static string getResourceImagePath(string image)
        {
            var Sep = System.IO.Path.DirectorySeparatorChar;
            return RES_DIR + Sep + IMAGE_DIR + Sep + image;
        }

        /// <summary>
        /// Returns new image to fit the stream deck icon, 144x144
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <returns></returns>
        // https://www.c-sharpcorner.com/UploadFile/ishbandhu2009/resize-an-image-in-C-Sharp/
        public static Image ResizeImage(Image imgToResize, int newWidth = 144, int newHeight = 144)
        {
            Size size = new Size(newWidth, newHeight);

            // Get the image current width  
            int sourceWidth = imgToResize.Width;
            // Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            // Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            // Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            // New Width
            int destWidth = (int)(sourceWidth * nPercent);
            // New Height
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap bmp = new Bitmap(destWidth, destHeight);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            graphics.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            graphics.Dispose();
            return bmp;
        }

        /// <summary>
        /// load and resizes the input image to 144x144.
        /// if any fails, falls back to the default image.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>base64image</returns>
        public static string ProcessImageFileToSDBase64(String path)
        {
            string base64 = null;
            try
            {
                Image img = Image.FromFile(path);
                base64 = Tools.ImageToBase64(ResizeImage(img), false);
                img.Dispose();
            }
            catch (Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Cannot read the image:" + path + ", " + e.Message);
            }
            return base64;
        }

        public static Image LoadBase64(string base64ImageString)
        {
            return Tools.Base64StringToImage(base64ImageString);
        }

        public static string ImageToBase64(Image image)
        {
            return Tools.ImageToBase64(image, false);
        }

        public static Image GetFallbackImage()
        {
            if(mFallbackImage == null)
            {
                mFallbackImage = Image.FromFile(GetFallbackImagePath());
            }
            return mFallbackImage;
        }

        public static string GetFallbackBase64Image()
        {
            return Tools.ImageToBase64(GetFallbackImage(), false);
        }

        public static Image LoadSpinner()
        {
            return Image.FromFile(getResourceImagePath("spinner.png"));
        }

        public static Image IconFromFile(string exePath)
        {
            FileInfo fileInfo = new FileInfo(exePath);
            
            Icon icon = null;
            try
            {
                //icon = Icon.ExtractAssociatedIcon(exePath);
                icon = new Icon(exePath, 256, 256);
            } catch(Exception e)
            {
                DLogger.LogMessage(TracingLevel.ERROR, "Cannot create image : " + e.Message);
            }
            if(icon == null)
            {
                return null;
            }
            return icon.ToBitmap();
        }
    }
}
