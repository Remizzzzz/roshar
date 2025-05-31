using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;


public class PieceMovement : MonoBehaviour
{
    //Public objects
    public Tilemap tileMap; // Reference to the tile of the map, mainly used by reachableTiles and to update the tiles's states
    public bool isFluct = true; //Specify the type of the piece (basically P1 or P2)
    [Range(1,5)] //For now, nbMov's range is between 1 and 5
    public int nbMov=1; //Nb of movement a piece can do
    public bool isSpecial=false; // The special pieces has different summoning turns, this bool is sent to TurnManager in isSummonable
    public bool enhancedTroop = false;
    
    //public method
    public static List<Vector3Int> detectTilesInRange(Vector3Int center, int range, Tilemap tileMap) //A lot of methods need a tile detection algorithm. As I started with PieceMovement, all elements are in it, it's not worth the time of relocating so I create this function here.
    {
        List<Vector3Int> validCoor = new List<Vector3Int> { center }; //The List where we'll keep our valid coordinates
        if (tileMap.GetTile(center)!=null)
        {
            List<Vector3Int> search = new List<Vector3Int>(); //The tempList used for searching

            for (int i = 0; i < range; i++) { //range determines the depth of search, each loop is one depth
                foreach (Vector3Int rSearch in validCoor) { //Search will go through each neighboor of the tiles in validCoor, and add them to validCoor if they're not alreday in it
                    foreach (Vector3Int neighbour in (Math.Abs(rSearch.y) % 2 == 1 ? neighbourOffsetOdd : neighbourOffset)){ //Change the offset depending on Y (odd or not)
                        if (!validCoor.Contains(neighbour + rSearch) && !search.Contains(neighbour + rSearch) && tileMap.GetTile(neighbour + rSearch) != null) { //Of course, we don't add coordinates that aren't on the map
                            search.Add(neighbour + rSearch); //If the coor isn't in valid or search and is in the map add it to search
                        }
                    }
                }
                validCoor.AddRange(search);
                search.Clear();
            }
        }
        return validCoor;
    }
    public void lockPiece() {
        isLocked=true; //Lock the piece, it can't move until it's unlocked
    }
    public void unlockPiece() {
        isLocked=false; //Unlock the piece, it can move again
    }
    //private & internal properties
    private bool isLocked = false; //Used to lock the piece when it is attacked by a Windrunner, so it can't move until it's unlocked
    private int curMov; //The movement the player used with this piece -> Allowing the player to move the same piece two times or more if nbMov>1
    private int curTurn=1; //Used to reset curMov, as it's its only purpose, it's not in TurnManager to simplify. Each piece will reset itself instead of a manager doing it.
    private static Vector3Int[] neighbourOffsetOdd = new Vector3Int[]{ //In an hexagonal point top map, the offset changes depending if y is odd or not.
        new Vector3Int(0,1,0), //up left
        new Vector3Int(1,1,0), //up right
        new Vector3Int(1,0,0), // right
        new Vector3Int(1,-1,0), //down right
        new Vector3Int(0,-1,0), //down left
        new Vector3Int(-1,0,0), //left
    };
    private static Vector3Int[] neighbourOffset = new Vector3Int[]{
        new Vector3Int(-1,1,0), //up left
        new Vector3Int(0,1,0), //up right
        new Vector3Int(1,0,0), // right
        new Vector3Int(0,-1,0), //down right
        new Vector3Int(-1,-1,0), //down left
        new Vector3Int(-1,0,0), //left
    };
    private bool onMap=false; //Managers only manage pieces on board, so pieces each have its own onMap verifier. It is used only for summon
    SpriteRenderer sr; //Only used to drag : when dragging the piece will appear to be above the others
    private bool waitClick=false; //Used for click and go moves
    //Cur pos and cellPos are the current coordinates and the destination coordinates : those are the main variables
    private Vector3Int curPos; 
    private Vector3Int cellPos; //New cell selected
    private List<Vector3Int> reachableTiles; //This will stock the coordinates of the tiles reachable by the piece.


    //Private methods
    private bool phaseValidation() {
        return ((PhaseManager.Instance.MovementPhase() && onMap)||(PhaseManager.Instance.SummoningPhase()&& !onMap));
    }
    //refreshMoves is used to refresh curMov
    private void refreshMoves(){
        if (!isLocked){
            if (curTurn<TurnManager.Instance.getTurnNumber()){
                curMov=nbMov;
                curTurn=TurnManager.Instance.getTurnNumber();
            }
        } else {
            curMov=0; //If the piece is locked, it can't move
        }
    }

    //useMove is used to decrement curMov of the usedMoves
    private void useMove(){
        bool notFound=true;
        int usedMoves=0;
        while (notFound){
            List<Vector3Int> validCoor= new List<Vector3Int>{curPos};
            List<Vector3Int> search = new List<Vector3Int>();
            //go to getReachableTiles for map traversal description
            for (int i=0; i<usedMoves;i++){
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
            if (validCoor.Contains(cellPos)) notFound=false;
            else {
                if (nbMov>usedMoves) usedMoves++; // It will go through each depth of the map until it finds the cellPos then will return the depth of the move and see if the piece can still move or not
                else notFound=false;
            }
        }
        curMov=curMov-usedMoves;
    }

    //Reset map is here to reset the "reachable" tiles after the move. It can be optimized 
    // but strange bugs appear when I don't check the whole map (some tile stays reachable) 
    // so it will be optimized later, for now the purpose is for the code to be functionnal
    private void resetMap(){
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

    //The conditions of summoning started to be overwhelming so I condensed them here
    private bool isSummonable(){
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
        - The max number of summon this turn is not reached (managed by TurnManager)
        */
        summonable=(tileSelected!=null && tileSelected.isSummoningTile && tileSelected.isFluct==this.isFluct && TileStateManager.Instance.isNotOccupied(cellPos) && TurnManager.Instance.isSummonable(isSpecial, isFluct));
        return summonable;
    } 
    //Main method of the class, returns the coordinates of the tiles reachable and update them in the TileStateManager for visual selection
    private List<Vector3Int> getReachableTiles() //Created before detectTilesInRange, not worth modifying
    { //Should only be called once onMap is true and if the game has started
        List<Vector3Int> validCoor = new List<Vector3Int> { curPos }; //The List where we'll keep our valid coordinates
        if (onMap)
        {
            List<Vector3Int> search = new List<Vector3Int>(); //The tempList used for searching

            for (int i = 0; i < curMov; i++)
            { //curMov determines the depth of search, each loop is one depth
                foreach (Vector3Int rSearch in validCoor)
                { //Search will go through each neighboor of the tiles in validCoor, and add them to validCoor if they're not alreday in it
                    foreach (Vector3Int neighbour in (Math.Abs(rSearch.y) % 2 == 1 ? neighbourOffsetOdd : neighbourOffset))
                    { //Change the offset depending on Y (odd or not)
                        if (!validCoor.Contains(neighbour + rSearch) && !search.Contains(neighbour + rSearch) && tileMap.GetTile(neighbour + rSearch) != null)
                        { //Of course, we don't add coordinates that aren't on the map
                            if (TileStateManager.Instance.isNotOccupied(neighbour + rSearch)) search.Add(neighbour + rSearch); //If the coor isn't in valid or search or in the map or is occupied, add it to search
                        }
                    }
                }
                validCoor.AddRange(search);
                search.Clear();
            }
        }
        validCoor.Remove(curPos); //Want to remove the original position from destination (bc it's dumb) but it don't work, it's not an urgent fix, it's just visual, I'll fix it later
        return validCoor;
    }


    //All the onMouse methods are below : they manage almost all interaction with the user, they works only if it's the player's turn (Asking to turn manager)
    void OnMouseDown(){
        if (TurnManager.Instance.isPlayerTurn(isFluct) && phaseValidation() && curMov>0){ //remember, isFluct is P1 or P2
            reachableTiles=null; //reset reachable tiles because of some bugs, didn't find where so I fixed it here

            //enable selectedState
            transform.localScale=new Vector3(1.3f,1.3f,1.3f);
            sr.sortingOrder=2;
            
            //updating piece state manager
            PieceStateManager.Instance.updateState(gameObject,PieceState.moving,isFluct);
            if (onMap){//If it's on map, then we have to know the possibles moves
                reachableTiles = getReachableTiles();
                foreach (Vector3Int tilePos in reachableTiles){
                    TileStateManager.Instance.updateState(tilePos,TileState.reachable);
                    tileMap.RefreshTile(tilePos);
                }
            }
        }
    }
    void OnMouseDrag(){   //Just a visual code for the piece to follow the mouse on drag
        if (TurnManager.Instance.isPlayerTurn(isFluct) && phaseValidation() && curMov>0){
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Assure que l'objet reste sur le plan 2D
            transform.localPosition = mousePosition;
        }
    }
    void OnMouseUp(){
        transform.localScale = new Vector3(1,1,1); // Reset the selected state of the piece
        sr.sortingOrder=1;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        cellPos = tileMap.WorldToCell(mousePosition); //Actu the cellPos coordinates to the current cell selected
        if (TurnManager.Instance.isPlayerTurn(isFluct) && PhaseManager.Instance.MovementPhase()){
            if (onMap) {
                if (cellPos==curPos ){ //If the player just clicked on the piece : Clicked behaviour
                    transform.position = tileMap.GetCellCenterWorld(curPos);
                    waitClick=true;
                    //We don't reset moving and the reachable piece
                } else { //If the player moved the piece
                    if (reachableTiles.Contains(cellPos)){
                        transform.position = tileMap.GetCellCenterWorld(cellPos); //Move the piece and actu the curPos and the state of the tiles (occupied)
                        TileStateManager.Instance.updateState(curPos,TileState.basic);
                        useMove();
                        curPos=cellPos;
                        PieceInteractionManager.Instance.updatePos(gameObject,curPos,isFluct);
                        TileStateManager.Instance.updateState(curPos,TileState.occupied);
                    } else {
                        transform.position = tileMap.GetCellCenterWorld(curPos);
                    }

                    PieceStateManager.Instance.updateState(gameObject,PieceState.basic,isFluct);
                    resetMap();
                    /*Former code to reset the reachableTiles -> It bugs, and I'm tired of searching why for now (it's not bc of the reset in on MouseDown, I added it later, but it might be because of the reactu of onMouseDown, then I should add an order for the function and I don't know how to do it for now (this is a very long comment))
                    foreach (Vector3Int tilePos in reachableTiles){
                        if (TileStateManager.Instance.getState(tilePos)==TileState.reachable){
                            TileStateManager.Instance.updateState(tilePos,TileState.basic);
                        }
                        tileMap.RefreshTile(tilePos);
                    }*/
                }
            }
        } else if (TurnManager.Instance.isPlayerTurn(isFluct) && PhaseManager.Instance.SummoningPhase()) {
            if (!onMap) { //If it's a summon
                if (cellPos!=curPos ){ //If the player moved the piece
                    if (isSummonable())
                    {
                        transform.position = tileMap.GetCellCenterWorld(cellPos); //Move the piece and actu the curPos, toggle the onMap properties.                        
                        curMov = 0; //The piece can't move anymore this turn
                        TurnManager.Instance.incrSummon(); //Update the nb of summons done this turn
                        curPos = cellPos;
                        PieceInteractionManager.Instance.updatePos(gameObject, curPos, isFluct);
                        TileStateManager.Instance.updateState(curPos, TileState.occupied);
                        onMap = true;
                        if (isFluct) WinCondition.Instance.UpdateFluctOnMap(true); //Update the number of pieces on the map
                        else WinCondition.Instance.UpdateFusOnMap(true);
                        if (enhancedTroop) TurnManager.Instance.enhancedSummonPerformed(isFluct); //If the troop is enhanced, we notify the turn manager

                        if (TurnManager.Instance.nbSummonVerif()) PhaseManager.Instance.nextPhase(); //Move onto the nextPhase (for accomodation)
                    }
                    else
                    {
                        transform.position = tileMap.GetCellCenterWorld(curPos); //If it's not a valid destination, return to position
                    }
                PieceStateManager.Instance.updateState(gameObject,PieceState.basic,isFluct); //Notify that the piece finished its movement
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
        //init curMov
        curMov=nbMov;
        if (tileMap.GetTile(curPos)!=null) {
            onMap=true;
            PieceInteractionManager.Instance.updatePos(gameObject, curPos, isFluct);
            TileStateManager.Instance.updateState(curPos, TileState.occupied);
        }
    }

    // Update is called once per frame
    void Update()
    {
        refreshMoves(); //Verify if the turn changed
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
                    if (TileStateManager.Instance.getState(cellPos)==TileState.reachable){ //If the move was valid (tile reachable)
                        transform.position = tileMap.GetCellCenterWorld(cellPos);
                        TileStateManager.Instance.updateState(curPos,TileState.basic);
                        useMove();
                        curPos=cellPos;
                        PieceInteractionManager.Instance.updatePos(gameObject,curPos,isFluct);
                        TileStateManager.Instance.updateState(curPos,TileState.occupied);
                    }
                }
                PieceStateManager.Instance.updateState(gameObject,PieceState.basic,isFluct); //Valid or not, we reset (click elsewhere to cancel)
                resetMap();
                /* Former code to reset the reachableTiles -> It bugs, and I'm tired of searching why for now (it's not bc of the reset in on MouseDown, I added it later, but it might be because of the reactu of onMouseDown, then I should add an order for the function and I don't know how to do it for now (this is a very long comment))
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
    public Vector3Int getCurPos() => curPos;
    public Vector3Int[] getNeighbourOffset() =>neighbourOffset;
    public Vector3Int[] getNeighbourOffsetOdd()=> neighbourOffsetOdd;
    //debug getter
    public int getCurMov() => curMov;
}
