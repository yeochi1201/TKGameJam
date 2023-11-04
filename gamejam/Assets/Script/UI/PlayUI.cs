using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    public PlayerController playerController;
    public SuperPower superPower;

    public Image HPSlider;
    public Image skillImage;
    public Button SkillBtn;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        HPSlider.fillAmount = playerController.Status.Hp / 100f;
        superPower = GameObject.FindWithTag("Player").GetComponent<SuperPower>();
        if(superPower != null) {
            Sprite skillSprite = Resources.Load<Sprite>("Skills/" + superPower.GetPowerType()); // 스킬 이미지는 Resources 폴더에 있어야 함
            skillImage.sprite = skillSprite;
            
        }

        // 스킬 이미지 업데이트
        // string skillName = playerController.CurrentSkillName;
        // Sprite skillSprite = Resources.Load<Sprite>("Skills/" + skillName); // 스킬 이미지는 Resources 폴더에 있어야 함
        // SkillImage.sprite = skillSprite;
    }
}