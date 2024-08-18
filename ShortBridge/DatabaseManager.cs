using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.Data;
using System.Diagnostics;

namespace ShortBridge
{
	public class DatabaseManager
	{
		SQLiteConnection con = new SQLiteConnection(@"Data Source=.\ShortBridge.sqlite;version=3");
        private DataTable? RunQuery(string query, object[,]? args = null)
		{
			try
			{
				con.Close();
				con.Open();
				DataTable result = new DataTable();
				using (SQLiteCommand com = new SQLiteCommand(query, con))
				{
					if (args != null)
					{
						for (int i = 0; i < args.GetLength(0); i++)
						{
							com.Parameters.AddWithValue(args[i, 0] + "", args[i, 1]);
						}
					}

					//com.ExecuteNonQuery(); execute reader also executes the query so we dont need this
					result.Load(com.ExecuteReader());
				}
				con.Close();
				return result;
			}
			catch (Exception e)
			{

				MessageBox.Show(e.ToString() + "");
			}
			return null;
		}

		public void OpenLink(string? filePath)
		{
			try
			{
				ProcessStartInfo inf = new ProcessStartInfo()
				{
					FileName = "cmd",
					Arguments = " /c start " + filePath,
					UseShellExecute = true,
					CreateNoWindow = true,
				};
				System.Diagnostics.Process.Start(inf);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		public delegate void OnLinkAdded();
		public event OnLinkAdded? OnBridgeLinkAdded;

		public void AddLink(string name, string link)
		{
			RunQuery("insert into Links (name,link) values (@name,@link)", new object[,] { { "@name", name }, { "@link", link } });
			OnBridgeLinkAdded?.Invoke();
		}

		public List<DB_BridgeLink> GetLinks(int startIndex = 0, int limit = 5, bool pre = false)
		{
			List<DB_BridgeLink> bridgeLinks = new List<DB_BridgeLink>();
			DataTable? read = RunQuery("select * from Links where id "
				+ (pre ? "<" : ">")
				+ " @start "
				+ (pre ? "order by id desc" : "")
				+ " limit @limit ",
				new object[,] { { "@start", startIndex }, { "@limit", limit } });
			if (read != null)
			{
				foreach (DataRow item in read.Rows)
				{
					bridgeLinks.Add(new DB_BridgeLink()
					{
						id = Convert.ToInt32(item["id"]),
						name = item["name"] + "",
						link = item["link"] + ""
					});
				}

				if (pre)
				{
					bridgeLinks.Reverse();
				}
			}
			return bridgeLinks;
		}


		public List<DB_BridgeLink> SearchLinks(string with)
		{
			List<DB_BridgeLink> bridgeLinks = new List<DB_BridgeLink>();
			DataTable? read = RunQuery("select * from Links where name like @with",
				new object[,] { { "@with", $"%{with}%" } });
			if (read != null)
			{
				foreach (DataRow item in read.Rows)
				{
					bridgeLinks.Add(new DB_BridgeLink()
					{
						id = Convert.ToInt32(item["id"]),
						name = item["name"] + "",
						link = item["link"] + ""
					});
				}
			}
			return bridgeLinks;
		}

		public delegate void OnLinkDeleted();
		public event OnLinkAdded? OnBridgeLinkDeleted;

		public void DeleteLink(string id)
		{
			RunQuery("delete from Links where id =@id", new object[,] { { "@id", id } });
			OnBridgeLinkDeleted?.Invoke();
		}
	}
}
