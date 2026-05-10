using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    SkillsManager skillsManager;

    Camera camera;


    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        skillsManager = SkillsManager.Instance;
        camera = Camera.main;
    }

    void Update()
	{
        if (EventSystem.current.IsPointerOverGameObject())
            return;


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
			switch (UnitData.CurrentAction)
			{
                case CurrentActionKind.CastingSkillshot:
                    skillsManager.StartCasting();
                    return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
		{
            if (SkillData.SkillPartGroupIndex > 0)
            {
                SkillData.SkillPartGroupIndex--;
            }
            else
            {
                var character = UnitData.ActiveUnit as Character;
                character.StopCasting();
            }
        }
    }

    public Vector3 GetMousePosition()
    {
        Vector3 worldPos = camera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        return worldPos;
    }
}
