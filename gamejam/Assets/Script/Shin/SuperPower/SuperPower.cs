using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPower : Item
{
    protected float coolTime;
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))Skill();
    }

    private void Skill()
    {
        Debug.Log("스킬 발동!!!!!");
    }

}
