using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHide : MonoBehaviour
{
    void Start()
    {
        SetButtonActive(false);
    }
    
    public void SetButtonActive(bool isActive){
        gameObject.SetActive(isActive);
    }
}
