using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InvestigateComplete : MonoBehaviour
{
    public Image _model;
    public Transform _text;

    public void OnInvestigateComplete()
    {
        GameData.Instance._sound.Stop_BGMSound();
        GameData.Instance._sound.Play_EffectSound(3); 

        gameObject.SetActive(true);

        _model.DOFade(1, 0.25f).SetEase(Ease.Linear);        
        _model.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear).OnComplete(()=>{
            _text.DOScaleY(1f, 0.5f).OnComplete(()=>{
                _text.DOScaleY(0f, 0.25f).SetDelay(0.75f);   
                _model.DOFade(0, 0.25f).SetEase(Ease.Linear).SetDelay(1f);  
                _model.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear).SetDelay(1f).OnComplete(()=>{
                    gameObject.SetActive(false);
                    GameData.Instance._event.OnNextEvent();
                });       
            });        
        });
    }
}
