using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class pointerCleanup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    float time = 0;
    float timeout = 2;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        time = 0;
        GameObject instance = GameObject.FindGameObjectWithTag("mouseOverText");
        if (instance != null)
        {
            Debug.Log("pointer cleanup on enter");
            GameObject.Destroy(instance);
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        GameObject instance = GameObject.FindGameObjectWithTag("mouseOverText");
        if(instance != null)
        {
            Debug.Log("pointer cleanup on exit");
            GameObject.Destroy(instance);
        }
    }

    void FixedUpdate()
    {
        time += Time.deltaTime;
        if(time > timeout)
        {
            GameObject instance = GameObject.FindGameObjectWithTag("mouseOverText");
            if (instance != null)
            {
                Debug.Log("pointer cleanup on timeout");
                GameObject.Destroy(instance);
            }
            time = 0;
        }
    }
}
