using System.Threading;
using System.Windows;
using Xamarin.Messaging.Integration;
using Xamarin.Messaging.Integration.Commands;

namespace Xma.Integration.Wpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		XmaInitializer initializer;

		public MainWindow ()
		{
			InitializeComponent ();

			initializer = new XmaInitializer ();
			initializer.Initialize ();
		}

		async void OnConnectClick (object sender, RoutedEventArgs e)
		{
			await initializer.CommandBus.ExecuteAsync (new SelectServerAsync (RemoteServerPlatform.Mac), CancellationToken.None);
		}
	}
}
