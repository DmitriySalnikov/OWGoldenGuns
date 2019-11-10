using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OWGoldGuns
{
	/// <summary>
	/// Interaction logic for HeroCard.xaml
	/// </summary>
	public partial class HeroCard : UserControl
	{
		public enum CardState
		{
			NotGoldNormal,
			NotGoldHover,
			GoldNormal,
			GoldHover,
			Disabled
		}

		public delegate void VoidBool(bool isgold);
		public event VoidBool OnIsGoldChanged;

		private CardState _CurrentState = CardState.Disabled;
		public CardState CurrentState
		{
			get => _CurrentState;
			set
			{
				_CurrentState = value;
				UpdateColors();
			}
		}

		private bool _isGold = false;
		public bool IsGold
		{
			get => _isGold;
			set
			{
				SaveData.Profiles.AddProfile(SaveData.Profiles.CurrentProfileName);
				if (SaveData.Profiles.CurrentProfile == null)
					return;
				_isGold = value;

				if (SaveData.Profiles.CurrentProfile.IsGoldHeroes.ContainsKey(HeroID))
				{
					if (!value)
						SaveData.Profiles.CurrentProfile.IsGoldHeroes.Remove(HeroID);
				}
				else
				{
					if (value)
						SaveData.Profiles.CurrentProfile.IsGoldHeroes.Add(HeroID, value);
				}
				SaveData.Save();

				OnIsGoldChanged?.Invoke(value);
				UpdateState();
			}
		}

		private string _heroID = "";
		public string HeroID
		{
			get => _heroID;
			set => _heroID = value;
		}

		private HeroesData.Hero _HeroData = null;
		public HeroesData.Hero HeroData
		{
			get => _HeroData;
			set
			{
				_HeroData = value;
				UpdateCard();
			}
		}

		private bool IsHovered = false;

		public HeroCard()
		{
			InitializeComponent();
			UpdateColors();
		}

		public void UpdateCard()
		{
			tb_hero_name.Text = HeroData.Name;
			i_hero_image.Source = HeroData.Icon;
		}

		private void UpdateState()
		{
			if (!IsEnabled)
			{
				CurrentState = CardState.Disabled;
				return;
			}

			if (IsGold)
			{
				if (IsHovered)
					CurrentState = CardState.GoldHover;
				else
					CurrentState = CardState.GoldNormal;
			}
			else
			{
				if (IsHovered)
					CurrentState = CardState.NotGoldHover;
				else
					CurrentState = CardState.NotGoldNormal;
			}
		}

		private void UpdateColors()
		{
			System.Windows.Media.Color bg_color1 = new System.Windows.Media.Color();
			System.Windows.Media.Color bg_color2 = new System.Windows.Media.Color();
			System.Windows.Media.Color border_color = new System.Windows.Media.Color();
			const int gradient_offset = 36;
			const int hover_offset = 16;
			const int alpha = 192;

			switch (_CurrentState)
			{
				case CardState.NotGoldNormal:
					bg_color1 = System.Windows.Media.Color.FromRgb(202, 197, 226);
					bg_color2 = System.Windows.Media.Color.FromRgb(202 - gradient_offset, 197 - gradient_offset, 226 - gradient_offset);
					border_color = System.Windows.Media.Color.FromRgb(228, 228, 232);
					break;
				case CardState.NotGoldHover:
					bg_color1 = System.Windows.Media.Color.FromRgb(202 - hover_offset, 197 - hover_offset, 226 - hover_offset);
					bg_color2 = System.Windows.Media.Color.FromRgb(202 - gradient_offset - hover_offset, 197 - gradient_offset - hover_offset, 226 - gradient_offset - hover_offset);
					border_color = System.Windows.Media.Color.FromRgb(228 - hover_offset, 228 - hover_offset, 232 - hover_offset);
					break;
				case CardState.GoldNormal:
					bg_color1 = System.Windows.Media.Color.FromRgb(255, 246, 65);
					bg_color2 = System.Windows.Media.Color.FromRgb(255 - gradient_offset, 246 - gradient_offset, 65 - gradient_offset);
					border_color = System.Windows.Media.Color.FromRgb(255, 205, 36);
					break;
				case CardState.GoldHover:
					bg_color1 = System.Windows.Media.Color.FromRgb(255 - hover_offset, 246 - hover_offset, 65 - hover_offset);
					bg_color2 = System.Windows.Media.Color.FromRgb(255 - gradient_offset - hover_offset, 246 - gradient_offset - hover_offset, 65 - gradient_offset - hover_offset);
					border_color = System.Windows.Media.Color.FromRgb(255 - hover_offset, 205 - hover_offset, 36 - hover_offset);
					break;
				case CardState.Disabled:
					bg_color1 = System.Windows.Media.Color.FromRgb(100,100, 100);
					bg_color2 = System.Windows.Media.Color.FromRgb(80, 80, 80);
					border_color = System.Windows.Media.Color.FromRgb(106, 106, 106);
					break;
			}

			bg_color1.A = border_color.A = alpha;

			if (b_card_background != null)
			{
				b_card_background.Background = new LinearGradientBrush(bg_color1, bg_color2, new System.Windows.Point(0.5, 0), new System.Windows.Point(0.5, 0.8));
				b_card_background.BorderBrush = new SolidColorBrush(border_color);
			}
			if (b_name_bg != null)
				b_name_bg.Background = new SolidColorBrush(border_color);
		}

		private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (IsEnabled)
			{
				if (!IsGold)
					IsGold = true;
				else
				{
					if (MessageBox.Show(string.Format("Do you really want to remove {0}’s golden gun", HeroData.Name), "Is it a mistake?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
						IsGold = false;
				}
			}
		}

		private void UserControl_MouseEnter(object sender, MouseEventArgs e)
		{
			IsHovered = true;
			UpdateState();
		}

		private void UserControl_MouseLeave(object sender, MouseEventArgs e)
		{
			IsHovered = false;
			UpdateState();
		}

		private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			UpdateState();
		}
	}
}
