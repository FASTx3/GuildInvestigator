using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LitJson;

public class TalkMN : MonoBehaviour
{
    private JsonData _jsonList;
    public IEnumerator SetEpisodeData()
    {        
        yield return StartCoroutine(LoadEpisodeData());
    }

    public IEnumerator LoadEpisodeData()
	{
		TextAsset t = (TextAsset)Resources.Load("episode1", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDataEpisode(t.text));
	}

    public IEnumerator SetDataEpisode(string jsonString)
	{
        GameData.Instance._episode.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);

        for(var i = 0; i< _jsonList.Count;i++)
        {        
            GameData.Instance._episodeData._event = System.Convert.ToInt32(_jsonList[i]["chapter"].ToString());
            GameData.Instance._episodeData._type = System.Convert.ToInt32(_jsonList[i]["type"].ToString());
            GameData.Instance._episodeData._source = System.Convert.ToInt32(_jsonList[i]["source"].ToString());
            GameData.Instance._episodeData._emotion = System.Convert.ToInt32(_jsonList[i]["emotion"].ToString());
            GameData.Instance._episodeData._eff = System.Convert.ToInt32(_jsonList[i]["eff"].ToString());

            GameData.Instance._episodeData._name = _jsonList[i]["name"].ToString();
            GameData.Instance._episodeData._function = _jsonList[i]["function"].ToString();

            if(!GameData.Instance._episode.ContainsKey(1)) GameData.Instance._episode.Add(1, new Dictionary<int, List<GameData.EpisodeData>>());
            if(!GameData.Instance._episode[1].ContainsKey(GameData.Instance._episodeData._event)) GameData.Instance._episode[1].Add(GameData.Instance._episodeData._event, new List<GameData.EpisodeData>());
            GameData.Instance._episode[1][GameData.Instance._episodeData._event].Add(GameData.Instance._episodeData);
        }

        yield return null;  
    }


    void Awake()
    {
        GameData.Instance._talk = this;
    }

    public Talk _talk;

    public void OnTalk(GameData.EpisodeData _data)
    {
        _talk.OnTalk(_data._name, _data._function);
    }
    
}
