using System;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Messaging.Integration;
using Xamarin.Messaging.Integration.Models;

namespace Xma.Integration.Wpf
{
	//This class needs to be implemented in order to provide the dialogs required by XMA
	//Each method receives a model injected that has all the properties and methods that should be used
	//It's strongly recommended to use ViewModels that receives the Models and only calls the methods and properties
	//The Model properties implements INotifyPropertyChanged, so it should be very straightforward to implement the ViewModels
	//Find a sample of how we do in XVS with the ViewModels: 
	//LoginViewModel: https://github.com/mauroa/XamarinVS/blob/projectsystems/src/Core/Xamarin.Messaging.Windows/ViewModels/LoginViewModel.cs
	//Base ViewModel: https://github.com/mauroa/XamarinVS/blob/projectsystems/src/Core/Xamarin.Messaging.Windows/ViewModels/ViewModel.cs
	public class TestDialogProvider : IServerDialogProvider
	{
		public Task ShowServerSelectorDialogAsync (ServerSelectorModel model)
		{
			MessageBox.Show ("This is the Server Selector dialog!");

			return Task.FromResult (true);
		}

		public Task ShowServerSelectorInstructionsDialogAsync (ServerSelectorModel model)
		{
			MessageBox.Show ("This is the Server Selector Instructions dialog!");

			return Task.FromResult (true);
		}

		public Task<LoginResult> ShowLoginDialogAsync (LoginModel model)
		{
			var result = MessageBox.Show ("This is the Login Dialog!");

			if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes) {
				return Task.FromResult (new LoginResult { Status = LoginStatus.Succeeded });
			} else {
				return Task.FromResult (new LoginResult { Status = LoginStatus.Failed, ErrorMessage = "Login Failed" });
			}
		}

		public Task<bool> ShowFingerprintMissmatchDialogAsync (FingerprintMissmatchModel model)
		{
			var result = MessageBox.Show ("This is the Fingerprint Missmatch Dialog!");

			return Task.FromResult (result == MessageBoxResult.OK || result == MessageBoxResult.Yes);
		}

		public Task<string> ShowManualServerDialogAsync (AddNewServerModel model)
		{
			MessageBox.Show ("This is the Manual Server dialog!");

			return Task.FromResult ("192.168.1.180");
		}

		public Task<bool> ShowForgetServerDialogAsync (ForgetServerModel model)
		{
			var result = MessageBox.Show ("This is the Forget Server Dialog!");

			return Task.FromResult (result == MessageBoxResult.OK || result == MessageBoxResult.Yes);
		}

		public void ShowMessageDialog (string text, string caption, MessageDialogType type)
		{
			MessageBox.Show (text, caption, MessageBoxButton.OK);
		}

		public Task<IProgress<string>> GetProgressDialogAsync (string message, string caption = "", bool allowCancel = true, int delaySeconds = 0, bool showProgress = false)
		{
			return Task.FromResult<IProgress<string>> (new Progress<string> (x => Console.WriteLine (x)));
		}
	}
}
