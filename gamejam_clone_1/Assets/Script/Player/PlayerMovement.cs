using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using Unity.Mathematics;

public class PlayerMovement : MonoBehaviour
{
    private float speed = 1f;
    // public GameObject image;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        // PhotonView.Instantiate(image, Vector3.zero, quaternion.Euler(Vector3.zero));
        Debug.Log("1");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Attack();
    }
    void Move(){
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
        transform.position += movePosition * speed * Time.deltaTime;
    }
    void Attack() {
        if(Input.GetKeyDown(KeyCode.F)) {
            animator.SetTrigger("Attack");
        }
    }
}
