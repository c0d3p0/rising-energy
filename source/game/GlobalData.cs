using SC = System.Collections;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class GlobalData : Node
{
	public void Get(string key, Godot.Object signalData)
	{
		object value;

		if(globalDataMap.TryGetValue(key, out value))
			signalData.Call(this.GetMethodSet(), value);
	}

	public void Put(Dictionary map)
	{
		SC.IDictionaryEnumerator it = map.GetEnumerator();

		while(it.MoveNext())
			Put(it.Entry.Key as string, it.Entry.Value);
	}

	public void Put(Dictionary<string, object> map)
	{
		SCG.IEnumerator<SCG.KeyValuePair<string, object>> it =
				map.GetEnumerator();

		while(it.MoveNext())
			Put(it.Current.Key, it.Current.Value);
	}

	public void Put(string key, object value)
	{
		if(globalDataMap.ContainsKey(key))
			globalDataMap[key] = value;
		else
			globalDataMap.Add(key, value);
	}

	public void Remove(string key)
	{
		if(globalDataMap.ContainsKey(key))
			globalDataMap.Remove(key);
	}

	public void Clear()
	{
		globalDataMap.Clear();
	}

	private void Initialize()
	{
		globalDataMap = new Dictionary<string, object>();
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	protected Dictionary<string, object> globalDataMap;
}
