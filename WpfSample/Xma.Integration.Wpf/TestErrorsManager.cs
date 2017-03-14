using System;
using System.Collections.Generic;
using Xamarin.Messaging.Integration;

namespace Xma.Integration.Wpf
{
	//These classes are meant to be used to write Errors and Warnings on the corresponding IDE Error List
	public class TestErrorsManager : IErrorsManager
	{
		IList<IErrorItem> errors;
		IList<IErrorItem> warnings;

		public TestErrorsManager ()
		{
			errors = new List<IErrorItem> ();
			warnings = new List<IErrorItem> ();
		}

		public IErrorItem AddError (string text, Action<IErrorItem> handler, Exception ex = null)
		{
			var item = new TestErrorItem { Text = text, Exception = ex };

			errors.Add (item);
			handler (item);

			return item;
		}

		public IErrorItem AddWarning (string text, Action<IErrorItem> handler, Exception ex = null)
		{
			var item = new TestErrorItem { Text = text, Exception = ex };

			warnings.Add (item);
			handler (item);

			return item;
		}

		public void ClearErrors ()
		{
			errors.Clear ();
			warnings.Clear ();
		}

		public void ShowErrors () { }
	}

	public class TestErrorItem : IErrorItem
	{
		public string Text { get; set; }

		public Exception Exception { get; set; }

		public void Remove () { }
	}
}
