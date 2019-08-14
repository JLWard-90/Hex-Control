using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenuController : MonoBehaviour
{
    UIController uiC;
    private void Awake()
    {
        uiC = GameObject.Find("UICanvas").GetComponent<UIController>();
    }

    public void OnExitToMenuPress()
    {
        uiC.OnExitToMainMenuPress();
    }

    public void OnExitToOSPress()
    {
        uiC.OnExitToOSPress();
    }

    public void OnResumeButtonPress()
    {
        GameObject.Destroy(gameObject);
    }
}
