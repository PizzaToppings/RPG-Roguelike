using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetUnitSkill", menuName = "ScriptableObjects/SkillParts/TargetUnitSkill")]
public class SO_AutoTargetSkill : SO_Skillpart
{
    [Header(" - TargetUnit Specific")]
    public TileColor SelectedTargetTileColor;

    [Space]
    public AutoTargetEnum Target;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots, Unit caster,
       BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit overwriteTarget = null)
    {
        base.OriginTileKind = OriginTileKind;
        TargetTileKind = TargetTileEnum.MouseOverTarget;

        base.Preview(mouseOverTile, skillshots, caster);
        TargetSkillsManager targetSkillsManager = TargetSkillsManager.Instance;
        targetSkillsManager.GetAOE(this);

        var target = FindTarget(PartData.TargetsHit, caster);

        if (HasDuplicateTarget(target))
            target = null;

        RotateCaster(InputManager.Instance.GetMousePosition());

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

    Unit FindTarget(List<Unit> units, Unit caster)
    {
        var boardManager = BoardManager.Instance;

        switch (Target)
        {
            case AutoTargetEnum.Closest:
                return units.OrderBy(x => boardManager.GetRangeBetweenTiles(OriginTiles[0], x.Tile)).FirstOrDefault();
        }

        return null;
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
