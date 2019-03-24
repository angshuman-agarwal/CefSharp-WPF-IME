using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CefSharp.Wpf;
using CefSharp;
using System.Windows.Threading;
using CefSharp.Structs;

namespace Cef_Ime.IME
{
    public class ChromiumWebBrowserIMESupport : ChromiumWebBrowser
    {
        public ChromiumWebBrowserIMESupport()
        {
            WpfKeyboardHandler = new IMEWpfKeyboardHandler(this);
        }

        public void SetUp()
        {
            (WpfKeyboardHandler as IMEWpfKeyboardHandler).InitialiseHWND();
        }

        static ChromiumWebBrowserIMESupport()
        {
            InputMethod.IsInputMethodEnabledProperty.OverrideMetadata(
                typeof(ChromiumWebBrowserIMESupport),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.Inherits,
                    (obj, e) =>
                    {
                        var browser = obj as ChromiumWebBrowserIMESupport;
                        if ((bool)e.NewValue && browser.GetBrowserHost() != null && Keyboard.FocusedElement == browser)
                        {
                            browser.GetBrowserHost().SendFocusEvent(true);
                            InputMethod.SetIsInputMethodSuspended(browser, true);
                        }
                    }));

            InputMethod.IsInputMethodSuspendedProperty.OverrideMetadata(
                typeof(ChromiumWebBrowserIMESupport),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.Inherits));
        }

        protected override void OnImeCompositionRangeChanged(Range selectedRange, CefSharp.Structs.Rect[] characterBounds)
        {
            var imeKeyboardHandler = WpfKeyboardHandler as IMEWpfKeyboardHandler;
            if (imeKeyboardHandler.IsActive)
            {
                var screenInfo = GetScreenInfo();
                var scaleFactor = screenInfo.Value.DeviceScaleFactor > 0 ? screenInfo.Value.DeviceScaleFactor : 1.0f;

                UiThreadRunAsync(() =>
                {
                    var parentWindow = GetParentWindow();
                    if (parentWindow != null)
                    {
                        var point = TransformToAncestor(parentWindow).Transform(new System.Windows.Point(0, 0));
                        var rects = new List<CefSharp.Structs.Rect>();

                        foreach (var item in characterBounds)
                            rects.Add(new CefSharp.Structs.Rect(
                                (int)((point.X + item.X) * scaleFactor),
                                (int)((point.Y + item.Y) * scaleFactor),
                                (int)(item.Width * scaleFactor),
                                (int)(item.Height * scaleFactor)));

                        imeKeyboardHandler.ChangeCompositionRange(selectedRange, rects);
                    }
                });
            }

            Visual GetParentWindow()
            {
                var current = VisualTreeHelper.GetParent(this);
                while (current != null && !(current is Window))
                    current = VisualTreeHelper.GetParent(current);

                return current as Window;
            }
        }

        private void UiThreadRunAsync(Action action, DispatcherPriority priority = DispatcherPriority.DataBind)
        {
            if (Dispatcher.CheckAccess())
            {
                action();
            }
            else if (!Dispatcher.HasShutdownStarted)
            {
                Dispatcher.BeginInvoke(action, priority);
            }
        }
    }
}
