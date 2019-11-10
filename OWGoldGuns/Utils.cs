using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OWGoldGuns
{
	public static class Utils
	{
		public static BitmapSource GetBitmapFromUri(string image_path)
		{
			BitmapImage img = new BitmapImage();

			img.BeginInit();
			try
			{
				img.UriSource = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + image_path, UriKind.RelativeOrAbsolute);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			img.EndInit();

			return img;
		}
	}
}
