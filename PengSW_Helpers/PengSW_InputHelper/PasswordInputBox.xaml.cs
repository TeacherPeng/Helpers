using System.Windows;

namespace PengSW.InputHelper
{
    /// <summary>
    /// PasswordInputBox.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordInputBox : Window
    {
        public PasswordInputBox(string aTitle = "输入密码", string aPrompt = "请输入密码：")
        {
            InitializeComponent();
            Title = aTitle;
            Prompt = aPrompt;
            this.DataContext = this;
        }

        public string Prompt { get; }
        public string Password => txtPassword.Password;

        private void OnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
