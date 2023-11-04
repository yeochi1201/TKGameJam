using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrokinetic : SuperPower
{   
    public float damageRadius = 1.0f;
    public LayerMask targetLayer;

    // Start is called before the first frame update
    void Start()
    {
        coolTime = 5.0f;
        skillDuration = 2.0f;
        powerType = PowerType.Electrokinetic;
        skillDamage=20;
        targetLayer = 1 << 3;
        
    }
    protected override void Skill()
    {
        base.Skill();
        StartCoroutine(SkillCoolTime());

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // 마우스 좌표의 z 값을 0으로 고정 (2D 게임이라면)
        
        // Instantiate 파티클 시스템
        GameObject lightningEffect = Instantiate(attackSkillPrefab, mousePosition, Quaternion.identity);


        // 파티클 시스템을 파괴 전에 시작
        ParticleSystem particleSystem = lightningEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        // 일정 시간 후에 파티클 시스템 파괴
        DealDamageToEnemies(lightningEffect);
        Destroy(lightningEffect, 1.0f);
    }
    private void DealDamageToEnemies(GameObject lightningEffect)
    {
        // hitPosition 주변의 적들에게 데미지를 입힙니다.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(lightningEffect.transform.position, damageRadius, targetLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Damageable damageable = enemy.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.HitDamage(skillDamage);
            }
        }
    }

    protected override IEnumerator SkillCoolTime()
    {
        yield return new WaitForSeconds(coolTime);
        canSkill = true;
    }
}