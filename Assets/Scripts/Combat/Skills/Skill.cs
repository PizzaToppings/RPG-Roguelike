using System;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public SO_MainSkill mainSkillSO;

    [Space]
    public List<SkillPartGroup> SkillPartGroups = new List<SkillPartGroup>(1);

    [Space]
    public int EnergyCost = 10;
    public int DefaultCharges = 1;
    public int DefaultCooldown = 0;

    public int Cooldown => SkillData.GetCooldown(this);

    public bool IsMagical => mainSkillSO.IsMagical;
    public int Charges => SkillData.GetCharges(this);

    public List<SkillAugment> Augments = new List<SkillAugment>();

    public void Init(SO_MainSkill skillSO)
    {
        mainSkillSO = skillSO;
        EnergyCost = skillSO.EnergyCost;
        DefaultCharges = skillSO.DefaultCharges;
        DefaultCooldown = skillSO.DefaultCooldown;

        // Deep-copy skill part groups so augments can safely modify
        // MaxRange / DamageEffects on runtime instances without mutating the shared SO asset.
        var originalToInstantiated = new System.Collections.Generic.Dictionary<SO_Skillpart, SO_Skillpart>();
        SkillPartGroups = new List<SkillPartGroup>(skillSO.SkillPartGroups.Count);
        if (skillSO.SkillPartGroups.Count == 0)
        {
            Debug.LogWarning($"Skill {skillSO.name} has no SkillPartGroups configured.");
        }   

        foreach (var spg in skillSO.SkillPartGroups)
        {
            if (spg == null)
            {
                Debug.LogWarning($"Skill {skillSO.name} has a null SkillPartGroup in its configuration.");
                continue;
            }

            var runtimeGroup = new SkillPartGroup();
            if (spg.skillParts == null || spg.skillParts.Count == 0)
            {
                Debug.LogWarning($"Skill {skillSO.name} has a SkillPartGroup with no skill parts configured.");
                continue;
            }

            foreach (var sp in spg.skillParts)
            {
                if (sp == null)
                {
                    Debug.LogWarning($"Skill {skillSO.name} has a null SkillPart in its SkillPartGroup.");
                    continue;
                }

                var instance = UnityEngine.Object.Instantiate(sp);
                originalToInstantiated[sp] = instance;
                runtimeGroup.skillParts.Add(instance);
            }
            SkillPartGroups.Add(runtimeGroup);
        }

        // Remap displacement Unit/TargetPosition references from original SOs to their
        // instantiated counterparts, so PartData (set at runtime on instances) is reachable.
        foreach (var spg in SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
            {
                var d = sp.displacementEffect;
                if (d == null || !d.UseDisplacement) continue;

                if (d.Unit != null && originalToInstantiated.TryGetValue(d.Unit, out var instUnit))
                    d.Unit = instUnit;

                if (d.TargetPosition != null && originalToInstantiated.TryGetValue(d.TargetPosition, out var instTarget))
                    d.TargetPosition = instTarget;
            }
        }
    }

    public void InitAugments(List<SO_SkillAugment> augmentSOs, Character character)
    {
        Augments.Clear();
        foreach (var so in augmentSOs)
        {
            var augment = new SkillAugment();
            augment.Init(so, this, character);
            Augments.Add(augment);
        }

        // Process any deferred AddSkillPartAugment insertions that were registered
        // during augment initialization. This ensures out-of-range-targeted
        // additions are inserted in order of their configured indices.
        AddSkillPartAugment.ProcessDeferredForSkill(this);
    }

    public void Init()
    {
        SkillData.SetCharges(this, DefaultCharges);
        // Ensure cooldown entry exists (do not override existing runtime cooldown if present)
        if (SkillData.GetCooldown(this) < 0)
            SkillData.SetCooldown(this, 0);
    }

    public void Preview(BoardTile mouseOverTile, Unit caster, BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit target = null)
    {
        var SkillPartGroupIndex = SkillData.SkillPartGroupIndex;

        for (int i = 0; i < SkillPartGroups[SkillPartGroupIndex].skillParts.Count; i++)
        {
            var skillPartList = SkillPartGroups[SkillPartGroupIndex].skillParts;
            skillPartList[i].Preview(mouseOverTile, skillPartList, caster, overwriteOriginTile, overwriteTargetTile, target);
        }

        if (SkillPartGroupIndex == 0)
            return;

        for (int i = 0; i < SkillPartGroupIndex; i++)
        {
            var spgd = SkillData.SkillPartGroupDatas[i];

            foreach (var spd in spgd.SkillPartDatas)
            {
                foreach (var tile in spd.TilesHit)
                {
                    tile.SetColor(mainSkillSO.castLockColor);
                }

                foreach (var unit in spd.TargetsHit)
                {
                    unit.Tile.SetColor(mainSkillSO.castLockColor);
                }
            }
        }
    }

    public float GetAttackRange()
    {
        var totalRange = 0f;

        foreach (var spg in SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
            {
                if (sp.IncludeInAutoMove)
                    totalRange += sp.MaxRange;
            }
        }

        return totalRange;
    }

    public void SetTargetAndTile(Unit target, BoardTile tile)
    {
        foreach (var spg in SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
            {
                sp.SetTargetAndTile(target, tile);
            }
        }
    }

    public virtual void Reset()
    {
        SkillData.SkillPartGroupIndex = 0;
    }
}
