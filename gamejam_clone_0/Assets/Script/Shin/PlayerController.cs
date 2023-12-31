using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour ,Damageable
{
    public PlayerStats Status;
    private Animator animator;
    public PhotonView pv;
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

    public GameObject[] items; // 드롭할 아이템 프리팹 배열
    void Start()
    {
        Status = new PlayerStats(100f, 5f, 10f);
        animator = GetComponent<Animator>();
        isSeeRight=true;
        if (pv.IsMine)
        {
            GameObject bodycam = GameObject.Find("BodyCam");
            bodycam.GetComponentInChildren<CameraController>().player = this.gameObject;

            GameObject playerui = GameObject.Find("PlayerUI");
            playerui.GetComponent<PlayUI>().playerController = this;
        }
    }
    void Update()
    {
        if (pv.IsMine)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = mousePosition - this.transform.position;
            if (!Eating)
            {
                move();
            }
                pv.RPC("flipRPC", RpcTarget.All, direction);
            if (canAttack && Input.GetMouseButtonDown(0)) Attack();
        }
        //if(this.Status.Hp<=0) Dead()함수 출력;
    }
    [PunRPC]
    void flipRPC(Vector3 direction)
    {
        if (direction.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    [PunRPC]
    void moveRPC(Vector3 mov_pos)
    {
        transform.position += mov_pos * this.Status.speed * Time.deltaTime;
    }
    void move()
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
        pv.RPC("moveRPC", RpcTarget.All, movePosition);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (pv.IsMine)
        {
            switch (other.tag)
            {
                case "Potion":
                    if (Input.GetKey(KeyCode.F))
                    {
                        Eating = true;
                        StartCoroutine(StartEating());
                        StartCoroutine(StartAddItemCooldown());
                        Potion item = other.GetComponent<Potion>();
                        ConsumeItem(item);
                        Destroy(other.gameObject);
                    }
                    break;
                case "SuperPower":
                    if (Input.GetKey(KeyCode.F))
                    {
                        Eating = true;
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
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RestricArea"))
        {
            //즉사, 추후에  즉사 파티클 추가
            this.Status.Hp = 0;
        }
    }
    [PunRPC]
    void increaseHP()
    {
        Status.Hp += 10;
    }
    [PunRPC]
    void increaseSPD()
    {
        Status.speed += 1f;
    }
    void increaseATK()
    {
        Status.attackDamage += 2f;
    }

    private void ConsumeItem(Potion item)
    {
        StartCoroutine(StartAddItemCooldown());
        switch (item.GetPotionType())
        {
        case PotionType.Hp:
            pv.RPC("increaseHP", RpcTarget.All);  
            Debug.Log("체력 물약을 먹었습니다.");
            break;
        case PotionType.Speed:
                pv.RPC("increaseSPD", RpcTarget.All);
                Debug.Log("스피드 물약을 먹었습니다.");
            break;
        case PotionType.Strength:
            pv.RPC("increaseATK",RpcTarget.All);
            Debug.Log("힘 물약을 먹었습니다.");
            break;    
        }   
    }
    [PunRPC]
    void AttachUnstoppable(SuperPower item)
    {
        mySuperPower = gameObject.AddComponent<Unstoppable>();
        if (item.attackSkillPrefab != null)
        {
            mySuperPower.attackSkillPrefab = item.attackSkillPrefab;
        }
    }
    [PunRPC]
    void AttachElectrokinetic(SuperPower item)
    {
        mySuperPower = gameObject.AddComponent<Electrokinetic>();
        if (item.attackSkillPrefab != null)
        {
            mySuperPower.attackSkillPrefab = item.attackSkillPrefab;
        }
    }
    void GetSuperPower(SuperPower item)
    {
        switch(item.GetPowerType())
        {
            case PowerType.Unstoppable:
                pv.RPC("AttachUnstoppable", RpcTarget.All, item);
                break;
            case PowerType.Electrokinetic:
                pv.RPC("AttachElectrokinetic", RpcTarget.All, item);
                break;
        }
        
    }
    private void Attack()
    {
        animator.SetTrigger("Attack");
        canAttack=false;
        Invoke("StartCoroutine(StartAttackCool());", 1.0f);
        StartCoroutine(StartAttackCool());
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);
        foreach (Collider2D target in hitTargets)
        {
            Damageable damageable = target.GetComponent<Damageable>();
            if (damageable != null) 
            {
                pv.RPC("HitDamage", RpcTarget.All, Status.attackDamage);
            }
        }
    }
    [PunRPC]
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
