using System;
using Merq;
using Xamarin.Messaging.Integration;

namespace Xma.Integration.Console
{
	//This is the class that defines a connection source
	//Each source represents a type of remote host that XMA will support
	//For each type of host, a ServerSource must be implemented and registered (Mac, IoT, Linux, Windows, etc)
	public class TestServerSource : RemoteServerSource
	{
		public TestServerSource(IFingerprintRetriever fingerprintRetriever,
			IRemoteServerSettings remoteSettings,
			IServerDialogProvider dialogProvider,
			IProgress<string> progress,
			IErrorsManager errorsManager,
			IAsyncManager asyncManager,
			IEventStream eventStream)
			: base(fingerprintRetriever, remoteSettings, dialogProvider, progress, errorsManager, asyncManager, eventStream)
		{
		}

		public override RemoteServerPlatform Platform => RemoteServerPlatform.Mac;

		public override RemoteServerType Type => RemoteServerType.Server;

		//This override is not mandatory
		public override IActivationContext GetActivationContext() => new TestActivationContext();

		//This override is not mandatory
		public override IServerDiscoveryService GetDiscoveryService() => new TestDiscoveryService();
	}
}
