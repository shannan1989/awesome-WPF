using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Shannan.Core.Framework
{
    internal class ImageQueueItem
    {
        public string Url { get; set; }
        public Image Image { get; set; }
    }

    internal static class ImageQueue
    {
        public delegate void DownloadCompletedEventHandler(Image image, string url, BitmapImage bitmapImage);

        public static event DownloadCompletedEventHandler DownloadCompleted;

        private static AutoResetEvent autoResetEvent;
        private static Queue<ImageQueueItem> stacks;

        static ImageQueue()
        {
            stacks = new Queue<ImageQueueItem>();
            autoResetEvent = new AutoResetEvent(true);

            Thread t = new Thread(new ThreadStart(DownloadImage));
            t.Name = "Download Image";
            t.IsBackground = true;
            t.Start();
        }

        private static void DownloadImage()
        {
            while (true)
            {
                GC.Collect();
                ImageQueueItem item = null;
                lock (stacks)
                {
                    if (stacks.Count > 0)
                    {
                        item = stacks.Dequeue();
                    }
                }
                if (item != null)
                {
                    try
                    {
                        Uri uri = new Uri(item.Url);
                        BitmapImage image = null;
                        if ("http".Equals(uri.Scheme, StringComparison.CurrentCultureIgnoreCase) || "https".Equals(uri.Scheme, StringComparison.CurrentCultureIgnoreCase))
                        {
                            WebClient wc = new WebClient();
                            using (Stream ms = new MemoryStream(wc.DownloadData(uri)))
                            {
                                image = new BitmapImage();
                                image.BeginInit();
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.StreamSource = ms;
                                image.EndInit();
                            }
                        }
                        else if ("file".Equals(uri.Scheme, StringComparison.CurrentCultureIgnoreCase))
                        {
                            using (Stream fs = new FileStream(item.Url, FileMode.Open))
                            {
                                image = new BitmapImage();
                                image.BeginInit();
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.StreamSource = fs;
                                image.EndInit();
                            }
                        }

                        if (image != null)
                        {
                            if (image.CanFreeze)
                            {
                                image.Freeze();
                            }
                            item.Image.Dispatcher.BeginInvoke(new Action<ImageQueueItem, BitmapImage>((i, bmp) =>
                            {
                                DownloadCompleted?.Invoke(i.Image, i.Url, bmp);
                            }), new object[] { item, image });
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                if (stacks.Count > 0)
                {
                    continue;
                }
                autoResetEvent.WaitOne();
            }
        }

        public static void Push(Image image, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            lock (stacks)
            {
                stacks.Enqueue(new ImageQueueItem { Image = image, Url = url });
                autoResetEvent.Set();
            }
        }
    }
}
