using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameOver : MonoBehaviour
{
    bool _trigger;
    public Text _text;
    void OnEnable()
    {
        _trigger = false;
        _text.DOFade(1, 1).SetEase(Ease.Linear).OnComplete(()=>{
            _trigger = true;
            _text.DOFade(0,0).SetDelay(2.5f).SetId("end").OnComplete(()=>{
                gameObject.SetActive(false);
                GameData.Instance._ui.OnActiveObject(0, true);

                GameData.Instance._sound.Play_BGMSound(0);
            });
        });
    }

    public void OnSkip()
    {
        if(!_trigger) return;

        DOTween.Complete("end");
    }
}
