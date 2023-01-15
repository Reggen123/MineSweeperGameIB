using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SaperLab2WPF
{
    public class CtrlKeyDownEventTrigger: EventTrigger
    {
        public CtrlKeyDownEventTrigger() : base("PreviewKeyDown") {
        }
        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
              if(e != null && (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl))
            {
                this.InvokeActions(eventArgs);
            }
        }
    }

    public class CtrlKeyUpEventTrigger : EventTrigger
    {
        public CtrlKeyUpEventTrigger() : base("PreviewKeyUp")
        {
        }
        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
            if (e != null && (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl))
            {
                this.InvokeActions(eventArgs);
            }
        }
    }
}
