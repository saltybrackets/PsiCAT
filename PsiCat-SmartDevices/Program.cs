namespace PsiCat.SmartDevices
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
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
			/*
			//YeelightAPI.Device device = new Device(IP);
			await deviceGroup.Connect();
			await deviceGroup.SetRGBColor(255, 255, 255);
			await deviceGroup.SetBrightness(100);
			
			
			ConsoleKeyInfo input = new ConsoleKeyInfo();

			do
			{
				input = Console.ReadKey(true);

				
				
				switch (input.Key)
				{
					case ConsoleKey.UpArrow:
						ColorFlow flow = new ColorFlow(0, ColorFlowEndAction.Restore);
						flow.Add(new ColorFlowRGBExpression(255, 0, 0, 1, 500));
						flow.Add(new ColorFlowRGBExpression(0, 255, 0, 1, 500));
						Console.Out.WriteLine("BLINK");
						await deviceGroup.StartColorFlow(flow);
						break;
					case ConsoleKey.DownArrow:
						await deviceGroup.StopColorFlow();
						Console.Out.WriteLine("STOP");
						break;
				}
				
				
				Thread.Sleep(40);
			}
			while (input.Key != ConsoleKey.Escape);
			*/
		}
	}
}