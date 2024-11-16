using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardManager))]
public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance;
    BoardManager boardManager;
    Camera camera;

    public void Init()
    {
        Instance = this;
        boardManager = GetComponent<BoardManager>();
        camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            UnitData.CurrentActiveUnit.CastSkill();
        }

        if (UnitData.CurrentAction == UnitData.CurrentActionKind.CastingSkillshot)
		{
            UnitData.CurrentActiveUnit.PreviewSkills(boardManager.currentMouseTile);
		}
    }

    public void PreviewLine(SO_LineSkill skillData, BoardTile targetTile)
    {
        List<int> directions = new List<int>();

        foreach (var originTile in skillData.OriginTiles)
        {
            foreach (var direction in skillData.Angles)
            {
                var dir = direction + GetDirection(originTile, skillData);
                directions.Add(dir);
            }

            boardManager.PreviewLineCast(directions.ToArray(), skillData);
        }
    } 

    public void PreviewCone(SO_ConeSkill skillData, BoardTile targetTile)
    {
        foreach (var originTile in skillData.OriginTiles)
        {
            var direction = GetDirection(originTile, skillData);
            boardManager.PreviewConeCast(direction, skillData);
        }
    }

    int GetDirection(BoardTile originTile, BoardTile targetTile, SO_Skillpart skillData)
    {
        if (skillData.TargetTileKind == TargetTileEnum.PreviousDirection)
        {
            skillData.FinalDirection = skillData.GetPreviousSkillPart().FinalDirection;
            return skillData.FinalDirection;
        }

        var tileDirectionIndex = 0;
        Vector2 dir = targetTile.Coordinates - originTile.Coordinates;
        float targetDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float?[] directions = new float?[originTile.connectedTiles.Length];

        for (int i = 0; i < originTile.connectedTiles.Length; i++)
        {
            if (originTile.connectedTiles[i] == null)
			{
                directions[i] = null;
                continue;
			}

            dir = originTile.connectedTiles[i].Coordinates - originTile.Coordinates;
			float? direction = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            directions[i] = direction;
        }

        float dif = float.MaxValue;

        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i] == null)
			{
                continue;
			}

            float difference = Mathf.Abs(targetDirection - (float)directions[i]);

            if (difference < dif)
			{
                dif = difference;
                tileDirectionIndex = i;
            }
        }

        skillData.FinalDirection = tileDirectionIndex;
        return tileDirectionIndex;
    }

    int GetDirection(BoardTile originTile, SO_Skillpart skillData)
    {
        if (skillData.TargetTileKind == TargetTileEnum.PreviousDirection)
        {
            skillData.FinalDirection = skillData.GetPreviousSkillPart().FinalDirection;
            return skillData.FinalDirection;
        }

        var tileDirectionIndex = 0;
        var mousePosition = GetMousePosition();
		Vector3 dir = (mousePosition - originTile.position).normalized;
        Vector2 mouseDirection = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.z));

        var directions = boardManager.Directions;
        for (int i = 0; i < directions.Length; i++)
        {
            if (mouseDirection == directions[i])
            {
                tileDirectionIndex= i;
                break;
            }
        }

		skillData.FinalDirection = tileDirectionIndex;
        return tileDirectionIndex;
    }

    Vector3 GetMousePosition()
	{
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero); // Flat plane at Y=0

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance); // Get the point where the ray intersects the plane
        }

        return Vector3.zero;
    }

    public void GetAOE(SO_Skillpart data)
    {
         boardManager.SetAOE(data.Range, data.OriginTiles, data);
    }
}
