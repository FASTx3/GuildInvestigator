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
    
    public Transform _talk_event_parent;
    public TalkKeyword _talk_keyword_ori;
    public List<TalkKeyword> _talk_keyword = new List<TalkKeyword>();

    public void OnTalkKeyword(int code)
    {
        //필요한 정보가 없으면 오브젝트 생성이 되지 않는다.
        if(GameData.Instance._eventData[code]._need > 0)
        {
            if(!GameData.Instance._item._inventory.ContainsKey(GameData.Instance._eventData[code]._need))  return;
        }

        _talk_keyword.Add(Instantiate(_talk_keyword_ori, _talk_event_parent));
        
        _talk_keyword[_talk_keyword.Count-1].transform.localScale = Vector3.one;
        _talk_keyword[_talk_keyword.Count-1].OnSet(code);
    }

    public void EndTalkKeyword()
    {
        for(var i = 0; i < _talk_keyword.Count; i++) Destroy(_talk_keyword[i].gameObject);
        _talk_keyword.Clear();
    }
}
