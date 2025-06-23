using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;


public class PieceMovement : MonoBehaviour
{
    /** This class is used to manage the movement of the pieces on the map.
     * It will handle the movement of the pieces, the detection of reachable tiles, and the interaction with the user.
     * It will also handle the summoning of pieces on the map.
     * The class is used by PieceInteractionManager to manage the interaction with the user.
     * It is also used by TurnManager to manage the turn of the pieces.
     * It is also used by PhaseManager to manage the phase of the game.
     * It unfortunately has a lot of global variables (is fluct, offset of tiles research, etc...), but the split will be done later
     */
    //Public objects
    public Tilemap tileMap; /// Reference to the tile of the map, mainly used by reachableTiles and to update the tiles's states
    public bool isFluct = true; ///Specify the type of the piece (basically P1 or P2)
    [Range(1,5)] ///For now, nbMov's range is between 1 and 5
    public int nbMov=1; ///Nb of movement a piece can do
    public bool isSpecial=false; /// The special pieces has different summoning turns, this bool is sent to TurnManager in isSummonable
    public bool enhancedTroop = false; /// If the troop is enhanced, it will notify the TurnManager that an enhanced summon was performed
    
    //public method
    public static List<Vector3Int> detectTilesInRange(Vector3Int center, int range, Tilemap tileMap) //A lot of methods need a tile detection algorithm. As I started with PieceMovement, all elements are in it, it's not worth the time of relocating so I create this function here.
    {
        /** This method will detect the tiles in range of a piece, it will return a list of Vector3Int containing the coordinates of the tiles in range.
         * The range is the depth of the search, each loop is one depth.
         * The center is the position of the piece on the map.
         * The tileMap is used to get the tiles on the map and to update their state.
         */
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
        isLocked=true; ///Lock the piece, it can't move until it's unlocked
    }
    public void unlockPiece() {
        isLocked=false; ///Unlock the piece, it can move again
    }
    public bool IsLocked(){
        return isLocked; ///Return if the piece is locked or not
    }

    public void moveTo(Vector3Int newPos){ /// This method is used to move the piece to a new position on the map, updating the position of the piece, the tile state and the piece state in the respecting managers.
        PieceStateManager.Instance.updateState(gameObject,PieceState.moving,isFluct);
        if (tileMap.GetTile(newPos)!=null){//If the tile selected is on the map
            transform.position = tileMap.GetCellCenterWorld(newPos);
            TileStateManager.Instance.updateState(curPos,TileState.basic);
            curPos=newPos;
            PieceInteractionManager.Instance.updatePos(gameObject,curPos,isFluct);
            TileStateManager.Instance.updateState(curPos,TileState.occupied);
        }
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,isFluct); 
        resetMap();
    }


    //private & internal properties
    private bool isLocked = false; ///Used to lock the piece when it is attacked by a Windrunner, so it can't move until it's unlocked
    private int curMov; ///The movement the player used with this piece -> Allowing the player to move the same piece two times or more if nbMov>1
    private int curTurn=1; ///Used to reset curMov, as it's its only purpose, it's not in TurnManager to simplify. Each piece will reset itself instead of a manager doing it.
    private static Vector3Int[] neighbourOffsetOdd = new Vector3Int[]{ /// This is the neighbour offset values for an Odd coordinates tiles In an hexagonal point top map, the offset changes depending if y is odd or not.
        new Vector3Int(0,1,0), //up left
        new Vector3Int(1,1,0), //up right
        new Vector3Int(1,0,0), // right
        new Vector3Int(1,-1,0), //down right
        new Vector3Int(0,-1,0), //down left
        new Vector3Int(-1,0,0), //left
    };
    private static Vector3Int[] neighbourOffset = new Vector3Int[]{ /// This is the neighbour offset values for an Even coordinates tiles In an hexagonal point top map, the offset changes depending if y is odd or not.
        new Vector3Int(-1,1,0), //up left
        new Vector3Int(0,1,0), //up right
        new Vector3Int(1,0,0), // right
        new Vector3Int(0,-1,0), //down right
        new Vector3Int(-1,-1,0), //down left
        new Vector3Int(-1,0,0), //left
    };
    private bool onMap=false; ///Managers only manage pieces on board, so pieces each have its own onMap verifier.
    SpriteRenderer sr; ///Only used to drag : when dragging the piece will appear to be above the others
    private bool waitClick=false; ///Used for "click and go" moves
    //Cur pos and cellPos are the current coordinates and the destination coordinates : those are the main variables
    private Vector3Int curPos; ///Current position of the piece
    private Vector3Int cellPos; ///New cell selected
    private List<Vector3Int> reachableTiles; ///This will stock the coordinates of the tiles reachable by the piece.


    //Private methods
    private bool phaseValidation() { /// This method is used to validate the phase of the game, it will return true if the phase is valid for the piece to move or summon.
        return ((PhaseManager.Instance.MovementPhase() && onMap)||(PhaseManager.Instance.SummoningPhase()&& !onMap));
    }
    private void refreshMoves(){
        ///refreshMoves is used to refresh curMov
        if (!isLocked){
            if (curTurn<TurnManager.Instance.getTurnNumber()){
                curMov=nbMov;
                curTurn=TurnManager.Instance.getTurnNumber();
            }
        } else {
            curMov=0; //If the piece is locked, it can't move
        }
    }

    private void useMove(){ ///useMove is used to decrement curMov of the number of moves used by the piece
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
    private void resetMap(){ ///This method is used to reset the map after a move or a summon, it will reset the state of the tiles to basic and refresh them.
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
    private bool isSummonable(){ /// This method is used to check if the piece can be summoned on the selected tile, it will return true if the piece can be summoned.
        bool summonable=true;
        TileBase tileS = tileMap.GetTile(cellPos);
        BaseTile tileSelected = tileS as BaseTile;
        /**
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
    private List<Vector3Int> getReachableTiles() 
    { 
        /** This method is used to get the reachable tiles of the piece based on the number of its move allowed each turn, it will return a list of Vector3Int containing the coordinates of the reachable tiles.
         * The method will use a map traversal algorithm to find the reachable tiles.
         * The curMov is used to determine the depth of the search, each loop is one depth.
         * The onMap is used to determine if the piece is on the map or not.
         */
        //Created before detectTilesInRange, not worth modifying
        //Should only be called once onMap is true and if the game has started
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

    //getter & setter
    public bool getOnMap() => onMap;
    public void setOnMap(bool onMap) => this.onMap = onMap; //Used to set the onMap property, mainly used by the summoning ability of stoneward
    public Vector3Int getCurPos() => curPos;
    public void setCurPos(Vector3Int curPos) {
        this.curPos = curPos; //Used to set the curPos property, mainly used by the summoning ability of stoneward
        transform.position = tileMap.GetCellCenterWorld(curPos); //Update the position of the piece on the map
    }
    public Vector3Int[] getNeighbourOffset() =>neighbourOffset;
    public Vector3Int[] getNeighbourOffsetOdd()=> neighbourOffsetOdd;
    //debug getter
    public int getCurMov() => curMov;
    public void setCurMov(int curMov) {
        this.curMov = curMov; //Used to set the curMov property, mainly used by the summoning ability of stoneward
    }
    public int getCurTurn() => curTurn; //Used to get the current turn of the piece, mainly used by the summoning ability of stoneward
    public void setCurTurn(int curTurn) {
        this.curTurn = curTurn; //Used to set the curTurn property, mainly used by the summoning ability of stoneward
    }


    //All the onMouse methods are below : they manage almost all interaction with the user, they works only if it's the player's turn (Asking to turn manager)
    void OnMouseDown(){
        /** This method is used to handle the mouse down event on the piece, it will update the state of the piece and the reachable tiles if it's the player's turn and if the phase is valid.
         * It will also reset the reachable tiles to avoid bugs.
         * The method will also update the state of the piece in the PieceStateManager.
         * It enables the "selected" state of the piece by scaling it up and changing its sorting order (for dragging effect).
         */
        if (TurnManager.Instance.isPlayerTurn(isFluct) && phaseValidation() && curMov>0 && !InterruptionManager.Instance.isInterruptionActive()){ //remember, isFluct is P1 or P2
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
    void OnMouseDrag(){   ///Just a visual code for the piece to follow the mouse on drag
        if (TurnManager.Instance.isPlayerTurn(isFluct) && phaseValidation() && curMov>0 && !InterruptionManager.Instance.isInterruptionActive()){
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Assure que l'objet reste sur le plan 2D
            transform.localPosition = mousePosition;
        }
    }
    void OnMouseUp(){
        /** This method is crucial for the class, it is the verifier that allow the move of the piece (or its summon).
         * it will handle the mouse up event on the piece, it will update the state of the piece and the reachable tiles if it's the player's turn and if the phase is valid.
         * It will also reset the reachable tiles to avoid bugs.
         * The method will also update the state of the piece in the PieceStateManager.
         * It disables the "selected" state of the piece by scaling it down and changing its sorting order (for dragging effect).
         */
        transform.localScale = new Vector3(1,1,1); // Reset the selected state of the piece
        sr.sortingOrder=1;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        cellPos = tileMap.WorldToCell(mousePosition); //Actu the cellPos coordinates to the current cell selected
        if (TurnManager.Instance.isPlayerTurn(isFluct) && PhaseManager.Instance.MovementPhase() && !InterruptionManager.Instance.isInterruptionActive()){
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
        ///Update is the second verifier of the move (for "click and go" moves), it will check if the player clicked on a tile and if the move is valid.
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


}
