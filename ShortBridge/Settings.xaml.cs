using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ShortBridge
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window
	{
		DatabaseManager dbm;
		public Settings(DatabaseManager dbm)
		{
			InitializeComponent();
			this.dbm = dbm;
		}

		OpenFileDialog openFileDialog = new OpenFileDialog();
		private void btn_SelectFile_Click(object sender, RoutedEventArgs e)
		{
			if (openFileDialog.ShowDialog() == true && openFileDialog.FileName != null)
			{
				txt_SelectedPath.Text=openFileDialog.FileName;
			}
		}

		private void btn_CloseSettings_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}

		private void btn_AddLink_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(txt_LinkName.Text) || string.IsNullOrEmpty(txt_SelectedPath.Text))
            {
				MessageBox.Show("All fields must be filled");
			}
			else
			{
				dbm.AddLink(txt_LinkName.Text, txt_SelectedPath.Text);
			}
        }


		#region Form Movement
		Point pivot;
		private void grid_NavBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			pivot = Mouse.GetPosition(null);
		}

		private void grid_NavBar_MouseMove(object sender, MouseEventArgs e)
		{
            if (e.LeftButton==MouseButtonState.Pressed)
            {
				this.Left += e.GetPosition(null).X - pivot.X;
				this.Top += e.GetPosition(null).Y - pivot.Y;
			}
        }
		#endregion

	}
}
