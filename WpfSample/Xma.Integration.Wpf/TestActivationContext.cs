using System;
using Xamarin.Messaging.Integration;

namespace Xma.Integration.Wpf
{
	//Only implement IActivationContext if Activation flow makes sense in the IDE 
	public class TestActivationContext : IActivationContext
	{
		public event EventHandler Deauthorized;

		public bool IsAuthorized () => true;
	}
}
