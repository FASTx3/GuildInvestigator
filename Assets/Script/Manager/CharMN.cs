using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LitJson;

public class CharMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._char = this;
    }

    private JsonData _jsonList;

    public IEnumerator SetCharData()
    {                
        yield return StartCoroutine(LoadCharData());
    }

    public IEnumerator LoadCharData()
	{
		TextAsset t = (TextAsset)Resources.Load("char", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDataChar(t.text));
	}

    public IEnumerator SetDataChar(string jsonString)
	{
        GameData.Instance._char_data.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);

        for(var i = 0; i< _jsonList.Count;i++) GameData.Instance._char_data.Add(System.Convert.ToInt32(_jsonList[i]["index"].ToString()), _jsonList[i]["name"].ToString());

        yield return null;  
    }

    public List<Sprite> _char_sprite = new List<Sprite>();

    public Image _char_model;

    public void OnChar(int code)
    {
        if(code < 0) 
        {
            _char_model.gameObject.SetActive(false);
        }
        else
        {
            _char_model.sprite = _char_sprite[code];
            //_char_model.SetNativeSize();
            _char_model.rectTransform.sizeDelta = _char_sprite[code].rect.size;
            
            _char_model.gameObject.SetActive(true);
        }
    }

    public void OnCharActive(bool active)
    {
        _char_model.gameObject.SetActive(active);
    }



    public List<int> _my_member = new List<int>();
    public Dictionary<int, List<int>> _map_member = new Dictionary<int, List<int>>();

    public void OnResetMember()
    {
        _my_member.Clear();
        _map_member.Clear();
    }
    
    public void MyMemberAdd(int code)
    {
        if(_my_member.Contains(code)) return;
        _my_member.Add(code);
    }

    public void MyMemberRemove(int code)
    {
        if(!_my_member.Contains(code)) return;
        _my_member.Remove(code);
    }

    public void MapMemberAdd(int map, int code)
    {
        if(!_map_member.ContainsKey(map)) _map_member.Add(map, new List<int>());
        if(_map_member[map].Contains(code)) return;
        _map_member[map].Add(code);
    }

    public void MapMemberRemove(int map, int code)
    {
        if(!_map_member.ContainsKey(map)) return;
        if(!_map_member[map].Contains(code)) return;
        _map_member[map].Remove(code);
    }
}
