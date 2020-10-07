using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixSlot : MonoBehaviour
{    
    public int _slot_code;//슬롯 번호

    public int _index;//선택된 정보 인덱스
    public Image _icon;
    public List<Text> _text = new List<Text>();
    
    public List<GameObject> _obj = new List<GameObject>();

    public void OnReset()
    {        
        _index = 0;
        
        _obj[0].SetActive(true);
        _obj[1].SetActive(false);
    }

    public void OnSet(int code)
    {
        _index = code;
        
        //_icon.sprite = 

        _text[0].text = GameData.Instance._item_data[code]._name;
        _text[1].text = GameData.Instance._item_data[code]._function;

        _obj[0].SetActive(false);
        _obj[1].SetActive(true);
    }


    
    public void LoadItem()
    {
        GameData.Instance._item.OpenItem(_slot_code);
    }
}
