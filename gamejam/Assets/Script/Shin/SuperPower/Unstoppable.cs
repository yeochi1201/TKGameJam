using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unstoppable : SuperPower
{   
    
    // Start is called before the first frame update
    void Start()
    {
        coolTime=5.0f;
        skillDuration=2.0f;
        powerType=PowerType.Unstoppable;
    }
    protected override void Skill()
    {
        base.Skill();
        StartCoroutine(SkillCoolTime());
        isUnstoppable = true;
        StartCoroutine(SkillActiveTime());
    }
    protected override IEnumerator SkillCoolTime()
    {
    yield return new WaitForSeconds(coolTime);
    canSkill = true;
    }
    protected override IEnumerator SkillActiveTime()
    {
    yield return new WaitForSeconds(skillDuration);
    isUnstoppable = false;
    }
}
