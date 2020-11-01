using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class GM : MonoBehaviour
{
    private JsonData _jsonList;
    public IEnumerator SetAnnounceData()
    {        
        yield return StartCoroutine(LoadAnnounceData());
    }

    public IEnumerator LoadAnnounceData()
	{
		TextAsset t = (TextAsset)Resources.Load("announce", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDataAnnounce(t.text));
	}

    public IEnumerator SetDataAnnounce(string jsonString)
	{
        int index;
        string text;

        GameData.Instance._announce_data.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);

        for(var i = 0; i< _jsonList.Count;i++)
        {        
            index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            text = _jsonList[i]["text"].ToString();

            GameData.Instance._announce_data.Add(index, text);
        }

        yield return null;  
    }


    void Awake()
    {
        GameData.Instance._gm = this;
    }

    void Start()
    {
        StartCoroutine(LoadData());
    }

    public IEnumerator LoadData()
    {
        yield return StartCoroutine(SetAnnounceData());
        yield return StartCoroutine(GameData.Instance._item.SetItemData());
        yield return StartCoroutine(GameData.Instance._talk.SetEpisodeData());
        yield return StartCoroutine(GameData.Instance._bg.SetMapData());
        yield return StartCoroutine(GameData.Instance._event.SetEventData());
        yield return StartCoroutine(GameData.Instance._char.SetCharData());

        //PlayerPrefs.DeleteAll();
        yield return StartCoroutine(GameData.Instance.LoadData());
        yield break;
    }

    [HideInInspector]
    public bool _new_data;

    public void NewGame(int episode)
    {        
        GameData.Instance._bg._map = -1;
        GameData.Instance._event._episode = episode;
        GameData.Instance._event._event_complete.Clear();
        GameData.Instance._item._inventory.Clear();

        _new_data = true;
        GameData.Instance._bg._fade.OnFadeIn(OnEpisodeEventData);        
    }

    public void GameStart()
    {
        GameData.Instance._ui.OnActiveObject(0, false);
    }

    public void OpenSaveDataUI()
    {
        GameData.Instance._ui.OnActiveObject(1, true);
    }

    public void OpenLoadDataUI()
    {
        GameData.Instance._ui.OnActiveObject(2, true);
    }

    public Alarm _alarm;

    public void OnAlarm(string text)
    {
        _alarm.OnSet(text);
    }

    public void OnGameEnd()
    {
        GameData.Instance._ui.OnActiveObject(8, true);
    }

    public void GameSave(int code)
    {
        if(!GameData.Instance._playerData.ContainsKey(code)) GameData.Instance._playerData.Add(code, new GameData.PlayerData());
        GameData.Instance._data_index = code;

        GameData.Instance._playerData[code]._map = GameData.Instance._bg._map;
        GameData.Instance._playerData[code]._episode = GameData.Instance._event._episode;

        GameData.Instance._playerData[code]._event_complete.Clear();
        foreach(var key1 in GameData.Instance._event._event_complete.Keys) GameData.Instance._playerData[code]._event_complete.Add(key1,GameData.Instance._event._event_complete[key1]);

        GameData.Instance._playerData[code]._inventory.Clear();
        foreach(var key2 in GameData.Instance._item._inventory.Keys) GameData.Instance._playerData[code]._inventory.Add(key2, GameData.Instance._item._inventory[key2]);

        GameData.Instance._playerData[code]._my_member.Clear();
        for(var key3 = 0; key3 < GameData.Instance._char._my_member.Count; key3++) GameData.Instance._playerData[code]._my_member.Add(GameData.Instance._char._my_member[key3]);

        GameData.Instance._playerData[code]._map_member.Clear();
        foreach(var key4 in GameData.Instance._char._map_member.Keys)
        {
            for(var i = 0; i < GameData.Instance._char._map_member[key4].Count; i++)
            {
                if(!GameData.Instance._playerData[code]._map_member.ContainsKey(key4)) GameData.Instance._playerData[code]._map_member.Add(key4, new List<int>());
                GameData.Instance._playerData[code]._map_member[key4].Add(GameData.Instance._char._map_member[key4][i]);
            }
        }

        GameData.Instance.SaveData();
    }
    
    public void GameLoad(int code)
    {
        if(!GameData.Instance._playerData.ContainsKey(code)) return;

        GameData.Instance._bg._map = GameData.Instance._playerData[code]._map;
        
        GameData.Instance._event._event_complete.Clear();
        foreach(var key1 in GameData.Instance._playerData[code]._event_complete.Keys) GameData.Instance._event._event_complete.Add(key1, GameData.Instance._playerData[code]._event_complete[key1]);

        GameData.Instance._item._inventory.Clear();        
        foreach(var key2 in GameData.Instance._playerData[code]._inventory.Keys) GameData.Instance._item._inventory.Add(key2, GameData.Instance._playerData[code]._inventory[key2]);

        GameData.Instance._char._my_member.Clear();
        for(var key3 = 0; key3 < GameData.Instance._playerData[code]._my_member.Count; key3++) GameData.Instance._char._my_member.Add(GameData.Instance._playerData[code]._my_member[key3]);

        GameData.Instance._char._map_member.Clear();
        foreach(var key4 in GameData.Instance._playerData[code]._map_member.Keys)
        {
            for(var i = 0; i < GameData.Instance._playerData[code]._map_member[key4].Count; i++)
            {
                if(!GameData.Instance._char._map_member.ContainsKey(key4)) GameData.Instance._char._map_member.Add(key4, new List<int>());
                GameData.Instance._char._map_member[key4].Add(GameData.Instance._playerData[code]._map_member[key4][i]);
            }
        }

        if(GameData.Instance._event._episode != GameData.Instance._playerData[code]._episode)
        {
            GameData.Instance._event._episode = GameData.Instance._playerData[code]._episode;

            _new_data = false;
            GameData.Instance._bg._fade.OnFadeIn(OnEpisodeEventData);
        }
        else
            GameData.Instance._bg._fade.OnFade(GameLoadDone, GameData.Instance._event.OnEventCheck);
    } 

    public void GameLoadDone()
    {
        GameData.Instance._ui.OnActiveObject(0, false);
        GameData.Instance._ui.OnActiveObject(2, false);
        GameData.Instance._bg.OnBackground();
    }

    public void OnEpisodeEventData()
    {
        if(_new_data) GameStart();
        else GameLoadDone();

        StartCoroutine(OnEpisodeEventParse());
    }

    public IEnumerator OnEpisodeEventParse()
    {
        yield return StartCoroutine(GameData.Instance._event.SetEpisodeEventData());

        if(_new_data) GameData.Instance._bg._fade.OnFadeOut(GameData.Instance._event.OnFirstEvent);
        else GameData.Instance._bg._fade.OnFadeOut(GameData.Instance._event.OnEventCheck);

        yield break;
    }
}

