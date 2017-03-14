using System;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Messaging.Integration;
using Xamarin.Messaging.Integration.Models;

namespace Xma.Integration.Console
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
		public Task ShowServerSelectorDialogAsync(ServerSelectorModel model)
		{
			Console.WriteLine("XMA: Server Selector dialog shown!");

			return Task.FromResult(true);
		}

		public Task ShowServerSelectorInstructionsDialogAsync(ServerSelectorModel model)
		{
			Console.WriteLine("XMA: Server Selector Instructions dialog shown!");

			return Task.FromResult(true);
		}

		public Task<LoginResult> ShowLoginDialogAsync(LoginModel model)
		{
			Console.WriteLine("XMA: Login dialog shown!");

			return Task.FromResult(new LoginResult { Status = LoginStatus.Succeeded });
		}

		public Task<bool> ShowFingerprintMissmatchDialogAsync(FingerprintMissmatchModel model)
		{
			Console.WriteLine("XMA: Fingerprint Missmatch dialog shown!");

			return Task.FromResult(true);
		}

		public Task<string> ShowManualServerDialogAsync(AddNewServerModel model)
		{
			Console.WriteLine("XMA: Manual Server dialog shown!");

			return Task.FromResult("192.168.1.180");
		}

		public Task<bool> ShowForgetServerDialogAsync(ForgetServerModel model)
		{
			Console.WriteLine("XMA: Forget Server dialog shown!");

			return Task.FromResult(true);
		}

		public void ShowMessageDialog(string text, string caption, MessageDialogType type)
		{
			Console.WriteLine(string.Format("XMA: Message dialog shown: Text: {0}, Caption: {1}", text, caption));
		}

		public Task<IProgress<string>> GetProgressDialogAsync(string message, string caption = "", bool allowCancel = true, int delaySeconds = 0, bool showProgress = false)
		{
			return Task.FromResult<IProgress<string>>(new Progress<string>(x => Console.WriteLine(x)));
		}
	}
}
