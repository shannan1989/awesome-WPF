using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Windows;

namespace Shannan.Core.Framework
{
    public class PrintTask
    {
        public string FileName { get; set; }
        public string PrintTitle { get; set; }
        public string PrinterName { get; set; }
        public int CopyCount { get; set; }
        public Duplex Duplex { get; set; }
        public string PaperName { get; set; }
    }

    public static class PrintQueue
    {
        public delegate void PrintCompletedEventHandler(PrintTask task);

        public static event PrintCompletedEventHandler PrintCompleted;

        private static AutoResetEvent autoResetEvent;
        private static Queue<PrintTask> tasks;

        static PrintQueue()
        {
            tasks = new Queue<PrintTask>();
            autoResetEvent = new AutoResetEvent(true);

            Thread t = new Thread(new ThreadStart(Print));
            t.Name = "Print";
            t.IsBackground = true;
            t.Start();
        }

        private static void Print()
        {
            while (true)
            {
                GC.Collect();
                PrintTask item = null;
                lock (tasks)
                {
                    if (tasks.Count > 0)
                    {
                        item = tasks.Dequeue();
                    }
                }
                if (item != null)
                {
                    try
                    {
                        using (PdfDocument document = PdfDocument.Load(item.FileName))
                        {
                            using (PrintDocument printDocument = document.CreatePrintDocument())
                            {
                                // Create the printer settings for our printer
                                PrinterSettings printerSettings = new PrinterSettings
                                {
                                    Copies = (short)item.CopyCount,
                                    Duplex = item.Duplex,
                                    PrinterName = item.PrinterName
                                };

                                // Create our page settings for the paper size selected
                                PageSettings pageSettings = new PageSettings()
                                {
                                    Margins = new Margins(0, 0, 0, 0),
                                    PrinterSettings = printerSettings
                                };

                                foreach (PaperSize paperSize in printerSettings.PaperSizes)
                                {
                                    if (paperSize.PaperName == item.PaperName)
                                    {
                                        pageSettings.PaperSize = paperSize;
                                        break;
                                    }
                                }

                                printDocument.PrinterSettings = printerSettings;
                                printDocument.DefaultPageSettings = pageSettings;
                                printDocument.PrintController = new StandardPrintController();
                                //printDocument.BeginPrint += PrintDocument_BeginPrint;
                                //printDocument.EndPrint += PrintDocument_EndPrint;
                                printDocument.DocumentName = item.PrintTitle;
                                printDocument.Print();

                                Application.Current.Dispatcher.BeginInvoke(new Action<PrintTask>((task) =>
                                {
                                    PrintCompleted?.Invoke(task);
                                }), new object[] { item });
                            }
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                if (tasks.Count > 0)
                {
                    continue;
                }
                autoResetEvent.WaitOne();
            }
        }

        public static void Push(string filename, string printerName, int copyCount, Duplex duplex, string paperName, string printTitle = "文件打印")
        {
            if (!File.Exists(filename))
            {
                return;
            }
            lock (tasks)
            {
                tasks.Enqueue(new PrintTask { FileName = filename, PrinterName = printerName, CopyCount = copyCount, Duplex = duplex, PaperName = paperName, PrintTitle = printTitle });
                autoResetEvent.Set();
            }
        }
    }
}
