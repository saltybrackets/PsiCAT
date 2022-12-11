namespace PsiCat.SmartDevices.Tests;

using System.Diagnostics;


public class DiscoveryTests
{
    [SetUp]
    public void Setup()
    {
    }


    [Test]
    public void AdHoc()
    {
        SsdpSocket socket = new SsdpSocket("239.255.255.250", 1982);

        string discoveryMessage = string.Join(
            "\r\n",
            "M-SEARCH * HTTP/1.1",
            "HOST: 239.255.255.250:1982",
            "MAN: \"ssdp:discover\"",
            "ST: wifi_bulb");
        
        Console.WriteLine("Sending Data...");
        string response = socket.SendMulticastData(discoveryMessage);
        Console.WriteLine("Response:\r\n" + response);
        
        //Console.WriteLine("Receiving Data...");
        //string receivedData = socket.ReceiveMulticastData();
        //Console.WriteLine(receivedData);
    }
}