using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    public PlayerController playerController;

    public Image HPSlider;
    // public Image skillImage;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        HPSlider.fillAmount = playerController.Status.Hp / 100f;

        // 스킬 이미지 업데이트
        // string skillName = playerController.CurrentSkillName;
        // Sprite skillSprite = Resources.Load<Sprite>("Skills/" + skillName); // 스킬 이미지는 Resources 폴더에 있어야 함
        // SkillImage.sprite = skillSprite;
    }
}
