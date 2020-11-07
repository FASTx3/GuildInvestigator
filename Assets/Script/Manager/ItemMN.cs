using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class ItemMN : MonoBehaviour
{
    private JsonData _jsonList;
    public IEnumerator SetItemData()
    {        
        yield return StartCoroutine(LoadItemData());
    }

    public IEnumerator LoadItemData()
	{
		TextAsset t = (TextAsset)Resources.Load("item", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDataItem(t.text));

        t = (TextAsset)Resources.Load("mix_item", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetMixItem(t.text));
	}

    public IEnumerator SetDataItem(string jsonString)
	{
        _item.Clear();
        GameData.Instance._item_data.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);

        for(var i = 0; i< _jsonList.Count;i++)
        {        
            GameData.Instance._itemData._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._itemData._type = System.Convert.ToInt32(_jsonList[i]["type"].ToString());

            GameData.Instance._itemData._name = _jsonList[i]["name"].ToString();
            GameData.Instance._itemData._function = _jsonList[i]["function"].ToString();
            GameData.Instance._itemData._hint = _jsonList[i]["hint"].ToString();

            GameData.Instance._itemData._episode = System.Convert.ToInt32(_jsonList[i]["episode"].ToString());
            GameData.Instance._itemData._array = System.Convert.ToInt32(_jsonList[i]["array"].ToString());

            GameData.Instance._item_data.Add(GameData.Instance._itemData._index, GameData.Instance._itemData);

            //에피소드 별 아이템 타입 갯수
            if(!_item_count.ContainsKey(GameData.Instance._itemData._episode)) _item_count.Add(GameData.Instance._itemData._episode, new Dictionary<int, int>());
            if(!_item_count[GameData.Instance._itemData._episode].ContainsKey(GameData.Instance._itemData._type)) _item_count[GameData.Instance._itemData._episode].Add(GameData.Instance._itemData._type, 0);  
            _item_count[GameData.Instance._itemData._episode][GameData.Instance._itemData._type]++;


            //에피소드 별 수집해야할 정보 아이템 정렬
            if(GameData.Instance._itemData._type == 1 || GameData.Instance._itemData._type == 3)
            {
                if(!_item.ContainsKey(GameData.Instance._itemData._episode)) _item.Add(GameData.Instance._itemData._episode, new Dictionary<int, int>());
                if(!_item[GameData.Instance._itemData._episode].ContainsKey(GameData.Instance._itemData._array)) _item[GameData.Instance._itemData._episode].Add(GameData.Instance._itemData._array, GameData.Instance._itemData._index);                
            }
        }

        yield return null;  
    }

    public IEnumerator SetMixItem(string jsonString)
	{
        int _mix_item = 0;
        int _item_index = 0;

        GameData.Instance._mix_item_data.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);

        for(var i = 0; i< _jsonList.Count;i++)
        {        
            _mix_item = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            _item_index = System.Convert.ToInt32(_jsonList[i]["item"].ToString());

            if(!GameData.Instance._mix_item_data.ContainsKey(_mix_item)) GameData.Instance._mix_item_data.Add(_mix_item, new List<int>());
            GameData.Instance._mix_item_data[_mix_item].Add(_item_index);
        }

        yield return null;  
    }

    public Dictionary<int, Dictionary<int, int>> _item = new Dictionary<int, Dictionary<int, int>>();
    public Dictionary<int, Dictionary<int, int>> _item_count = new Dictionary<int, Dictionary<int, int>>();

    void Awake()
    {
        GameData.Instance._item = this;
    }

    public Dictionary<int, int> _inventory = new Dictionary<int, int>();

    public Announce _announce;
    public void GetItem(int code)
    {
        string txt = "";

        switch(GameData.Instance._item_data[code]._type)
        {
            case 1 ://획득 정보
                txt = string.Format(GameData.Instance._announce_data[1], GameData.Instance._item_data[code]._name);
            break;

            case 2 ://소지품
                txt = string.Format(GameData.Instance._announce_data[2], GameData.Instance._item_data[code]._name);
            break; 

            case 3 ://분석 정보
                txt = string.Format(GameData.Instance._announce_data[1], GameData.Instance._item_data[code]._name);
            break;
        }     
        
        _announce.OnAnnounceItem(GameData.Instance._item_data[code]._name, GameData.Instance._item_data[code]._function, txt);

        if(!_inventory.ContainsKey(code)) _inventory.Add(code, 1);
        else _inventory[code] = 1;

        GameData.Instance._event.OnItemEventCheck(code);

        GameData.Instance._sound.Play_EffectSound(2);
    }


    public bool _mix_window;//조합창 오픈 여부
    public int _mix_slot_sel;//선택되 조합 슬롯
    public List<MixSlot> _mix_slot = new List<MixSlot>();
    public Transform _item_list_parent;
    public Item _item_list_ori;
    public Dictionary<int, Item> _item_list = new Dictionary<int, Item>(); 

    public void OpenItem(int code)//수집한 정보창 열기
    {
        _mix_slot_sel = code-1; //0 : 일반 아이템 오픈, 1 : 1번 슬롯, 2 : 2번 슬롯

        OnItem();
        
        if(_mix_window) GameData.Instance._ui.OnActiveObject(5, false);
        else GameData.Instance._ui.OnActiveObject(7, true);

        GameData.Instance._ui.OnActiveObject(6, true);

        GameData.Instance._sound.Play_EffectSound(1);
    }

    public void CloseItem()//수집한 정보창 닫기
    {
        GameData.Instance._ui.OnActiveObject(6, false);

        if(_mix_window) GameData.Instance._ui.OnActiveObject(5, true);
        else GameData.Instance._ui.OnActiveObject(7, false);

        for(var i = 1; i < _item_list.Count+1; i++) Destroy(_item_list[i].gameObject);

        GameData.Instance._sound.Play_EffectSound(1);
    }

    public void OnItem()
    {
        _item_list_parent.localPosition = Vector3.zero;

        _item_list.Clear();

        for(var i = 1; i < _item[GameData.Instance._event._episode].Count+1; i++)
        {
            _item_list.Add(i, Instantiate(_item_list_ori, _item_list_parent));
            _item_list[i].transform.localScale = Vector3.one;
            _item_list[i].OnSet(_item[GameData.Instance._event._episode][i]);
        }
    }


    public void OpenMix()//정보 조합창 열기
    {
        _mix_window = true;

        GameData.Instance._ui.OnActiveObject(7, true);
        GameData.Instance._ui.OnActiveObject(5, true);    

        GameData.Instance._sound.Play_EffectSound(1);    
    }

    public void CloseMix()//정보 조합창 닫기
    {
        _mix_window = false;

        _mix_slot[0].OnReset();
        _mix_slot[1].OnReset();

        GameData.Instance._ui.OnActiveObject(5, false);
        GameData.Instance._ui.OnActiveObject(7, false);

        GameData.Instance._sound.Play_EffectSound(1);
    }

    public void OnSellectItem(int index)
    {
        if(!_mix_window) return;

        if(_mix_slot[0]._index > 0) _mix_slot[_mix_slot_sel].OnSet(index);
        else _mix_slot[0].OnSet(index);

        CloseItem();
    }

    public void OnMixItem()
    {   
        if(_mix_slot[0]._index == 0 || _mix_slot[1]._index == 0) //조합할 정보가 없습니다.
        {
            _announce.OnAnnounce(GameData.Instance._announce_data[3]);
            GameData.Instance._sound.Play_EffectSound(5);
            return;
        }
        if(_mix_slot[0]._index == _mix_slot[1]._index) //같은 정보끼리는 조합 할 수 없습니다.
        {
            _announce.OnAnnounce(GameData.Instance._announce_data[4]);
            GameData.Instance._sound.Play_EffectSound(5);
            return;
        }

        int _mix_complete = 0;

        foreach(var i in GameData.Instance._mix_item_data.Keys)
        {
            if(GameData.Instance._mix_item_data[i].Contains(_mix_slot[0]._index) && GameData.Instance._mix_item_data[i].Contains(_mix_slot[1]._index)) //아이템 조합 가능
                _mix_complete = i;
        }

        if(_mix_complete > 0)
        {
            if(_inventory.ContainsKey(_mix_complete))  //이미 획득한 정보입니다.
            {
                _announce.OnAnnounce(GameData.Instance._announce_data[5]);
                GameData.Instance._sound.Play_EffectSound(5);
            }
            else
            {
                _mix_slot[0].OnReset();
                _mix_slot[1].OnReset();

                GetItem(_mix_complete);
            }
        }
        else //아이템 조합 불가능
        {
            _announce.OnAnnounce(GameData.Instance._announce_data[6]);
            GameData.Instance._sound.Play_EffectSound(5);
        }
    }
}
