using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseCaster : SuperPower
{
    public float auraRange = 6.0f;
    public LayerMask targetLayer;
    public GameObject AuraPrefab;
    void Start()
    {
        coolTime = 15.0f;
        skillDuration = 5.0f;
        powerType = PowerType.CurseCaster;
        skillDamage=5;
        targetLayer = 1 << 3;
    }
    protected override void Skill()
    {
        base.Skill();
        StartCoroutine(SkillCoolTime());


    }
    protected override IEnumerator SkillCoolTime()
    {
        yield return new WaitForSeconds(coolTime);
        canSkill = true;
    }
}
