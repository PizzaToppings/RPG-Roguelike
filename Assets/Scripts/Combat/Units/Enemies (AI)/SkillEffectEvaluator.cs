using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Evaluates the weighted value of a skill's effects against a set of target units.
/// Used by EnemySkillSimulator to score candidate targeting options.
/// Does not modify any game state.
/// </summary>
public static class SkillEffectEvaluator
{
    /// <summary>
    /// Returns a score representing how valuable it is for this enemy to use this skill
    /// if the given units would be hit.
    /// </summary>
    public static float EvaluateEffects(SO_EnemySkill enemySkill, List<Unit> targetsHit, SO_EffectWeightConfig weights)
    {
        if (enemySkill?.Skill == null || targetsHit == null || targetsHit.Count == 0) return 0f;
        if (weights == null) weights = DefaultWeights();

        float score = 0f;

        foreach (var spg in enemySkill.Skill.SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
            {
                if (sp == null) continue;
                score += EvaluateSkillPart(sp, targetsHit, weights);
            }
        }

        return score;
    }

    private static float EvaluateSkillPart(SO_Skillpart part, List<Unit> targetsHit, SO_EffectWeightConfig w)
    {
        float score = 0f;

        // Damage / Healing / Shield effects
        if (part.DamageEffects != null)
        {
            foreach (var dmg in part.DamageEffects)
            {
                if (dmg == null) continue;
                switch (dmg.HitType)
                {
                    case HitTypeEnum.Damage:
                        // Count targets that are characters (offensive)
                        foreach (var t in targetsHit)
                            if (!t.Friendly) { } // enemy hitting self — ignore (faction safety already handled upstream)
                            else if (t.Friendly) score += dmg.Power * w.DamageWeight;
                        // Simplification: each target hit gets the full per-hit score
                        score += dmg.Power * w.DamageWeight * CountCharacters(targetsHit);
                        break;
                    case HitTypeEnum.Healing:
                        score += dmg.Power * w.HealWeight * CountAllies(targetsHit);
                        break;
                    case HitTypeEnum.Shield:
                        score += dmg.Power * w.ShieldWeight * CountAllies(targetsHit);
                        break;
                }
            }
        }

        // Status effects
        if (part.StatusEffects != null)
        {
            foreach (var se in part.StatusEffects)
            {
                if (se == null) continue;
                bool isDoT = se.StatusEffectType == StatusEffectEnum.Burn
                          || se.StatusEffectType == StatusEffectEnum.Bleed
                          || se.StatusEffectType == StatusEffectEnum.Poison;
                bool isBuff = se.Buff;

                if (isDoT)
                    score += w.DoTWeight * se.Power * CountCharacters(targetsHit);
                else if (!isBuff)
                    score += w.DebuffWeight * CountCharacters(targetsHit);
                else
                    score += w.BuffWeight * CountAllies(targetsHit);
            }
        }

        // Displacement bonus (control value)
        if (part.displacementEffect != null && part.displacementEffect.UseDisplacement)
            score += w.DisplacementWeight * CountCharacters(targetsHit);

        return score;
    }

    private static int CountCharacters(List<Unit> units)
    {
        int count = 0;
        foreach (var u in units) if (u != null && u.Friendly) count++;
        return count;
    }

    private static int CountAllies(List<Unit> units)
    {
        int count = 0;
        foreach (var u in units) if (u != null && !u.Friendly) count++;
        return count;
    }

    private static SO_EffectWeightConfig DefaultWeights()
    {
        // ScriptableObject.CreateInstance is not usable from static context without MonoBehaviour.
        // Return a lightweight default via a cached instance.
        return _defaultWeights ?? (_defaultWeights = ScriptableObject.CreateInstance<SO_EffectWeightConfig>());
    }
    private static SO_EffectWeightConfig _defaultWeights;
}
