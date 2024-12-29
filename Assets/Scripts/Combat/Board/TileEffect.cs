using UnityEngine;

public class TileEffect 
{
    public SO_TileEffect OriginalTileEffect;

    public int Duration;

    public void Init(SO_TileEffect originalTileEffect)
	{
        OriginalTileEffect = originalTileEffect;
        Duration = originalTileEffect.MaxDuration;
    }
}
