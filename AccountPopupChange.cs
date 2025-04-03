using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HearthHelper
{
    public partial class AccountPopupChange : Window
    {
        private ObservableCollection<AccountItemWhole> AccountList;
        private string account = "";

        private string token = "";

        public AccountPopupChange(ObservableCollection<AccountItemWhole> accountList)
        {
            InitializeComponent();
            ConfigAccountName.Items.Clear();
            AccountList = accountList;
            foreach (var accountItemWhole in accountList)
            {
                ConfigAccountName.Items.Add(accountItemWhole.Email);
            }
        }

        private void ConfigAccountNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = ConfigAccountName.SelectedValue.ToString();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var accountItemWhole = AccountList.ToList().Find(x => x.Email == text);
            if (accountItemWhole == null)
            {
                return;
            }

            ConfigToken.Text = accountItemWhole.Token;
        }

        public string GetAccount()
        {
            return account;
        }

        public string GetToken()
        {
            return token;
        }

        private void ConfigItemButtonChange_Click(object sender, RoutedEventArgs e)
        {
            account = ConfigAccountName.Text;
            token = ConfigToken.Text;
            this.DialogResult = true;
        }
    }
}