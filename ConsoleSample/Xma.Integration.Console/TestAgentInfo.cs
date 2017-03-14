using System;
using Xamarin.Messaging;

namespace Xma.Integration.Console
{
	//Sample Agent Info to simulate an Agent registration
	public class TestAgentInfo : AgentInfo
	{
		public TestAgentInfo() : base ("Test", "1.0.0")
		{
		}
	}
}
