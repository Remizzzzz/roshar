using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum TileState {reachable,occupied,basic,inAttackRange}
public class TileStateManager : MonoBehaviour
{
    public static TileStateManager Instance;
    public Tilemap tileMap;
    private Dictionary<Vector3Int, TileState> states = new();
    public void updateState(Vector3Int p,TileState s){states[p]=s;}
    public TileState getState(Vector3Int p){return states[p];}
    public bool isNotOccupied(Vector3Int p){return states[p]!=TileState.occupied;}
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
