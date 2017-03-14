using System;
using Xamarin.Messaging.Integration;
using Xamarin.Messaging.Integration.Commands;
using System.Threading;
using System.Collections.Generic;
using Xamarin.Messaging;
using Xamarin.Messaging.Ssh;

namespace Xma.Integration.Console
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Initializing XMA...");

			var initializer = new XmaInitializer();

			initializer.Initialize();

			Console.WriteLine("Executing possible commands...");

			Console.WriteLine("1 - Showing Server Selector...");

			initializer
				.CommandBus
				.ExecuteAsync(new SelectServerAsync(RemoteServerPlatform.Mac), CancellationToken.None)
				.Wait ();

			Console.WriteLine("2 - Registering Agents...");

			initializer
				.CommandBus
				.Execute(new RegisterAgents(RemoteServerPlatform.Mac, new List<AgentInfo> { { new TestAgentInfo() } }));

			Console.WriteLine("3 - Starting Agent...");

			initializer
				.CommandBus
				.ExecuteAsync(new StartAgentAsync(RemoteServerPlatform.Mac, new TestAgentInfo()), CancellationToken.None)
				.Wait();

			Console.WriteLine("4 - Disconnecting Server...");

			initializer
				.CommandBus
				.ExecuteAsync(new DisconnectServerAsync(RemoteServerPlatform.Mac), CancellationToken.None)
				.Wait();

			Console.WriteLine("5 - Sample of how to listen for a RemoteServer connection and how to get an existing connection...");

			var remoteServerProvider = new RemoteServerProvider(initializer.EventStream);
			var remoteServer = remoteServerProvider.GetServer(RemoteServerPlatform.Mac);
			var messagingService = remoteServer?.MessagingService;

			if (remoteServer == null)
			{
				Console.WriteLine("There is no active XMA connection yet");
			}

			remoteServerProvider.ServerConnected += (sender, e) =>
			{
				Console.WriteLine("A new XMA server has been connected");

				remoteServer = e;
				messagingService = remoteServer.MessagingService;
			};

			Console.ReadKey();
		}
	}
}
