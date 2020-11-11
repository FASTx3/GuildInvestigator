using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LitJson;


public class EventMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._event = this;
    }
    private JsonData _jsonList;

    public IEnumerator SetEventData()
    {                
        yield return StartCoroutine(LoadEventData());
    }

    public IEnumerator LoadEventData()
	{
		TextAsset t = (TextAsset)Resources.Load("event", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDataEvent(t.text));
	}

    public IEnumerator SetDataEvent(string jsonString)
	{
        GameData.Instance._eventData.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);

        for(var i = 0; i< _jsonList.Count;i++)
        {   
            GameData.Instance._event_data._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._event_data._map = System.Convert.ToInt32(_jsonList[i]["map"].ToString());
            GameData.Instance._event_data._episode = System.Convert.ToInt32(_jsonList[i]["episode"].ToString());
            GameData.Instance._event_data._type = System.Convert.ToInt32(_jsonList[i]["type"].ToString());
            GameData.Instance._event_data._member = System.Convert.ToInt32(_jsonList[i]["member"].ToString());
            GameData.Instance._event_data._need = System.Convert.ToInt32(_jsonList[i]["need"].ToString());
            GameData.Instance._event_data._event = System.Convert.ToInt32(_jsonList[i]["event"].ToString());

            GameData.Instance._event_data._txt = _jsonList[i]["txt"].ToString();

            GameData.Instance._eventData.Add(GameData.Instance._event_data._index, GameData.Instance._event_data);
        }

        yield return null;  
    }

    public Dictionary<int, List<int>> _map_event = new Dictionary<int, List<int>>();
    public Dictionary<int, List<int>> _talk_event = new Dictionary<int, List<int>>();
    public Dictionary<int, int> _item_event = new Dictionary<int, int>();

    public IEnumerator SetEpisodeEventData()//에피소드 별 이벤트 정리
	{
        _map_event.Clear();
        _talk_event.Clear();
        _item_event.Clear();

        foreach(var key in GameData.Instance._eventData.Keys)
        {   
            if(GameData.Instance._eventData[key]._episode != _episode) continue;
            
            switch(GameData.Instance._eventData[key]._type)
            {
                case 0 : //맵 이동시 이벤트 발생
                    if(!_map_event.ContainsKey(GameData.Instance._eventData[key]._map)) _map_event.Add(GameData.Instance._eventData[key]._map, new List<int>());
                    _map_event[GameData.Instance._eventData[key]._map].Add(key);
                break;

                case 1 : //대화 이벤트
                    if(!_talk_event.ContainsKey(GameData.Instance._eventData[key]._member)) _talk_event.Add(GameData.Instance._eventData[key]._member, new List<int>());
                    _talk_event[GameData.Instance._eventData[key]._member].Add(key);
                break;

                case 2 : //아이템 획득에 따른 이벤트
                    _item_event.Add(GameData.Instance._eventData[key]._need, GameData.Instance._eventData[key]._event);
                break;
            }                     
        }

        yield return null;  
    }

    public int _episode;
    public int _event_code;
    public int _count;
    public int _event_type;
    public bool _event_progress;

    public Dictionary<int, bool> _event_complete = new Dictionary<int, bool>();

    public InvestigateComplete _investigate_complete;


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

        if(!_event_complete.ContainsKey(event_code)) _event_complete.Add(event_code, false);

        OnEventShow();
        GameData.Instance._ui.CloseBottom();
    }

    public void OnEventCheck()//맵 이동시 자동 이벤트 여부 체크
    {
        int event_confirm = -1;

        if(_map_event.ContainsKey(GameData.Instance._bg._map))
        {

            for(var i = 0; i <_map_event[GameData.Instance._bg._map].Count; i++)
            {
                int code = _map_event[GameData.Instance._bg._map][i];

                if(GameData.Instance._eventData[code]._need == 0)
                {
                    if(_event_complete.ContainsKey(GameData.Instance._eventData[code]._event)) continue;
                    event_confirm = GameData.Instance._eventData[code]._event;
                    break;
                }
                else
                {                    
                    if(!GameData.Instance._item._inventory.ContainsKey(GameData.Instance._eventData[code]._need)) continue;
                    if(_event_complete.ContainsKey(GameData.Instance._eventData[code]._event)) continue;
                    event_confirm = GameData.Instance._eventData[code]._event;
                    break;
                }    
            }
        }


        if(event_confirm > -1)             
        {
            _event_type = 0;
            OnEventStart(event_confirm);
        }
        else
        {
            //GameData.Instance._bg._bg_now.OnChar(); 
            GameData.Instance._ui.OpenBottom(); 
        }
    }

    public void OnEventCompleteCheck()//이벤트 완료 확인
    {
        if(GameData.Instance._episode[_episode][_event_code].Count <= _count) //이벤트 완료
        {
            _count = 0;

            //GameData.Instance._bg._bg_now.OnChar();   

            switch(_event_type)
            {
                case 0 : //자동 이벤트
                    GameData.Instance._ui.OpenBottom();                              
                break;

                case 1 : //대화 이벤트
                    GameData.Instance._ui.CloseBottom();
                    GameData.Instance._ui.OnActiveObject(3, true);
                    GameData.Instance._char.OnCharActive(false);
                break;

                case 2 : //조사 이벤트
                     OnSearch();
                break;

                case 3 : //게임 타이틀 이동
                     GameData.Instance._gm.OnGameEnd();
                break;
            }

            if(!_event_complete[_event_code]) _event_complete[_event_code] = true;

            _event_progress = false;

            OnItemEvent();//아이템 이벤트 실행
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

        _event_show = 0;
        OnEventCompleteCheck();        
    }

    public int _event_show;
    public bool _event_bgm;

    public void OnEventShow()//이벤트 내용
    {
        _trigger = true;        

        switch(GameData.Instance._episode[_episode][_event_code][_count]._type)
        {
            case 0 : //대화
                _event_show = 1;
                GameData.Instance._char.OnChar(GameData.Instance._episode[_episode][_event_code][_count]._source);
                GameData.Instance._talk.OnTalk(GameData.Instance._episode[_episode][_event_code][_count]);                
            break;

            case 1 : //배경 변경                
                GameData.Instance._bg.OnChangeMap(true, GameData.Instance._episode[_episode][_event_code][_count]._source);
            break;

            case 2 : //배경음 변경
                if(GameData.Instance._episode[_episode][_event_code][_count]._source == -1) _event_bgm = false;
                else _event_bgm = true;

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
                {
                    _event_show = 2;
                    GameData.Instance._item.GetItem(GameData.Instance._episode[_episode][_event_code][_count]._source);
                }
            break;

            case 5 : //효과음                  
                GameData.Instance._sound.Play_EffectSound(GameData.Instance._episode[_episode][_event_code][_count]._source); 
                OnNextEvent();
            break;

            case 6 : //동행자 추가                  
                GameData.Instance._char.MyMemberAdd(GameData.Instance._episode[_episode][_event_code][_count]._source); 
                OnNextEvent();
            break;

            case 7 : //동행자 제거                  
                GameData.Instance._char.MyMemberRemove(GameData.Instance._episode[_episode][_event_code][_count]._source); 
                OnNextEvent();
            break;

            case 8 : //맵내 npc 추가                  
                GameData.Instance._char.MapMemberAdd(GameData.Instance._bg._map, GameData.Instance._episode[_episode][_event_code][_count]._source); 
                OnNextEvent();
            break;

            case 9 : //맵내 npc 제거                 
                GameData.Instance._char.MapMemberRemove(GameData.Instance._bg._map, GameData.Instance._episode[_episode][_event_code][_count]._source); 
                OnNextEvent();
            break;

            case 10 : //튜토리얼 & 안내 메세지
                _event_show = 3;    
                GameData.Instance._char.OnCharActive(false);
                OnTutorial(GameData.Instance._episode[_episode][_event_code][_count]._source);                                   
                //OnNextEvent();
            break;

            case 11 : //에피소드 종료       
                _event_type = 3;                
                OnNextEvent();
            break;

            case 12 : //이동 지역 추가                 
                GameData.Instance._bg.OnMoveMapAdd(GameData.Instance._episode[_episode][_event_code][_count]._source); 
                OnNextEvent();
            break;

            case 13 : //이동 지역 제거                 
                GameData.Instance._bg.OnMoveMapRemove(GameData.Instance._episode[_episode][_event_code][_count]._source); 
                OnNextEvent();
            break;

            case 14 : //조사 완료
                _investigate_complete.OnInvestigateComplete();                               
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
        //GameData.Instance._char.OnCharActive(false);
    }

    public void CloseSearch()
    {
        GameData.Instance._ui.OpenBottom();
        GameData.Instance._bg.OnSearch(false);
        //GameData.Instance._char.OnCharActive(true);

        _search = false;
    }

    public void OnTalk(int member)
    {
        /*        
        if(GameData.Instance._char._map_member.ContainsKey(GameData.Instance._bg._map))
        {
            for(var i = 0; i < GameData.Instance._char._map_member[GameData.Instance._bg._map].Count; i++)
            {
                int member = GameData.Instance._char._map_member[GameData.Instance._bg._map][i];
                for(var j = 0; j < _talk_event[member].Count; j++) GameData.Instance._talk.OnTalkKeyword(_talk_event[member][j]);
            }
        }
        else return;
        */

        for(var j = 0; j < _talk_event[member].Count; j++) GameData.Instance._talk.OnTalkKeyword(_talk_event[member][j]);                

        GameData.Instance._ui.CloseBottom();
        GameData.Instance._ui.OnActiveObject(3, true);
    }

    public void CloseTalk()
    {
        GameData.Instance._ui.OpenBottom();
        GameData.Instance._ui.OnActiveObject(3, false);

        GameData.Instance._talk.EndTalkKeyword();
    }

    public int _item_event_code;
    public void OnItemEventCheck(int code)//아이템 이벤트 체크
    {
        if(_item_event.ContainsKey(code)) 
        {
            _item_event_code = _item_event[code]; 
            return;
        }

        //수집 정보 모두 획득시
        if(_item_event.ContainsKey(-1))
        {
            int item_count = 0;

            foreach(var key in GameData.Instance._item._inventory.Keys)
            {
                if(GameData.Instance._item_data[key]._type == 1) item_count++;
            }
            if(GameData.Instance._item._item_count[_episode][1] == item_count) 
            {
                if(!_event_complete.ContainsKey(_item_event[-1])) _item_event_code = _item_event[-1]; 
            }
        }
    }

    public void OnItemEvent()//특정 아이템 획득에 따른 이벤트 실행
    {
        if(_item_event_code > 0)
        {                
            if(GameData.Instance._talk._talk_trigger) GameData.Instance._talk._talk_trigger = false;
            GameData.Instance._ui.CloseUI_HotKey();
            GameData.Instance._sound.Stop_EffectSound();

            _event_type = 0;
            OnEventStart(_item_event_code);

            _item_event_code = 0;
        }
    }

    public void OnTutorial(int code)
    {     
        GameData.Instance._ui.OpenBottom();

        switch(code)
        {
            case 1 :
                GameData.Instance._gm.OnAlarm(GameData.Instance._announce_data[7]);  
                GameData.Instance._ui.OnTutorialBtn(0, true);
            break;

            case 2 :
                GameData.Instance._gm.OnAlarm(GameData.Instance._announce_data[8]);
                GameData.Instance._ui.OnTutorialBtn(3, true);
            break;

            case 3 :
                GameData.Instance._gm.OnAlarm(GameData.Instance._announce_data[9]);
                GameData.Instance._ui.OnTutorialBtn(2, true);
            break;

            case 4 :
                GameData.Instance._gm.OnAlarm(GameData.Instance._announce_data[10]);
                GameData.Instance._ui.OnTutorialBtn(1, true);
            break;
        }
    }
}
