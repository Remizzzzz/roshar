using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum TileState {reachable,occupied,basic,inAttackRange,summoningTile}
public class TileStateManager : MonoBehaviour
{
    /** * TileStateManager is a singleton that manages the state of each tile in the tilemap.
     * It keeps track of the state of each tile and allows for updating and checking the state
        * of tiles. The states are stored in a dictionary with the tile position as the key.
    */
    public static TileStateManager Instance;
    public Tilemap tileMap;
    public Tile fluctSummoningTile;
    public List<Vector3Int> highPlayerPos;
    public Tile fusSummoningTile;
    public List<Vector3Int> lowPlayerPos;

    private Dictionary<Vector3Int, TileState> states = new();
    public void updateState(Vector3Int p,TileState s){states[p]=s;}
    public TileState getState(Vector3Int p){return states[p];}
    public bool isNotOccupied(Vector3Int p){return states[p]!=TileState.occupied;}
    public void resetMap(){
        BoundsInt bounds = tileMap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin) ///Set the boundaries of the map
        {
            if (tileMap.HasTile(pos) && TileStateManager.Instance.isNotOccupied(pos)) ///Go through each tile and reset those that aren't occupied
            {
                TileStateManager.Instance.updateState(pos,TileState.basic);
                tileMap.RefreshTile(pos);
            }
        }
    }
    private void initDict(){
        BoundsInt bounds = tileMap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tileMap.HasTile(pos))
            {
                states.Add(pos,TileState.basic);
            }
        }
    }
    public void setBoardSide(bool fluctIsHighPlayer){ /// Sets the board side depending on which player is the high player
        if (fluctIsHighPlayer) //If the fluctuomancer player starts first, set the first player tiles as fluct summoning tiles
        {
            foreach (Vector3Int pos in highPlayerPos)
            {
                tileMap.SetTile(pos, fluctSummoningTile);
            }
            foreach(Vector3Int pos in lowPlayerPos) //Set the second player tiles as fused summoning tiles
            {
                tileMap.SetTile(pos, fusSummoningTile);
            }
        }
        else //If the fused player starts first, set the first player tiles as fused summoning tiles
        {
            foreach (Vector3Int pos in highPlayerPos)
            {
                tileMap.SetTile(pos, fusSummoningTile);
            }
            foreach(Vector3Int pos in lowPlayerPos) //Set the second player tiles as fused summoning tile
            {
                tileMap.SetTile(pos, fluctSummoningTile);
            }
        }
    }
    void Awake(){
        Instance=this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initDict();
        setBoardSide(GameData.highPlayer); // Set the board side depending on which player is the high player
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
