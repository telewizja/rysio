using rysio.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rysio
{
	public class ProgramData
	{
		public School CurrSchool { get; set; }

		public SettingsEntry PwSettings { get; set; }

		public SettingsEntry WumSettings { get; set; }

		public List<HistoryItem> QueriesHistory { get; set; }
	}
}
