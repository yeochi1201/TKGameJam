using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour,Damageable
{
    public GameObject[] possibleItems;

    public float Hp=20;

    private void Update()
    {
        if(this.Hp<=0)Destroy(this.gameObject);
    }
    public void HitDamage(float damage)
    {
        this.Hp-=damage;
    }

    void OnDestroy()
    {
        SpawnRandomItem();   
    }
    void SpawnRandomItem()
    {
        if (possibleItems.Length > 0)
        {
            int randomIndex = Random.Range(0, possibleItems.Length);
            GameObject selectedItem = possibleItems[randomIndex];

            // 선택된 아이템을 인스턴스화하여 생성
            Instantiate(selectedItem, transform.position, Quaternion.identity);
        }
    }
}
