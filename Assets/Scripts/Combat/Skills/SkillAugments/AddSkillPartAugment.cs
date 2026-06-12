using UnityEngine;

[CreateAssetMenu(fileName = "AddSkillPartAugment", menuName = "ScriptableObjects/SkillAugments/AddSkillPartAugment")]
public class AddSkillPartAugment : SO_SkillAugment
{
    [Tooltip("Index of the group to add to.")]
    public int TargetSkillPartGroupIndex = 0;

    [Tooltip("Index inside the target group to insert the new part. -1 to append.")]
    public int TargetSkillPartIndex = -1;

    public SO_Skillpart SkillPartToAdd;
    public int Copies = 1;

    public override void Init(Skill skill, SkillAugment augment, Character character)
    {
        if (skill == null || SkillPartToAdd == null)
            return;

        if (TargetSkillPartGroupIndex < 0)
            return;

        int effectiveGroupIndex = TargetSkillPartGroupIndex;
        if (effectiveGroupIndex >= skill.SkillPartGroups.Count)
            effectiveGroupIndex = skill.SkillPartGroups.Count - 1;

        var targetGroup = skill.SkillPartGroups[effectiveGroupIndex];
        if (targetGroup?.skillParts == null)
            return;

        int copyCount = Mathf.Max(1, Copies);

        // Determine desired insertion index. If TargetSkillPartIndex < 0 we append immediately.
        int baseIndex = TargetSkillPartIndex;

        for (int i = 0; i < copyCount; i++)
        {
            var clone = SkillAugmentRuntimeCloneUtility.CloneSkillPart(SkillPartToAdd);
            if (clone == null)
                continue;

            // If baseIndex < 0 -> append immediately
            if (baseIndex < 0)
            {
                targetGroup.skillParts.Add(clone);
                continue;
            }

            // If the desired index is within current bounds, insert now
            if (baseIndex <= targetGroup.skillParts.Count - 1)
            {
                int insertAt = Mathf.Min(baseIndex + i, targetGroup.skillParts.Count);
                targetGroup.skillParts.Insert(insertAt, clone);
                continue;
            }

            // Otherwise the configured index is out-of-range relative to the current
            // parts. Defer insertion so multiple out-of-range adds can be ordered by
            // their configured index.
            DeferredAddRegister(skill, effectiveGroupIndex, baseIndex + i, clone);
        }
    }

    class DeferredAdd
    {
        public Skill Skill;
        public int GroupIndex;
        public int TargetIndex;
        public SO_Skillpart Clone;
        public int Id;
    }

    static readonly System.Collections.Generic.List<DeferredAdd> deferredAdds = new System.Collections.Generic.List<DeferredAdd>();
    static int nextDeferredId = 0;

    static void DeferredAddRegister(Skill skill, int groupIndex, int targetIndex, SO_Skillpart clone)
    {
        deferredAdds.Add(new DeferredAdd { Skill = skill, GroupIndex = groupIndex, TargetIndex = targetIndex, Clone = clone, Id = nextDeferredId++ });
    }

    // Public API so other augment types can register deferred insertions that
    // will be processed together with AddSkillPartAugment.ProcessDeferredForSkill.
    public static void RegisterDeferredAdd(Skill skill, int groupIndex, int targetIndex, SO_Skillpart clone)
    {
        DeferredAddRegister(skill, groupIndex, targetIndex, clone);
    }

    public static void ProcessDeferredForSkill(Skill skill)
    {
        if (skill == null) return;

        var toProcess = deferredAdds.FindAll(d => d.Skill == skill);
        if (toProcess.Count == 0) return;

        // Sort by target index then by registration order
        toProcess.Sort((a, b) => a.TargetIndex != b.TargetIndex ? a.TargetIndex.CompareTo(b.TargetIndex) : a.Id.CompareTo(b.Id));

        foreach (var d in toProcess)
        {
            if (d.GroupIndex < 0 || d.GroupIndex >= skill.SkillPartGroups.Count)
                continue;

            var group = skill.SkillPartGroups[d.GroupIndex];
            if (group?.skillParts == null) continue;

            int insertAt = Mathf.Clamp(d.TargetIndex, 0, group.skillParts.Count);
            group.skillParts.Insert(insertAt, d.Clone);
        }

        deferredAdds.RemoveAll(d => d.Skill == skill);
    }
}
