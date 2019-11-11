using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace OWGoldenGuns
{
	public static class SettingsData
	{
		public class Data
		{
			public delegate void VoidString(string locale);
			public event VoidString OnLocaleChanged;

			[JsonProperty("show_only_available_langs")]
			private bool _ShowOnlyAvailableLocales = true;
			[JsonIgnore]
			public bool ShowOnlyAvailableLocales
			{
				get => _ShowOnlyAvailableLocales;
				set
				{
					_ShowOnlyAvailableLocales = value;
					Save();
				}
			}

			[JsonProperty("locale")]
			private string _CurrentLocale = "";
			[JsonIgnore]
			public string CurrentLocale
			{
				get => _CurrentLocale;
				set
				{
					_CurrentLocale = value;
					Save();

					System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(value);
					OnLocaleChanged?.Invoke(value);
				}
			}

			[JsonProperty("mf_top")]
			public int? MainWindowTop = null;
			[JsonProperty("mf_left")]
			public int? MainWindowLeft = null;
			[JsonProperty("mf_width")]
			public int? MainWindowWidth = null;
			[JsonProperty("mf_height")]
			public int? MainWindowHeight = null;
			[JsonProperty("mf_maximized")]
			public bool MainWindowMaximized = false;
		}

		const string SaveFile = "Settings.json";

		public static Data Settings = new Data();

		public static void Save()
		{
			try
			{
				File.WriteAllText(SaveFile, JsonConvert.SerializeObject(Settings, Formatting.Indented));
			}
			catch (Exception e)
			{
				MessageBox.Show(LocalizationUtils.GetString("settingsdata_cant_write", "Can't write profiles file!") + "\n" + e.Message, LocalizationUtils.GetString("error", "Error"));
			}
		}

		public static void Load()
		{
			if (File.Exists(SaveFile))
			{
				string text;
				try
				{
					text = File.ReadAllText(SaveFile);
				}
				catch (Exception e)
				{
					MessageBox.Show(LocalizationUtils.GetString("settingsdata_cant_read", "Can't read profiles file!") + "\n" + e.Message, LocalizationUtils.GetString("error", "Error"));
					return;
				}

				try
				{
					Settings = JsonConvert.DeserializeObject<Data>(text);
				}
				catch (Exception e)
				{
					MessageBox.Show(LocalizationUtils.GetString("settingsdata_cant_parse", "Can't parse profiles file!") + "\n" + e.Message, LocalizationUtils.GetString("error", "Error"));
					return;
				}
			}
		}
	}
}