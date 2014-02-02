using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ADSB;
using System.Diagnostics;
using System.Collections;

namespace adsbView_WPF
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		Hashtable planeTable = new Hashtable();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Task.Factory.StartNew(() =>
			{
				var bs = new ADSBStream("192.168.11.9", 31001);
				while (true) {
					var data = new byte[24];
					if (bs.GetByteLine(data)){
						string line = "";
						foreach (var x in data)
						{
							if (x == 0)
								break;
							line += x.ToString("x2") + " ";
						}
						if (line != "")
						{
							this.Dispatcher.BeginInvoke(new Action(() =>
							{
								//adsbPacketBox.Items.Add(line);
							}));
						}
						try
						{
							var d = new Data(data);
							var str = "" + d.DataType;
							ModeS s;
							if (d.DataType == DataTypes.S_Long || d.DataType == DataTypes.S_Short)
							{
								s = new ModeS(d.DataFrame);
								str = "" + d.DataType + "\t" + s.DownLinkType.ToString() + "\tICAO:" + s.ICAOCodeString + "\tAlt:" + s.Altitude;
								planeTable[s.ICAOCodeString] = s;
							}

							try
							{
								
								this.Dispatcher.BeginInvoke(new Action(() =>
								{
									adsbPacketBox.Items.Add(str);
									planeBox.Items.Clear();
									lock (planeTable)
									{
										foreach (ModeS i in planeTable.Values)
										{
											if (i.Altitude > 1)
												planeBox.Items.Add(i.ICAOCodeString + "\t" + i.Altitude);
										}
									}
									//planeBox.ItemsSource = planeTable;
								}));
							}
							catch
							{

							}
							//MessageBox.Show(str);
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Broken packet " + ex.Message);
						}
					}
					Thread.Sleep(10);
				}
			});


		}

	}
}
