using System;
using System.Collections.Generic;
using Merq;
using Xamarin.Messaging.Integration;
using Xamarin.Messaging.Integration.CommandHandlers;
using Xamarin.Messaging.Integration.Commands;

namespace Xma.Integration.Console
{
	/// This is just a helper class that configures the required dependencies for all the XMA instances,
	/// the Command Bus command handlers registrations and also the ServerSource registrationsS
	/// In XVS we resolve the dependencies via MEF, so this is not required
	public class XmaInitializer
	{
		bool isInitialized;

		public IEventStream EventStream { get; private set; }

		public ICommandBus CommandBus { get; private set; }

		public IRemoteServerSourceManager ServerSourceManager { get; private set; }

		public void Initialize()
		{
			if (isInitialized) return;

			//Dependencies Initialization
			//In XVS this is done automatically via MEF, here we need to do it manually
			var fingerprintRetriever = new FingerprintRetriever();
			var settings = new InMemoryRemoteServerSettings();
			var dialogProvider = new TestDialogProvider();
			var progress = new Progress<string>(x => Console.WriteLine(x));
			var errorsManager = new TestErrorsManager();
			var asyncManager = new AsyncManager();

			EventStream = new EventStream();

			//The Server Source represents a type of server that you want to support connection
			//If you want to connect against a Linux Server, a Mac Server, a Windows Server, an IoT Server, etc,
			//you would need to define a Server Source for each of those types
			var serverSource = new TestServerSource(fingerprintRetriever,
				settings,
				dialogProvider,
				progress,
				errorsManager,
				asyncManager,
				EventStream);

			//Dependencies initialization for the command handlers
			//In XVS this is done automatically via MEF, here we need to do it manually
			ServerSourceManager = new RemoteServerSourceManager();

			var connectionManager = new TestServerConnectionManager(ServerSourceManager, asyncManager, EventStream);

			//Command handlers registration
			//The command handlers are needed to be registered in the Command Bus,
			//in order to let the XMA framework to invoke and handle commands and events
			//In XVS this is done automatically via MEF, here we need to do it manually
			var handlers = new List<ICommandHandler> {
				new DisconnectServerHandler (ServerSourceManager),
				new RegisterAgentsHandler (ServerSourceManager),
				new RegisterServerSourceHandler (ServerSourceManager, connectionManager),
				new SelectServerHandler (ServerSourceManager),
				new StartAgentHandler (ServerSourceManager),
				new StartConsoleHandler (ServerSourceManager)
			};

			CommandBus = new CommandBus(handlers);

			//This is how we register every server source defined
			CommandBus.Execute(new RegisterServerSource(serverSource, connectAutomatically: true));

			isInitialized = true;
		}
	}
}
