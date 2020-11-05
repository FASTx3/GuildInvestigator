using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG : MonoBehaviour
{
    public int _index;

    public List<int> _event = new List<int>();

    public GameObject _search_obj;

    public void OnSet(int index)
    {
        _index = index;

    }

    public void OpenSearch(bool show)
    {
        if(_search_obj != null) _search_obj.SetActive(show);
    }

    public void OnSearchEvent(int code)
    {
        OpenSearch(false);
        
        GameData.Instance._event._event_type = 2;
        GameData.Instance._event.OnEventStart(code);
    }

    public void CloseSearch()
    {
        GameData.Instance._event.CloseSearch();
    }
}
