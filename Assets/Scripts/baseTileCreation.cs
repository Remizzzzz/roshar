using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Custom/Base Tile")]
public class BaseTile : Tile
{
    /**This class is used to create a base tile that can be used in the game.
     * It is a custom tile that can be used to create different types of tiles.
     * It is used to create the base tile for the game. So it's mainly an editor script.
     * It parameters each tile to change it's color based on the tile state.
     * It also allows to shrink the tile to fit the grid better.
     */
    [Range(0.1f, 1f)]
    public float shrinkFactor = 0.9f;
    public bool isFluct=true;
    public bool isSummoningTile=false;
    internal Color originalColor;

    private void OnEnable(){
        originalColor=this.color;
    }
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        Vector3 scale = Vector3.one * shrinkFactor;
        tileData.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
        if (TileStateManager.Instance != null) {
            if (TileStateManager.Instance.getState(position)==TileState.reachable) tileData.color = new Color(185f/255f,1f,1f);
            else if (TileStateManager.Instance.getState(position)==TileState.inAttackRange) tileData.color = new Color(181f/255f,181f/255f,125f/255f);
            else if (TileStateManager.Instance.getState(position) == TileState.summoningTile) tileData.color = new Color(0f,175f/255f,50f/255f);
            else tileData.color = color;
        } else tileData.color = color;
    }
}
