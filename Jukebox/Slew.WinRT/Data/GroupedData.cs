using System.Collections.Generic;

namespace Slew.WinRT.Data
{
	public class GroupedData<TData> : List<TData>
	{
		public string Key { get; set; }
	}
}