using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void StartBtnClick() {
        SceneManager.LoadScene(1);
    }
    public void QuitBtnClick() {
        Application.Quit();
    }
}
