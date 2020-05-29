namespace PsiCat.SmartDevices
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Timers;
	using PsiCat.SmartDevices;
	using YeelightAPI;
	using YeelightAPI.Models;
	using YeelightAPI.Models.ColorFlow;


	internal class Program
	{
		private const string IP = "192.168.0.17";


		private static async Task Main(string[] args)
		{
			NetworkDiscovery.PingAll();
			/*
			var lights = await DeviceLocator.Discover();

			if (lights.Count < 1)
			{
				Console.Out.WriteLine("No lights found.");
			}
			
			foreach (var light in lights)
			{
				Console.Out.WriteLine($"LIGHT: {light.Name}");	
			}
			
			
			YeelightAPI.Device device = new Device(IP);
			await device.Connect();
			await device.SetRGBColor(255, 255, 255);
			await device.SetBrightness(100);
			*/
			ConsoleKeyInfo input = new ConsoleKeyInfo();

			do
			{
				input = Console.ReadKey(true);

				
				/*
				switch (input.Key)
				{
					case ConsoleKey.UpArrow:
						ColorFlow flow = new ColorFlow(0, ColorFlowEndAction.Restore);
						flow.Add(new ColorFlowRGBExpression(255, 0, 0, 1, 500));
						flow.Add(new ColorFlowRGBExpression(0, 255, 0, 1, 500));
						await device.StartColorFlow(flow);
						break;
					case ConsoleKey.DownArrow:
						await device.StopColorFlow();
						break;
				}
				*/
				
				Thread.Sleep(40);
			}
			while (input.Key != ConsoleKey.Escape);
		}
	}
}