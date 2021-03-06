﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public GameObject _lock;

    public Image _icon;
    public List<Text> _text = new List<Text>();
    
    public int _index;

    public void OnSet(int code)
    {
        _index = code;

        _icon.sprite = GameData.Instance._item._icon[code-1];

        _text[0].text = GameData.Instance._item_data[code]._name;
        _text[1].text = GameData.Instance._item_data[code]._function;

        if(GameData.Instance._item._inventory.ContainsKey(_index)) _lock.SetActive(false);
    }

    public void OnSellect()
    {
        GameData.Instance._item.OnSellectItem(_index);
    }

    public void OnHint()
    {
        GameData.Instance._gm.OnAlarm("<정보 획득 힌트>\n\n" + GameData.Instance._item_data[_index]._hint);
    }
}
