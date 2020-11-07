using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkMember : MonoBehaviour
{
    public int _index;
    public Text _text;
    public Image _img;
    public void OnSet(int code)
    {
        _index = code;

        _img.sprite = GameData.Instance._char._char_sprite[code];
        _text.text = GameData.Instance._char_data[code];
    }

    public void OnTalk()
    {
        GameData.Instance._talk.OpenTalk(_index);

        GameData.Instance._sound.Play_EffectSound(1);
    }
}
