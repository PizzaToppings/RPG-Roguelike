using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    public int MaxSkillShotAmount = 4;
    public List<bool> SkillshotsEquipped;
    public List<SO_MainSkill> skills = new List<SO_MainSkill>();

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

	public override void SetStats()
	{
		base.SetStats();

        if (skills?.Count == 0)
            return;

        SetSkillShots();
    }

    void SetSkillShots()
    {
        SkillData.Caster = this;

        for (int i = 0; i < MaxSkillShotAmount; i++)
        {
            if (i >= skills.Count)
            {
                SkillshotsEquipped.Add(false);
                continue;
            }

            if (skills[i] != null)
                SkillshotsEquipped.Add(true);
            else
                SkillshotsEquipped.Add(true);
        }
    }

    void UseSkills()
    {
        if (UnitData.CurrentActiveUnit != this || UnitData.CurrentAction != CurrentActionKind.Moving)
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

            UnitData.CurrentAction = CurrentActionKind.CastingSkillshot; 
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
        UnitData.CurrentAction = CurrentActionKind.Moving;
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

		UnitData.CurrentAction = CurrentActionKind.Moving;
	}
}
