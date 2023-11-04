using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RestricArea : MonoBehaviour
{
    [SerializeField]
    float shrinkSpeed = 0.01f; // �ʱ� �ӵ�
    [SerializeField]
    float minshrinkSpeed = 0.001f; // ���� �ӵ�
    [SerializeField]
    float decreaseRate = 0.02f; // ������

    private float shrinkDuration;

    private void Start()
    {
        shrinkDuration = shrinkSpeed;
    }
    void Update()
    {
        if (shrinkDuration > minshrinkSpeed)
            shrinkDuration -= (shrinkSpeed - minshrinkSpeed) * decreaseRate * Time.deltaTime;
        Debug.Log(shrinkDuration);
        gameObject.transform.localScale -= new Vector3(shrinkDuration, shrinkDuration, 0);
    }
}
