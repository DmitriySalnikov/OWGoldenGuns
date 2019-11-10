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
using System.Globalization;

namespace OWGoldenGuns
{
	/// <summary>
	/// Interaction logic for LanguageSelect.xaml
	/// </summary>
	public partial class LanguageSelect : Window
	{
		private class LangLine
		{
			public CultureInfo info;
			public override string ToString()
			{
				return info.DisplayName;
			}
		}

		public string selected_lang = "en";

		public LanguageSelect()
		{
			InitializeComponent();

			UpdateLocale();

			cb_show_only_available.IsChecked = SettingsData.Settings.ShowOnlyAvailableLocales;
			BuildLanguagesList();

			SettingsData.Settings.OnLocaleChanged += Settings_OnLocaleChanged;
		}

		private void Settings_OnLocaleChanged(string locale)
		{
			UpdateLocale();
		}

		public void UpdateLocale()
		{
			Title = LocalizationUtils.GetString("selectlang_title", "Select Language");
			btn_accept.Content = LocalizationUtils.GetString("apply", "Apply");
			cb_show_only_available.Content = LocalizationUtils.GetString("selectlang_show_only", "Show only available locales");
		}

		private void BuildLanguagesList()
		{
			lb_languages.Items.Clear();

			var cur_cul = CultureInfo.CurrentCulture;
			var langs = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
			var langs_sorted = new List<LangLine>();
			LangLine line_to_sel = null;

			bool only_available = cb_show_only_available.IsChecked == true ? true : false;
			var available_langs = LocalizationUtils.GetAvailableLocales();

			langs.ForEach((c) =>
			{
				var new_line = new LangLine() { info = c };
				if (!only_available || available_langs.Contains(c.Name))
				{
					langs_sorted.Add(new_line);

					if (c.Name == cur_cul.Name)
						line_to_sel = new_line;
				}

			});
			langs_sorted.Sort((l1, l2) => l1.ToString().CompareTo(l2.ToString()));

			langs_sorted.ForEach((l) => lb_languages.Items.Add(l));

			if (line_to_sel != null)
				lb_languages.SelectedItem = line_to_sel;

			UpdateButton();
		}

		private void UpdateButton()
		{
			if (lb_languages.SelectedItem != null)
				btn_accept.IsEnabled = true;
			else
				btn_accept.IsEnabled = false;
		}

		private void btn_accept_Click(object sender, RoutedEventArgs e)
		{
			if (lb_languages.SelectedItem != null)
			{
				selected_lang = ((LangLine)(lb_languages.SelectedItem)).info.Name;

				DialogResult = true;
			}
		}

		private void lb_languages_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateButton();
		}

		private void cb_show_only_available_Click(object sender, RoutedEventArgs e)
		{
			SettingsData.Settings.ShowOnlyAvailableLocales = cb_show_only_available.IsChecked == true ? true : false;
			BuildLanguagesList();
		}
	}
}
