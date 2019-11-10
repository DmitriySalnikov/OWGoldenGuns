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

namespace OWGoldGuns
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
				MessageBox.Show("Heroes.json not found or corrupted.\nTry to reinstall this program.", "Error");
			}

			UpdateProfilesList();
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

		private void LoadHeroesList()
		{
			wp_heroes.Children.Clear();

			foreach (var h in HeroesData.Heroes)
			{
				bool enabled = true;
				bool is_gold = false;

				if (SaveData.Profiles.CurrentProfile != null)
				{
					if (SaveData.Profiles.CurrentProfile.IsGoldHeroes.ContainsKey(h.Key))
						is_gold = SaveData.Profiles.CurrentProfile.IsGoldHeroes[h.Key];
				}
				else
				{
					enabled = false;
				}

				var new_card = new HeroCard()
				{
					HeroData = h.Value,
					HeroID = h.Key,
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
					if (SaveData.Profiles.CurrentProfile.IsGoldHeroes.ContainsKey(c.HeroID))
					{
						c.IsGold = SaveData.Profiles.CurrentProfile.IsGoldHeroes[c.HeroID];
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
			tb_golds.Text = $"Gold {golds}/{HeroesData.Heroes.Count}  ";
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
			SaveData.Profiles.RemoveProfile((string)cb_profiles.SelectedItem);

			UpdateProfilesList();
			UpdateCards();
		}
	}
}
