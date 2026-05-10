using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Stateful per-enemy skill picker. Call Pick() each turn to get the next skill.
///
/// Priority order each turn:
///  1. First turn + AlwaysStartWithFirst  → always return skills[0]
///  2. ForcedNextSkillIndex on the last skill used   → return that skill
///  3. Filter out skills that have hit their MaxConsecutiveUses cap
///  4. Weighted random by Chance among the remaining candidates
/// </summary>
public class EnemySkillSequencer
{
    int  lastUsedIndex    = -1;
    int  consecutiveCount = 0;
    bool isFirstTurn      = true;

    public SO_EnemySkill Pick(List<SO_EnemySkill> skills, bool alwaysStartWithFirst)
    {
        if (skills == null || skills.Count == 0) return null;

        // ── 1. First-turn fixed opener ──────────────────────────────────
        if (isFirstTurn && alwaysStartWithFirst)
        {
            isFirstTurn = false;
            SetLast(0);
            return skills[0];
        }
        isFirstTurn = false;

        // ── 2. Forced follow-up ─────────────────────────────────────────
        if (lastUsedIndex >= 0 && lastUsedIndex < skills.Count)
        {
            int forced = skills[lastUsedIndex].ForcedNextSkillIndex;
            if (forced >= 0 && forced < skills.Count)
            {
                consecutiveCount = (forced == lastUsedIndex) ? consecutiveCount + 1 : 1;
                lastUsedIndex    = forced;
                return skills[forced];
            }
        }

        // ── 3. Filter by consecutive cap ───────────────────────────────
        var candidates = new List<(SO_EnemySkill skill, int index)>();
        for (int i = 0; i < skills.Count; i++)
        {
            var s = skills[i];
            bool capped = s.MaxConsecutiveUses > 0
                          && i == lastUsedIndex
                          && consecutiveCount >= s.MaxConsecutiveUses;
            if (!capped)
                candidates.Add((s, i));
        }

        // Safety: if every skill is capped (bad data) fall back to all
        if (candidates.Count == 0)
            candidates = skills.Select((s, i) => (s, i)).ToList();

        // ── 4. Weighted random ─────────────────────────────────────────
        int        pickedIndex = candidates.Last().index;
        SO_EnemySkill picked   = candidates.Last().skill;

        float total = candidates.Sum(c => c.skill.Chance);
        if (total > 0)
        {
            float roll       = Random.Range(0f, total);
            float cumulative = 0f;
            foreach (var (skill, index) in candidates)
            {
                cumulative += skill.Chance;
                if (roll < cumulative)
                {
                    pickedIndex = index;
                    picked      = skill;
                    break;
                }
            }
        }
        else
        {
            // All chances are 0 — uniform random
            var r  = candidates[Random.Range(0, candidates.Count)];
            pickedIndex = r.index;
            picked      = r.skill;
        }

        consecutiveCount = (pickedIndex == lastUsedIndex) ? consecutiveCount + 1 : 1;
        lastUsedIndex    = pickedIndex;
        return picked;
    }

    void SetLast(int index)
    {
        lastUsedIndex    = index;
        consecutiveCount = 1;
    }

    /// <summary>Reset sequencer state (e.g. on a phase change).</summary>
    public void Reset()
    {
        lastUsedIndex    = -1;
        consecutiveCount = 0;
        isFirstTurn      = true;
    }
}
