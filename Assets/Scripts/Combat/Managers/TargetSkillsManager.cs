using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TargetSkillsManager : MonoBehaviour
{
    public static TargetSkillsManager Instance;

    BoardManager boardManager;
    SkillsManager skillsManager;
    InputManager inputManager;

    public void CreateInstance()
    {
        Instance = this;
    }

	public void Init()
	{
        boardManager = BoardManager.Instance;
        skillsManager = SkillsManager.Instance;
        inputManager = InputManager.Instance;
    }

    public void PreviewLine(SO_LineSkill skillData)
    {
        List<int> directions = new List<int>();

        foreach (var originTile in skillData.OriginTiles)
        {
            foreach (var direction in skillData.Angles)
            {
                var dir = direction + GetDirection(skillData);
                directions.Add(dir);
            }

            PreviewLineCast(originTile, directions.ToArray(), skillData);
        }
    }

    public void PreviewCone(SO_ConeSkill skillData)
    {
        foreach (var originTile in skillData.OriginTiles)
        {
            var direction = GetDirection(skillData);
            PreviewConeCast(direction, skillData);
            CorrectConeCast(originTile, skillData);
        }
    }

    public void PreviewHalfCircle(SO_HalfCircleSkill skillData)
    {
        foreach (var originTile in skillData.OriginTiles)
        {
            var direction = GetDirection(skillData);
            PreviewHalfCircleCast(direction, skillData);
            CorrectConeCast(originTile, skillData);
        }
    }

    int GetDirection(SO_Skillpart skillData)
    {
        if (skillData.TargetTileKind == TargetTileEnum.PreviousDirection)
        {
            skillData.FinalDirection = skillData.GetPreviousSkillPart().FinalDirection;
            return skillData.FinalDirection;
        }

        var directionAnchorTile = GetDirectionAnchorTile(skillData);

        var tileDirectionIndex = 0;
        var mousePosition = inputManager.GetMousePosition();
        Vector3 dir = (mousePosition - directionAnchorTile.position).normalized;
        Vector2 mouseDirection = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.z));

        var directions = boardManager.Directions;
        for (int i = 0; i < directions.Length; i++)
        {
            if (mouseDirection == directions[i])
            {
                tileDirectionIndex = i;
                break;
            }
        }

        skillData.FinalDirection = tileDirectionIndex;
        return tileDirectionIndex;
    }

    public BoardTile GetDirectionAnchorTile(SO_Skillpart skillpart)
    {
        var tiles = new List<BoardTile>();
        var previousTargetsHit = new List<Unit>();
        var previousTilesHit = new List<BoardTile>();

        if (skillpart.SkillPartIndex > 0)
        {
            previousTargetsHit = SkillData.GetPreviousTargetsHit(skillpart.SkillPartIndex);
            previousTilesHit = SkillData.GetPreviousTilesHit(skillpart.SkillPartIndex);
        }

        switch (skillpart.DirectionAnchor)
        {
            case OriginTileEnum.Caster:
                SkillData.Caster = UnitData.ActiveUnit;
                tiles.Add(SkillData.Caster.Tile);
                break;

            case OriginTileEnum.LastTargetTile:
                if (previousTargetsHit.Count == 0)
                    return null;

                previousTargetsHit.ForEach(x => tiles.Add(x.Tile));
                break;

            case OriginTileEnum.LastTile:
                tiles.AddRange(previousTilesHit);
                break;

            case OriginTileEnum.GetFromSkillPart:
                var tileList = skillpart.OriginTileSkillParts.SelectMany(x => x.PartData.TilesHit).ToList();
                tiles.AddRange(tileList);
                break;
        }

        return tiles[0];
    }

    public void GetAOE(SO_Skillpart data)
    {
        boardManager.SetAOE(data.MaxRange, data.OriginTiles, data);
    }

    public void SetSkillAOE(float skillRangeLeft, BoardTile currentTile, SO_Skillpart skillData)
    {
        if (skillRangeLeft <= 0)
            return;

        var skillPartIndex = skillData.SkillPartIndex;

        foreach (var tile in currentTile.connectedTiles)
        {
            if (tile == null)
                continue;

            var nextSkillRange = skillRangeLeft;
            nextSkillRange -= boardManager.GetRangeReduction(currentTile, tile);

            var index = skillData.SkillPartIndex;
            while (tile.skillshotsRangeLeft.Count <= index)
                tile.skillshotsRangeLeft.Add(-0.5f);

            if (nextSkillRange > tile.skillshotsRangeLeft[index])
            {
                tile.skillshotsRangeLeft[index] = nextSkillRange;

                SkillData.AddTileToCurrentList(skillPartIndex, tile);

                tile.PreviousTile = currentTile;

                SetSkillAOE(nextSkillRange, tile, skillData);
            }
        }
    }

    public void FilterAOESkillTileList(int skillPartIndex, SO_Skillpart skillData)
    {
        var tiles = new List<BoardTile>();

        foreach (var tile in SkillData.GetCurrentTilesHit(skillPartIndex))
        {
            if (skillData.FreeSpacesOnly && tile.currentUnit != null)
                continue;

            if (skillData.MaxRange - tile.skillshotsRangeLeft[skillPartIndex] < skillData.MinRange)
                continue;

            if (tile.IsBlocked || (boardManager.TileIsBehindClosedTile(tile, skillData.OriginTiles[0]) && skillData.AffectedByBlockedTiles))
                continue;

            if (UnitData.ActiveUnit.Friendly)
                tile.SetColor(skillData.tileColor);

            var target = boardManager.FindTarget(tile, skillData);
            SkillData.AddTargetToCurrentList(skillPartIndex, target);

            tiles.Add(tile);
        }

        skillData.PartData.TilesHit = tiles;
    }

    public void PreviewLineCast(BoardTile originTile, int[] directions, SO_LineSkill skillpart)
    {
        Vector2Int startCoordinates = originTile.Coordinates;
        Vector2Int curentCoordinates = new Vector2Int();
        Vector2Int nextCoordinates = new Vector2Int();
        List<Vector2Int> line = new List<Vector2Int>();

        var skillPartIndex = skillpart.SkillPartIndex;
        int pierceAmount = skillpart.PierceAmount;

        foreach (var dir in directions)
        {
            curentCoordinates = nextCoordinates = startCoordinates;
            var direction = GetCorrectedDirection(dir);

            for (float r = 0; r < skillpart.MaxRange;)
            {
                curentCoordinates = nextCoordinates;
                nextCoordinates = curentCoordinates + boardManager.Directions[direction];

                if (r >= skillpart.MinRange)
                    line.Add(nextCoordinates);

                r += boardManager.GetRangeReduction(curentCoordinates, nextCoordinates);
                curentCoordinates = nextCoordinates;
            }

            foreach (var coordinate in line)
            {
                var tile = boardManager.GetBoardTile(coordinate);

                if (tile == null)
                    continue;

                if (tile.IsBlocked && skillpart.AffectedByBlockedTiles)
                    break;

                tile.SetColor(skillpart.tileColor);

                SkillData.AddTileToCurrentList(skillPartIndex, tile);

                var target = boardManager.FindTarget(tile, skillpart);
                if (target != null)
                {
                    SkillData.AddTargetToCurrentList(skillPartIndex, target);
                    if (pierceAmount != -1)
                    {
                        if (pierceAmount == 0)
                            break;
                        pierceAmount--;
                    }
                }
            }

            if (skillpart.GetLastTileOnly)
			{
                var lastTile = skillpart.PartData.TilesHit[^1];

                skillpart.PartData.TilesHit.Clear();
                SkillData.AddTileToCurrentList(skillPartIndex, lastTile);

                skillpart.PartData.TargetsHit.Clear();
                var target = boardManager.FindTarget(lastTile, skillpart);
                SkillData.AddTargetToCurrentList(skillpart.SkillPartIndex, target);

                lastTile.SetColor(skillpart.endColor);
            }

            line.Clear();
        }
    }

    public void PreviewConeCast(int direction, SO_ConeSkill skillpart)
    {
        Vector2Int startCoordinates = SkillData.Caster.Tile.Coordinates;
        Vector2Int curentCoordinates = new Vector2Int();
        Vector2Int nextCoordinates = new Vector2Int();
        List<List<Vector2Int>> lines = new List<List<Vector2Int>>();
        List<Vector2Int> line = new List<Vector2Int>();

        var skillPartIndex = skillpart.SkillPartIndex;

        var width = skillpart.isWide ? 2 : 1;

        foreach (var originTile in skillpart.OriginTiles)
        {
            for (int i = -1; i < width; i++)
            {
                var dir = GetCorrectedDirection(direction + i);

                line = new List<Vector2Int>();
                curentCoordinates = nextCoordinates = startCoordinates;

                for (float r = 0; r < skillpart.MaxRange;)
                {
                    curentCoordinates = nextCoordinates;
                    nextCoordinates = curentCoordinates + boardManager.Directions[dir];
                    line.Add(nextCoordinates);

                    r += boardManager.GetRangeReduction(curentCoordinates, nextCoordinates);
                    curentCoordinates = nextCoordinates;
                }

                foreach (var coordinate in line)
                {
                    var tile = boardManager.GetBoardTile(coordinate);

                    if (tile == null)
                        continue;

                    if (boardManager.GetRangeBetweenTiles(originTile.Coordinates, coordinate) < skillpart.MinRange)
                        continue;

                    if (tile.IsBlocked || (boardManager.TileIsBehindClosedTile(tile, skillpart.OriginTiles[0]) && skillpart.AffectedByBlockedTiles))
                        continue;

                    SkillData.AddTileToCurrentList(skillpart.SkillPartIndex, tile);
                }
                lines.Add(line);
            }

            if (skillpart.isWide == false)
            {
                FillConeCast(lines, skillpart, originTile);
            }
            else
            {
                var firstList = new List<List<Vector2Int>> { lines[0], lines[1] };
                FillConeCast(firstList, skillpart, originTile);
                var secondList = new List<List<Vector2Int>> { lines[1], lines[2] };
                FillConeCast(secondList, skillpart, originTile);
            }

            lines.Clear();
        }
    }

    int GetCorrectedDirection(int dir)
    {
        var direction = dir % 8;
        while (direction < 0)
            direction += 8;

        return direction;
    }

    void FillConeCast(List<List<Vector2Int>> lines, SO_Skillpart skillpart, BoardTile originTile)
    {
        List<BoardTile> tileList = new List<BoardTile>();

        var skillPartIndex = skillpart.SkillPartIndex;

        lines.Sort((x, y) => x.Count.CompareTo(y.Count));
        var shortLine = lines[0];
        var longLine = lines[1];

        var direction = shortLine[0] - longLine[0];
        var range = 0;

        for (int i = 1; i < shortLine.Count; i++)
        {
            range = i + 1;
            for (int r = 1; r < range; r++)
            {
                var coordinates = longLine[i] + direction * r;
                var tile = boardManager.GetBoardTile(coordinates);

                if (tile != null && tileList.Contains(tile) == false)
                    tileList.Add(tile);
            }
        }

        var rangeReductor = 1;
        for (int i = shortLine.Count; i < longLine.Count; i++)
        {
            range = Mathf.RoundToInt(range / rangeReductor);
            for (int r = 1; r < range; r++)
            {
                var coordinates = longLine[i] + direction * r;
                var tile = boardManager.GetBoardTile(coordinates);

                if (tile != null)
                    tileList.Add(tile);
            }
            rangeReductor++;
        }

        for (int t = 0; t < tileList.Count; t++)
        {
            if (boardManager.GetRangeBetweenTiles(originTile, tileList[t]) < skillpart.MinRange)
                continue;

            var tile = tileList[t];

            if (tile.IsBlocked || (boardManager.TileIsBehindClosedTile(tile, skillpart.OriginTiles[0]) && skillpart.AffectedByBlockedTiles))
                continue;

            SkillData.AddTileToCurrentList(skillpart.SkillPartIndex, tile);
        }
    }

    public void PreviewHalfCircleCast(int direction, SO_HalfCircleSkill skillpart)
    {
        Vector2Int startCoordinates = SkillData.Caster.Tile.Coordinates;
        Vector2Int curentCoordinates = new Vector2Int();
        Vector2Int nextCoordinates = new Vector2Int();
        List<List<Vector2Int>> lines = new List<List<Vector2Int>>();
        List<Vector2Int> line = new List<Vector2Int>();

        var skillPartIndex = skillpart.SkillPartIndex;

        foreach (var originTile in skillpart.OriginTiles)
        {
            for (int i = -2; i < 3; i++)
            {
                var dir = GetCorrectedDirection(direction + i);

                line = new List<Vector2Int>();
                curentCoordinates = nextCoordinates = startCoordinates;

                for (float r = 0; r < skillpart.MaxRange;)
                {
                    curentCoordinates = nextCoordinates;
                    nextCoordinates = curentCoordinates + boardManager.Directions[dir];
                    line.Add(nextCoordinates);

                    r += boardManager.GetRangeReduction(curentCoordinates, nextCoordinates);
                    curentCoordinates = nextCoordinates;
                }

                foreach (var coordinate in line)
                {
                    var tile = boardManager.GetBoardTile(coordinate);

                    if (tile == null)
                        continue;

                    if (boardManager.GetRangeBetweenTiles(originTile.Coordinates, coordinate) < skillpart.MinRange)
                        continue;

                    SkillData.AddTileToCurrentList(skillpart.SkillPartIndex, tile);
                }
                lines.Add(line);
            }

            for (int i = 1; i < 5; i++)
            {
                var fillLines = new List<List<Vector2Int>> { lines[i - 1], lines[i] };
                FillConeCast(fillLines, skillpart, originTile);
            }

            lines.Clear();
        }
    }

    public void CorrectConeCast(BoardTile tile, SO_Skillpart data)
    {
        var tilesHitOld = new List<BoardTile>(data.PartData.TilesHit);

        data.PartData.TilesHit.Clear();
        SetSkillAOE(data.MaxRange, tile, data);

        var finalPath = tilesHitOld.Intersect(data.PartData.TilesHit).ToList();
        data.PartData.TilesHit = finalPath;
        data.PartData.TargetsHit.Clear();

        data.PartData.TilesHit.ForEach(t => skillsManager.TargetTileWithSkill(t, data));
    }
}
