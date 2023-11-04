using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour ,Damageable
{
    public PlayerStats Status;
    private Animator animator;
    
    private float attakSpeed = 0.3f;
    public bool canAttack=true;

    public float attackRange = 3f;
    public LayerMask targetLayer;

    void Start()
    {
        Status = new PlayerStats(100f, 5f, 10f);
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        if(canAttack&&Input.GetKey(KeyCode.H))Attack();        
        //if(this.Status.Hp<=0) Dead()함수 출력;
    }

    void Move()
     {
        Vector3 movePosition = Vector3.zero;
        float verticalMove = Input.GetAxisRaw("Vertical");
        if(verticalMove != 0) {
            movePosition = new Vector3(0, verticalMove, 0);
            animator.SetBool("isWalk", true);
        }
        else {
            if(Input.GetAxisRaw("Horizontal") < 0) {
                movePosition = Vector3.left;
                GetComponent<SpriteRenderer>().flipX = true;
                animator.SetBool("isWalk", true);
            }
            else if(Input.GetAxisRaw("Horizontal") > 0) {
                movePosition = Vector3.right;
                GetComponent<SpriteRenderer>().flipX = false;
                animator.SetBool("isWalk", true);
            }
            else {
                animator.SetBool("isWalk", false);
            }
        }
        transform.position += movePosition * this.Status.speed * Time.deltaTime;
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
        animator.SetTrigger("Attack");
        canAttack=false;
        StartCoroutine(StartAttackCool());
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
    public IEnumerator StartAttackCool()
    {
    yield return new WaitForSeconds(attakSpeed);
    canAttack = true;
    }
}
