using System.Text;
using ST = System.Threading;
using SCG = System.Collections.Generic;

using Godot;


// Can't lock the access to the variables here or the game will slowdown.
// So a LinkedList was used to handle the requests.
// The finished requests will be removed from the list when the LinkedList.Count
// is bigger that 3, that will make sure both threads will not change the same data
// at the same time and cause a crash.
public class TaskRunner : Node
{
	public void Put(Node requester, string callbackMethod)
	{
		object[] data = new object[3];
		data[REQUEST_FINISHED] = false;
		data[REQUESTER] = requester;
		data[CALLBACK_METHOD] = callbackMethod;
		requestDataList.AddLast(data);
	}

	public void Clear()
	{
		lock(requestDataList)
		{
			requestDataList.Clear();
		}
	}
	
	public void ProcessRequests()
	{
		while(!IsQueuedForDeletion() && IsInsideTree())
		{
			manualResetEvent.WaitOne(ST.Timeout.Infinite);
			object[] data = null;
			Node r = null;
			string callbackMethod = null;

			try
			{
				if(requestDataList.Count > 0)
				{
					data = GetFirstValidRequest();

					if(data != null)
					{
						r = data[REQUESTER] as Node;
						callbackMethod = data[CALLBACK_METHOD] as string;
						r.Call(callbackMethod);
					}
				}
			}
			catch(System.Exception e)
			{
				GD.PushWarning(GetErrorMessage(r, callbackMethod, e));
			}
		}
	}

	private object[] GetFirstValidRequest()
	{
		if(requestDataList.Count > 0)
		{
			SCG.LinkedListNode<object[]> lln = requestDataList.First;
			object[] data;

			while(lln != null)
			{			
				data = lln.Value;

				if(data != null && !((bool)data[REQUEST_FINISHED]) &&
					data[REQUESTER] != null && data[CALLBACK_METHOD] != null)
				{
					data[REQUEST_FINISHED] = true;
					return data;
				}
				else
					lln = lln.Next;
			}
		}
		
		return null;
	}

	private void RemoveFirstProcessedRequest()
	{
		if(requestDataList.Count > 3)
		{
			object[] data = requestDataList.First.Value;

			if((bool)data[REQUEST_FINISHED])
				requestDataList.RemoveFirst();
		}
	}

	private string GetErrorMessage(Node requester, string callbackMethod,
			System.Exception exception)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("An error happened when trying to run the task:\n");

		try
		{
			if(requester != null)
				sb.Append("Object: ").Append(requester.Name).Append("\n");

			if(callbackMethod != null)
				sb.Append("Method: ").Append(callbackMethod).Append("\n");
		}
		catch(System.Exception e)
		{
			sb.Append("Error trying to get where the task failed!\n");
		}

		sb.Append(exception.Message).Append('\n').Append(exception.StackTrace);
		return sb.ToString();
	}

	private void Initialize()
	{
		requestDataList = new SCG.LinkedList<object[]>();
		manualResetEvent = new ST.ManualResetEvent(false);
		taskThread = new ST.Thread(new ST.ThreadStart(ProcessRequests));
		taskThread.Start();
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Process(float delta)
	{
		RemoveFirstProcessedRequest();
	}

	public void SetActive(bool active)
	{
		if(active)
			manualResetEvent.Set();
		else
			manualResetEvent.Reset();
	}


	private SCG.LinkedList<object[]> requestDataList;
	private ST.Thread taskThread;
	private ST.ManualResetEvent manualResetEvent;
	

	private const byte REQUEST_FINISHED = 0;
	private const byte REQUESTER = 1;
	private const byte CALLBACK_METHOD = 2;
}
