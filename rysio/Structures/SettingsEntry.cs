using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rysio
{
	[Serializable]
	public class SettingsEntry
	{
		public School School
		{ 
			get;
			set;
		}

		public string Login
		{
			get;
			set;
		}

		public byte[] Password
		{ get;
			set;
		}
	}
}
