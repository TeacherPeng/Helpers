using PengSW.RuntimeLog;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PengSW.WindowHelper
{
    public static class UIHelper
    {
        public static void Exec(this FrameworkElement aUIElement, Action aAction, bool aShowWaiting = true)
        {
            try
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Wait;
                aAction();
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
                RL.WriteExceptionLog(ex, true, true);
            }
        }

        public static T Func<T>(this FrameworkElement aUIElement, Func<T> aFunc, T aDefaultValue, bool aShowWaiting = true)
        {
            try
            {
                return aFunc();
            }
            catch (Exception ex)
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
                RL.WriteExceptionLog(ex, true, true);
                return aDefaultValue;
            }
        }

        public static T FindVisualParent<T>(this DependencyObject aObject) where T : class
        {
            while (aObject != null)
            {
                if (aObject is T)
                    return aObject as T;

                aObject = VisualTreeHelper.GetParent(aObject);
            }
            return null;
        }
    }
}
