using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour ,Damageable
{
    public PlayerStats Status;
    private Animator animator;
    
    private float attakSpeed = 0.3f;
    public bool canAttack=true;

    public bool canAddItem = true;
    private float AdditemCoolTime = 1.0f;

    private float eatTime=1.0f;
    public bool Eating=false;

    public float attackRange = 0.5f;
    public LayerMask targetLayer;

    Vector3 mousePosition;
    Vector3 direction;

    public SuperPower superPower;

    void Start()
    {
        Status = new PlayerStats(100f, 5f, 10f);
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - this.transform.position;
        if(!Eating)Move();
        if (direction.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
             GetComponent<SpriteRenderer>().flipX = false;
        }
        if(canAttack&&Input.GetMouseButtonDown(0))Attack();        
        //if(this.Status.Hp<=0) Dead()함수 출력;
    }


    void Move()
     {
        Vector3 movePosition = Vector3.zero;
        float verticalMove = Input.GetAxisRaw("Vertical");
        // float HorizontalMove = Input.GetAxisRaw("Horizontal");
        if(verticalMove != 0) {
            movePosition += new Vector3(0, verticalMove, 0);
            animator.SetBool("isWalk", true);
        }
        if(Input.GetAxisRaw("Horizontal") < 0) {
                movePosition += Vector3.left;
                GetComponent<SpriteRenderer>().flipX = true;
                animator.SetBool("isWalk", true);
        }
        else if(Input.GetAxisRaw("Horizontal") > 0) {
            movePosition += Vector3.right;
            GetComponent<SpriteRenderer>().flipX = false;
            animator.SetBool("isWalk", true);
        }
        else {
            animator.SetBool("isWalk", false);
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
                    Eating=true;
                    StartCoroutine(StartEating());
                    StartCoroutine(StartAddItemCooldown());
                    Potion item = other.GetComponent<Potion>();
                    ConsumeItem(item);
                    Destroy(other.gameObject);  
                }
                break;
            case "SuperPower":
                if(Input.GetKey(KeyCode.F))
                {
                    Eating=true;
                    StartCoroutine(StartEating());
                    StartCoroutine(StartAddItemCooldown());
                    SuperPower item = other.GetComponent<SuperPower>();
                    GetSuperPower(item);
                    Destroy(other.gameObject);  
                    //other.gameObject.SetActive(false);
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
    void GetSuperPower(SuperPower item)
    {
        SuperPower superPower = item.GetComponent<SuperPower>();
        if (superPower != null)
        {
            SuperPower newSuperPower = gameObject.AddComponent<SuperPower>();
            newSuperPower=superPower;
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
    public IEnumerator StartAddItemCooldown()
    {
    yield return new WaitForSeconds(AdditemCoolTime);
    canAddItem = true;
    }
    public IEnumerator StartEating()
    {
    yield return new WaitForSeconds(eatTime);
    Eating = false;
    }
}
