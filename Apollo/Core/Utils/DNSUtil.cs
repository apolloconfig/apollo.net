using System;
using System.Collections.Generic;
using System.Net;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class DnsUtil
	{
		public static List<string> Resolve (string domainName)
		{
			var result = new List<string> ();

			var addresses = Dns.GetHostAddresses (domainName);
			foreach (var a in addresses) {
				result.Add (a.ToString ());
			}

			return result;
		}
	}
}

