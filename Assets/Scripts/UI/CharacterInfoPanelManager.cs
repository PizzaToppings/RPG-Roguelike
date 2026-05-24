using UnityEngine;
using TMPro;

public class CharacterInfoPanelManager : BaseInfoPanelManager
{
    public static CharacterInfoPanelManager Instance;

    [Header("Panel Root")]
    [SerializeField] private GameObject characterInfoPanel;

    protected override GameObject PanelRoot => characterInfoPanel;

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    private void Update()
    {
        if (!isVisible) return;
    }

    /// <summary>Call from Character.OnMouseEnter to show the panel.</summary>
    public void ShowPanel(Character character)
    {
        if (characterInfoPanel == null) return;

        PopulateBaseStats(character);
        PopulateStatusEffects(character);

        characterInfoPanel.SetActive(true);
        isVisible = true;
    }

    /// <summary>Call from Character.OnMouseExit to hide the panel.</summary>
    public override void HidePanel()
    {
        base.HidePanel();
    }
}

