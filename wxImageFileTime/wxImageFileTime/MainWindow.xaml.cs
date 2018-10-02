using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using PengSW.WindowHelper;

namespace wxImageFileTime
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _Model = new wxImageFileTimeModel();
            this.DataContext = _Model;
        }

        private wxImageFileTimeModel _Model;

        private void OnSelectFiles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog aDlg = new OpenFileDialog
            {
                Multiselect = true,
            };
            if (aDlg.ShowDialog() != true) return;
            this.Exec(() => _Model.AddFiles(aDlg.FileNames));
        }

        private void OnSelectFiles_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnStart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Exec(_Model.Start);
        }

        private void OnStart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.Items.Count > 0;
        }

        private void OnSelectFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog aDlg = new System.Windows.Forms.FolderBrowserDialog();
            if (aDlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            this.Exec(() => _Model.AddFolder(aDlg.SelectedPath));
        }

        private void OnSelectFolder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnClearAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Exec(_Model.ClearAll);
        }

        private void OnClearAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model?.Items != null && _Model.Items.Count > 0;
        }
    }
}
