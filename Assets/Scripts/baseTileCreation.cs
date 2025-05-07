using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Custom/Base Tile")]
public class BaseTile : Tile
{
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
            else tileData.color = color;
        } else tileData.color = color;
    }
}
