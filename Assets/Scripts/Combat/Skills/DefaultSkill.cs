using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSkill", menuName = "ScriptableObjects/Skills/DefaultSkill")]
public class DefaultSkill : SO_MainSkill
{
    public override void Preview(BoardTile mouseOverTile)
    {
        base.Preview(mouseOverTile);
	}
}
