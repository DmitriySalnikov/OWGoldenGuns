using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace OWGoldenGuns
{
	public static class HeroesData
	{
		public class Hero
		{
			[JsonProperty("loc_name"), JsonRequired]
			Dictionary<string, string> LocName = new Dictionary<string, string>();

			[JsonProperty("icon_name"), JsonRequired]
			private string _iconPath = "";
			[JsonIgnore]
			public string IconPath
			{
				get => _iconPath;
			}

			[JsonIgnore]
			public System.Windows.Media.Imaging.BitmapSource Icon
			{
				get => Utils.GetBitmapFromUri("Icons/" + _iconPath);
			}

			[JsonIgnore]
			public string Name
			{
				get
				{
					string locale = CultureInfo.CurrentCulture.Name;
					if (LocName.ContainsKey(locale))
					{
						return LocName[locale];
					}

					locale = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
					if (LocName.ContainsKey(locale))
					{
						return LocName[locale];
					}

					if (LocName.ContainsKey("en"))
					{
						return LocName["en"];
					}

					if (LocName.Count > 0)
					{
						return LocName.First().Value;
					}

					return "Hero";
				}
			}
		}

		const string HeroesDataFile = "Heroes.json";
		public static Dictionary<string, Hero> Heroes = new Dictionary<string, Hero>();

		// Only for initial crate of this file
		public static void Save()
		{
			try
			{
				File.WriteAllText(HeroesDataFile, JsonConvert.SerializeObject(Heroes, Formatting.Indented));
			}
			catch (Exception e)
			{
				MessageBox.Show("Error! Can't write heroes data file!\n" + e.Message);
			}
		}

		public static void Load()
		{
			if (File.Exists(HeroesDataFile))
			{
				string text;
				try
				{
					text = File.ReadAllText(HeroesDataFile);
				}
				catch (Exception e)
				{
					MessageBox.Show("Error! Can't read heroes data file!\n" + e.Message);
					return;
				}

				try
				{
					Heroes = JsonConvert.DeserializeObject<Dictionary<string, Hero>>(text);
				}
				catch (Exception e)
				{
					MessageBox.Show("Error! Can't parse heroes data file!\n" + e.Message);
					return;
				}
			}
		}
	}
}
