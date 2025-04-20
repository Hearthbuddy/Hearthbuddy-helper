using System.ComponentModel;

namespace HearthHelper
{
	public enum GameMode
	{
		ModeNormal = 0,
		ModeMerc,
		ModeBattle,
	}

	public class StringConst
	{
		public static string[] rule0 =
		{
			"狂野模式",
			"标准模式",
			"经典模式",
			"休闲模式",
			"幻变模式",
		};

		public static string[] behavior0 =
		{
            "丨通用丨不设惩罚",
            "丨通用丨暗牧",
            "丨通用丨酸鱼人萨",
            "丨标准丨酸快攻德",
            "丨标准丨元素法",
            "丨标准丨元素萨",
            "丨狂野丨号角骑",
            "丨狂野丨奥秘法",
            "丨狂野丨剑鱼贼",
            "丨狂野丨偶数萨",
            "丨狂野丨暗龙牧",
            "丨狂野丨快攻暗牧",
            "丨狂野丨锁喉剑鱼贼",
            "丨过时丨任务海盗战",
        };

        public static string[] rule1 =
		{
			"刷全图",
			"刷碎片",
			"自动解锁地图",
			"自动佣兵任务",
			"自动主线任务",
			"自动解锁装备",
			"真人PVP",
			"挂机收菜",
		};

		public static string[] behavior1 =
		{
			"PVE通用",
			"PVP通用",
		};
	}

	public class AccountItemSingle : INotifyPropertyChanged
	{
		//公共
		private int mode;
		private bool enable;
		private int startTimeHour;
		private int startTimeMin;
		private int endTimeHour;
		private int endTimeMin;
		private string info;

		//天梯
		private int normalRule;
		private int normalBehavior;
		private string normalDeck;

		//佣兵
		private int mercRule;
		private int mercBehavior;
		private string mercTeam;
		private string mercMap;
		private int mercInterval;
		private bool mercConcede;
		private bool mercCraft;
		private bool mercUpdate;

		public AccountItemSingle(int myMode)
		{
			//天梯
			normalRule = 0;     //狂野
			normalBehavior = 1; //防战
			normalDeck = "请配置账号";

			//佣兵
			mercRule = 0;
			mercBehavior = 0;
			mercMap = "1-1";
			mercTeam = "请配置队伍";
			mercInterval = 22;
			mercConcede = false;
			mercCraft = true;
			mercUpdate = false;

			//公共
			mode = myMode;
			enable = true;
			startTimeHour = 0;
			startTimeMin = 0;
			endTimeHour = 23;
			endTimeMin = 59;
			mergInfo();
		}

		public AccountItemSingle(
			int myMode, bool myEnable,
			int myStartTimeHour, int myStartTimeMin, int myEndTimeHour, int myEndTimeMin,string myInfo,
			int myNormalRule, int myNormalBehavior, string myNormalDeck,
			int myMercRule, int myMercBehavior, string myMercTeam, string myMercMap, int myMercInterval,
			bool myMercConcede, bool myMercCraft, bool myMercUpdate)
		{
			mode = myMode;
			enable = myEnable;
			startTimeHour = myStartTimeHour;
			startTimeMin = myStartTimeMin;
			endTimeHour = myEndTimeHour;
			endTimeMin = myEndTimeMin;
			info = myInfo;
			normalRule = myNormalRule;
			normalBehavior = myNormalBehavior;
			normalDeck = myNormalDeck;
			mercRule = myMercRule;
			mercBehavior = myMercBehavior;
			mercTeam = myMercTeam;
			mercMap = myMercMap;
			mercInterval = myMercInterval;
			mercConcede = myMercConcede;
			mercCraft = myMercCraft;
			mercUpdate = myMercUpdate;
		}

		public AccountItemSingle Clone()
		{
			AccountItemSingle MySelf = new AccountItemSingle(mode, enable,
				startTimeHour, startTimeMin, endTimeHour, endTimeMin, info,
				normalRule, normalBehavior, normalDeck,
				mercRule, mercBehavior, mercTeam, mercMap, mercInterval,
				mercConcede, mercCraft, mercUpdate);
			return MySelf;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public int Mode
		{
			get { return mode; }
			set { mode = value; OnPropertyChanged("Mode"); }
		}
		public bool Enable
		{
			get { return enable; }
			set { enable = value; mergInfo(); OnPropertyChanged("Enable"); }
		}
		public int StartTimeHour
		{
			get { return startTimeHour; }
			set
			{
				startTimeHour = (value >= 0 && value <= 23) ? value : 0;
				mergInfo();
				OnPropertyChanged("StartTimeHour");
			}
		}
		public int StartTimeMin
		{
			get { return startTimeMin; }
			set
			{
				startTimeMin = (value >= 0 && value <= 59) ? value : 0;
				mergInfo();
				OnPropertyChanged("StartTimeMin");
			}
		}
		public int EndTimeHour
		{
			get { return endTimeHour; }
			set
			{
				endTimeHour = (value >= 0 && value <= 23) ? value : 23;
				mergInfo();
				OnPropertyChanged("EndTimeHour");
			}
		}
		public int EndTimeMin
		{
			get { return endTimeMin; }
			set
			{
				endTimeMin = (value >= 0 && value <= 59) ? value : 59;
				mergInfo();
				OnPropertyChanged("EndTimeMin");
			}
		}
		public string Info
		{
			get { return info; }
			set { info = value; OnPropertyChanged("Info"); }
		}
		public void mergInfo()
		{
			if (mode == (int)GameMode.ModeNormal)
			{
				info = string.Format("[天梯]--[{0}:{1}-{2}:{3}]--[{4}]--[{5}]--[{6}]",
					startTimeHour.ToString("D2"), startTimeMin.ToString("D2"),
					endTimeHour.ToString("D2"), endTimeMin.ToString("D2"),
					StringConst.rule0[normalRule], StringConst.behavior0[normalBehavior], normalDeck);
			}
			else if (mode == (int)GameMode.ModeMerc)
			{
				info = string.Format("[佣兵]--[{0}:{1}-{2}:{3}]--[{4}]--[{5}]--[{6}]--[{7}]",
					startTimeHour.ToString("D2"), startTimeMin.ToString("D2"),
					endTimeHour.ToString("D2"), endTimeMin.ToString("D2"),
					StringConst.rule1[mercRule], StringConst.behavior1[mercBehavior], mercTeam, mercMap);
			}
			else if (mode == (int)GameMode.ModeBattle)
			{
				info = string.Format("[酒馆]--[{0}:{1}-{2}:{3}]",
					startTimeHour.ToString("D2"), startTimeMin.ToString("D2"),
					endTimeHour.ToString("D2"), endTimeMin.ToString("D2"));
			}
		}

		public int NormalRule
		{
			get { return normalRule; }
			set { normalRule = value; mergInfo(); OnPropertyChanged("NormalRule"); }
		}
		public int NormalBehavior
		{
			get{return normalBehavior; }
			set{normalBehavior = value;
				mergInfo(); OnPropertyChanged("NormalBehaviour");}
		}
		public string NormalDeck
		{
			get { return normalDeck; }
			set { normalDeck = value; mergInfo(); OnPropertyChanged("NormalDeck"); }
		}

		public int MercRule
		{
			get { return mercRule; }
			set { mercRule = value;
				/* 同步更改下拉框？待解决
				if (value == 2)
				{
					MercBehavior = 1;
					OnPropertyChanged("MercBehaviour");
				}
				if (value == 3)
				{
					MercBehavior = 2;
					OnPropertyChanged("MercBehaviour");
				}
				*/
				mergInfo(); OnPropertyChanged("MercRule"); }
		}
		public int MercBehavior
		{
			get { return mercBehavior; }
			set { mercBehavior = value;
				mergInfo(); OnPropertyChanged("MercBehaviour"); }
		}
		public string MercTeam
		{
			get { return mercTeam; }
			set { mercTeam = value; mergInfo(); OnPropertyChanged("MercTeam"); }
		}
		public string MercMap
		{
			get { return mercMap; }
			set { mercMap = value; mergInfo(); OnPropertyChanged("MercMap"); }
		}
		public int MercInterval
		{
			get { return mercInterval; }
			set { mercInterval = (value >= 27 || value <= 10) ? 22 : value;
				  mergInfo(); OnPropertyChanged("MercInterval"); }
		}
		public bool MercConcede
		{
			get { return mercConcede; }
			set { mercConcede = value; mergInfo(); OnPropertyChanged("MercConcede"); }
		}
		public bool MercCraft
		{
			get { return mercCraft; }
			set { mercCraft = value; mergInfo(); OnPropertyChanged("MercCraft"); }
		}
		public bool MercUpdate
		{
			get { return mercUpdate; }
			set { mercUpdate = value; mergInfo(); OnPropertyChanged("MercUpdate"); }
		}
	}
}

