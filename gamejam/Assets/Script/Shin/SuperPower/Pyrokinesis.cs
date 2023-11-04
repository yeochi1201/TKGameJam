using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyrokinesis : SuperPower
{
    public float damageRadius = 1.0f;
    public LayerMask targetLayer;
    void Start()
    {
        coolTime = 5.0f;
        skillDuration = 2.0f;
        powerType = PowerType.Pyrokinesis;
        skillDamage=20;
        targetLayer = 1 << 3;
        
    }

    protected override void Skill()
    {
        base.Skill();
        StartCoroutine(SkillCoolTime());

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; 

        GameObject fireBullet = Instantiate(attackSkillPrefab, mousePosition, Quaternion.identity);



    }
}
