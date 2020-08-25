using SC = System.Collections;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class JsonSerializer : Node
{
	public T Get<T>(string key)
	{
		if(dataMap.Contains(key))
			return (T) dataMap[key];
		
		return default(T);
	}

	public void Put(Dictionary map)
	{
		SC.IDictionaryEnumerator it = map.GetEnumerator();

		while(it.MoveNext())
			Put(it.Entry.Key as string, it.Entry.Value);
	}

	public void Put<TK, TV>(Dictionary<TK, TV> map)
	{
		SCG.IEnumerator<SCG.KeyValuePair<TK, TV>> it =
				map.GetEnumerator();

		while(it.MoveNext())
			Put(it.Current.Key, it.Current.Value);
	}

	public void Put(object key, object value)
	{
		if(dataMap.Contains(key))
			dataMap[key] = value;
		else
			dataMap.Add(key, value);
	}

	public void Remove(string key)
	{
		if(dataMap.Contains(key))
			dataMap.Remove(key);
	}

	public void Clear()
	{
		dataMap.Clear();
	}

	public void Save(string filePath)
	{
		Save(dataMap, filePath);
	}

	public void Save(Dictionary dataMap, string filePath)
	{
		File f = new File();
		f.Open(filePath, File.ModeFlags.Write);
		f.StoreString(JSON.Print(dataMap, "	"));
		f.Close();
		f.Dispose();
	}

	public Dictionary Load(string filePath, bool notNull)
	{
		File f = new File();
		string content;

		if(f.FileExists(filePath))
		{
			f.Open(filePath, File.ModeFlags.Read);
			content = f.GetAsText();
		}
		else
			content = "{}";

		f.Close();
		f.Dispose();
		dataMap = JSON.Parse(content).Result as Dictionary;
		return (notNull && dataMap == null) ? new Dictionary() : dataMap;
	}


	private Dictionary dataMap;
}
