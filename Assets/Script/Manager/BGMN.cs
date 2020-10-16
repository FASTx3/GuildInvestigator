using System.Collections;
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
            GameData.Instance._map.Add(_jsonList[i]["name"].ToString());
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

        if(_map < 0) return;
        
        _bg_now = Instantiate(_bg[_map], _bg_parent);

        _bg_now.transform.localPosition = Vector3.zero;
        _bg_now.transform.localScale = Vector3.one;

        _bg_now.OnSet(_map);

        GameData.Instance._sound.Play_BGMSound(_map);
    }

    public bool _possible_map;
    public List<GameObject> _move_btn = new List<GameObject>();
    public List<Text> _move_btn_txt = new List<Text>();
    public void OnMoveBtnReset()
    {
        for(var i = 0; i < _move_btn.Count; i++) _move_btn[i].SetActive(false);
    }

    public void OnMoveBtnText(int code, string txt)
    {
        _move_btn_txt[code].text = txt;
    }

    public void OnMoveBtnActive(int code, bool show)
    {
        _move_btn[code].SetActive(show);
    }

    public void OpenMap()
    {
        if(!_possible_map) return;

        _bg_now.OnMoveMapCheck();

        GameData.Instance._ui.CloseBottom();
        GameData.Instance._ui.OnActiveObject(4, true);
    }

    public void CloseMap()
    {
        GameData.Instance._ui.OpenBottom();
        GameData.Instance._ui.OnActiveObject(4, false);
    }   

    public void OnMoveMap(int code)
    {
        CloseMap();
        OnChangeMap(false, _bg_now._move[code]);
    }

    public void OnSearch(bool active)
    {
        if(_bg_now._search_obj == null) return;
        
        _bg_now._search_obj.SetActive(active);
    }
}
