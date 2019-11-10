using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

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

			if (!LocalizationUtils.IsAvailableLocale(SettingsData.Settings.CurrentLocale))
				SettingsData.Settings.CurrentLocale = "en";
			System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(SettingsData.Settings.CurrentLocale);

			LocalizationUtils.LoadLocalizations();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			SettingsData.Save();
		}
	}
}
