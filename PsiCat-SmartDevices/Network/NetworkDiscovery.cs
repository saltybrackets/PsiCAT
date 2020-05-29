namespace PsiCat.SmartDevices
{
	using System;
	using System.Net;
	using System.Net.NetworkInformation;
	using System.Net.Sockets;
	using System.Threading;


	public class NetworkDiscovery
	{
		public static string GetNetworkGateway()
		{
			string ip = null;

			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					GatewayIPAddressInformationCollection gatewayAddresses = networkInterface.GetIPProperties().GatewayAddresses;
					foreach (GatewayIPAddressInformation gatewayAddress in gatewayAddresses)
					{
						ip = gatewayAddress.Address.ToString();
					}
				}
			}

			return ip;
		}


		public static void Ping(string host, int attempts, int timeout)
		{
			for (int i = 0; i < attempts; i++)
			{
				new Thread(delegate()
					{
						try
						{
							Ping ping = new Ping();
							ping.PingCompleted += new PingCompletedEventHandler(PingCompleted);
							ping.SendAsync(host, timeout, host);
						}
						catch
						{
							// Do nothing and let it try again until the attempts are exausted.
							// Exceptions are thrown for normal ping failurs like address lookup
							// failed.  For this reason we are supressing errors.
						}
					}).Start();
			}
		}


		public static void PingAll()
		{

			string gateIP = GetNetworkGateway();
           
			//Extracting and pinging all other ip's.
			string[] array = gateIP.Split('.');

			for (int i = 2; i <= 255; i++)
			{  
                
				string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + i;   

				//time in milliseconds           
				Ping(ping_var, 4, 4000);

			}          
            
		}


		public static string GetHostName(string ipAddress)
		{
			try
			{
				IPHostEntry entry = Dns.GetHostEntry(ipAddress);
				if (entry!= null)
				{
					return entry.HostName;
				}
			}
			catch (SocketException)
			{
			}

			return null;
		}


		//Get MAC address
		public static string GetMacAddress(string ipAddress)
		{
			string macAddress = string.Empty;
			System.Diagnostics.Process Process = new System.Diagnostics.Process();
			Process.StartInfo.FileName = "arp";
			Process.StartInfo.Arguments = "-a " + ipAddress;
			Process.StartInfo.UseShellExecute = false;
			Process.StartInfo.RedirectStandardOutput = true;
			Process.StartInfo.CreateNoWindow = true;
			Process.Start();
			string strOutput = Process.StandardOutput.ReadToEnd();
			string[] substrings = strOutput.Split('-');
			if (substrings.Length >= 8)
			{
				macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
							+ "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
							+ "-" + substrings[7] + "-"
							+ substrings[8].Substring(0, 2);
				return macAddress;
			}

			else
			{
				return "OWN Machine";
			}
		}


		private static void PingCompleted(object sender, PingCompletedEventArgs e)
		{
			string ip = (string)e.UserState;
			if (e.Reply != null && e.Reply.Status == IPStatus.Success)
			{
				string hostName = GetHostName(ip);
				string macAddress = GetMacAddress(ip);
				
				var result = new PingResult()
							{
								IP = ip,
								HostName = hostName,
								MacAddress = macAddress
							};
				
				Console.Out.WriteLine($"Ping Result: {ip} - {hostName} - {macAddress}");
			}
		}


		private class PingResult
		{
			public string IP;
			public string HostName;
			public string MacAddress;
		}
	}
}