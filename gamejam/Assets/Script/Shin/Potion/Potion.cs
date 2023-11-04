using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PotionType
{
    Hp,       // 기본값
    Speed,    // 보통 포션
    Strength,  // 특별한 포션
    None 
}

public class Potion : Item
{
    protected PotionType potionType= PotionType.None;

    public PotionType GetPotionType()
    {
        return this.potionType;
    }
}