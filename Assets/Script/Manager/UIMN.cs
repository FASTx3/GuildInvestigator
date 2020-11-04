using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIMN : MonoBehaviour
{   
    public Image _fade;
    void Awake()
    {
        GameData.Instance._ui = this;
    }

    public List<GameObject> _pop = new List<GameObject>();
    public List<int> _pop_code = new List<int>();
    public void OnActiveObject(int code, bool active)
    {        
        _pop[code].SetActive(active);

        if(active) _pop_code.Add(code);
        else _pop_code.Remove(code);
    }

    public GameObject _bottom;

    public void OpenBottom()
    {
        _bottom.SetActive(true);
        /*
        _bottom.DOLocalMoveY(-315, 0.2f).SetEase(Ease.Linear).OnComplete(()=>{

        });
        */
    }

    public void CloseBottom()
    {
        _bottom.SetActive(false);
        /*
        _bottom.DOLocalMoveY(-405, 0.2f).SetEase(Ease.Linear).OnComplete(()=>{

        });
        */
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape)) CloseUI_HotKey();
    }

    public void CloseUI_HotKey()
    {   
        if(GameData.Instance._event._event_progress) 
        {
            if(GameData.Instance._event._event_show == 1) GameData.Instance._talk._talk.OnTalkDone();
            else if(GameData.Instance._event._event_show == 2) GameData.Instance._item._announce.OnAnnounceDone();
            else if(GameData.Instance._event._event_show == 3) GameData.Instance._gm._alarm.CloseAlarm();

            return;
        }
        if(GameData.Instance._event._search) GameData.Instance._event.CloseSearch();

        if(GameData.Instance._item._announce._trigger)
        {
            GameData.Instance._item._announce.OnAnnounceDone();
            return;
        }
        
        if(_pop_code.Count < 1) return;

        switch(_pop_code[_pop_code.Count-1])
        {
            case 1 :
                OnActiveObject(1, false);
            break;

            case 2 :
                OnActiveObject(2, false);
            break;

            case 3 :
                GameData.Instance._talk.CloseTalk();
            break;

            case 4 :
                GameData.Instance._bg.CloseMap();
            break;

            case 5 :
                GameData.Instance._item.CloseMix();
            break;

            case 6 :
                GameData.Instance._item.CloseItem();
            break;

            case 9 :
                GameData.Instance._gm._alarm.CloseAlarm();
            break;

            case 10 :
                GameData.Instance._talk.CloseTalkMember();
            break;
        }
    }  

    public List<Button> _bottom_btn = new List<Button>(); //하단 버튼

    public void OnTutorialSet()
    {
        for(var i = 0; i < _bottom_btn.Count; i++) OnTutorialBtn(i, false);
    }
    public void OnTutorialBtn(int code, bool show)
    {
        _bottom_btn[code].interactable = show;
    }
}
