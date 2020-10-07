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
    public int _pop_code;
    public void OnActiveObject(int code, bool active)
    {
        if(active) _pop_code = code;
        _pop[code].SetActive(active);
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
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if(_pop_code < 1) return;
            if(GameData.Instance._event._event_progress) return;

            if(GameData.Instance._event._search) GameData.Instance._event.CloseSearch();

            if(GameData.Instance._item._announce._trigger)
            {
                GameData.Instance._item._announce.OnAnnounceDone();
                return;
            }

            switch(_pop_code)
            {
                case 1 :
                    OnActiveObject(1, false);
                break;

                case 2 :
                    OnActiveObject(2, false);
                break;

                case 3 :
                    GameData.Instance._event.CloseTalk();
                break;

                case 4 :
                    GameData.Instance._bg.CloseMap();
                break;

                case 5 :
                    GameData.Instance._item.CloseMix();
                break;

                case 6 :
                    GameData.Instance._item.CloseItem();
                    if(GameData.Instance._item._mix_window) return;
                break;
            }

            _pop_code = -1;
        }
    }   
}
