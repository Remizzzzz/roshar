using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class PieceMovement : MonoBehaviour
{
    //Public objects
    public Tilemap tileMap;
    public bool isFluct = true;
    [Range(1,5)]
    public int nbMov=1;
    //private objects
    private Vector3Int[] neighbourOffsetOdd = new Vector3Int[]{
        new Vector3Int(0,1,0), //up left
        new Vector3Int(1,1,0), //up right
        new Vector3Int(1,0,0), // right
        new Vector3Int(1,-1,0), //down right
        new Vector3Int(0,-1,0), //down left
        new Vector3Int(-1,0,0), //left
    };
    private Vector3Int[] neighbourOffset = new Vector3Int[]{
        new Vector3Int(-1,1,0), //up left
        new Vector3Int(0,1,0), //up right
        new Vector3Int(1,0,0), // right
        new Vector3Int(0,-1,0), //down right
        new Vector3Int(-1,-1,0), //down left
        new Vector3Int(-1,0,0), //left
    };
    private bool onMap=false;
    private bool moving = false;
    private bool waitClick=false;
    private Vector3Int curPos;
    private Vector3Int cellPos; //New cell selected
    private List<Vector3Int> reachableTiles;
    //Private methods
    bool IsInvokable(){
            bool invokable=false;
            TileBase tileS = tileMap.GetTile(cellPos);
            BaseTile tileSelected = tileS as BaseTile;
            invokable=(tileSelected!=null && tileSelected.isInvocationTile && tileSelected.isFluct==this.isFluct && TileStateManager.Instance.isNotOccupied(cellPos));
            return invokable;
    }
    List<Vector3Int> GetReachableTiles(){ //Should only be called once onMap is true and if the game has started
        List<Vector3Int> validCoor= new List<Vector3Int>{curPos};
        if(onMap){
            List<Vector3Int> search = new List<Vector3Int>();

            for (int i=0; i<nbMov;i++){
                foreach(Vector3Int rSearch in validCoor){
                    foreach(Vector3Int neighbour in (Math.Abs(rSearch.y)%2==1?neighbourOffsetOdd:neighbourOffset)){ //Change the offset depending on Y (odd or not)
                        if (!validCoor.Contains(neighbour+rSearch) && !search.Contains(neighbour+rSearch) && tileMap.GetTile(neighbour+rSearch)!=null) {
                            if (TileStateManager.Instance.isNotOccupied(neighbour+rSearch)) search.Add(neighbour+rSearch); //If the coor isn't in valid or search or in the map or is occupied, add it to search
                        }
                    }
                }
                validCoor.AddRange(search);
                search.Clear();
            }
        }
        validCoor.Remove(curPos);
        return validCoor;
    }
    //onMouse
    void OnMouseDown(){
        transform.localScale=new Vector3(1.3f,1.3f,1.3f);
        moving=true;
        if (onMap){
            reachableTiles = GetReachableTiles();
            foreach (Vector3Int tilePos in reachableTiles){
                TileStateManager.Instance.updateState(tilePos,TileState.reachable);
                tileMap.RefreshTile(tilePos);
            }
        }
    }
    void OnMouseDrag(){   
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Assure que l'objet reste sur le plan 2D
        transform.localPosition = mousePosition;
    }
    void OnMouseUp(){
        transform.localScale = new Vector3(1,1,1); // Reset the selected state of the piece
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        cellPos = tileMap.WorldToCell(mousePosition); //Actu the cellPos coordinates to the current cell selected
        if (!onMap) {
            if (cellPos!=curPos ){ //If the player moved the piece
                if (IsInvokable()){
                    transform.position = tileMap.GetCellCenterWorld(cellPos); //Move the piece and actu the curPos, toggle the onMap properties.
                    curPos=cellPos;
                    TileStateManager.Instance.updateState(curPos,TileState.occupied);
                    onMap=true;
                } else {
                    transform.position = tileMap.GetCellCenterWorld(curPos);
                }
                moving=false;
            }
        } else {
            if (cellPos==curPos ){ //If the player just clicked on the piece : Clicked behaviour
                transform.position = tileMap.GetCellCenterWorld(curPos);
                waitClick=true;
                //We don't reset moving and the reachable piece
            } else { //If the player moved the piece
                if (reachableTiles.Contains(cellPos)){
                    transform.position = tileMap.GetCellCenterWorld(cellPos); //Move the piece and actu the curPos and the state of the tiles (occupied)
                    TileStateManager.Instance.updateState(curPos,TileState.basic);
                    curPos=cellPos;
                    TileStateManager.Instance.updateState(curPos,TileState.occupied);
                } else {
                    transform.position = tileMap.GetCellCenterWorld(curPos);
                }

                moving=false;
                foreach (Vector3Int tilePos in reachableTiles){
                    if (TileStateManager.Instance.getState(tilePos)==TileState.reachable){
                        TileStateManager.Instance.updateState(tilePos,TileState.basic);
                    }
                    tileMap.RefreshTile(tilePos);
                }
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //align on grid
        Vector3Int cellPos = tileMap.WorldToCell(transform.position);
        transform.position = tileMap.GetCellCenterWorld(cellPos);
        curPos=cellPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitClick && Input.GetMouseButtonDown(0)) 
        {
            waitClick = false; // Stop waiting for click
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get Mouse Position
            mousePosition.z = 0; 
            cellPos = tileMap.WorldToCell(mousePosition); // Convert into tilemap coor

            // If destination valid
            if (cellPos!=curPos)
            {
                if (reachableTiles.Contains(cellPos)){
                    transform.position = tileMap.GetCellCenterWorld(cellPos);
                    TileStateManager.Instance.updateState(curPos,TileState.basic);
                    curPos=cellPos;
                    TileStateManager.Instance.updateState(curPos,TileState.occupied);
                }
                moving = false; //Valid or not, we reset (click elsewhere to cancel)
                foreach (Vector3Int tilePos in reachableTiles){
                    if (TileStateManager.Instance.getState(tilePos)==TileState.reachable){
                        TileStateManager.Instance.updateState(tilePos,TileState.basic);
                    }
                    tileMap.RefreshTile(tilePos);
                }
            }

        }
    }

    //getter
    public bool getMoving() => moving;
    public bool getOnMap() => onMap;
}
