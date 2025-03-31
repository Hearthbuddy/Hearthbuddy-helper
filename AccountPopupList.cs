using System.Linq;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace HearthHelper
{
	public partial class AccountPopupList : Window
	{
		public AccountItemWhole AccountItemWhole;

		public AccountPopupList(ref AccountItemWhole whole)
		{
			AccountItemWhole = whole;
            DataContext = AccountItemWhole;
			InitializeComponent();
			ItemListBox.ItemsSource = AccountItemWhole.itemList =
				new ObservableCollection<AccountItemSingle>(
					AccountItemWhole.itemList.OrderBy(item => item.StartTimeHour));
		}

		//天梯添加
		private void ConfigItemButtonAdd_Click0(object sender, RoutedEventArgs e)
		{
			AccountItemSingle single = new AccountItemSingle((int)GameMode.ModeNormal);
			AccountPopupNormal accountConfigPopup = new AccountPopupNormal(ref single);
			accountConfigPopup.Left = Left + (Width - accountConfigPopup.Width) / 2.0;
			accountConfigPopup.Top = Top + (Height - accountConfigPopup.Height) / 2.0;
			if ((bool)accountConfigPopup.ShowDialog())
			{
				AccountItemWhole.itemList.Add(single);
				ItemListBox.ItemsSource = AccountItemWhole.itemList =
					new ObservableCollection<AccountItemSingle>(
						AccountItemWhole.itemList.OrderBy(item => item.StartTimeHour));
			}
		}

		//佣兵添加
		private void ConfigItemButtonAdd_Click1(object sender, RoutedEventArgs e)
		{
			AccountItemSingle single = new AccountItemSingle((int)GameMode.ModeMerc);
			AccountPopupMerc accountConfigPopup = new AccountPopupMerc(ref single);
			accountConfigPopup.Left = Left + (Width - accountConfigPopup.Width) / 2.0;
			accountConfigPopup.Top = Top + (Height - accountConfigPopup.Height) / 2.0;
			if ((bool)accountConfigPopup.ShowDialog())
			{
				AccountItemWhole.itemList.Add(single);
				ItemListBox.ItemsSource = AccountItemWhole.itemList =
					new ObservableCollection<AccountItemSingle>(
						AccountItemWhole.itemList.OrderBy(item => item.StartTimeHour));
			}	
		}

		//战旗添加
		private void ConfigItemButtonAdd_Click2(object sender, RoutedEventArgs e)
		{
			AccountItemSingle single = new AccountItemSingle((int)GameMode.ModeBattle);
			AccountPopupBacon accountPopupBacon = new AccountPopupBacon(ref single);
			accountPopupBacon.Left = Left + (Width - accountPopupBacon.Width) / 2.0;
			accountPopupBacon.Top = Top + (Height - accountPopupBacon.Height) / 2.0;
			if ((bool)accountPopupBacon.ShowDialog())
			{
				AccountItemWhole.itemList.Add(single);
				ItemListBox.ItemsSource = AccountItemWhole.itemList =
					new ObservableCollection<AccountItemSingle>(
						AccountItemWhole.itemList.OrderBy(item => item.StartTimeHour));
			}
		}

		//修改
		private void ConfigItemButtonModify_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			if (button != null)
			{
				bool save = false;
				AccountItemSingle single = button.Tag as AccountItemSingle;
				AccountItemSingle singleOld = single.Clone();
				if (single.Mode == (int)GameMode.ModeNormal)
				{
					AccountPopupNormal accountConfigPopup = new AccountPopupNormal(ref single);
					accountConfigPopup.Left = Left + (Width - accountConfigPopup.Width) / 2.0;
					accountConfigPopup.Top = Top + (Height - accountConfigPopup.Height) / 2.0;
					save = (bool)accountConfigPopup.ShowDialog();
				}
				else if (single.Mode == (int)GameMode.ModeMerc)
				{
					AccountPopupMerc accountConfigPopup = new AccountPopupMerc(ref single);
					accountConfigPopup.Left = Left + (Width - accountConfigPopup.Width) / 2.0;
					accountConfigPopup.Top = Top + (Height - accountConfigPopup.Height) / 2.0;
					save = (bool)accountConfigPopup.ShowDialog();
				}
				else if (single.Mode == (int)GameMode.ModeBattle)
				{
					AccountPopupBacon accountConfigPopup = new AccountPopupBacon(ref single);
					accountConfigPopup.Left = Left + (Width - accountConfigPopup.Width) / 2.0;
					accountConfigPopup.Top = Top + (Height - accountConfigPopup.Height) / 2.0;
					save = (bool)accountConfigPopup.ShowDialog();
				}
				AccountItemWhole.itemList.Remove(single);
				AccountItemWhole.itemList.Add(save ? single : singleOld);
				ItemListBox.ItemsSource = AccountItemWhole.itemList = new ObservableCollection<AccountItemSingle>(
					AccountItemWhole.itemList.OrderBy(item => item.StartTimeHour));
			}
		}

		//删除
		private void ConfigItemButtonDelete_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			if (button != null)
			{
				if (System.Windows.MessageBox.Show("确定删除此模式？", "警告",
					MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
				{
					AccountItemSingle single = button.Tag as AccountItemSingle;
					AccountItemWhole.itemList.Remove(single);
					ItemListBox.ItemsSource = AccountItemWhole.itemList = new ObservableCollection<AccountItemSingle>(
						AccountItemWhole.itemList.OrderBy(item => item.StartTimeHour));
				}
			}
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
