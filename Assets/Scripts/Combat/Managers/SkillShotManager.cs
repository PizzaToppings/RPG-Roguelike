using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardManager))]
public class SkillShotManager : MonoBehaviour
{
    public static SkillShotManager skillShotManager;
    BoardManager boardManager;

    public List<bool> CastingSkillshot = new List<bool>();

    public bool castingAnySkillshot;

    public void Init()
    {
        skillShotManager = this;
        boardManager = GetComponent<BoardManager>();
    }

    void Update()
    {
        // CastingSkillshot = SkillshotData.CastingSkillshot;
    }
}
