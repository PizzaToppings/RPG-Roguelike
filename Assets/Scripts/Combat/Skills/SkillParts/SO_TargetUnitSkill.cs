using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetUnitSkill", menuName = "ScriptableObjects/SkillParts/TargetUnitSkill")]
public class SO_TargetUnitSkill : SO_Skillpart
{
    [Header(" - TargetUnit Specific")]
    public TileColor SelectedTargetTileColor;

    [Space]
    public bool CanTargetSelf;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots, Unit caster,
       BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit overwriteTarget = null)
    {
        base.OriginTileKind = OriginTileKind;
        TargetTileKind = TargetTileEnum.MouseOverTarget;

        base.Preview(mouseOverTile, skillshots, caster);
        TargetSkillsManager targetSkillsManager = TargetSkillsManager.Instance;
        targetSkillsManager.GetAOE(this);

		var target = PartData.TargetsHit.Find(x => x.IsTargeted);

		if (HasDuplicateTarget(target))
			target = null;

        if (CanTargetSelf == false && target == SkillData.Caster)
            target = null;

        if (target != null)
		{
            TargetUnit(target);
            return this;
        }

        PartData.TargetsHit.Clear();
        PartData.TilesHit.Clear();

        PartData.CanCast = false;

        return this;
    }

    public void TargetUnit(Unit target)
	{
        target.Tile.SetColor(SelectedTargetTileColor);

        PartData.TargetsHit.Clear();
        PartData.TilesHit.Clear();
        PartData.TargetsHit.Add(target);

        if (AddProjectileLine)
            ShowProjectileLine(SkillData.Caster.position, PartData.TargetsHit[0].position);
    }

    public override void SetTargetAndTile(Unit target, BoardTile tile)
    {
        if (target != null)
            TargetUnit(target);
    }

    public override bool NoTargetsInRange()
    {
        SetInitData(null);

        TargetSkillsManager targetSkillsManager = TargetSkillsManager.Instance;
        targetSkillsManager.GetAOE(this);

        var targetsHit = PartData.TargetsHit;
        targetsHit.RemoveAll(x => HasDuplicateTarget(x));

        if (targetsHit.Count == 0)
            return true;

        return false;
    }
}
