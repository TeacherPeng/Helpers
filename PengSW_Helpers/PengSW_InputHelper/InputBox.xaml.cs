using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PengSW.InputHelper
{
    /// <summary>
    /// InputBox.xaml 的交互逻辑
    /// </summary>
    public partial class InputBox : Window
    {
        public InputBox(string aCaption, string aPrompt, string aDefaultValue, string aOkPattern = null, string aNotOkPattern = null, IEnumerable<string> aValues = null)
        {
            Caption = aCaption;
            Prompt = aPrompt;
            Value = aDefaultValue;
            if (aOkPattern != null) _OkRegex = new Regex(aOkPattern);
            if (aNotOkPattern != null) _NotOkRegex = new Regex(aNotOkPattern);
            Values = aValues;
            InitializeComponent();
            if (Values == null || Values.Count() == 0)
            {
                cboValue.Visibility = Visibility.Hidden;
            }
            else
            {
                txtValue.Visibility = Visibility.Hidden;
            }
            this.DataContext = this;
        }

        public string Caption { get; private set; }
        public string Prompt { get; private set; }
        public string Value { get; set; }
        public IEnumerable<string> Values { get; set; }

        // TODO: 如何设置校验信息？
        public string ValidateInfo { get; set; }

        private Regex _OkRegex = null;
        private Regex _NotOkRegex = null;

        private void OnOk_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_OkRegex != null)
                e.CanExecute = _OkRegex.IsMatch(Value);
            else if (_NotOkRegex != null)
                e.CanExecute = !_NotOkRegex.IsMatch(Value);
            else
                e.CanExecute = true;
        }

        private void OnOk_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            if (txtValue.Visibility != Visibility.Hidden && !txtValue.IsFocused)
            {
                txtValue.Focus();
                txtValue.SelectAll();
            }
            else if (cboValue.Visibility != Visibility.Hidden && !cboValue.IsFocused)
            {
                cboValue.Focus();
            }
        }
    }
}
