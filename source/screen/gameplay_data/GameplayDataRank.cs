using System.Text;
using System.Security.Cryptography;

using Godot;
using Godot.Collections;


public class GameplayDataRank : Node
{
	public int IsValidDataMap(Dictionary dataMap)
	{
		if(ContainsAllKeys(dataMap, normalGameDataKeys))
		{
			string[] hashData = GetNormalGameHashData(dataMap);
			string hash = GetSha256HashString(hashData[0], hashData[1], hashData[2]);
			return hash.Equals(dataMap[normalGameDataKeys[4]] as string) ? 0 : -1;
		}
		else if(ContainsAllKeys(dataMap, survivalGameDataKeys))
		{
			string[] hashData = GetSurvivalGameHashData(dataMap);
			string hash = GetSha256HashString(hashData[0], hashData[1], hashData[2]);
			return hash.Equals(dataMap[survivalGameDataKeys[1]] as string) ? 1 : -1;
		}

		return -1;
	}

	public void Load()
	{
		normalGameDataMap = LoadDataMap(normalGameDataFileName);
		survivalGameDataMap = LoadDataMap(survivalGameDataFileName);
		normalGameRankedDataMap = GetRankedDataMap(normalGameDataMap,
				normalGameDataKeys[3], 0);
		survivalGameRankedDataMap = GetRankedDataMap(survivalGameDataMap,
				survivalGameDataKeys[0], 1);
	}

	public void Save(Dictionary dataMap, bool normalGame)
	{
		if(normalGame)
		{
			string[] hashData = GetNormalGameHashData(dataMap);
			dataMap.Add("ac", GetSha256HashString(hashData[0], hashData[1], hashData[2]));
			normalGameDataMap = LoadDataMap(normalGameDataFileName);
			normalGameDataMap.Add(normalGameDataMap.Count.ToString(), dataMap);
			normalGameRankedDataMap = GetRankedDataMap(normalGameDataMap,
					normalGameDataKeys[3], 0);
			this.EmitSignal(this.GetSignalSave(), normalGameRankedDataMap,
					GetFilePath(normalGameDataFileName));
		}
		else
		{
			string[] hashData = GetSurvivalGameHashData(dataMap);
			dataMap.Add("ac", GetSha256HashString(hashData[0], hashData[1], hashData[2]));
			survivalGameDataMap = LoadDataMap(survivalGameDataFileName);
			survivalGameDataMap.Add(survivalGameDataMap.Count.ToString(), dataMap);
			survivalGameRankedDataMap = GetRankedDataMap(survivalGameDataMap,
					survivalGameDataKeys[0], 1);
			this.EmitSignal(this.GetSignalSave(), survivalGameRankedDataMap,
					GetFilePath(survivalGameDataFileName));
		}
	}

	public Dictionary GetDataMap(string gameTypeKey)
	{
		if(gameTypeKey.Equals("normalGame"))
			return normalGameRankedDataMap;
		else if(gameTypeKey.Equals("survivalGame"))
			return survivalGameRankedDataMap;
		
		return new Dictionary();
	}

	private Dictionary GetRankedDataMap(Dictionary dataMap, string rankKey, int dataMapType)
	{
		long currentValue;
		Dictionary currentRankMap;
		string indexText;
		long biggestValue = long.MinValue;
		string biggestRankKey = null;
		Dictionary rankedDataMap = new Dictionary();
		Dictionary selectedMap = new Dictionary();
		int iterations = dataMap.Count < 10 ? dataMap.Count : 10;
		
		for(int i = 0; i < 3; i++)
		{
			for(int j = 0; j < iterations; j++)
			{
				indexText = j.ToString();

				if(dataMap.Contains(indexText) && !selectedMap.Contains(indexText))
				{
					currentRankMap = dataMap[indexText] as Dictionary;

					if(IsValidDataMap(currentRankMap) == dataMapType)
					{
						currentValue = long.Parse(currentRankMap[rankKey] as string);

						if(currentValue > biggestValue)
						{
							biggestValue = currentValue;
							biggestRankKey = indexText;
						}
					}
				}
			}

			if(dataMap.Contains(biggestRankKey) && !selectedMap.Contains(biggestRankKey))
			{
				rankedDataMap.Add(i.ToString(), dataMap[biggestRankKey]);
				selectedMap.Add(biggestRankKey, null);
			}

			biggestValue = long.MinValue;
			biggestRankKey = null;
		}

		return rankedDataMap;
	}

	private Dictionary LoadDataMap(string fileName)
	{
		return this.EmitSignal<Dictionary>(this, this.GetSignalLoad(),
				GetFilePath(fileName), true);
	}

	private bool ContainsAllKeys(Dictionary dataMap, string[] keys)
	{
		for(int i = 0; i < keys.Length; i++)
		{
			if(!dataMap.Contains(keys[i]))
				return false;
		}
		
		return true;
	}

	private string[] GetNormalGameHashData(Dictionary dataMap)
	{
		char[] array = (dataMap[normalGameDataKeys[1]] as string).ToCharArray();
		System.Array.Reverse(array);
		string prefix = new string(array);
		string data = dataMap[normalGameDataKeys[3]] as string;
		string sufix = new StringBuilder(dataMap[normalGameDataKeys[2]] as string).
				Append(dataMap[normalGameDataKeys[0]] as string).ToString();
		return new string[]{prefix, data, sufix};
	}

	private string[] GetSurvivalGameHashData(Dictionary dataMap)
	{
		char[] array = (survivalGameDataKeys[0] as string).ToCharArray();
		System.Array.Reverse(array);
		string prefix = new string(array);
		string data = dataMap[survivalGameDataKeys[0]] as string;
		string sufix = survivalGameDataKeys[1] as string;
		return new string[]{prefix, data, sufix};
	}

	private string GetSha256HashString(string prefix, string data, string sufix)  
	{  
		using(SHA256 sha256Hash = SHA256.Create())  
		{ 
			StringBuilder finalData = new StringBuilder(prefix).
					Append(data).Append(sufix);
			byte[] bytes = sha256Hash.ComputeHash(
					Encoding.UTF8.GetBytes(finalData.ToString()));  
			finalData = new StringBuilder();

			for(int i = 0; i < bytes.Length; i++)   
				finalData.Append(bytes[i].ToString("x2"));  

			return finalData.ToString();  
		}
	}

	private string GetFilePath(string fileName)
	{
		return this.GetFilePath(fileName, "json", gameplayDataFolderName);
	}

	private void Initialize()
	{
		normalGameDataKeys = new string[]{"rank", "totalTime", "deaths", "rankScore", "ac"};
		survivalGameDataKeys = new string[]{"playerScore", "ac"};
		this.CreateFolder(gameplayDataFolderName);
	}

	public override void _EnterTree()
	{
		Initialize();
	}


	[Export]
	public string gameplayDataFolderName;

	[Export]
	public string normalGameDataFileName;

	[Export]
	public string survivalGameDataFileName;


	private Dictionary normalGameDataMap;
	private Dictionary normalGameRankedDataMap;
	private Dictionary survivalGameDataMap;
	private Dictionary survivalGameRankedDataMap;
	private string[] normalGameDataKeys;
	private string[] survivalGameDataKeys;
}
