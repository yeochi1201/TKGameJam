using UnityEngine;

public class RestricArea : MonoBehaviour
{
    [SerializeField]
    float shrinkSpeed = 0.005f; // 초기 속도
    [SerializeField]
    float minshrinkSpeed = 0.002f; // 최종 속도
    [SerializeField]
    float decreaseRate = 0.01f; // 감쇼율
    [SerializeField]
    GameObject areaPrefab;
    [SerializeField]
    GameObject[] finalPoints;
    [SerializeField]
    float stopTime = 115;

    private GameObject area;
    private bool isStop = false;
    private float time;

    private float shrinkDuration;

    private void Start()
    {
        int index = Random.Range(0, finalPoints.Length);
        area = Instantiate(areaPrefab, finalPoints[index].transform.position, Quaternion.identity);
        shrinkDuration = shrinkSpeed;
        Invoke("IsStop", stopTime);
    }

    void Update()
    {
        if (!isStop)
        if (!isStop)
        {
            if (shrinkDuration > minshrinkSpeed)
                shrinkDuration -= (shrinkSpeed - minshrinkSpeed) * decreaseRate * Time.deltaTime;
            Debug.Log(shrinkDuration);
            time += Time.deltaTime;
            Debug.Log(time);
            area.transform.localScale -= new Vector3(shrinkDuration, shrinkDuration, 0);
        }
    }
    void IsStop()
    {
        isStop = true;
    }    
}
