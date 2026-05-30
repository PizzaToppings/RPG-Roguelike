using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAI : Enemy
{
    [HideInInspector] public BoardTile OptimalTile;
    [HideInInspector] public Unit Target;
    [HideInInspector] public SO_EnemySkill CurrentSkill;

    EnemySkillSequencer sequencer = new EnemySkillSequencer();

    public override void Init()
    {
        base.Init();
        // Pick the initial skill so intent can be displayed from the start
        CurrentSkill = sequencer.Pick(enemySO?.Skills, enemySO != null && enemySO.AlwaysStartWithFirstSkill);
    }

    public override List<SO_SKillVFX> GetSkillVFXList()
    {
        var result = new List<SO_SKillVFX>();
        if (enemySO != null)
            foreach (var s in enemySO.Skills)
                if (s?.Skill?.SkillVFX != null)
                    result.AddRange(s.Skill.SkillVFX);
        return result;
    }

    public override IEnumerator StartTurn()
    {
        // CurrentSkill is already picked (either in Init or at the end of the previous turn)
        if (CurrentSkill == null)
        {
            EndTurn();
            yield break;
        }

        yield return StartCoroutine(base.StartTurn());

        FindOptimalTile();
        yield return StartCoroutine(MoveToTile());

        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(Attack());
    }

    public virtual IEnumerator Attack()
    {
        var skill = CurrentSkill.Skill;

        Unit target = null;
        float closestTargetRange = 0;

        foreach (var character in UnitData.Characters)
        {
            var blocked = boardManager.TileIsBehindClosedTile(Tile, character.Tile);
            if (blocked)
                continue;

            var range = boardManager.GetRangeBetweenTiles(Tile, character.Tile);
            if (range < skill.MinRange || range > skill.MaxRange)
                continue;

            if (target == null)
            {
                target = character;
                closestTargetRange = range;
            }
            else
            {
                if (range < closestTargetRange)
                {
                    target = character;
                    closestTargetRange = range;
                }
            }
        }

        if (target == null)
        {
            EndTurn();
            yield break;
        }

        StartCoroutine(uiManager.ShowActivityText(CurrentSkill.SkillName));
        yield return new WaitForSeconds(0.3f);

        Rotate(target.transform.position);
        skill.PartData = new SkillPartData();
        skill.PartData.TargetsHit.Add(target);
        StartCoroutine(skillsManager.CastSkillsPart(skill, this));

        yield return new WaitForSeconds(2);

        // Pick next skill and update intent for next turn
        CurrentSkill = sequencer.Pick(enemySO?.Skills, false);
        (ThisHealthbar as FloatingHealthbar)?.UpdateIntent(CurrentSkill);

        EndTurn();
    }

    public void FindOptimalTile()
    {
        boardManager.SetAOE(MoveSpeedLeft, Tile, null);

        var preferredTarget = GetTargetPreference(CurrentSkill.TargetPreference, UnitData.Characters);

        PossibleMovementTiles.Add(Tile);

        PossibleMovementTiles.ForEach(x => x.EnemyPreferenceRating = 0);

        foreach (var tile in PossibleMovementTiles)
        {
            foreach (var character in UnitData.Characters)
            {
                int preferedBonus = character == preferredTarget ? 50 : 0;

                var rangeToCharacter = boardManager.GetRangeBetweenTiles(tile, character.Tile);

                float skillMinRange = CurrentSkill.Skill != null ? CurrentSkill.Skill.MinRange : 0f;
                if (rangeToCharacter >= skillMinRange && rangeToCharacter <= CurrentSkill.OptimalRange)
                    tile.EnemyPreferenceRating += 200 + preferedBonus;
                else if (rangeToCharacter < skillMinRange)
                    tile.EnemyPreferenceRating -= 300; // Inside minimum range, can't attack from here

                // Prefer tiles closest to the optimal range
                float distanceFromOptimal = Mathf.Abs(rangeToCharacter - CurrentSkill.OptimalRange);
                tile.EnemyPreferenceRating -= 10 * distanceFromOptimal;
            }

            foreach (var enemy in UnitData.Enemies)
            {
                if (enemy == this)
                    continue;

                var rangeToEnemy = boardManager.GetRangeBetweenTiles(tile, enemy.Tile);

                if (rangeToEnemy <= 5) // TODO Edit this value based on something. Do enemies want to be close to each other? Or far away?
                    tile.EnemyPreferenceRating += 50; // currently it's a bonus to be close to other enemies
            }
        }

        OptimalTile = PossibleMovementTiles.OrderByDescending(x => x.EnemyPreferenceRating).First();

        // Fallback: if the best tile is where we already stand AND we are not already
        // in a valid attack position, approach the preferred target to close the distance.
        float fallbackMinRange = CurrentSkill.Skill != null ? CurrentSkill.Skill.MinRange : 0f;
        bool alreadyInRange = UnitData.Characters.Any(c =>
        {
            var r = boardManager.GetRangeBetweenTiles(Tile, c.Tile);
            return r >= fallbackMinRange && r <= CurrentSkill.OptimalRange;
        });
        if (OptimalTile == Tile && !alreadyInRange && UnitData.Characters.Count > 0)
        {
            var approachTarget = preferredTarget ?? GetTargetPreference(TargetEnum.closestTarget, UnitData.Characters);
            if (approachTarget != null)
            {
                var approachTile = PossibleMovementTiles
                    .OrderBy(t => boardManager.GetRangeBetweenTiles(t, approachTarget.Tile))
                    .First();

                if (approachTile != Tile)
                    OptimalTile = approachTile;
            }
        }
    }

    public Unit SetTarget()
    {
        List<Character> targetsInRange = new List<Character>();

        foreach (var character in UnitData.Characters)
        {
            var rangeToCharacter = boardManager.GetRangeBetweenTiles(Tile, character.Tile);

            if (rangeToCharacter <= CurrentSkill.OptimalRange)
                targetsInRange.Add(character);
        }

        var preferedTarget = GetTargetPreference(CurrentSkill.TargetPreference, targetsInRange);
        if (preferedTarget != null)
            return preferedTarget;
        else
            return targetsInRange.FirstOrDefault();
    }

    public Unit GetTargetPreference(TargetEnum targetPreference, List<Character> targetList)
    {
        switch (targetPreference)
        {
            case TargetEnum.closestTarget:
                Character closestCharacter = null;
                float shortestDistance = float.MaxValue;

                foreach (var character in targetList)
                {
                    var rangeToCharacter = boardManager.GetRangeBetweenTiles(Tile, character.Tile);

                    if (rangeToCharacter < shortestDistance)
                    {
                        shortestDistance = rangeToCharacter;
                        closestCharacter = character;
                    }
                }
                return closestCharacter;

            case TargetEnum.LowestHealthTarget:
                Character lowestHealthCharacter = targetList.OrderBy(x => x.Hitpoints).First();
                return lowestHealthCharacter;
        }

        return null;
    }

    public IEnumerator MoveToTile()
    {
        if (OptimalTile == Tile)
            yield break;

        yield return StartCoroutine(boardManager.MoveToTile());
    }
}
