using UnityEngine;
using UnityEngine.UI;

public class BGLoop : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float width = 2000;
    [SerializeField]
    int order = 1;    //¼ø¼­

    private float startX,startY;
    void Awake()
    {
        startX = transform.position.x;
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= -width/2)
        {
            RePosition();
        }
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.left * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }
    void RePosition()
    {
        if(order==2)
            transform.position = new Vector3(startX, startY, 0);
        else
            transform.position = new Vector3(startX+ width, startY, 0);
    }
}