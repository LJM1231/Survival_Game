﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public string handName; //맨손이나 너클 구분
    public float range; //범위
    public int damage; //공격력
    public float workSpeed; //작업속도
    public float attackDelay; //공격딜레이
    public float attackDelayA; //공격 활성화 시점 (공격 활성화 전 딜레이)
    public float attackDelayB; //공격 비활성화 시점 (공격 비활성화 전 딜레이)

    public Animator anim; // 애니메이션

}