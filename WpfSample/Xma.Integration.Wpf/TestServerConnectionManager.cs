using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Merq;
using Xamarin.Messaging.Integration;
using Xamarin.Messaging.Integration.Events;

namespace Xma.Integration.Wpf
{
	//This class is meant to handle automatic connections and disconnection
	//based on operating system events like system shutdown, user session disconnection, user session switch, etc.
	//It also relies on listening solution events like solution closed, solution created, startup projects changing,
	//projects creation, etc
	//All the system and solution events code has been commented given that are Windows and VS only
	//Please replace them with the corresponding IDE and OS implementations
	public class TestServerConnectionManager : IRemoteServerConnectionManager
	{
		readonly IRemoteServerSourceManager serverSourceManager;
		//readonly ISystemEvents systemEvents;
		//readonly ISolutionState solutionState;
		readonly IAsyncManager asyncManager;
		readonly IEventStream eventStream;
		readonly ConcurrentDictionary<RemoteServerPlatform, bool> connectionTypes;

		public TestServerConnectionManager (
			IRemoteServerSourceManager serverSourceManager,
			IAsyncManager asyncManager,
			IEventStream eventStream)
		{
			this.serverSourceManager = serverSourceManager;
			//this.systemEvents = systemEvents;
			//this.solutionState = solutionState;
			this.asyncManager = asyncManager;
			this.eventStream = eventStream;

			connectionTypes = new ConcurrentDictionary<RemoteServerPlatform, bool> ();

			//systemEvents.PowerModeChanged += OnPowerChange;
			//systemEvents.SessionSwitch += OnSessionSwitch;
			//systemEvents.SessionEnded += OnSessionEnded;

			//this.solutionState.SolutionReady += async (sender, args) => {
			//	await CheckServerSourcesAsync ().ConfigureAwait (continueOnCapturedContext: false);
			//};

			//this.solutionState.SolutionClosed += async (sender, args) => {
			//	await DisconnectAsync (unregisterServerSource: true).ConfigureAwait (continueOnCapturedContext: false);
			//};

			eventStream
				.Of<ProjectChanged> ()
				.Subscribe (async ev => {
					if (ev.HasProjects) {
						await CheckServerSourceAsync (ev.Platform)
							.ConfigureAwait (continueOnCapturedContext: false);
					}
				});

			//eventStream
			//	.Of<StartupProjectsChanged> ()
			//	.Subscribe (async ev => {
			//		foreach (var project in ev.StartupProjects) {
			//			await TryConnectAsync (project.GetPlatform ())
			//				.ConfigureAwait (continueOnCapturedContext: false);
			//		}
			//	});
		}

		public void Configure (RemoteServerPlatform platform, bool connectAutomatically)
		{
			var currentConnectionType = default (bool);

			if (connectionTypes.TryGetValue (platform, out currentConnectionType)) {
				if (currentConnectionType == false) {
					connectionTypes.TryUpdate (platform, connectAutomatically, currentConnectionType);
				}
			} else {
				connectionTypes.TryAdd (platform, connectAutomatically);
			}
		}

		//protected virtual async void OnPowerChange (object sender, PowerModeChangedEventArgs e)
		//{
		//	switch (e.Mode) {
		//		case PowerModes.Resume:
		//			await TryConnectAsync ().ConfigureAwait (continueOnCapturedContext: false);
		//			break;
		//		case PowerModes.Suspend:
		//			await DisconnectAsync ().ConfigureAwait (continueOnCapturedContext: false);
		//			break;
		//	}
		//}

		//protected virtual async void OnSessionSwitch (object sender, SessionSwitchEventArgs e)
		//{
		//	switch (e.Reason) {
		//		case SessionSwitchReason.SessionLogoff:
		//		case SessionSwitchReason.SessionLock:
		//			await DisconnectAsync ().ConfigureAwait (continueOnCapturedContext: false);
		//			break;
		//		case SessionSwitchReason.SessionUnlock:
		//		case SessionSwitchReason.SessionLogon:
		//			await TryConnectAsync ().ConfigureAwait (continueOnCapturedContext: false);
		//			break;
		//	}
		//}

		//protected virtual async void OnSessionEnded (object sender, SessionEndedEventArgs e)
		//{
		//	switch (e.Reason) {
		//		case SessionEndReasons.Logoff:
		//		case SessionEndReasons.SystemShutdown:
		//			await DisconnectAsync ().ConfigureAwait (continueOnCapturedContext: false);
		//			break;
		//	}
		//}

		async Task CheckServerSourcesAsync ()
		{
			var platforms = serverSourceManager.Platforms;

			foreach (var platform in platforms) {
				await CheckServerSourceAsync (platform)
					.ConfigureAwait (continueOnCapturedContext: false);
			}
		}

		async Task CheckServerSourceAsync (RemoteServerPlatform platform)
		{
			try {
				await asyncManager.SwitchToBackground ();

				if (platform == RemoteServerPlatform.Unknown) {
					return;
				}

				//if (!solutionState.ContainProjects (platform)) {
				//	return;
				//}

				var isActive = false;
				var source = serverSourceManager.GetSource (platform, out isActive);

				if (!isActive) {
					serverSourceManager.AddSource (source);
				}
			} catch (Exception ex) {
				//Add tracing
			}
		}

		async Task TryConnectAsync ()
		{
			var platforms = serverSourceManager.Platforms;

			foreach (var platform in platforms) {
				await TryConnectAsync (platform)
					.ConfigureAwait (continueOnCapturedContext: false);
			}
		}

		async Task TryConnectAsync (RemoteServerPlatform platform)
		{
			try {
				await asyncManager.SwitchToBackground ();

				if (platform == RemoteServerPlatform.Unknown) {
					return;
				}

				//if (!solutionState.ContainProjects (platform)) {
				//	return;
				//}

				await CheckServerSourceAsync (platform)
					.ConfigureAwait (continueOnCapturedContext: false);

				var source = serverSourceManager
					.ActiveSources
					.FirstOrDefault (s => s.Platform == platform);
				var server = source.GetServer ();
				var connectAutomatically = default (bool);

				connectionTypes.TryGetValue (server.Platform, out connectAutomatically);

				if (!connectAutomatically) {
					return;
				}

				if (server.IsConnected) {
					await server
						.StartAgentsAsync ()
						.ConfigureAwait (continueOnCapturedContext: false);
				} else {
					await server
						.TryConnectAsync ()
						.ConfigureAwait (continueOnCapturedContext: false);
				}
			} catch (Exception ex) {
				//Add tracing
			}
		}

		async Task DisconnectAsync (bool unregisterServerSource = false)
		{
			var platforms = serverSourceManager
				.ActiveSources
				.Select (s => s.Platform);

			foreach (var platform in platforms) {
				await DisconnectAsync (platform)
					.ConfigureAwait (continueOnCapturedContext: false);
			}
		}

		async Task DisconnectAsync (RemoteServerPlatform platform, bool unregisterServerSource = false)
		{
			try {
				await asyncManager.SwitchToBackground ();

				if (platform == RemoteServerPlatform.Unknown) {
					return;
				}

				var isActive = false;
				var source = serverSourceManager.GetSource (platform, out isActive);

				if (!isActive) {
					return;
				}

				if (unregisterServerSource) {
					serverSourceManager.RemoveSource (platform);
				}

				var server = source.GetServer ();

				await server
					.DisconnectAsync ()
					.ConfigureAwait (continueOnCapturedContext: false);
			} catch (Exception ex) {
				//Add tracing
			}
		}
	}
}