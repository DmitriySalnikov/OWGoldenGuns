using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace OWGoldenGuns
{
	static class LocalizationUtils
	{
		const string LangsDir = "Langs";

		private static Dictionary<string, Dictionary<string, string>> LoadedLocales = new Dictionary<string, Dictionary<string, string>>();

		public static bool IsAvailableLocale(string locale)
		{
			return LoadedLocales.ContainsKey(locale);
		}

		public static List<string> GetAvailableLocales()
		{
			var res = LoadedLocales.Keys.ToList();
			res.Insert(0, "en");
			return res;
		}

		public static void LoadLocalizations()
		{
			try
			{
				if (Directory.Exists(LangsDir))
				{
					foreach (var f in Directory.GetFiles(LangsDir))
					{
						if (Path.GetExtension(f) == ".json")
						{
							string loc = Path.GetFileNameWithoutExtension(f);
							if (IsValidLocale(loc))
							{
								string text = File.ReadAllText(f);

								try
								{
									var lang_data = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
									if (lang_data != null)
										LoadedLocales.Add(loc, lang_data);
								}
								catch (Exception e)
								{
									MessageBox.Show($"Localization for '{CultureInfo.GetCultureInfo(loc).DisplayName}' in '{f}' cant be read!\n" + e.Message, "Localization Error!");
								}


							}
						}
					}
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("Cant load localization!\n" + e.Message, "Localization Error!");
			}
		}

		public static string GetString(string key, string default_english)
		{
			string locale = CultureInfo.CurrentCulture.Name;
			if (LoadedLocales.ContainsKey(locale))
			{
				if (LoadedLocales[locale].ContainsKey(key))
					return LoadedLocales[locale][key];
			}

			locale = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			if (LoadedLocales.ContainsKey(locale))
			{
				if (LoadedLocales[locale].ContainsKey(key))
					return LoadedLocales[locale][key];
			}

			return default_english;
		}

		private static bool IsValidLocale(string loc)
		{
			try
			{
				CultureInfo.GetCultureInfo(loc);
				return true;
			}
			catch
			{
				return false;
			}
		}

	}
}
