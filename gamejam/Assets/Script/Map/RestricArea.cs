using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RestricArea : MonoBehaviour
{
    [SerializeField]
    float shrinkDuration = 0.01f; // Ŭ���� ���� �پ��

    void Update()
    {
         gameObject.transform.localScale -= new Vector3(shrinkDuration, shrinkDuration, 0);
    }
}
