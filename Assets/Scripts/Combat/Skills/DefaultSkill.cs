using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSkill", menuName = "ScriptableObjects/Skills/DefaultSkill")]
public class DefaultSkill : SO_MainSkill
{
    public override void Preview(BoardTile mouseOverTile, BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit target = null)
    {
        base.Preview(mouseOverTile, overwriteOriginTile, overwriteTargetTile, target);
	}
}
