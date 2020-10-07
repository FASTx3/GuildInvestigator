using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._char = this;
    }

    public List<Sprite> _char_sprite = new List<Sprite>();

    public Image _char_model;

    public void OnChar(int code)
    {
        if(code < 0) 
        {
            _char_model.gameObject.SetActive(false);
        }
        else
        {
            _char_model.sprite = _char_sprite[code];
            //_char_model.SetNativeSize();
            _char_model.rectTransform.sizeDelta = _char_sprite[code].rect.size;
            
            _char_model.gameObject.SetActive(true);
        }
    }

    public void OnCharActive(bool active)
    {
        _char_model.gameObject.SetActive(active);
    }
}
