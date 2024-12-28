using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LineSkill", menuName = "ScriptableObjects/SkillParts/LineSkill")]
public class SO_LineSkill : SO_Skillpart
{
    [Header(" - Line Specific")]
    public int[] Angles;
    public int PierceAmount = -1;
    public bool GetLastTileOnly;
    public TileColor endColor;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots,
       BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit Target = null)
    {
        base.Preview(mouseOverTile, skillshots);
        TargetSkillsManager targetSkillsManager = TargetSkillsManager.Instance;
        targetSkillsManager.PreviewLine(this);

        PartData.TilesHit.RemoveAll(x => HasDuplicateTile(x));
        
        return this;
    }
}
