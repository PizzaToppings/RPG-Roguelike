using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoScreen : MonoBehaviour
{
    public static InfoScreen Instance;

    public bool IsActive = false;

    public void Init()
    {
        Instance = this;
    }

    void Update()
    {
        
    }

    public void SetActive(bool active)
    {
        IsActive = active;
        gameObject.SetActive(IsActive);
    }
}
