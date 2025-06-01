using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum TileState {reachable,occupied,basic,inAttackRange,summoningTile}
public class TileStateManager : MonoBehaviour
{
    public static TileStateManager Instance;
    public Tilemap tileMap;
    private Dictionary<Vector3Int, TileState> states = new();
    public void updateState(Vector3Int p,TileState s){states[p]=s;}
    public TileState getState(Vector3Int p){return states[p];}
    public bool isNotOccupied(Vector3Int p){return states[p]!=TileState.occupied;}
    public void resetMap(){
        BoundsInt bounds = tileMap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin) //Set the boundaries of the map
        {
            if (tileMap.HasTile(pos) && TileStateManager.Instance.isNotOccupied(pos)) //Go through each tile and reset those that aren't occupied
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
    void Awake(){
        Instance=this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initDict();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
