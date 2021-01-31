using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public static class GameNodeExtension
{
	public static Dictionary<string, TV> GetResourceMap<TV>(this Node gdNode,
			Node caller, Node globalData, Array<string> resourceKeyList)
			where TV : Godot.Object
	{
		Dictionary<string, TV> resourceMap = new Dictionary<string, TV>();

		if(resourceKeyList != null)
		{
			SCG.IEnumerator<string> it = resourceKeyList.GetEnumerator();

			while(it.MoveNext())
			{
				resourceMap.Add(it.Current, caller.Call<TV>(globalData,
						caller.GetMethodGet(), it.Current));
			}
		}
		else
		{
			if(OS.IsDebugBuild())
				GD.PushWarning("ResourceKeyList called by '" + caller.Name + "' is null");
		}
		
		return resourceMap;
	}

	public static Array<T> GetResourceList<T>(this Node gdNode,
			Node caller, Node globalPrefab, Array<string> resourceKeyList)
			where T : Godot.Object
	{
		Array<T> resourceList = new Array<T>();

		if(resourceKeyList != null)
		{
			SCG.IEnumerator<string> it = resourceKeyList.GetEnumerator();

			while(it.MoveNext())
			{
				resourceList.Add(caller.Call<T>(globalPrefab,
						caller.GetMethodGet(), it.Current));
			}
		}
		else
		{
			if(OS.IsDebugBuild())
				GD.PushWarning("ResourceKeyList called by " + caller.Name + " is null");
		}
		
		return resourceList;
	}
}
