using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShortBridge
{
	/// <summary>
	/// Interaction logic for SearchBridge.xaml
	/// </summary>
	public partial class SearchBridge : Window
	{
		DatabaseManager dbm;
		public SearchBridge(DatabaseManager dbm)
		{
			InitializeComponent();
			this.dbm = dbm;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}

		private void txtbox_BridgeName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				DoSearch(txtbox_BridgeName.Text);
			};
		}

		public void DoSearch(string with)
		{
			List<DB_BridgeLink> result = dbm.SearchLinks(with);
			if (result.Count <= 0)
			{
				MessageBox.Show("No results found");
			}
			else
			{
				BridgeLinksHolder.Children.Clear();
				foreach (var item in result)
				{
					CreateBridgeLink(item);
				}
			}
		}

		public void CreateBridgeLink(DB_BridgeLink item)
		{
			Border holder = new Border()
			{
				Margin = new Thickness(5),
				Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
				CornerRadius = new CornerRadius(5),
				Cursor = Cursors.Hand,
				Tag = item.link
			};

			StackPanel stacker = new StackPanel()
			{
				Margin = new Thickness(2),
				Tag = item.id + ""
			};

			Label BridgeName = new Label()
			{
				Foreground = new SolidColorBrush(Colors.White),
				FontSize = 13,
				Padding = new Thickness(0),
				Content = item.name
			};

			Label BridgeLink = new Label()
			{
				Foreground = new SolidColorBrush(Colors.Silver),
				FontSize = 7,
				Padding = new Thickness(0),
				Content = item.link
			};

			stacker.Children.Add(BridgeName);
			stacker.Children.Add(BridgeLink);
			holder.Child = stacker;
			BridgeLinksHolder.Children.Add(holder);

			holder.MouseDown += Holder_MouseDown;
		}

		private void Holder_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Border sent = (Border)sender;
			if (e.LeftButton == MouseButtonState.Pressed && !string.IsNullOrEmpty(sent.Tag + ""))
			{
				dbm.OpenLink(sent.Tag + "");
			}
			if (e.RightButton == MouseButtonState.Pressed && sent.Child != null)
			{

				if (sent.Child != null)
				{
					StackPanel child = (StackPanel)sent.Child;
					dbm.DeleteLink(child.Tag + "");
					BridgeLinksHolder.Children.Remove(sent);
				}
			}
		}

		#region Form Movement
		Point pivot;
		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			pivot = Mouse.GetPosition(null);
		}

		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.Left += e.GetPosition(null).X - pivot.X;
				this.Top += e.GetPosition(null).Y - pivot.Y;
			}
		}
		#endregion

		private void btn_CloseSearchBridge_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DoSearch("");
		}
	}
}
