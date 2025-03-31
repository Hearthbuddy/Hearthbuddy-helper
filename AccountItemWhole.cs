using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HearthHelper
{
	public class AccountItemWhole : INotifyPropertyChanged
	{
		private bool selected;
		private string email;
		private string emailShow;
		private bool running;
		private int stonePid;
		public AccountItemSingle currItem;
		public ObservableCollection<AccountItemSingle> itemList =
			new ObservableCollection<AccountItemSingle>();

		public AccountItemWhole(bool isSelected, string whichEmail)
		{
			selected = isSelected;
			email = whichEmail;
			emailShow = UtilsCom.ReplaceWithSpecialChar(whichEmail);
			running = false;
			stonePid = 0;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public bool Selected
		{
			get { return selected; }
			set { selected = value; OnPropertyChanged("Selected"); }
		}
		public string Email
		{
			get { return email; }
			set
			{
				email = value; OnPropertyChanged("Email");
				EmailShow = UtilsCom.ReplaceWithSpecialChar(value);
			}
		}
		public string EmailShow
		{
			get { return emailShow; }
			set { emailShow = value; OnPropertyChanged("EmailShow"); }
		}
		public bool Running
		{
			get{return running;}
			set{running = value;OnPropertyChanged("Running"); }
		}
		public int StonePid
		{
			get { return stonePid; }
			set { stonePid = value; OnPropertyChanged("StonePid"); }
		}
	}
}

