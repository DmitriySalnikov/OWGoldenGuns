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
using System.Windows.Shapes;

namespace OWGoldenGuns
{
	/// <summary>
	/// Interaction logic for AddProfile.xaml
	/// </summary>
	public partial class AddProfile : Window
	{
		public string NewProfileName = "";
		public AddProfile()
		{
			InitializeComponent();

			tb_name.Focus();
			tb_name.SelectAll();
			CheckIsValidText();
			UpdateLocale();

			SettingsData.Settings.OnLocaleChanged += Settings_OnLocaleChanged;
		}

		private void Settings_OnLocaleChanged(string locale)
		{
			UpdateLocale();
		}

		public void UpdateLocale()
		{
			Title = LocalizationUtils.GetString("add_profile_title", "Add New Profile");
			l_profile_name.Content = LocalizationUtils.GetString("add_profile_label", "New profile name:");
			btn_ok.Content = LocalizationUtils.GetString("add_profile_button", "Add Profile");
		}

		private void CheckIsValidText()
		{
			if (btn_ok == null)
				return;

			if (tb_name.Text.Length == 0)
			{
				btn_ok.IsEnabled = false;
				return;
			}

			if (SaveData.Profiles.SavedProfiles.ContainsKey(tb_name.Text))
			{
				btn_ok.IsEnabled = false;
				return;
			}

			btn_ok.IsEnabled = true;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			NewProfileName = tb_name.Text;

			DialogResult = true;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CheckIsValidText();
		}
	}
}
