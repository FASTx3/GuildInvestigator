using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Announce : MonoBehaviour
{
    public int _announce_type;//아나운스 종류 0 : 기본, 1 : 아이템 획득

    public bool _trigger;//ui 매니저에서 아나운스 사용 여부 확인 및 UI 더블 터치 방지
    public bool _item_trigger;//아이템 획득 연출 진행 여부
    public bool _announcing;//메세지 애니메이션 진행 여부
    
    
    public GameObject _announce;
    public Image _announce_item;
    public Transform _announce_pop;
    public List<Text> _announce_txt = new List<Text>();

    //기본 아나운스
    public void OnAnnounce(string txt)
    {
        _announce_type = 0;

        _trigger = true;
        _announcing = true;        

        _announce_txt[2].text = "";
        gameObject.SetActive(true);

        _announce.SetActive(true);

        _announce_txt[2].DOText(txt, txt.Length*0.05f).SetEase(Ease.Linear).SetId("announce").OnComplete(()=>{
            _announcing = false;
        });
    }

    public void CloseAnnounce()
    {
        gameObject.SetActive(false);
        _announce.SetActive(false);
        _announce_pop.localPosition = new Vector3(1280, 0, 0);

        _announce_txt[0].text = "";
        _announce_txt[1].text = "";
        _announce_txt[2].text = "";
        
        GameData.Instance._event.OnNextEvent();     
    }

    //아이템 획득 아나운스
    public void OnAnnounceItem(string txt1, string txt2, string txt3)
    {
        _announce_type = 1;

        _trigger = true;
        _item_trigger = true;
        _announcing = true;        
       
        _announce_txt[0].text = txt1;
        _announce_txt[1].text = txt2;
        _announce_txt[2].text = "";

        gameObject.SetActive(true);

        _announce_pop.DOLocalMoveX(0, 0.25f).SetEase(Ease.Linear).OnComplete(()=>{
            _item_trigger = false;
            _announce.SetActive(true);
            _announce_txt[2].DOText(txt3, txt3.Length*0.05f).SetEase(Ease.Linear).SetId("announce").OnComplete(()=>{
                _announcing = false;
            });
        });
    }

    public void CloseAnnounceItem()
    {
        _announce_pop.DOLocalMoveX(-1280, 0.25f).SetEase(Ease.Linear).OnComplete(()=>{
            CloseAnnounce();            
        });
    }

    public void OnAnnounceDone()
    {
        if(_item_trigger) return;

        if(_announcing)
        {
            DOTween.Complete("announce");
            _announcing = false;
        }            
        else
        {
            if(_trigger)     
            {
                if(_announce_type == 0) CloseAnnounce();
                else if(_announce_type == 1)CloseAnnounceItem();                 
                _trigger = false;
            }                 
        }
    }
}
