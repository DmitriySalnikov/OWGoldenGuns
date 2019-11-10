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
			[JsonProperty("name_en"), JsonRequired]
			string NameEn = "";

			[JsonProperty("hero_id"), JsonRequired]
			string _HeroID = "";
			[JsonIgnore]
			public string HeroID
			{
				get => _HeroID;
			}

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
				get => LocalizationUtils.GetString(_HeroID, NameEn);
			}
		}

		const string HeroesDataFile = "Heroes.json";
		public static List<Hero> Heroes = new List<Hero>();

		/// <summary>
		/// Return sorted heroes IDs
		/// </summary>
		/// <returns></returns>
		public static List<string> GetHeroIDsSortedByLocalizedNames()
		{
			var names = Heroes.ToList();
			names.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));

			List<string> res = new List<string>();
			names.ForEach((n) => res.Add(n.HeroID));

			return res;
		}

		// Only for initial create of this file
		public static void Save()
		{
			try
			{
				File.WriteAllText(HeroesDataFile, JsonConvert.SerializeObject(Heroes, Formatting.Indented));
			}
			catch (Exception e)
			{
				MessageBox.Show(LocalizationUtils.GetString("heroesdata_cant_write", "Can't write heroes data file!") + "\n" + e.Message, LocalizationUtils.GetString("error", "Error"));
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
					MessageBox.Show(LocalizationUtils.GetString("heroesdata_cant_read", "Can't read heroes data file!") + "\n" + e.Message, LocalizationUtils.GetString("error", "Error"));
					return;
				}

				try
				{
					Heroes = JsonConvert.DeserializeObject<List<Hero>>(text);
				}
				catch (Exception e)
				{
					MessageBox.Show(LocalizationUtils.GetString("heroesdata_cant_parse", "Can't parse heroes data file!") + "\n" + e.Message, LocalizationUtils.GetString("error", "Error"));
					return;
				}
			}
		}
	}
}
