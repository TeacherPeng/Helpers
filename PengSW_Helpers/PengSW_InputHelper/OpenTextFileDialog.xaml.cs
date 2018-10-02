using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace PengSW.InputHelper
{
    /// <summary>
    /// OpenTextFileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OpenTextFileDialog : Window, INotifyPropertyChanged
    {
        public OpenTextFileDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private static readonly Brush _NormalBrush = new SolidColorBrush(Colors.Black);
        private static readonly Brush _ExceptionBrush = new SolidColorBrush(Colors.Red);

        public Brush PreviewBrush { get { return _PreviewBrush; } set { if (_PreviewBrush == value) return; _PreviewBrush = value; OnPropertyChanged(nameof(PreviewBrush)); } }
        private Brush _PreviewBrush = _NormalBrush;

        public string PreviewText { get { return _PreviewText; } set { if (_PreviewText == value) return; _PreviewText = value; OnPropertyChanged(nameof(PreviewText)); } }
        private string _PreviewText;

        public string FileName { get { return _FileName; } set { if (_FileName == value) return; _FileName = value; OnPropertyChanged(nameof(FileName)); Preview(); } }
        private string _FileName;

        public Encoding CurrentEncoding { get { return _CurrentEncoding; } set { if (_CurrentEncoding == value) return; _CurrentEncoding = value; OnPropertyChanged(nameof(CurrentEncoding)); Preview(); } }
        private static Encoding _CurrentEncoding = Encoding.Default;

        public static Encoding[] Encodings { get; } = new Encoding[] { Encoding.Default, Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32, Encoding.UTF7 };

        private void OnPropertyChanged([CallerMemberName]string aPropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog aDlg = new OpenFileDialog()
            {
                Filter = "文本文件|*.txt;*.xml;*.json|所有文件 (*.*)|*.*"
            };
            if (aDlg.ShowDialog() != true) return;
            FileName = aDlg.FileName;
        }

        private void Preview()
        {
            if (string.IsNullOrWhiteSpace(FileName))
            {
                PreviewText = null;
                PreviewBrush = _NormalBrush;
                return;
            }
            try
            {
                byte[] aBytes = new byte[2048];
                int aLen;
                using (FileStream aFileStream = File.OpenRead(FileName))
                {
                    aLen = aFileStream.Read(aBytes, 0, aBytes.Length);
                }
                PreviewText = CurrentEncoding.GetString(aBytes, 0, aLen);
                PreviewBrush = _NormalBrush;
            }
            catch (Exception ex)
            {
                PreviewText = ex.Message;
                PreviewBrush = _ExceptionBrush;
            }
        }

        private void OnPreview_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Preview();
        }

        private void OnPreview_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrWhiteSpace(FileName);
        }

        private void OnOk_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void OnOk_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrWhiteSpace(FileName);
        }
    }
}
