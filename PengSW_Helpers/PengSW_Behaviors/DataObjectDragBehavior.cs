using System.Windows;
using System.Windows.Interactivity;

namespace PengSW.Behaviors
{
    /// <summary>
    /// 提供对DataContext绑定了数据对象的界面元素的数据对象的Drag操作。
    /// </summary>
    public class DataObjectDragBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            _IsDragging = false;
        }

        private bool _IsDragging = false;

        private void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as FrameworkElement).ReleaseMouseCapture();
            if (!_IsDragging) return;
            _IsDragging = false;
            if (AssociatedObject.DataContext == null) return;
            DragDrop.DoDragDrop(AssociatedObject, AssociatedObject.DataContext, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as FrameworkElement).ReleaseMouseCapture();
            _IsDragging = false;
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (AssociatedObject.DataContext == null) return;
            (sender as FrameworkElement).CaptureMouse();
            _IsDragging = true;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            _IsDragging = false;
        }
    }
}
