public class SignalData : Godot.Object
{
	public void Set(object data)
	{
		this.data = data;
		this.dataReceived = true;
	}

	public T Get<T>()
	{
		return (T) data;
	}

	public bool IsDataReceived()
	{
		return dataReceived;
	}

	public void Clear()
	{
		this.dataReceived = false;
		data = null;
	}


	private object data;
	private bool dataReceived;
}
