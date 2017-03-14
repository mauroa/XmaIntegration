using System.Collections.Generic;
using System.Linq;
using Xamarin.Messaging.Integration;
using Xamarin.Messaging.Integration.Models;

namespace Xma.Integration.Console
{
	//Use a better storage mechanism for the Server Settings. In XVS we use Windows Registry
	public class InMemoryRemoteServerSettings : IRemoteServerSettings
	{
		IList<KnownServer> knownServers;

		public InMemoryRemoteServerSettings()
		{
			knownServers = new List<KnownServer> {
				new KnownServer {
					BuildServer = "192.168.1.111",
					Ip = "192.168.1.111",
					Fingerprint = "24-86-6B-AF-14-14-5A-38-78-5B-E4-0F-AF-53-57-94",
					Platform = RemoteServerPlatform.Mac,
					Username = "test"
				}
			};

			HostName = "Test-MacBook-Pro.local";
			Username = "test";
			SkipInstructions = true;
			SkipForgetConfirmation = false;
			SkipAutoConnectionUI = true;
			UseRemoteSimulator = false;
		}

		public string HostName { get; set; }

		public string Username { get; set; }

		public bool SkipInstructions { get; set; }

		public bool SkipForgetConfirmation { get; set; }

		public bool SkipAutoConnectionUI { get; set; }

		public bool UseRemoteSimulator { get; set; }

		public int DefaultActivationTimeoutSeconds => 60;

		public IEnumerable<KnownServer> KnownServers => knownServers;

		public void AddAddress(ServerData server)
		{
			knownServers.Add(new KnownServer
			{
				BuildServer = server.IpAddress.ToString(),
				Ip = server.IpAddress.ToString(),
				OldIp = server.OlderKnownIp,
				Port = server.Port,
				Username = server.Username,
				Fingerprint = server.Fingerprint,
				Platform = server.Platform,
			});
		}

		public void RemoveAddress(string name)
		{
			var serverToRemove = knownServers.FirstOrDefault(s => s.BuildServer == name);

			knownServers.Remove(serverToRemove);
		}
	}
}
