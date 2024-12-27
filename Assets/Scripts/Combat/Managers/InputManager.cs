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
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}
