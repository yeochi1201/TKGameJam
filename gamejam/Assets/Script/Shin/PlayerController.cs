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

    SuperPower mySuperPower;

    public bool isSeeRight;
    public bool isDead=false;

    public GameObject AttackBox;

    public GameObject[] items; // 드롭할 아이템 프리팹 배열

    void Start()
    {
        Status = new PlayerStats(100f,100f, 5f, 10f);
        animator = GetComponent<Animator>();
        isSeeRight=true;
        AttackBox.SetActive(false);
    }
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - this.transform.position;
        if(!isDead&&!Eating)Move();
        if (direction.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
             GetComponent<SpriteRenderer>().flipX = false;
        }
        if(canAttack&&Input.GetMouseButtonDown(0));     
        if(this.Status.Hp<=0) Daed();
        if(Input.GetKey(KeyCode.T))this.Status.Hp=0;
        if(canAttack&&Input.GetKey(KeyCode.F))Attack();
    }

    private void Move()
    {
        Vector3 movePosition = Vector3.zero;
        float verticalMove = Input.GetAxisRaw("Vertical");
        if (verticalMove < 0)
            isSeeRight=false;
        else if (verticalMove > 0)
            isSeeRight=true;
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
                if(Input.GetKey(KeyCode.H))
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
                if(mySuperPower==null&&Input.GetKey(KeyCode.F))
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
    private void OnTriggerEnter2D(Collider2D other)
     {
      if (other.CompareTag("RestricArea"))
      {
        this.Status.Hp=0;
      }
      if(other.CompareTag("Attack")&& AttackBox!=other.gameObject)
      {
        PlayerController pl = other.GetComponentInParent<PlayerController>();
        //Status.Hp-=pl.Status.attackDamage;
        HitDamage(pl.Status.attackDamage)
      }
    } 
    private void ConsumeItem(Potion item)
    {
        StartCoroutine(StartAddItemCooldown());
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

    private void GetSuperPower(SuperPower item)
    {
        switch(item.GetPowerType())
        {
            case PowerType.Unstoppable:
                mySuperPower=gameObject.AddComponent<Unstoppable>();
                break;
            case PowerType.Electrokinetic:
                mySuperPower=gameObject.AddComponent<Electrokinetic>();
                break;
            case PowerType.Pyrokinesis:
                mySuperPower=gameObject.AddComponent<Pyrokinesis>();
                break;
        }
        mySuperPower.isEquipted=true;
        if(item.attackSkillPrefab!=null)
        {
            mySuperPower.attackSkillPrefab=item.attackSkillPrefab;
        }   
    }
    public void Attack()
    {
        animator.SetTrigger("Attack");
        canAttack=false;
        StartCoroutine(StartAttackCool());
        AttackBox.SetActive(true);
        StartCoroutine(AttackTime());



    }
    public void HitDamage(float damage)
    {   

        if(mySuperPower == null || mySuperPower.isUnstoppable == false)
        {
            this.Status.Hp -= damage;
        }
        
    }
    private void Daed()
    {
        if(!isDead)
        {
            DeropItem();
        }
    }
    void DeropItem()
    {
        isDead=true;
        animator.SetTrigger("Dead");
        foreach (GameObject itemPrefab in items)
        {
            // 아이템을 생성하고 무작위 위치에 떨어뜨림
            Vector2 randomPosition = Random.insideUnitCircle * 1.2f; // 반지름 2.0f 내에서 무작위 위치
            Vector3 dropPosition = new Vector3(transform.position.x + randomPosition.x, transform.position.y + randomPosition.y, 0f);
            Instantiate(itemPrefab, dropPosition, Quaternion.identity);
        }
        Invoke("Destroy", 1.5f);
    }

    public IEnumerator StartAttackCool()
    {
    yield return new WaitForSeconds(2.0f);
    canAttack = true;
    }
    public IEnumerator AttackTime()
    {
    yield return new WaitForSeconds(0.5f);
    AttackBox.SetActive(false);
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
