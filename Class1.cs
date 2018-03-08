using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Diagnostics;

namespace Random_Imgur_Downloader.Resources
{
    class RID
    {
        //Static variables
        public static string imgurLink = "http://i.imgur.com/";
        public int maxThreads = 150;
        public static string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
        private static Random random = new Random();
        public static int urlSize = 5;
        public static ReaderWriterLock locker = new ReaderWriterLock();

        //Generates the random string with optional urlSize to be passed to it.
        public static string GenerateString(int urlSize = 5)
        {
            var stringChar = new char[(urlSize)];
            for (int i = 0; i < (urlSize); i++)
            {
                stringChar[i] = chars[random.Next(chars.Length)];
            }

            var imageString = new String(stringChar);
            //Append .jpg, OS handles openning in the correct format. Base64
            return imageString + ".jpg";
        }

        //Combines the randomly generate url with the url - optional for later changes to url
        public static string CreateURL(string imgName, string url = "http://i.imgur.com/")
        {
            return url + imgName;
        }

        //Downloads the image and returns a image object from the stream provided by the http request
        public static System.Drawing.Image DownloadImageFromUrl(string imageUrl)
        {
            System.Drawing.Image image = null;

            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                //Timeout should probably be lower to prevent thread overload. w/e
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);

                webResponse.Close();
            }
            //Logging in future releases~~~ XD
            catch (Exception ex)
            {
                return null;
            }
            return image;
        }

        //Takes the write lock and saves the image to the targetDirectory and uses the fileName
        public static bool SaveImage(string targetDirectory, string fileName, System.Drawing.Image image)
        {
            locker.AcquireWriterLock(int.MaxValue);
            string filePath = System.IO.Path.Combine(targetDirectory, fileName);
            try
            {
                image.Save(filePath);
            }
            //Writes regardless so we can delete an empty file. w/e works
            catch (Exception)
            {
                File.Delete(filePath);
                locker.ReleaseWriterLock();
                return true;
            }
            //Why the default missing image change from 509 to 538 imgur please.
            long imageBytes = new System.IO.FileInfo(filePath).Length;
            if (imageBytes < 538)
            {
                File.Delete(filePath);
                locker.ReleaseWriterLock();
                return true;
            }
            else
            {
                locker.ReleaseWriterLock();
                return false;
            }
        }

        //Handles the creation of the url, download of image, and writelock saving of image
        public static void PrimaryThread(string targetDirectory, int urlSize, int runSize)
        {
            bool catchBool = true;
            while (catchBool == true)
            {
                string targetString = GenerateString(urlSize);
                string targetURL = CreateURL(targetString);
                System.Drawing.Image image = DownloadImageFromUrl(targetURL);
                catchBool = SaveImage(targetDirectory, targetString, image);
            }
        }
        //Not a real queue manager, just delays creating new threads to max 40 per 5 seconds
        public static void QueueManager(string targetDirectory, int urlSize, int runSize)
        {
                for (int i = 0; i < runSize; i++)
                {
                if ((i % 40) == 0)
                {
                    System.Threading.Thread.Sleep(5000);
                }
                Thread t = new Thread(() => PrimaryThread(targetDirectory, urlSize, runSize));
                t.Start();
            }
        }

        public static void KickOff(string targetDirectory, int urlSize, int runSize)
        {
            Thread t = new Thread(() => QueueManager(targetDirectory, urlSize, runSize));
            t.Start();
        }
    }
}
