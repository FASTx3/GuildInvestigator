using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TalkKeyword : MonoBehaviour
{
    public int _event_index;
    public Text _txt;
    public void OnSet(int code)
    {
        _event_index = GameData.Instance._eventData[code]._event;
        _txt.text = GameData.Instance._eventData[code]._txt;
    }

    public void OnTalkStart()
    {
        GameData.Instance._ui.CloseBottom();   
        GameData.Instance._ui.OnActiveObject(3, false);

        GameData.Instance._event._event_type = 1;
        GameData.Instance._event.OnEventStart(_event_index);

        GameData.Instance._sound.Play_EffectSound(1);
    }
}
