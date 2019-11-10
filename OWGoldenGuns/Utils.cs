using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OWGoldenGuns
{
	public static class Utils
	{
		public static BitmapSource GetBitmapFromUri(string image_path)
		{
			string f = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + image_path;

			if (File.Exists(f))
			{
				BitmapImage img = new BitmapImage();
				img.BeginInit();
				try
				{
					img.UriSource = new Uri(f, UriKind.RelativeOrAbsolute);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
				img.EndInit();

				return img;
			}

			return null;
		}
	}
}
