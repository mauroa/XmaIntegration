using System;
using System.Collections.Generic;
using System.Net;
using Xamarin.Messaging.Integration;

namespace Xma.Integration.Console
{
	//This class should be implemented to give XMA the ability to discover remote hosts
	//It's recommended to use a DiscoveryService implementation per type of server (Mac, IoT, etc)
	//A base class can be used to centralize all the shared logic and provide serve type filtering
	//on the concrete implementations (MacDiscoveryService, IoTDiscoveryService, etc)
	public class TestDiscoveryService : IServerDiscoveryService
	{
		IList<ServerDiscoveryInfo> servers;

		public TestDiscoveryService()
		{
			servers = new List<ServerDiscoveryInfo>();
		}

		public event EventHandler<ServerDiscoveryInfo> ServiceFound;

		public event EventHandler<ServerDiscoveryInfo> ServiceLost;

		public bool IsStarted { get; private set; }

		public void Browse()
		{
			if (servers.Count > 0) return;

			var fooServer = new ServerDiscoveryInfo(Dns.GetHostEntry(IPAddress.Loopback), "Foo", "Test");

			servers.Add(fooServer);
			ServiceFound?.Invoke(this, fooServer);

			var barServer = new ServerDiscoveryInfo(Dns.GetHostEntry(Dns.GetHostName()), "Bar", "Test");

			servers.Add(barServer);
			ServiceFound?.Invoke(this, barServer);
		}

		public bool Start() => IsStarted = true;

		public void Stop() => IsStarted = false;
	}
}
