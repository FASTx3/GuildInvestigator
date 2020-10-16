using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG : MonoBehaviour
{
    public int _index;

    public List<int> _event = new List<int>();

    public GameObject _search_obj;

    public int _auto_event = 0;
    public int _map_member = -1;

    public List<int> _move = new List<int>();
    public List<int> _move_need = new List<int>();
    
    public void OnSet(int index)
    {
        _index = index;

        _map_member = -1;

        OnMoveSet();
    }

    public void OnMoveSet() //맵의 이동 지역 체크
    {
        if(!GameData.Instance._map_moveData.ContainsKey(_index)) 
        {
            GameData.Instance._bg._possible_map = false;
            return;
        }
        
        for(var i = 0; i < GameData.Instance._map_moveData[_index].Count; i++)
        {
            if(GameData.Instance._map_moveData[_index][i]._episode == GameData.Instance._event._episode) 
            {
                _move.Add(GameData.Instance._map_moveData[_index][i]._move);
                _move_need.Add(GameData.Instance._map_moveData[_index][i]._need);
            }
        }

        if(_move.Count > 0)
        {
            GameData.Instance._bg._possible_map = true;

            GameData.Instance._bg.OnMoveBtnReset();            
            for(var i = 0; i < _move.Count; i++) GameData.Instance._bg.OnMoveBtnText(i, GameData.Instance._map[_move[i]]);
        }
        else
        {
            GameData.Instance._bg._possible_map = false;
        }
    }

    public void OnMoveMapCheck()//이동 지역 해금 여부 체크
    {
         for(var i = 0; i < _move.Count; i++) 
         {
            if(_move_need[i] > 0)
            {
                 GameData.Instance._bg.OnMoveBtnActive(i, false);
            }
            else GameData.Instance._bg.OnMoveBtnActive(i, true);
         }
    }

    public void OnSearchEvent(int code)
    {
        if(_search_obj != null) _search_obj.SetActive(false);
        
        GameData.Instance._event._event_type = 2;
        GameData.Instance._event.OnEventStart(code);
    }

    public void OnChar()
    {
        GameData.Instance._char.OnChar(_map_member);
    }
}
