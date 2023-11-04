using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image skIcon;
    public Image skcool;
    public Slider HPbar;
    public Image Minimap;

    private PlayerController target;
    public void setTarget(PlayerController target)
    {
        if(target == null)
        {
            Debug.LogError("Mssing Player for Player UI");
        }
        this.target = target;

    }

    public void Update()
    {
        if(HPbar != null)
        {
            HPbar.value = target.Status.Hp;
        }
        if(target == null)
        {
            Destroy(this.gameObject);
            return;
        }      
    }
}
