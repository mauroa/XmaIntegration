using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Merq;
using Microsoft.VisualStudio.Threading;

namespace Xma.Integration.Console
{
	//Implementation of Merq.IAsyncManager, according to recommended best practices for IDE development
	//The Microsoft.VisualStudio.Threading package uses JoinableTaskFactory, which has a deadlock free implementation
	//to perform async and sync operations on the UI thread without deadlock risks
	//The package can be used in any type of library
	//On XVS we have a default implementation, but here we need to implement it by hand
	public class AsyncManager : IAsyncManager
	{
		readonly JoinableTaskContext context;

		public AsyncManager()
		{
			context = new JoinableTaskContext();
		}

		public void Run(Func<Task> asyncMethod)
		{
			context.Factory.Run(asyncMethod);
		}

		public TResult Run<TResult>(Func<Task<TResult>> asyncMethod)
		{
			return context.Factory.Run(asyncMethod);
		}

		public IAwaitable RunAsync(Func<Task> asyncMethod)
		{
			return new JoinableTaskAwaitable(context.Factory.RunAsync(asyncMethod));
		}

		public IAwaitable<TResult> RunAsync<TResult>(Func<Task<TResult>> asyncMethod)
		{
			return new JoinableTaskAwaitable<TResult>(context.Factory.RunAsync(asyncMethod));
		}

		public IAwaitable SwitchToBackground()
		{
			return new TaskSchedulerAwaitable(TaskScheduler.Default);
		}

		public IAwaitable SwitchToMainThread()
		{
			return new MainThreadAwaitable(context.Factory.SwitchToMainThreadAsync());
		}
	}

	public class JoinableTaskAwaitable<T> : IAwaitable<T>
	{
		readonly JoinableTask<T> task;

		public JoinableTaskAwaitable(JoinableTask<T> task)
		{
			this.task = task;
		}

		public IAwaiter<T> GetAwaiter() => new JoinableTaskAwaiter<T>(task.GetAwaiter());
	}

	public class JoinableTaskAwaitable : IAwaitable
	{
		readonly JoinableTask task;

		public JoinableTaskAwaitable(JoinableTask task)
		{
			this.task = task;
		}

		public IAwaiter GetAwaiter() => new JoinableTaskAwaiter(task.GetAwaiter());
	}

	public class JoinableTaskAwaiter<T> : IAwaiter<T>
	{
		readonly TaskAwaiter<T> awaiter;

		public JoinableTaskAwaiter(TaskAwaiter<T> awaiter)
		{
			this.awaiter = awaiter;
		}

		public bool IsCompleted => awaiter.IsCompleted;

		public T GetResult() => awaiter.GetResult();

		public void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);
	}

	public class JoinableTaskAwaiter : IAwaiter
	{
		readonly TaskAwaiter awaiter;

		public JoinableTaskAwaiter(TaskAwaiter awaiter)
		{
			this.awaiter = awaiter;
		}

		public bool IsCompleted => awaiter.IsCompleted;

		public void GetResult() => awaiter.GetResult();

		public void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);
	}

	public class MainThreadAwaitable : IAwaitable
	{
		readonly JoinableTaskFactory.MainThreadAwaitable awaitable;

		public MainThreadAwaitable(JoinableTaskFactory.MainThreadAwaitable awaitable)
		{
			this.awaitable = awaitable;
		}

		public IAwaiter GetAwaiter() => new MainThreadAwaiter(awaitable.GetAwaiter());
	}

	public class TaskSchedulerAwaitable : IAwaitable
	{
		readonly TaskScheduler scheduler;

		public TaskSchedulerAwaitable(TaskScheduler scheduler)
		{
			this.scheduler = scheduler;
		}

		public IAwaiter GetAwaiter() => scheduler.GetAwaiter();
	}

	public class MainThreadAwaiter : IAwaiter
	{
		JoinableTaskFactory.MainThreadAwaiter awaiter;

		public MainThreadAwaiter(JoinableTaskFactory.MainThreadAwaiter awaiter)
		{
			this.awaiter = awaiter;
		}

		public bool IsCompleted => awaiter.IsCompleted;

		public void GetResult() => awaiter.GetResult();

		public void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);
	}
}
