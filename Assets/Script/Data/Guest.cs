using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour
{
    public int _want_type;//퀘스트 타입 (1 : 채집, 2 : 토벌)
    public int _want_place;//퀘스트 장소
    public int _want_goal;//퀘스트 목표
    public int _want_goal_count;//퀘스트 목표 횟수
    public int _want_reward;//보상 종류
    public int _want_reward_count;//보상양
    public int _want_date;//마감일

    public void OnWantSet()
    {
        _want_type = 0;
        _want_place = 0;
        _want_goal = 0;
        _want_goal_count = 0;
        _want_reward = 0;
        _want_reward_count = 0;
        _want_date = 0;
    }
}
