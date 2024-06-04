using Kiwi.Scenes.Home;
using System.Collections.Generic;

namespace Kiwi.Core
{
    internal class SContentControlStack
    {
        private static SContentControlStack _instance;

        private readonly Stack<SContentControl> _stack;
        private readonly Home _window;

        private SContentControlStack(Home window)
        {
            _stack = new Stack<SContentControl>();
            _window = window;
        }

        public static void CreateInstance(Home window)
        {
            _instance = new SContentControlStack(window);
        }

        public static void DestroyInstance()
        {
            _instance = null;
        }

        public static SContentControlStack Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Clear()
        {
            _stack.Clear();
        }

        private void Push(SContentControl c)
        {
            _stack.Push(c);
        }

        private void Pop()
        {
            if (_stack.Count <= 0)
            {
                return;
            }
            _stack.Pop();
        }

        public SContentControl Top
        {
            get
            {
                return _stack.Peek();
            }
        }

        public int Count
        {
            get
            {
                return _stack.Count;
            }
        }

        private void Show(string action = "Replace")
        {
            if (_stack.Count <= 0)
            {
                return;
            }
            SContentControl c = _stack.Peek();
            if (c != null)
            {
                _window.LoadRightContent(c, action);
            }
        }

        public void Reset()
        {
            Clear();
            _window.ClearRightContent();
        }

        /// <summary>
        /// 关闭所有页面，打开到某个页面
        /// </summary>
        public void ReLaunch(SContentControl c)
        {
            Clear();
            Push(c);
            Show();
        }

        /// <summary>
        /// 关闭当前页面，返回到上一页面
        /// </summary>
        public void NavigateBack()
        {
            Pop();
            if (Count > 0)
            {
                Show("Backword");
            }
            else
            {
                Reset();
            }
        }

        /// <summary>
        /// 保留当前页面，跳转到某个页面
        /// </summary>
        public void NavigateTo(SContentControl c)
        {
            Push(c);
            if (Count > 1)
            {
                Show("Forword");
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// 关闭当前页面，跳转到某个页面
        /// </summary>
        public void RedirectTo(SContentControl c)
        {
            Pop();
            Push(c);
            Show();
        }
    }
}
