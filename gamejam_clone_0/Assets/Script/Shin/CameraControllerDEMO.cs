using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerDEMO : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    void Start()
    {
        target.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        target.transform.position = transform.position;
    }
}
