using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OWGoldenGuns
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		bool profile_changed_by_code = false;
		public MainWindow()
		{
			InitializeComponent();

			HeroesData.Load();
			SaveData.Load();
			//SaveData.Save();

			if (HeroesData.Heroes.Count != 0)
			{
				LoadHeroesList();
			}
			else
			{
				MessageBox.Show(LocalizationUtils.GetString("mainform_heroes_json_error", "Heroes.json not found or corrupted.\nTry to reinstall this program."),
					LocalizationUtils.GetString("error", "Error")
					);
			}

			UpdateProfilesList();
			SortHeroCards();
			UpdateLocale();

			SettingsData.Settings.OnLocaleChanged += Settings_OnLocaleChanged;

			if (SettingsData.Settings.MainWindowTop != null)
				Top = (int)SettingsData.Settings.MainWindowTop;
			if (SettingsData.Settings.MainWindowTop != null)
				Left = (int)SettingsData.Settings.MainWindowLeft;
			if (SettingsData.Settings.MainWindowWidth != null)
				Width = (int)SettingsData.Settings.MainWindowWidth;
			if (SettingsData.Settings.MainWindowHeight != null)
				Height = (int)SettingsData.Settings.MainWindowHeight;

			WindowState = SettingsData.Settings.MainWindowMaximized ? WindowState.Maximized : WindowState.Normal;
		}

		public void UpdateLocale()
		{
			btn_add_profile.Content = LocalizationUtils.GetString("mainform_add_profile", "Add");
			btn_remove_profile.Content = LocalizationUtils.GetString("mainform_remove_profile", "Remove");
			l_about_app.Content = LocalizationUtils.GetString("mainform_about_this_app", "About this app");
		}

		private void UpdateProfilesList()
		{
			profile_changed_by_code = true;
			cb_profiles.Items.Clear();

			foreach (var s in SaveData.Profiles.SavedProfiles.Keys)
			{
				cb_profiles.Items.Add(s);
			}

			cb_profiles.SelectedItem = SaveData.Profiles.CurrentProfileName;
			profile_changed_by_code = false;
		}

		// mb its not optimal, but its a working way of sorting :)
		private void SortHeroCards()
		{
			var children_array = new UIElement[wp_heroes.Children.Count];
			wp_heroes.Children.CopyTo(children_array, 0);
			wp_heroes.Children.Clear();
			List<UIElement> prevChildren = children_array.ToList();
			List<HeroCard> note_used = new List<HeroCard>();

			List<string> ids = HeroesData.GetHeroIDsSortedByLocalizedNames();

			foreach (string n in ids)
			{
				bool isfound = false;
				HeroCard prev_card = null;
				foreach (HeroCard u in prevChildren)
				{
					if (u.HeroData.HeroID == n)
					{
						prev_card = u;
						wp_heroes.Children.Add(u);
						isfound = true;
					}
				}

				if (isfound)
				{
					prevChildren.Remove(prev_card);
				}
			}

			prevChildren.ForEach((e) => wp_heroes.Children.Add(e));
		}

		private void LoadHeroesList()
		{
			wp_heroes.Children.Clear();

			foreach (var h in HeroesData.Heroes)
			{
				bool enabled = true;
				bool is_gold = false;

				if (SaveData.Profiles.CurrentProfile != null)
				{
					if (SaveData.Profiles.CurrentProfile.IsGoldHeroes.ContainsKey(h.HeroID))
						is_gold = SaveData.Profiles.CurrentProfile.IsGoldHeroes[h.HeroID];
				}
				else
				{
					enabled = false;
				}

				var new_card = new HeroCard()
				{
					HeroData = h,
					IsGold = is_gold,
				};
				new_card.OnIsGoldChanged += New_card_OnIsGoldChanged;
				new_card.IsEnabled = enabled;

				wp_heroes.Children.Add(new_card);
			}

			UpdateGoldsCounter();
		}

		private void UpdateCards()
		{
			foreach (HeroCard c in wp_heroes.Children)
			{
				if (SaveData.Profiles.CurrentProfile != null)
				{
					c.IsEnabled = true;
					if (SaveData.Profiles.CurrentProfile.IsGoldHeroes.ContainsKey(c.HeroData.HeroID))
					{
						c.IsGold = SaveData.Profiles.CurrentProfile.IsGoldHeroes[c.HeroData.HeroID];
					}
					else
					{
						c.IsGold = false;
					}
				}
				else
				{
					c.IsEnabled = false;
				}
			}
		}

		private void UpdateGoldsCounter()
		{
			int golds = 0;
			foreach (HeroCard h in wp_heroes.Children)
			{
				if (h.IsGold)
					golds++;
			}

			// 2 spaces in end of line for strange OW font
			tb_golds.Text = $"{golds}/{HeroesData.Heroes.Count}  ";
		}

		private void Settings_OnLocaleChanged(string locale)
		{
			SortHeroCards();
			UpdateLocale();
		}

		private void New_card_OnIsGoldChanged(bool isgold)
		{
			UpdateGoldsCounter();
		}

		private void cb_profiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!profile_changed_by_code)
			{
				SaveData.Profiles.CurrentProfileName = (string)cb_profiles.SelectedItem;
				UpdateCards();
			}
		}

		private void btn_add_profile_Click(object sender, RoutedEventArgs e)
		{
			var add_dlg = new AddProfile();
			var res = add_dlg.ShowDialog();
			if (res != null && res == true)
			{
				SaveData.Profiles.AddProfile(add_dlg.NewProfileName);
				SaveData.Profiles.CurrentProfileName = add_dlg.NewProfileName;
			}

			UpdateProfilesList();
			UpdateCards();
		}

		private void btn_remove_profile_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show(string.Format(LocalizationUtils.GetString("mainform_confirm_remove_text", "Do you really want to remove profile {0}?"), (string)cb_profiles.SelectedItem),
				LocalizationUtils.GetString("mainform_confirm_remove_title", "Please confirm..."), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				SaveData.Profiles.RemoveProfile((string)cb_profiles.SelectedItem);

				UpdateProfilesList();
				UpdateCards();
			}
		}

		private void i_langs_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var lang_sel = new LanguageSelect();
			bool? res = lang_sel.ShowDialog();

			if (res != null && res == true)
			{
				SettingsData.Settings.CurrentLocale = lang_sel.selected_lang;
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SettingsData.Settings.MainWindowTop = (int)Top;
			SettingsData.Settings.MainWindowLeft = (int)Left;
			SettingsData.Settings.MainWindowWidth = (int)Width;
			SettingsData.Settings.MainWindowHeight = (int)Height;
			SettingsData.Settings.MainWindowMaximized = WindowState == WindowState.Maximized ? true : false;
		}

		private void l_about_app_MouseDown(object sender, MouseButtonEventArgs e)
		{
			new AboutApp().ShowDialog();
		}
	}
}
