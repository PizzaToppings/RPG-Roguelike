using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetTileSkill", menuName = "ScriptableObjects/SkillParts/TargetTileSkill")]
public class SO_TargetBoardtileSkill : SO_Skillpart
{
    public TileColor SelectedTileColor;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.GetAOE(this);

        if (MatchedSkillPartData.TilesHit.Contains(mouseOverTile))
        {
            MatchedSkillPartData.TilesHit.Clear();
            mouseOverTile.SetColor(SelectedTileColor);
            MatchedSkillPartData.TilesHit.Add(mouseOverTile);

            return this;
        }
        MatchedSkillPartData.TilesHit.Clear();

        return this;
    }
}
