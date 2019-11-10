using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.Globalization;

namespace OWGoldenGuns
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			SettingsData.Load();
			LocalizationUtils.LoadLocalizations();

			if (SettingsData.Settings.CurrentLocale != "")
			{
				if (!LocalizationUtils.IsAvailableLocale(SettingsData.Settings.CurrentLocale))
					SettingsData.Settings.CurrentLocale = "en";
			}
			else
			{
				if (LocalizationUtils.IsAvailableLocale(CultureInfo.CurrentCulture.Name))
					SettingsData.Settings.CurrentLocale = CultureInfo.CurrentCulture.Name;
				else
					SettingsData.Settings.CurrentLocale = "en";
			}

			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(SettingsData.Settings.CurrentLocale);
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			SettingsData.Save();
		}
	}
}
