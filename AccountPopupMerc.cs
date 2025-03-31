using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace HearthHelper
{
	public partial class AccountPopupMerc : Window
	{
		public AccountItemSingle AccountItem;

		public AccountPopupMerc(ref AccountItemSingle item)
		{
			AccountItem = item;
            DataContext = AccountItem;
			InitializeComponent();
		}

		private void ConfigItemButtonSave_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
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
