using Shannan.Core.Debugging;
using Shannan.Core.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Kiwi.Core
{
    public class SBaseControl : UserControl
    {
        public SBaseControl()
        {
        }

        internal void EnableKeyboard(UIElement form)
        {
            form.KeyUp -= Form_KeyUp;
            form.KeyUp += Form_KeyUp;
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            if (!(Keyboard.FocusedElement is UIElement focusElement))
            {
                return;
            }
            if (focusElement.GetType() == typeof(TextBox))
            {
                if (string.IsNullOrEmpty((focusElement as TextBox).Text.Trim()))
                {
                    return;
                }
            }
            else if (focusElement.GetType() == typeof(ComboBox))
            {
                if ((focusElement as ComboBox).SelectedIndex < 0)
                {
                    (focusElement as ComboBox).IsDropDownOpen = true;
                    return;
                }
            }
            else
            {
                // 其他表单控件
            }

            focusElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            e.Handled = true;

            if (Keyboard.FocusedElement is UIElement newFocusElement)
            {
                if (newFocusElement.GetType() == typeof(Button))
                {
                    (newFocusElement as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
                else
                {
                    // 其他控件
                }
            }
        }

        internal void StartRequest(string uri, NameValueCollection parameters, string token)
        {
            StartRequest(uri, parameters, token, null);
        }

        internal void StartRequest(string uri, NameValueCollection parameters, string token, Dictionary<string, string> files)
        {
            DataRequest request = new DataRequest(uri, parameters)
            {
                CallerName = GetType().FullName,
                Token = token
            };

            if (files != null)
            {
                foreach (KeyValuePair<string, string> item in files)
                {
                    request.AddUploadFile(item.Key, item.Value);
                }
            }

            request.Completed += Request_Completed;
            request.Start();
        }

        private void Request_Completed(DataRequestEventArgs e)
        {
            string methodName = "On" + e.Request.Token;
            MethodInfo mi = GetType().GetMethod(methodName, new Type[] { typeof(DataRequestEventArgs) });
            if (mi != null)
            {
                mi.Invoke(this, new object[] { e });
            }
            else
            {
                Log("缺少方法 " + methodName);
            }
        }

        internal void StartDownloadTask(string filename, string uri, NameValueCollection parameters, string token)
        {
            DownloadTask downloadTask = new DownloadTask(filename, uri, parameters)
            {
                CallerName = GetType().FullName,
                Token = token
            };
            downloadTask.DownloadUpdated += DownloadTask_Updated;
            downloadTask.Start();
        }

        private void DownloadTask_Updated(DownloadTaskEventArgs e)
        {
            if (e.Status == DownloadStatus.Finished)
            {
                // 下载完成
            }
            else if (e.Status == DownloadStatus.Aborted)
            {
                // 下载中断
            }
            else if (e.Status == DownloadStatus.Running)
            {
                Log("DownloadTask " + e.Status + " " + (100.0 * e.Task.NumberOfDownload / e.Task.NumberOfTotal).ToString("N1") + "%");
            }
            else if (e.Status == DownloadStatus.Pending)
            {
                // 下载Pending
            }

            string methodName = "On" + e.Task.Token;
            MethodInfo mi = GetType().GetMethod(methodName, new Type[] { typeof(DownloadTaskEventArgs) });
            if (mi != null)
            {
                mi.Invoke(this, new object[] { e });
            }
            else
            {
                Log("缺少方法 " + methodName);
            }
        }

        internal void Log(string value)
        {
            ConsoleManager.Log(GetType().FullName + ": " + value);
        }
    }
}
