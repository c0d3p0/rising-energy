using System.Text;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public static class NodeExtension
{
	// A node specific to hold enemies and projectiles spawned should be
	// added as the first child of the scene. This is done so that enemies with
	// GUI elements do not get drawn over the PauseScreen, which will make impossible
	// to any GUI interaction with the PauseScreen.
	public static void AddChildToEnemyContainer(this Node gdNode, Node caller, Node enemy)
	{
		if(caller.IsInsideTree())
		{
			caller.GetTree().CurrentScene.GetChild(0).
					CallDeferred(caller.GetGDMethodAddChild(), enemy);
		}
	}

	public static void AddChildToEnemyContainer(this Node gdNode, Node caller,
			int enemyContainerIndex, Node enemy)
	{
		if(caller.IsInsideTree())
		{
			caller.GetTree().CurrentScene.GetChild(enemyContainerIndex).
					CallDeferred(caller.GetGDMethodAddChild(), enemy);
		}
	}

	public static string GetPrefabPath(this Node gdNode, string prefabPath)
	{
		return new StringBuilder("res://prefab/").Append(prefabPath).
				Append(".tscn").ToString();
	}

	public static string GetScenePath(this Node gdNode, string scenePath)
	{
		return new StringBuilder("res://scene/").Append(scenePath).
				Append(".tscn").ToString();
	}

	public static Dictionary<TK, TV> GetNodeMap<TK, TV>(this Node gdNode,
			Node caller, Dictionary<TK, NodePath> nodePathMap) where TV : Godot.Object
	{
		Dictionary<TK, TV> nodeMap = new Dictionary<TK, TV>();

		if(nodePathMap != null)
		{
			SCG.IEnumerator<SCG.KeyValuePair<TK, NodePath>> it =
					nodePathMap.GetEnumerator();

			while(it.MoveNext())
				nodeMap.Add(it.Current.Key, caller.GetNode<TV>(it.Current.Value));
		}
		else
			GD.PushWarning("NodePathMap called by '" + caller.Name + "' is null");
		
		return nodeMap;
	}

	public static string CreateUniqueNodeName(this Node gdNode,
			Node node)
	{
		return new StringBuilder(node.Name).Append('_').
				Append(node.GetInstanceId()).ToString();
	}
}
