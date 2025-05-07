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
    public bool isSpecial=false;
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
    SpriteRenderer sr;
    private bool waitClick=false;
    private Vector3Int curPos;
    private Vector3Int cellPos; //New cell selected
    private List<Vector3Int> reachableTiles;
    //Private methods
    private void resetMap(){
        BoundsInt bounds = tileMap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tileMap.HasTile(pos) && TileStateManager.Instance.isNotOccupied(pos))
            {
                TileStateManager.Instance.updateState(pos,TileState.basic);
                tileMap.RefreshTile(pos);
            }
        }
    }
    private bool IsSummonable(){
        bool summonable=true;
        TileBase tileS = tileMap.GetTile(cellPos);
        BaseTile tileSelected = tileS as BaseTile;
        /*
        Base Conditions for a base piece to be summonable : 
        - The tile selected is on the map
        - The tile selected is an Summoning Tile
        - The tile selected is an Summoning Tile of the color of the piece
        - The tile selected is not occupied
        - The turn on which the piece has been summoned is a summoning turn

        */
        summonable=(tileSelected!=null && tileSelected.isSummoningTile && tileSelected.isFluct==this.isFluct && TileStateManager.Instance.isNotOccupied(cellPos) && TurnManager.Instance.isSummonable(isSpecial));
        return summonable;
    } 
    private List<Vector3Int> GetReachableTiles(){ //Should only be called once onMap is true and if the game has started
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
        if (TurnManager.Instance.isPlayerTurn(isFluct)){
            reachableTiles=null;
            //enable selectedState
            transform.localScale=new Vector3(1.3f,1.3f,1.3f);
            sr.sortingOrder=2;
            //updating static state manager
            PieceStateManager.Instance.updateState(this,PieceState.moving,isFluct);
            if (onMap){
                reachableTiles = GetReachableTiles();
                foreach (Vector3Int tilePos in reachableTiles){
                    TileStateManager.Instance.updateState(tilePos,TileState.reachable);
                    tileMap.RefreshTile(tilePos);
                }
            }
        }
    }
    void OnMouseDrag(){   
        if (TurnManager.Instance.isPlayerTurn(isFluct)){
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Assure que l'objet reste sur le plan 2D
            transform.localPosition = mousePosition;
        }
    }
    void OnMouseUp(){
        if (TurnManager.Instance.isPlayerTurn(isFluct)){
            transform.localScale = new Vector3(1,1,1); // Reset the selected state of the piece
            sr.sortingOrder=1;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            cellPos = tileMap.WorldToCell(mousePosition); //Actu the cellPos coordinates to the current cell selected
            if (!onMap) {
                if (cellPos!=curPos ){ //If the player moved the piece
                    if (IsSummonable()){
                        transform.position = tileMap.GetCellCenterWorld(cellPos); //Move the piece and actu the curPos, toggle the onMap properties.                        
                        curPos=cellPos;
                        TileStateManager.Instance.updateState(curPos,TileState.occupied);
                        onMap=true;
                        TurnManager.Instance.updateTurn();
                    } else {
                        transform.position = tileMap.GetCellCenterWorld(curPos);
                    }
                PieceStateManager.Instance.updateState(this,PieceState.basic,isFluct);
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
                        TurnManager.Instance.updateTurn();
                        curPos=cellPos;
                        TileStateManager.Instance.updateState(curPos,TileState.occupied);
                    } else {
                        transform.position = tileMap.GetCellCenterWorld(curPos);
                    }

                    PieceStateManager.Instance.updateState(this,PieceState.basic,isFluct);
                    resetMap();/*
                    foreach (Vector3Int tilePos in reachableTiles){
                        if (TileStateManager.Instance.getState(tilePos)==TileState.reachable){
                            TileStateManager.Instance.updateState(tilePos,TileState.basic);
                        }
                        tileMap.RefreshTile(tilePos);
                    }*/
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
        //init renderer
        sr= GetComponent<SpriteRenderer>();
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
                if (tileMap.GetTile(cellPos)!=null){//If the tile selected is on the map
                    if (TileStateManager.Instance.getState(cellPos)==TileState.reachable){ //If the move was valid
                        transform.position = tileMap.GetCellCenterWorld(cellPos);
                        TileStateManager.Instance.updateState(curPos,TileState.basic);
                        curPos=cellPos;
                        TileStateManager.Instance.updateState(curPos,TileState.occupied);
                        TurnManager.Instance.updateTurn();
                    }
                }
                PieceStateManager.Instance.updateState(this,PieceState.basic,isFluct); //Valid or not, we reset (click elsewhere to cancel)
                resetMap();
                /*
                foreach (Vector3Int tilePos in reachableTiles){
                    if (TileStateManager.Instance.getState(tilePos)==TileState.reachable){
                        TileStateManager.Instance.updateState(tilePos,TileState.basic);
                    }
                    tileMap.RefreshTile(tilePos);
                }*/
            }

        }
    }

    //getter
    public bool getOnMap() => onMap;
}
