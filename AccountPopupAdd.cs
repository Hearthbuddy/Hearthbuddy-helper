using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace HearthHelper
{
    public partial class AccountPopupAdd : Window
    {
        private string account = "";

        private string token = "";

        public AccountPopupAdd()
        {
            InitializeComponent();
        }

        public string GetAccount()
        {
            return account;
        }

        public string GetToken()
        {
            return token;
        }

        private void ConfigItemButtonSave_Click(object sender, RoutedEventArgs e)
        {
            account = ConfigAccountName.Text;
            token = ConfigToken.Text;
            this.DialogResult = true;
        }
    }
}