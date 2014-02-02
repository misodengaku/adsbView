using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ADSB
{
	public class ADSBStream
	{
		TcpClient client;
		Stream tcpStream;
		bool IsOpened;
		//byte[] dataBuffer = new byte[1024];
		List<byte> dataBuffer = new List<byte>();

		public ADSBStream(string host, int port)
		{
			client = new TcpClient(host, port);
			tcpStream = client.GetStream();
			IsOpened = true;
		}

		/// <summary>
		/// Beast バイナリーフォーマットの1パケット分を読み出します。
		/// </summary>
		/// <returns></returns>
		public bool GetByteLine(byte[] retData)
		{
			if (dataBuffer.Count == 0)
			{
				while (tcpStream.ReadByte() != 0x1a) ; //頭出し
				dataBuffer.Add(0x1a);
			}

			while (client.Available > 1)
			{
				var data = (byte)tcpStream.ReadByte();
				if (data == 0x1a)
				{
					dataBuffer.CopyTo(0, retData, 0, dataBuffer.Count - 1);
					dataBuffer.Clear();
					dataBuffer.Add(0x1a);
					return true;
				}
				dataBuffer.Add(data);
			}
			return false;

		}
	}
}
