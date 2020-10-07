using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fade : MonoBehaviour
{
    public Image _fade;
    public GameObject _lock;

    public delegate void Call();
    public void OnFade(Call _call_1, Call _call_2)
    {
        _lock.SetActive(true);

        _fade.DOFade(1, 0.5f).SetEase(Ease.Linear).OnComplete(()=>{
            
            if(_call_1 != null) _call_1();

            _fade.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(()=>{                
                _lock.SetActive(false);
                if(_call_2 != null) _call_2();
            });            
        });
    }

    public void OnFadeIn(Call _call)
    {   
        if(_lock != null) _lock.SetActive(true);

        _fade.DOFade(1 ,0.1f).SetEase(Ease.Linear).OnComplete(()=>{
            if(_lock != null) _lock.SetActive(false);
            if(_call != null) _call();
        });
    }

    public void OnFadeOut(Call _call)
    {
        if(_lock != null) _lock.SetActive(true);

        _fade.DOFade(0 ,0.1f).SetEase(Ease.Linear).OnComplete(()=>{
            if(_lock != null) _lock.SetActive(false);
            if(_call != null) _call();
        });
    }

    public void OnSparkle(Call _call)
    {
         _lock.SetActive(true);

        _fade.DOFade(1 ,0.1f).SetEase(Ease.Linear).OnComplete(()=>{
            _fade.DOFade(0 ,0.1f).SetEase(Ease.Linear).OnComplete(()=>{
                _lock.SetActive(false);
                if(_call != null) _call();
            });
        });
    }
}
