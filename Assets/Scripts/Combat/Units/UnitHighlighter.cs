using UnityEngine;

/// <summary>
/// Attach to any Unit GameObject that has a SpriteRenderer child (modelSprite).
/// Assign the SpriteOutline material asset to <see cref="outlineMaterialSource"/> in the Inspector.
/// Call SetHighlight(true/false) to toggle the outline hover effect.
/// </summary>
[RequireComponent(typeof(Unit))]
public class UnitHighlighter : MonoBehaviour
{
    [Tooltip("Drag the SpriteOutline material asset here.")]
    [SerializeField] Material outlineMaterialSource;
    [SerializeField] Color outlineColor = new Color(1f, 1f, 0f, 1f);
    [SerializeField, Range(0f, 10f)] float outlineThickness = 1.5f;

    static readonly int OutlineColorId     = Shader.PropertyToID("_OutlineColor");
    static readonly int OutlineThicknessId = Shader.PropertyToID("_OutlineThickness");

    Unit unit;
    Material outlineMaterial;
    Material originalMaterial;
    bool initialized;
    bool highlighted;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    /// <summary>
    /// Deferred setup: runs the first time SetHighlight is called, after Unit.Init()
    /// has had a chance to assign modelSprite.
    /// </summary>
    void EnsureInitialized()
    {
        if (initialized) return;
        initialized = true;

        if (unit.modelSprite == null)
        {
            Debug.LogWarning($"[UnitHighlighter] {name}: modelSprite is null. Make sure Unit.Init() has run before the first highlight call.");
            return;
        }

        if (outlineMaterialSource == null)
        {
            Debug.LogWarning($"[UnitHighlighter] {name}: No outline material assigned. Create a material using the 'Sprites/Outline' shader and assign it in the Inspector.");
            return;
        }

        originalMaterial = unit.modelSprite.sharedMaterial;

        // Create a per-instance copy so units don't share colour/thickness state.
        outlineMaterial = new Material(outlineMaterialSource);
        outlineMaterial.SetColor(OutlineColorId, outlineColor);
        outlineMaterial.SetFloat(OutlineThicknessId, outlineThickness);
    }

    public void SetHighlight(bool enable)
    {
        EnsureInitialized();

        if (unit.modelSprite == null || outlineMaterial == null) return;
        if (highlighted == enable) return;

        highlighted = enable;
        unit.modelSprite.material = enable ? outlineMaterial : originalMaterial;
    }

    void OnDestroy()
    {
        if (outlineMaterial != null)
            Destroy(outlineMaterial);
    }
}
