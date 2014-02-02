using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSB
{
	public enum DataTypes{
		AC = '1', S_Short = '2', S_Long = '3'
	}

	public enum DownLinkTypes{
		DF0 = 0, //	Fig. 2	Short Air to Air ACAS
		DF4 = 4, //	Bild 2	Surveillance (roll call) Altitude
		DF5 = 5, //	Bild 2	Surveillance (roll call) IDENT Reply
		DF11 = 11, //	Fig. 2	Mode S Only All-Call Reply (Acq. Squitter if II=0)
		DF16 = 16, //	Fig. 3	Long Air to Air ACAS
		DF17 = 17, //	Fig. 3	1090 Extended Squitter
		DF19 = 19, //	 	Military Extended Squitter
		DF20 = 20, //
		DF21 = 21, //	Fig. 3	Comm. B Altitude, IDENT Reply
		DF22 = 22, //	 	Military use only
		DF24 = 24 //	Fig. 4	Comm. D Extended Length Message (ELM)
	}

	public class Data
	{
		public DataTypes DataType { get; set; }
		public byte[] MLAT { get; set; }
		public byte SignalLevel { get; set; }
		public byte[] DataFrame { get; set; }

		public Data(byte[] data)
		{
			MLAT = new byte[6];
			if (data[0] != 0x1a)
				throw new FormatException("データフォーマットが不正です。");
			DataType = (DataTypes)data[1];
			for (int i = 0; i < 6; i++)
				MLAT[i] = data[i + 3];
			SignalLevel = data[8];
			switch (DataType)
			{
				case DataTypes.AC:
					DataFrame = new byte[2];
					for (int i = 0; i < 2; i++)
						DataFrame[i] = data[i + 9];
					break;
				case DataTypes.S_Long:
					DataFrame = new byte[14];
					for (int i = 0; i < 14; i++)
						DataFrame[i] = data[i + 9];
					break;
				case DataTypes.S_Short:
					DataFrame = new byte[7];
					for (int i = 0; i < 7; i++)
						DataFrame[i] = data[i + 9];
					break;
				default:
					throw new FormatException("タイプ指定が不正です。");
			}
		}

	}

	public class ModeS
	{
		public DownLinkTypes DownLinkType { get; set; }
		public byte[] ICAOCode { get; set; }
		public string ICAOCodeString { get; set; }
		public double Altitude { get; set; }

		public ModeS(byte[] data)
		{
			ICAOCode = new byte[3];
			ICAOCodeString = "";
			DownLinkType = (DownLinkTypes)((data[0] >> 3)); // first 2 bits C0
			for (int i = 0; i < 3; i++)
			{
				ICAOCode[i] = data[i + 1];
				ICAOCodeString += data[i + 1].ToString("X2");
				Altitude = ((int)data[1] << 4 | (int)data[2] >> 4) * 0.3048;
			}
		}
	}

	/*
	public class ModeAC
	{
		public DataType _DataType { get; }
		public byte[] MLAT { get; }
		public byte SignalLevel { get; }
		public byte[] ACData { get; }

		public ModeAC(byte[] data)
		{

		}
	}

	public class ModeS
	{
		public DataType _DataType { get; }
		public byte[] MLAT { get; }
		public byte SignalLevel { get; }
		public byte[] ModeS_Frame { get; }

		public ModeS(byte[] data)
		{

		}
	}
	 * */
}
