using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._sound = this;
    }

    public List<AudioSource> _play_sound = new List<AudioSource>();//음악 재생
    public bool _playBGM = true; // 배경음 사용 여부
	public bool _playEFF = true; //효과음 사용 여부
    public List<AudioClip> _bgm_sound = new List<AudioClip>(); //배경음 목록
    public List<AudioClip> _eff_sound = new List<AudioClip>(); //효과음 목록
	public List<AudioClip> _voice_sound = new List<AudioClip>(); //효과음 목록

	public void SetSoundInit()
	{
		SetSoundMute();
		SetEffMute();
	}

	int _bgm_code;

    public void Play_BGMSound(int num){
        if(_playBGM)
		{
			//if( _bgm_sound[num] != null)
			if(num < _bgm_sound.Count)
			{
				if(num < 0)	Stop_BGMSound();
				else 
				{
					if(_bgm_code == num) return;

					_bgm_code = num;

					_play_sound[0].clip = _bgm_sound[num];
					_play_sound[0].Play();
				}
			}
		}
    }

    public void Play_EffectSound(int num){
        if(_playEFF)
		{
			if( _eff_sound[num] != null)
			{
				_play_sound[1].PlayOneShot(_eff_sound[num]);
			}
		}
    }

	public void Stop_BGMSound(){
        if(_playBGM)
		{
			_bgm_code = -1;
			_play_sound[0].Stop();
		}
    }

	public void Stop_EffectSound(){
        if(_playEFF)
		{
			_play_sound[1].Stop();
		}
    }
    
	public void SoundMute()
	{
		if(PlayerPrefs.GetInt("SoundMute") != 1 || !PlayerPrefs.HasKey("SoundMute"))
		{
		PlayerPrefs.SetInt("SoundMute",1);
		}
		else
		{
		PlayerPrefs.SetInt("SoundMute",0);
		}
		SetSoundMute();
	}

   public void SetSoundMute()
   {
		if(PlayerPrefs.GetInt("SoundMute") == 0 || !PlayerPrefs.HasKey("SoundMute"))
		{// 음소거 상태 아님.
			_play_sound[0].mute = false;
			/*
			if(GameData.Instance._uimn!=null ){
				GameData.Instance._uimn.OnObjActive(11,0,false);
				GameData.Instance._uimn.OnText(11,0, LocalizationMN.Get("Game.Text151"));//끄기
			} 
			 */			
		}
		else
		{// 음소거 상태. 
			_play_sound[0].mute = true;
			/*
			if(GameData.Instance._uimn!=null ){
				GameData.Instance._uimn.OnObjActive(11,0,true);
				GameData.Instance._uimn.OnText(11,0, LocalizationMN.Get("Game.Text152"));//켜기
			}          
			*/
		}
   }

	public void EffMute()
	{
		if(PlayerPrefs.GetInt("EffMute") != 1 || !PlayerPrefs.HasKey("EffMute"))
		{
		PlayerPrefs.SetInt("EffMute",1);
		}
		else
		{
		PlayerPrefs.SetInt("EffMute",0);
		}
		SetEffMute();
	}

	public void SetEffMute()
	{
	  if(PlayerPrefs.GetInt("EffMute") == 0 || !PlayerPrefs.HasKey("EffMute"))
      {// 음소거 상태 아님.     
         _play_sound[1].mute = false;
		 /*
         if(GameData.Instance._uimn!=null ){
			 GameData.Instance._uimn.OnObjActive(11,1,false);
			 GameData.Instance._uimn.OnText(11,1,LocalizationMN.Get("Game.Text151"));//끄기
			 _playEFF = true;
		 }
		 */
      }
      else
      {// 음소거 상태.           
         _play_sound[1].mute = true;
		 /*
         if(GameData.Instance._uimn!=null ){
			 GameData.Instance._uimn.OnObjActive(11,1,true);
			 GameData.Instance._uimn.OnText(11,1,LocalizationMN.Get("Game.Text152"));//켜기
			 _playEFF = false;
		 }    
		 */     
      }
    }
}
