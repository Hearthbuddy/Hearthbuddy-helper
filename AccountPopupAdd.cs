using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace HearthHelper
{
	public partial class AccountPopupAdd : Window
	{
		private string account="";

		public AccountPopupAdd()
		{
			InitializeComponent();
		}

		public string GetAccount()
		{
			return account;
		}

		private void ConfigItemButtonSave_Click(object sender, RoutedEventArgs e)
		{
			account=ConfigAccountName.Text;
			this.DialogResult = true;
		}

		//数字输入
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = false;
			e.Handled = true;
		}
	}
}
