using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject ItemStatusPrefab; 
    protected GameObject ItemStatus;
    protected Vector3 ItemStatusPos;
    
    void Awake()
    {
        ItemStatusPos = transform.position + new Vector3(0f, 2.0f, 0f);
        ItemStatus = Instantiate(ItemStatusPrefab, ItemStatusPos, Quaternion.identity);
        ItemStatus.transform.parent = this.transform;
        ItemStatus.SetActive(false);
    }
   
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player")) ItemStatus.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player")) ItemStatus.SetActive(false);
    }
}
