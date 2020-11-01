using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alarm : MonoBehaviour
{
    public Text _text;

    public void OnSet(string txt)
    {        
        _text.text= txt;
        GameData.Instance._ui.OnActiveObject(9, true);
    }

    public void CloseAlarm()
    {                
        GameData.Instance._ui.OnActiveObject(9, false);
        _text.text= "";

        GameData.Instance._event.OnNextEvent();
    }
}
