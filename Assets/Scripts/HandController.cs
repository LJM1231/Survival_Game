using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //현재 장착된 Hand형 타입 무기(?)
    [SerializeField]
    private Hand currentHand;

    //공격중??
    private bool isAttack = false; // false면 공격가능, true면 좌클릭해도 공격 불가능
    private bool isSwing = false;

    private RaycastHit hitInfo; // 공격할 때 닿은 정보 저장


    // Update is called once per frame
    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if(!isAttack)
        }
    }
}
