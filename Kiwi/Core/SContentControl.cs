using Shannan.Core.Extensions;
using System;

namespace Kiwi.Core
{
    public class SContentControl : SBaseControl
    {
        public event EventHandler ShowUpCompleted;

        public event EventHandler Ready;

        public event EventHandler Failed;

        public SContentControl()
        {
            Loaded += delegate
            {
                Opacity = 0;
                this.BeginDoubleAnimation(OpacityProperty, 1, TimeSpan.FromSeconds(.1), delegate
                {
                    ShowUpCompleted?.Invoke(this, new EventArgs());
                });
            };
        }

        public bool NeedZoom { get; protected set; } = false;

        internal void TriggerReady()
        {
            Ready?.Invoke(this, new EventArgs());
        }

        internal void TriggerFailed()
        {
            Failed?.Invoke(this, new EventArgs());
        }
    }
}
