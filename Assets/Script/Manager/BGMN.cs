﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class BGMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._bg = this;
    }

    private JsonData _jsonList;
    public IEnumerator SetMapData()
    {                
        yield return StartCoroutine(LoadMap());//맵 이름 정보 파싱
        yield return StartCoroutine(LoadMapMoveData());//맵 이동 정보 파싱
    }

    //맵이름 호출
    public IEnumerator LoadMap()
	{
		TextAsset t = (TextAsset)Resources.Load("bg", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDataMap(t.text));
	}

    public IEnumerator SetDataMap(string jsonString)
	{
        GameData.Instance._map.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);

        for(var i = 0; i< _jsonList.Count;i++)
        {   
            GameData.Instance._map_data._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._map_data._name = _jsonList[i]["name"].ToString();
            GameData.Instance._map_data._bgm = System.Convert.ToInt32(_jsonList[i]["bgm"].ToString());

            GameData.Instance._map.Add(GameData.Instance._map_data._index, GameData.Instance._map_data);
        }

        yield return null;  
    }

    public IEnumerator LoadMapMoveData()
	{
		TextAsset t = (TextAsset)Resources.Load("bg_move", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDataMapMove(t.text));
	}

    public IEnumerator SetDataMapMove(string jsonString)
	{
        GameData.Instance._map_moveData.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);

        for(var i = 0; i< _jsonList.Count;i++)
        {   
            GameData.Instance._map_move._episode = System.Convert.ToInt32(_jsonList[i]["episode"].ToString());
            GameData.Instance._map_move._map = System.Convert.ToInt32(_jsonList[i]["map"].ToString());
            GameData.Instance._map_move._move = System.Convert.ToInt32(_jsonList[i]["move"].ToString());
            GameData.Instance._map_move._need = System.Convert.ToInt32(_jsonList[i]["need"].ToString());

            if(!GameData.Instance._map_moveData.ContainsKey(GameData.Instance._map_move._map)) GameData.Instance._map_moveData.Add(GameData.Instance._map_move._map, new List<GameData.MapMoveData>());
            GameData.Instance._map_moveData[GameData.Instance._map_move._map].Add(GameData.Instance._map_move);
        }

        yield return null;  
    }

    public int _map;
    public Transform _bg_parent;
    public BG _bg_now;
    public List<BG> _bg = new List<BG>();

    public Fade _fade;

    public void OnChangeMap(bool auto, int code)
    {
        _map = code;

        if(auto) _fade.OnFade(OnBackground, GameData.Instance._event.OnNextEvent);
        else _fade.OnFade(OnBackground, GameData.Instance._event.OnEventCheck);
    }

    public void OnBackground()
    {
        GameData.Instance._char.OnChar(-1);
        GameData.Instance._ui.CloseBottom();
        
        if(_bg_now != null) Destroy(_bg_now.gameObject);
        
        
        

        if(_map < 0) 
        {
            if(!GameData.Instance._event._event_bgm) GameData.Instance._sound.Stop_BGMSound();
        }
        else 
        {            
            _bg_now = Instantiate(_bg[_map], _bg_parent);

            _bg_now.transform.localPosition = Vector3.zero;
            _bg_now.transform.localScale = Vector3.one;

            _bg_now.OnSet(_map);

            if(!GameData.Instance._event._event_bgm) GameData.Instance._sound.Play_BGMSound(GameData.Instance._map[_map]._bgm);
        }
    }

    public bool _possible_map;

    public List<int> _move_map = new List<int>();

    public List<GameObject> _move_btn = new List<GameObject>();
    public List<Text> _move_btn_txt = new List<Text>();

    public Dictionary<int, int> _move_code = new Dictionary<int, int>();

    public void OnMoveBtn()
    {
        _move_code.Clear();
        for(var i = 0; i < _move_btn.Count; i++) _move_btn[i].SetActive(false);

        int _btn_index = 0;

        for(var j = 0 ; j < _move_map.Count; j++)
        {
            if(_move_map[j] == _map) continue;

            _move_code.Add(_btn_index, _move_map[j]);
            _move_btn_txt[_btn_index].text = GameData.Instance._map[_move_map[j]]._name;
            OnMoveBtnActive(_btn_index, true);

            _btn_index++;
        }
    }

    public void OnMoveBtnActive(int code, bool show)
    {
        _move_btn[code].SetActive(show);
    }

    public void OpenMap()
    {
        //if(!_possible_map) return;
        //_bg_now.OnMoveMapCheck();

        if(_move_map.Count <= 0) return;     

        OnMoveBtn();   

        GameData.Instance._ui.CloseBottom();
        GameData.Instance._ui.OnActiveObject(4, true);

        GameData.Instance._sound.Play_EffectSound(1);
    }

    public void CloseMap()
    {
        GameData.Instance._ui.OpenBottom();
        GameData.Instance._ui.OnActiveObject(4, false);

        GameData.Instance._sound.Play_EffectSound(1);
    }   

    public void OnMoveMap(int code)
    {
        CloseMap();
        OnChangeMap(false, _move_code[code]);
    }

    public void OnSearch(bool active)
    {
        _bg_now.OpenSearch(active);
    }

    public void OnMoveMapAdd(int code)
    {
        if(_move_map.Contains(code)) return;
        _move_map.Add(code);
    }

    public void OnMoveMapRemove(int code)
    {
        if(!_move_map.Contains(code)) return;
        _move_map.Remove(code);
    }
}
