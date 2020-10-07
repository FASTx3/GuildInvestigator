using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public int _type;//퀘스트 타입 (1 : 채집, 2 : 토벌)
    public int _place;//퀘스트 장소
    public int _goal;//퀘스트 목표
    public int _goal_count;//퀘스트 목표 횟수
    public int _reward;//보상 종류
    public int _reward_count;//보상양
    public int _date;//마감일
    public int _member;//담당 모험가
    public int _client;//의뢰자
}
