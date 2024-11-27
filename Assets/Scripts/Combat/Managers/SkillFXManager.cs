using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFXManager : MonoBehaviour
{
    public static SkillFXManager Instance;



    public void Init()
    {
        Instance = this;
    }
}
