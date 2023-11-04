using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerType
{
    Unstoppable,
    None 
}
public class SuperPower : Item
{
    protected float coolTime;
    public bool canSkill=true;

    protected float skillDuration;

    public bool isUnstoppable=false;
    protected bool isGhost=false;

    protected PowerType powerType= PowerType.None;

    protected void Update()
    {
        if(canSkill&&Input.GetKey(KeyCode.Space))Skill();
    }

    protected virtual void Skill()
    {
        canSkill=false;
        Debug.Log("스킬 발동!!");
    }

    public PowerType GetPowerType()
    {
        return this.powerType;
    }

    protected virtual IEnumerator SkillCoolTime()
    {
    yield return new WaitForSeconds(coolTime);
    
    }
    protected virtual IEnumerator SkillActiveTime()
    {
    yield return new WaitForSeconds(skillDuration);
    
    }

}
