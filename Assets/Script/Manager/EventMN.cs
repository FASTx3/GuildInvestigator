using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EventMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._event = this;
    }

    public int _episode;
    public int _event_code;
    public int _count;
    public int _event_type;
    public bool _event_progress;

    public Dictionary<int, bool> _auto_event = new Dictionary<int, bool>();
    public bool _auto_event_check;

    public void OnFirstEvent()
    {
        _event_code = 1;
        _event_type = 0;
        OnEventStart(_event_code);
    }

    public void OnEventStart(int event_code)
    {
        _event_progress = true;

        _event_code = event_code;
        _count = 0;

        OnEventShow();
        GameData.Instance._ui.CloseBottom();
    }

    public void OnEventCheck()//맵 이동시 자동 이벤트 여부 체크
    {
        if(GameData.Instance._bg._bg_now == null) return;

        if(GameData.Instance._bg._bg_now._auto_event > 0)
        {
            if(!_auto_event.ContainsKey(GameData.Instance._bg._bg_now._auto_event)) _auto_event.Add(GameData.Instance._bg._bg_now._auto_event, true);

            if(_auto_event[GameData.Instance._bg._bg_now._auto_event])  
            {
                _auto_event_check = true;
                OnEventStart(GameData.Instance._bg._bg_now._auto_event);
            }  
            else   
            {
                GameData.Instance._bg._bg_now.OnChar(); 
                GameData.Instance._ui.OpenBottom();     
            }       
        }
        else 
        {
            GameData.Instance._bg._bg_now.OnChar(); 
            GameData.Instance._ui.OpenBottom(); 
        }
    }

    public void OnEventCompleteCheck()//이벤트 완료 확인
    {
        if(GameData.Instance._episode[_episode][_event_code].Count <= _count) //이벤트 완료
        {
            _count = 0;

            GameData.Instance._bg._bg_now.OnChar();   

            switch(_event_type)
            {
                case 0 : //자동 이벤트
                    GameData.Instance._ui.OpenBottom();                              
                break;

                case 1 : //대화 이벤트
                     OnTalk();
                break;

                case 2 : //조사 이벤트
                     OnSearch();
                break;
            }

            if(_auto_event_check)//자동 이벤트 중복 실행 막기      
            {
                _auto_event_check = false;
                _auto_event[GameData.Instance._bg._bg_now._auto_event] = false;
            }  
            
            _event_progress = false;
        }        
        else
        {
            OnEventShow();
        }
    }
    
    public bool _trigger;

    public void OnNextEvent()
    {
        if(_trigger) _trigger = false;
        else return;

        _count++;
        OnEventCompleteCheck();
    }

    public void OnEventShow()//이벤트 내용
    {
        _trigger = true;

        if(GameData.Instance._episode[_episode][_event_code][_count]._eff > -1)
            GameData.Instance._sound.Play_EffectSound(GameData.Instance._episode[_episode][_event_code][_count]._eff);

        switch(GameData.Instance._episode[_episode][_event_code][_count]._type)
        {
            case 0 : //대화
                GameData.Instance._char.OnChar(GameData.Instance._episode[_episode][_event_code][_count]._source);
                GameData.Instance._talk.OnTalk(GameData.Instance._episode[_episode][_event_code][_count]);                
            break;

            case 1 : //배경 변경                
                GameData.Instance._bg.OnChangeMap(true, GameData.Instance._episode[_episode][_event_code][_count]._source);
            break;

            case 2 : //배경음 변경
                GameData.Instance._sound.Play_BGMSound(GameData.Instance._episode[_episode][_event_code][_count]._source);
                OnNextEvent();
            break;

            case 3 : //연출                  
                OnEventProduction(GameData.Instance._episode[_episode][_event_code][_count]._source);  
                OnNextEvent();
            break;

            case 4 : //아이템 획득
                if(GameData.Instance._item._inventory.ContainsKey(GameData.Instance._episode[_episode][_event_code][_count]._source))
                    OnNextEvent();
                else 
                    GameData.Instance._item.GetItem(GameData.Instance._episode[_episode][_event_code][_count]._source);
            break;
        }
    }

    public List<Fade> _fade = new List<Fade>();

    public void OnEventProduction(int code)
    {
        switch(code)
        {
            case 0 : //깜빢이는 연출
                _fade[0].OnSparkle(null);
            break;

            case 1 : //독백시작 독백 시작
                _fade[1].OnFadeIn(null);
            break;

            case 2 : //독백시작 독백 종료
                _fade[1].OnFadeOut(null);
            break;
        }
    }

    public bool _search;
    public void OnSearch()
    {
        _search = true;

        GameData.Instance._ui.CloseBottom();
        GameData.Instance._bg.OnSearch(true);
        GameData.Instance._char.OnCharActive(false);
    }

    public void CloseSearch()
    {
        _search = false;

        GameData.Instance._ui.OpenBottom();
        GameData.Instance._bg.OnSearch(false);
        GameData.Instance._char.OnCharActive(true);
    }

    public bool _possible_talk;
    public void OnTalk()
    {
        if(!_possible_talk) return;

        GameData.Instance._ui.CloseBottom();
        GameData.Instance._ui.OnActiveObject(3, true);
    }

    public void CloseTalk()
    {
        GameData.Instance._ui.OpenBottom();
        GameData.Instance._ui.OnActiveObject(3, false);
    }

    public void OnTalkNPC(int code)
    {       
        GameData.Instance._ui.CloseBottom();   
        GameData.Instance._ui.OnActiveObject(3, false);
        GameData.Instance._bg._bg_now.OnTalkEvent(code);
    }
}
