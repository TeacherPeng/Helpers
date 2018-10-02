using static PengSW.RuntimeLog.RL;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Configuration;
using System.IO;

namespace PengSW.WindowHelper
{
    public static class UIHelper
    {
        public static void Exec(this FrameworkElement aUIElement, Action aAction, bool aShowWaiting = true)
        {
            if (aAction == null) return;
            try
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Wait;
                aAction();
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
            }
            catch (ApplicationException ex)
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
                L(ex.Message, 0, true, true);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
                E(ex, null, false, true);
                MessageBox.Show($"请检查目标服务器是否正常启动！\n{ex.Message}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
                E(ex, null, true, true);
            }
        }

        public static T Func<T>(this FrameworkElement aUIElement, Func<T> aFunc, T aDefaultValue, bool aShowWaiting = true)
        {
            if (aFunc == null) return aDefaultValue;
            try
            {
                return aFunc();
            }
            catch (ApplicationException ex)
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
                L(ex.Message, 0, true, true);
                return aDefaultValue;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
                E(ex, null, false, true);
                MessageBox.Show($"请检查目标服务器是否正常启动！\n{ex.Message}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return aDefaultValue;
            }
            catch (Exception ex)
            {
                if (aShowWaiting) aUIElement.Cursor = Cursors.Arrow;
                E(ex, null, true, true);
                return aDefaultValue;
            }
        }

        public static T FindVisualParent<T>(this DependencyObject aObject, DependencyObject aRoot = null) where T : class
        {
            while (aObject != aRoot)
            {
                if (aObject is T)
                    return aObject as T;

                aObject = VisualTreeHelper.GetParent(aObject);
            }
            return null;
        }

        public static T FindVisualChild<T>(this DependencyObject aObject) where T : DependencyObject
        {
            Queue<DependencyObject> aQueue = new Queue<DependencyObject>();
            aQueue.Enqueue(aObject);
            while (aQueue.Count > 0)
            {
                aObject = aQueue.Dequeue();
                if (aObject is T) return aObject as T;
                int aCount = VisualTreeHelper.GetChildrenCount(aObject);
                for (int i = 0; i < aCount; i++)
                    aQueue.Enqueue(VisualTreeHelper.GetChild(aObject, i));
            }
            return null;
        }

        public static T FindVisualChild<T>(this DependencyObject aObject, Func<T, bool> aCondition) where T : DependencyObject
        {
            Queue<DependencyObject> aQueue = new Queue<DependencyObject>();
            aQueue.Enqueue(aObject);
            while (aQueue.Count > 0)
            {
                aObject = aQueue.Dequeue();
                if (aObject is T && aCondition(aObject as T)) return aObject as T;
                int aCount = VisualTreeHelper.GetChildrenCount(aObject);
                for (int i = 0; i < aCount; i++)
                    aQueue.Enqueue(VisualTreeHelper.GetChild(aObject, i));
            }
            return null;
        }

        public static List<T> FindVisualChildren<T>(this DependencyObject aObject) where T : DependencyObject
        {
            List<T> aChildren = new List<T>();
            Queue<DependencyObject> aQueue = new Queue<DependencyObject>();
            aQueue.Enqueue(aObject);
            while (aQueue.Count > 0)
            {
                aObject = aQueue.Dequeue();
                if (aObject is T) aChildren.Add(aObject as T);
                int aCount = VisualTreeHelper.GetChildrenCount(aObject);
                for (int i = 0; i < aCount; i++)
                    aQueue.Enqueue(VisualTreeHelper.GetChild(aObject, i));
            }
            return aChildren;
        }

        public static void CreateDataGridRowNumbers(this DataGrid aDataGrid)
        {
            foreach (var aItem in aDataGrid.Items)
            {
                DataGridRow aRow = (DataGridRow)aDataGrid.ItemContainerGenerator.ContainerFromItem(aItem);
                if (aRow != null)
                {
                    aRow.Header = aRow.GetIndex() + 1;
                }
            }
        }

        public static void ResetDataGridCellFocus(this DataGrid aDataGrid, object aItem, int aColumnIndex)
        {
            DataGridRow aDataGridRow = (DataGridRow)aDataGrid.ItemContainerGenerator.ContainerFromItem(aItem);
            if (aDataGridRow == null) return;
            DataGridCellsPresenter aCellPresenter = aDataGridRow.FindVisualChild<DataGridCellsPresenter>();
            if (aCellPresenter == null) return;
            DataGridCell aCell = (DataGridCell)aCellPresenter.ItemContainerGenerator.ContainerFromIndex(aColumnIndex);
            if (aCell != null) aCell.Focus();
        }

        public static void ResetDataGridFocus(this DataGrid aDataGrid, int aColumnIndex)
        {
            aDataGrid.Focus();
            if (aDataGrid.SelectedItem == null) return;
            aDataGrid.ScrollIntoView(aDataGrid.SelectedItem);
            aDataGrid.ResetDataGridCellFocus(aDataGrid.SelectedItem, aColumnIndex);
        }

        public static void CheckUserConfig(bool aShowDialog = true)
        {
            try
            {
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }
            catch (ConfigurationErrorsException ex)
            {
                if (aShowDialog)
                    MessageBox.Show("配置信息已损坏，将重置配置信息，如仍存在问题，请与管理员联系！", "重置提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Directory.Delete(Path.GetDirectoryName(ex.Filename), true);
            }
        }

        private static readonly DispatcherOperationCallback _ExitFrameCallback = new DispatcherOperationCallback(ExitFrame);
        public static void DoEvents()
        {
            DispatcherFrame aFrame = new DispatcherFrame();
            DispatcherOperation aExitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, _ExitFrameCallback, aFrame);
            Dispatcher.PushFrame(aFrame);
            if (aExitOperation.Status != DispatcherOperationStatus.Completed)
            {
                aExitOperation.Abort();
            }
        }
        private static object ExitFrame(object aState)
        {
            DispatcherFrame aFrame = aState as DispatcherFrame;
            aFrame.Continue = false;
            return null;
        }
    }
}
