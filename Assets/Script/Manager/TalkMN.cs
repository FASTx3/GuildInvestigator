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

    public Transform _talk_member_parent;
    public TalkMember _talk_member_ori;
    public List<TalkMember> _talk_member = new List<TalkMember>();

    public void OnTalkMember(int code)//대화 대상 오브젝트 생성
    {
        _talk_member.Add(Instantiate(_talk_member_ori, _talk_member_parent));
        
        _talk_member[_talk_member.Count-1].transform.localScale = Vector3.one;
        _talk_member[_talk_member.Count-1].OnSet(code);
    }

    public void EndTalkMember()//대화 대상 오브젝트 파괴
    {
        for(var i = 0; i < _talk_member.Count; i++) Destroy(_talk_member[i].gameObject);
        _talk_member.Clear();
    }

    public void OpenTalkMember()
    {
        //동료와의 대화
        if(GameData.Instance._char._my_member.Count > 0)
        {
            for(var j = 0; j < GameData.Instance._char._my_member.Count; j++)
            {
                OnTalkMember(GameData.Instance._char._my_member[j]);
            }
        }

        //주변 사람과의 대화
        if(GameData.Instance._char._map_member.ContainsKey(GameData.Instance._bg._map))
        {
            for(var i = 0; i < GameData.Instance._char._map_member[GameData.Instance._bg._map].Count; i++)
            {
                OnTalkMember(GameData.Instance._char._map_member[GameData.Instance._bg._map][i]);
            }
        }
        
        GameData.Instance._ui.CloseBottom();
        GameData.Instance._ui.OnActiveObject(10, true);
    }
    public void CloseTalkMember()
    {
        GameData.Instance._ui.OpenBottom();
        GameData.Instance._ui.OnActiveObject(10, false);

        EndTalkMember();
    }
   
    public Transform _talk_event_parent;
    public TalkKeyword _talk_keyword_ori;
    public List<TalkKeyword> _talk_keyword = new List<TalkKeyword>();

    public void OnTalkKeyword(int code)//대화 키워드 오브젝트 생성
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

    public void EndTalkKeyword()//대화 키워드 오브젝트 삭제
    {
        for(var i = 0; i < _talk_keyword.Count; i++) Destroy(_talk_keyword[i].gameObject);
        _talk_keyword.Clear();
    }

    public void OpenTalk(int member)
    {
        //대화 대상 닫기
        GameData.Instance._ui.OnActiveObject(10, false);
        EndTalkMember();

        for(var j = 0; j < GameData.Instance._event._talk_event[member].Count; j++) OnTalkKeyword(GameData.Instance._event._talk_event[member][j]); 
        GameData.Instance._ui.OnActiveObject(3, true);
    }

    public void CloseTalk()
    {
        GameData.Instance._ui.OnActiveObject(3, false);
        EndTalkKeyword();

        OpenTalkMember();
    }
}
