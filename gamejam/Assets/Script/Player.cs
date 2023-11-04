using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour ,Damageable
{
    public PlayerStats Status;
    
    public float moveSpeed = 5f; // 이동 속도

    public float attackRange = 3f;
    public LayerMask targetLayer;

    void Start()
    {
        Status = new PlayerStats(100, 5f, 10);
    }
    void Update()
    {
        // X축 및 Y축 움직임 입력을 받아 이동
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
        
        if (Input.GetKeyDown(KeyCode.F))
        {   
            Debug.Log("공격중");
            Attack();
        }
    }
    void Attack()
    {
        // 플레이어의 현재 위치와 방향을 기준으로 공격 범위 내의 대상을 찾음
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

        // 찾은 대상을 순회하면서 처리
        foreach (Collider2D target in hitTargets)
        {
            Damageable damageable = target.GetComponent<Damageable>();
            if (damageable != null)
            {
                Debug.Log("머지");
                damageable.HitDamage(Status.attackDamage); 
            }
        }
    }
    public void HitDamage(int damage)
    {
        // 여기서는 단순히 오브젝트를 파괴하도록 예시를 들었지만, 원하는 동작으로 수정 가능
        this.Status.Hp-=damage;
    }
}
