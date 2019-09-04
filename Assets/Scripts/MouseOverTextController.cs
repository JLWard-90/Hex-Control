using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseOverTextController : MonoBehaviour
{
    [SerializeField]
    GameObject mouseOverTextPrefab;
    private void OnMouseOver()
    {
        //When mouse hovers over a button that this script is a component of
        
    }

    private string getButtonInfo()
    {
        string infoText = "Sample text";
        return infoText;
    }
}
