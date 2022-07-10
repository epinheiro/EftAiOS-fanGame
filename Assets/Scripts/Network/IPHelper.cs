using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;

public static class IPHelper
{
    public static bool IsIPValid(string testIP)
    {
        // https://docs.microsoft.com/pt-br/dotnet/api/system.text.regularexpressions.regex?view=netframework-4.8
        // https://www.regular-expressions.info/numericranges.html
        string pattern = @"\b([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\b";
        Regex rgx = new Regex(pattern);

        return rgx.IsMatch(testIP);
    }

    // Based on the Stackoverflow answer https://stackoverflow.com/a/6803109
    public static string GetLocalIPAddress()
    {
        // https://docs.microsoft.com/pt-br/dotnet/api/system.text.regularexpressions.regex?view=netframework-4.8
        string pattern = @"^\d+\.\d+\.\d+\.\d+";
        Regex rgx = new Regex(pattern);

        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                string testedIp = ip.ToString();
                if(rgx.IsMatch(testedIp)){
                    return ip.ToString(); 
                }                
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    // Based on the Stackoverflow answer https://stackoverflow.com/a/6803109
    public static string GetExternalIPAddress()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
