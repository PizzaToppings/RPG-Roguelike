using System.Collections;
using UnityEngine;

public class Character : Unit
{
    public override void Init()
    {
        Friendly = true;
        base.Init();
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
        if (SkillData.CurrentSkillshotIndex == skillIndex)
        {
            StopCasting();
        }
        // turn on
        else
        {
            if (skills[skillIndex].MagicalDamage && statusEffects.Find(x => x.statusEfectType == StatusEfectEnum.Silenced) != null)
                return;

            SkillData.Reset();

            UnitData.CurrentAction = UnitData.CurrentActionKind.CastingSkillshot; 
            SkillData.CurrentMainSkill = skills[skillIndex];
            SkillData.CurrentSkillshotIndex = skillIndex;

            for (var i = 0; i < skills[skillIndex].SkillPartGroups.Count; i++)
			{
                var spg = skills[skillIndex].SkillPartGroups[i];
                var skillPartGroupData = new SkillPartGroupData();
                SkillData.SkillPartGroupDatas.Add(skillPartGroupData);

                for (var s = 0; s < spg.skillParts.Count; s++)
				{
                    var skillPartData = new SkillPartData
                    {
                        Index = s
                    };

                    spg.skillParts[s].SkillPartIndex = s;
                    spg.skillParts[s].PartData = skillPartData;
                    spg.skillParts[s].MatchedSkillPartGroupData = skillPartGroupData;

                    skillPartGroupData.SkillPartDatas.Add(skillPartData);
                }
			}

            //preview skill
            skills[skillIndex].Reset();
            foreach (var skillPartGroup in skills[skillIndex].SkillPartGroups)
			{
                foreach (var skillPart in skillPartGroup.skillParts)
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
					    skillPart.Preview(currentTile, skillPartGroup.skillParts);
				    }
				}
			}
		}
    }

    public void StopCasting()
    {
        boardManager.Clear();
        UnitData.CurrentAction = UnitData.CurrentActionKind.Moving;
        SkillData.Reset();
        boardManager.SetAOE(MoveSpeedLeft, currentTile, null);
    }

    public override void PreviewSkills(BoardTile mouseOverTile)
    {
        base.PreviewSkills(mouseOverTile);

        for (int i = 0; i < SkillshotsEquipped.Count; i++)
        {
            if (SkillData.CurrentSkillshotIndex == i && SkillshotsEquipped[i])
            {
                skills[i].Preview(mouseOverTile);
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
