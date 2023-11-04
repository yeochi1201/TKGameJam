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
        Status = new PlayerStats(100f, 5f, 10f);
    }
    void Update()
    {
        // X축 및 Y축 움직임 입력을 받아 이동
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
        
        if (Input.GetKeyDown(KeyCode.H))
        {   
            Debug.Log("공격중");
            Attack();
        }
        //if(this.Status.Hp<=0) Dead()함수 출력;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        switch(other.tag)
        {
            case "Potion":
                if(Input.GetKey(KeyCode.F))

                {
                    Potion item = other.GetComponent<Potion>();
                    ConsumeItem(item);
                    Destroy(other.gameObject);  
                }
            break;
        }
    }
    void ConsumeItem(Potion item)
    {
        switch (item.GetPotionType())
        {
        case PotionType.Hp:
            Status.Hp += 10;  
            Debug.Log("체력 물약을 먹었습니다.");
            break;
        case PotionType.Speed:
            Status.speed += 1f; 
            Debug.Log("스피드 물약을 먹었습니다.");
            break;
        case PotionType.Strength:
            Status.attackDamage += 2f; 
            Debug.Log("힘 물약을 먹었습니다.");
            break;    
        }   
    }
    void Attack()
    {
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);
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
    public void HitDamage(float damage)
    {
        this.Status.Hp-=damage;
    }
}
