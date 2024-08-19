using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShortBridge
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DatabaseManager dbm = new DatabaseManager();
		SearchBridge search;
		Settings settingsMenu;


		public MainWindow()
		{
			InitializeComponent();

			search = new SearchBridge(dbm);
			search.Hide();

			settingsMenu = new Settings(dbm);
			settingsMenu.Hide();

			dbm.OnBridgeLinkAdded += Dbm_OnBridgeLinkAdded;
			dbm.OnBridgeLinkDeleted += Dbm_OnBridgeLinkDeleted;
		}

		private void Dbm_OnBridgeLinkDeleted()
		{
			minLinkId = -2;
			maxLinkId = -1;
			BridgeLinksHolder.Children.Clear();
			ShowBridgeLinks();
		}

		private void Dbm_OnBridgeLinkAdded()
		{
			minLinkId = -2;
			maxLinkId = -1;
			BridgeLinksHolder.Children.Clear();
			ShowBridgeLinks();
			MessageBox.Show("bridge added");
		}

		private void btn_Exit_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure to exit?", "you are about to leave app", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Application.Current.Shutdown();
			}
		}

		private void btn_Minimize_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void btn_SearchLink_Click(object sender, RoutedEventArgs e)
		{
			if (search.Visibility == Visibility.Visible)
			{
				search.Hide();
			}
			else
			{
				search.Show();
			}
		}

		private void btn_Settings_Click(object sender, RoutedEventArgs e)
		{
			if (settingsMenu.Visibility == Visibility.Visible)
			{
				settingsMenu.Hide();
			}
			else
			{
				settingsMenu.Show();
			}
		}

		int displayLimit = 5;
		int minLinkId = -2;
		int maxLinkId = -1;

		public void ShowBridgeLinks(bool pre = false)
		{
			List<DB_BridgeLink> links = dbm.GetLinks(pre ? minLinkId : maxLinkId, displayLimit, pre);
			if (links.Count <= 0)
			{
				lbl_BridgeCount.Content="no bridges";
			}
			else
			{
				lbl_BridgeCount.Content = null;
				maxLinkId = links[links.Count - 1].id;
				minLinkId = links[0].id;
				foreach (var item in links)
				{
					CreateBridgeLink(item);
				}
			}
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			ShowBridgeLinks();
			
			this.Height = this.ActualHeight + Math.Clamp(BridgeLinksHolder.Children.Count * 50, 50, this.ActualHeight / 2);
			this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (this.ActualHeight / 2);
			
		}

		public void CreateBridgeLink(DB_BridgeLink item)
		{
			Button bridgeLink = new Button() { Style = (Style)Application.Current.Resources["BridgeLink"] };
			bridgeLink.Tag = item.link;
			bridgeLink.Content = item.name;
			bridgeLink.Click += BridgeLink_Click;
			BridgeLinksHolder.Children.Add(bridgeLink);
		}

		private void BridgeLink_Click(object sender, RoutedEventArgs e)
		{
			Button whichLink = (Button)sender;
			dbm.OpenLink(whichLink.Tag?.ToString());
		}



		private void btn_NextPage_Click(object sender, RoutedEventArgs e)
		{
			ShowBridgeLinks();
		}

		private void btn_PreviousPage_Click(object sender, RoutedEventArgs e)
		{
			ShowBridgeLinks(true);
		}


	}
}
