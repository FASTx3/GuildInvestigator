using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Talk : MonoBehaviour
{
    public List<Text> _txt = new List<Text>();
    public bool _talking;

    public void OnTalk(string name, string talk)
    {
        _txt[0].text = name;
        _txt[1].text = "";
        if(!gameObject.activeSelf) gameObject.SetActive(true);

        _talking = true;

        _txt[1].DOText(talk, talk.Length*0.05f).SetEase(Ease.Linear).SetId("talk").OnComplete(()=>{
            _talking = false;
        });
    }

    public void OnTalkDone()
    {
        if(_talking)
        {
            DOTween.Complete("talk");
            _talking = false;
        }            
        else
        {
            CloseTalk();              
            GameData.Instance._event.OnNextEvent(); 
        }
    }

    public void CloseTalk()
    {
        if(gameObject.activeSelf) gameObject.SetActive(false);
        _txt[0].text = "";
        _txt[1].text = "";
    }
}
