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

namespace adsbView_WPF
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Task.Factory.StartNew(() =>
			{
				var bs = new BeastData("localhost", 31001);
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
						this.Dispatcher.BeginInvoke(new Action(() =>
						{
							adsbPacketBox.Items.Add(line);
						}));
					}
					Thread.Sleep(100);
				}
			});


		}

	}
}
