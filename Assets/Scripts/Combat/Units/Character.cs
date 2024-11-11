using System.Collections;
using UnityEngine;

public class Character : Unit
{
    public override void Init()
    {
        Friendly = true;
        base.Init();

        /// remove
        // var SE = new DefaultStatusEffect
        // {
        //     statusEfectType = StatusEfectEnum.Rooted,
        //     duration = 2
        // };
        // statusEffects.Add(SE);
    }

    public override void Update()
    {
        if (UnitData.CurrentActiveUnit == this)
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Space))
                EndTurn();

            UseSkills();
        }
    }

    void UseSkills()
    {
        if (UnitData.CurrentActiveUnit != this)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ToggleSkill(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ToggleSkill(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ToggleSkill(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            ToggleSkill(3);

    }

    public void ToggleSkill(int skillIndex)
    {
        boardManager.Clear();
        // turn off
        if (SkillshotData.CurrentSkillshotIndex == skillIndex)
        {
            UnitData.CurrentAction = UnitData.CurrentActionKind.Moving; 
            SkillshotData.CurrentSkillshotIndex = null;
            boardManager.SetAOE(MoveSpeedLeft, currentTile, null);
        }
        // turn on
        else
        {
            if (skillshots[skillIndex].MagicalDamage && statusEffects.Find(x => x.statusEfectType == StatusEfectEnum.Silenced) != null)
                return;

            UnitData.CurrentAction = UnitData.CurrentActionKind.CastingSkillshot; 
            SkillshotData.CurrentMainSkillshot = skillshots[skillIndex];
            SkillshotData.CurrentSkillshotIndex = skillIndex;

            // preview skill
            foreach (var skillPart in skillshots[skillIndex].SkillParts)
			{
                if (skillPart.OriginTileKind == OriginTileEnum.Caster)
				{
                    foreach (var tile in currentTile.connectedTiles)
					{
                        if (tile == null)
                            continue;

                        skillPart.TargetTile = tile;
                        break;
					}
                    skillPart.Preview(currentTile, skillshots[skillIndex].SkillParts);
				}
            }
		}
    }

    public override void PreviewSkills(BoardTile mouseOverTile)
    {
        base.PreviewSkills(mouseOverTile);

        for (int i = 0; i < SkillshotsEquipped.Count; i++)
        {
            if (SkillshotData.CurrentSkillshotIndex == i && SkillshotsEquipped[i])
            {
                skillshots[i].Preview(mouseOverTile);
            }
        }
    }

    public override IEnumerator StartTurn()
    {
        yield return null;
        StartCoroutine(base.StartTurn());

		UnitData.CurrentAction = UnitData.CurrentActionKind.Moving;
	}
}
