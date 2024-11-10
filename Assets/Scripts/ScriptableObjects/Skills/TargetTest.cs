using UnityEngine;

[CreateAssetMenu(fileName = "TargetTest", menuName = "ScriptableObjects/Skillshots/TargetTest")]
public class TargetTest : SO_MainSkill
{
    public override void Preview(BoardTile mouseOverTile)
    {
        base.Preview(mouseOverTile);

        SkillParts[0].Preview(mouseOverTile, SkillParts);
    }
}
