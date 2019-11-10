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
	public static class SaveData
	{
		public class Data
		{
			[JsonProperty]
			public Dictionary<string, bool> IsGoldHeroes = new Dictionary<string, bool>();
		}

		public class ProfilesData
		{
			[JsonProperty("current_profile")]
			public string CurrentProfileName = "";
			[JsonProperty("profiles")]
			public Dictionary<string, Data> SavedProfiles = new Dictionary<string, Data>();

			[JsonIgnore]
			public Data CurrentProfile
			{
				get
				{
					if (CurrentProfileName != null && SavedProfiles.ContainsKey(CurrentProfileName))
						return SavedProfiles[CurrentProfileName];

					return null;
				}
			}

			public void AddProfile(string name)
			{
				if (name != null && name != "" && !SavedProfiles.ContainsKey(name))
				{
					SavedProfiles.Add(name, new Data());
				}

				Save();
			}

			public void RemoveProfile(string name)
			{
				if (name != null && name != "" && SavedProfiles.ContainsKey(name))
				{
					SavedProfiles.Remove(name);
				}

				if (!Profiles.SavedProfiles.ContainsKey(Profiles.CurrentProfileName))
				{
					if (Profiles.SavedProfiles.Count != 0)
						Profiles.CurrentProfileName = Profiles.SavedProfiles.First().Key;
					else
						Profiles.CurrentProfileName = "";
				}

				Save();
			}
		}

		const string SaveFile = "Profiles.json";

		public static ProfilesData Profiles = new ProfilesData();

		public static void Save()
		{
			try
			{
				File.WriteAllText(SaveFile, JsonConvert.SerializeObject(Profiles, Formatting.Indented));
			}
			catch (Exception e)
			{
				MessageBox.Show("Error! Can't write save file!\n" + e.Message);
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
					MessageBox.Show("Error! Can't read save file!\n" + e.Message);
					return;
				}

				try
				{
					Profiles = JsonConvert.DeserializeObject<ProfilesData>(text);
				}
				catch (Exception e)
				{
					MessageBox.Show("Error! Can't parse save file!\n" + e.Message);
					return;
				}
			}
		}
	}
}
