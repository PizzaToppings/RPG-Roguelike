using System.Collections.Generic;
using UnityEngine;

public class DisplacementPreviewManager : MonoBehaviour
{
    public static DisplacementPreviewManager Instance;

    [SerializeField] GameObject projectionPrefab;
    
    Dictionary<Unit, GameObject> activeProjections = new Dictionary<Unit, GameObject>();

    public void CreateInstance()
    {
        Instance = this;
    }

    public void ShowDisplacementPreviews(Skill activeSkill)
    {
        if (activeSkill == null)
        {
            HideAllProjections();
            return;
        }

        // Track which units have projections this frame
        HashSet<Unit> unitsWithProjections = new HashSet<Unit>();

        // IMPORTANT: Use the runtime Skill.SkillPartGroups, not mainSkillSO.SkillPartGroups
        var currentSpg = activeSkill.SkillPartGroups[SkillData.SkillPartGroupIndex];
        
        foreach (var skillPart in currentSpg.skillParts)
        {
            if (skillPart.displacementEffect == null || !skillPart.displacementEffect.UseDisplacement)
                continue;

            if (skillPart.PartData == null || !skillPart.PartData.CanCast)
                continue;

            var unitsToDisplace = skillPart.displacementEffect.Unit?.PartData?.TargetsHit;
            var targetTiles = skillPart.displacementEffect.TargetPosition?.PartData?.TilesHit;

            if (unitsToDisplace == null || targetTiles == null)
                continue;

            // Match units to their target positions
            for (int i = 0; i < Mathf.Min(unitsToDisplace.Count, targetTiles.Count); i++)
            {
                var unit = unitsToDisplace[i];
                var targetTile = targetTiles[i];

                if (unit == null || targetTile == null)
                    continue;

                unitsWithProjections.Add(unit);
                ShowProjection(unit, targetTile.position);
            }
        }

        // Hide projections for units that are no longer displaced
        List<Unit> toRemove = new List<Unit>();
        foreach (var kvp in activeProjections)
        {
            if (!unitsWithProjections.Contains(kvp.Key))
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var unit in toRemove)
        {
            HideProjection(unit);
        }
    }

    void ShowProjection(Unit unit, Vector3 targetPosition)
    {
        GameObject projection;

        // Reuse existing projection or create new one
        if (activeProjections.ContainsKey(unit))
        {
            projection = activeProjections[unit];
        }
        else
        {
            // Create projection from prefab or dynamically
            if (projectionPrefab != null)
            {
                projection = Instantiate(projectionPrefab, transform);
            }
            else
            {
                // Create projection dynamically
                projection = new GameObject($"Projection_{unit.name}");
                projection.transform.SetParent(transform);
                var spriteRenderer = projection.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Units";
                spriteRenderer.sortingOrder = -1; // Behind the real units
            }

            activeProjections[unit] = projection;
        }

        // Update projection appearance
        var projectionSprite = projection.GetComponent<SpriteRenderer>();
        if (projectionSprite != null && unit.modelSprite != null)
        {
            projectionSprite.sprite = unit.modelSprite.sprite;
            projectionSprite.flipX = unit.modelSprite.flipX;
            
            // Grey out and reduce alpha
            Color greyColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
            projectionSprite.color = greyColor;
        }

        // Update projection position with the same offset as the unit's sprite
        Vector3 spriteOffset = Vector3.zero;
        if (unit.modelSprite != null)
        {
            // Get the local position offset of the sprite relative to the unit
            spriteOffset = unit.modelSprite.transform.localPosition;
        }
        
        projection.transform.position = targetPosition + spriteOffset;
        projection.SetActive(true);
    }

    void HideProjection(Unit unit)
    {
        if (activeProjections.ContainsKey(unit))
        {
            var projection = activeProjections[unit];
            if (projection != null)
            {
                projection.SetActive(false);
            }
        }
    }

    public void HideAllProjections()
    {
        foreach (var kvp in activeProjections)
        {
            if (kvp.Value != null)
            {
                kvp.Value.SetActive(false);
            }
        }
    }

    public void ClearAllProjections()
    {
        foreach (var kvp in activeProjections)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        activeProjections.Clear();
    }

    void OnDestroy()
    {
        ClearAllProjections();
    }
}
