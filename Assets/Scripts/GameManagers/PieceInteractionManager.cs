using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using utils;
public class PieceInteractionManager : MonoBehaviour
{
    /** This class manages the interactions between pieces in the game.
     It handles the addition, removal, and updating of pieces on the map,
     as well as managing target selection and filtering target for attacks or effects.
     It also provides methods to check for neighboring pieces and to handle attacks.
    */
    public static PieceInteractionManager Instance;
    public GameObject fused; // Fused pieces are all childs of this GameObject
    public GameObject fluctuomancers; // Fluctuomancer pieces are all childs of this GameObject
    public Color targetColor = new Color(165f/255f,165f/255f,165f/255f); /// Color used to highlight target pieces
    internal Tilemap tileMap;
    private Dictionary<Vector3Int,GameObject> fluct=new(); /// Dictionary to store fluctuomancer pieces with their coordinates as keys
    private Dictionary<Vector3Int,GameObject> fus=new(); /// Dictionary to store fused pieces with their coordinates as keys
    private List<Vector3Int> listOfTargets=new(); /// List to store the coordinates of targeted pieces
    private List<Vector3Int> attackRange=new(); /// List to store the coordinates of tiles in attack range (it could be used for effects, but for now it's only used for attacks)
    internal GameObject targeter; ///The piece that is currently selecting targets (the one that is attacking or using an effect)
    
    //public methods

    public bool isAPiece(Vector3Int coor) {return (fluct.ContainsKey(coor) || fus.ContainsKey(coor));}
    public Dictionary<Vector3Int, GameObject> getFluctDictionnary() { return fluct; }
    public Dictionary<Vector3Int, GameObject> getFusDictionnary() { return fus; }
    public GameObject getPiece(Vector3Int coor) { /// This method returns the GameObject of a piece at the given coordinates
        if (fluct.ContainsKey(coor)) return fluct[coor];
        else if (fus.ContainsKey(coor)) return fus[coor];
        return null;
    }
    public bool isATarget(Vector3Int coor) { return listOfTargets.Contains(coor); } /// This method checks if the given coordinates are in the list of targets
    public void setTargeter(GameObject a){ targeter=a;} /// This method sets the targeter GameObject, which is the piece that is currently selecting targets
    public void addTarget(Vector3Int target){ /// This method adds a target to the list of targets if it is not already present
        if (!listOfTargets.Contains(target)) {
            listOfTargets.Add(target);
        }
    }
    public void updatePos(GameObject p, Vector3Int pos,bool isFluct){
        /** This method updates the position of a piece in the interaction dictionaries.
         It checks if the piece is a fluctuomancer or a fused piece and updates the corresponding dictionary.
         If the piece already exists in the dictionnary, it updates its coordinates.
         If it doesn't exist, it adds the piece to the dictionary at the new position.
         */
        if (isFluct) {
            //The following process cost less than to add all the map in initDicts
            List<Vector3Int> old = Utils.GetKeysByValue(fluct,p);
            if(fluct.ContainsKey(pos)) fluct[pos]=p; //This case should not happen in the current implementation
            else {
                foreach(Vector3Int oldKey in old){
                    fluct.Remove(oldKey); //Stockage efficiency first, but if we remove that, each time a piece goes onto a new tile, it's added to its interaction dictionary. Who knows, it can maybe be used later for a fog mode, or a for a stat windows
                    //If we don't remove, we have to do this at least : 
                    //fluct[oldKey]=null;
                }
                fluct.Add(pos,p);
            }
        }
        else {
            List<Vector3Int> old = Utils.GetKeysByValue(fus,p);
            if (fus.ContainsKey(pos)) fus[pos]=p;
            else {
                foreach(Vector3Int oldKey in old){
                    fus.Remove(oldKey);
                    ///fus[oldKey]=null;
                }
                fus.Add(pos,p);
            }
        }
    }
    public void addPiece(GameObject p, Vector3Int pos, bool isFluct){
        /** This method adds a piece to the interaction dictionaries.
         It checks if the piece is a fluctuomancer or a fused piece and adds it to the corresponding dictionary.
         If a piece already exists at the given position, it logs a warning.
         */
        if (isFluct) {
            if (!fluct.ContainsKey(pos)) fluct.Add(pos,p);
            else Debug.LogWarning("Piece already exists at position " + pos + " in fluctuomancers dictionary.");
        } else {
            if (!fus.ContainsKey(pos)) fus.Add(pos,p);
            else Debug.LogWarning("Piece already exists at position " + pos + " in fused dictionary.");
        }
    }
    private void initDicts(){/// This method initializes the interaction dictionaries with the current pieces in the scene.
        int idCount=-100;
        foreach (Transform piece in fused.transform){
            if (!fus.ContainsValue(piece.gameObject)){
                fus.Add(new Vector3Int(-1,idCount--,0),piece.gameObject);
            }
        }
        idCount=-100;
        foreach (Transform piece in fluctuomancers.transform){
            if (!fluct.ContainsValue(piece.gameObject)){
                fluct.Add(new Vector3Int(-1,idCount--,0),piece.gameObject);
            }
        }
    }
    public void remove(Vector3Int key){ /// This method removes a piece from the interaction dictionaries based on its coordinates.
        if (fluct.ContainsKey(key)) fluct.Remove(key);
        else if (fus.ContainsKey(key)) fus.Remove(key);
    }
    public List<Vector3Int> getAttackTargets(List<Vector3Int> tiles, bool isFluct){
        /** This method retrieves the attack targets based on the given tiles and whether the piece is a fluctuomancer or a fused piece.
         It updates the tile states to indicate that they are in attack range and returns a list of target coordinates.
         If targeter is a fluctuomancer, it will look for and return fused pieces as targets, and vice versa.
         */
        attackRange.Clear();
        List<Vector3Int> targets = new List<Vector3Int>();
        foreach (Vector3Int tile in tiles){
            if (isFluct){ //If the piece is fluct, its ennemies are fus
                if (fus.ContainsKey(tile)){
                    targets.Add(tile);
                }
            } else {
                if (fluct.ContainsKey(tile)){
                    targets.Add(tile);
                }
            }
            if (TileStateManager.Instance.getState(tile)!=TileState.occupied){
                TileStateManager.Instance.updateState(tile,TileState.inAttackRange);
                TileStateManager.Instance.tileMap.RefreshTile(tile);
                attackRange.Add(tile);
            }
        }
        return targets;
    }

    public List<Vector3Int> getTargetOnMap(bool isFluct){
        /** This method retrieves the target pieces on the map based on whether the piece is a fluctuomancer or a fused piece.
         if we select fluctuomancers, it will return the coordinates of all fluctuomancers that are on the map, and vice versa.
         */
        List<Vector3Int> list = new List<Vector3Int>();
        if (isFluct){
            foreach (Vector3Int tile in fluct.Keys){
                if (tileMap.HasTile(tile)){
                    list.Add(tile);
                }
            }
        } else {
            foreach (Vector3Int tile in fus.Keys){
                if (tileMap.HasTile(tile)){
                    list.Add(tile);
                }
            }
        }
        if (list.Count == 0){
            return null; //No target found
        }
        return list;
    }

    public List<Vector3Int> areTargeted(List<Vector3Int> targetZone, bool isFluct){
        /** This method targets the pieces in the given area. If we enter isFluct as true, it will target all fused in the area, and vice versa.
         It updates the color of the targeted pieces to indicate that they are selected as targets.
         It returns a list of coordinates of the targeted pieces. So this method also works as a filter to take only pieces from the zone target.
         */
        listOfTargets.Clear();
        foreach(Vector3Int tile in targetZone){
            if (isFluct){
                if (fus.ContainsKey(tile)){ //Redundant because already verified with attack target, but in case another type of target appears, I keep it for modularity
                    SpriteRenderer sr = fus[tile].GetComponent<SpriteRenderer>();
                    sr.color = targetColor;
                    listOfTargets.Add(tile);
                }
            } else {
                if (fluct.ContainsKey(tile)){
                    SpriteRenderer sr = fluct[tile].GetComponent<SpriteRenderer>();
                    sr.color = targetColor;
                    listOfTargets.Add(tile);
                }
            }
        }
        List<Vector3Int> result = listOfTargets;
        return result;
    }
    public void resetTargets(bool allies=false){
        /** This method resets the targets' colors and clears the list of targets.
         It also updates the tile states to indicate that they are no longer in attack range.
         If the bool allies is true, it will reset the colors of the allies' pieces, otherwise it will reset the enemies' pieces.
         */
        Debug.Log("List of targets size: " + listOfTargets.Count);
        foreach(Vector3Int piece in listOfTargets){// A SIMPLIFIER
            if (targeter!=null){
                if (targeter.GetComponent<PieceMovement>().isFluct && !allies){
                    if (fus.ContainsKey(piece)){
                        SpriteRenderer sr = fus[piece].GetComponent<SpriteRenderer>();
                        sr.color = new Color(1,1,1);
                    }
                } else if (targeter.GetComponent<PieceMovement>().isFluct && allies){
                    if (fluct.ContainsKey(piece)){
                        SpriteRenderer sr = fluct[piece].GetComponent<SpriteRenderer>();
                        sr.color = new Color(1,1,1);
                    }
                } else if (!allies){
                    if (fluct.ContainsKey(piece)){
                        SpriteRenderer sr = fluct[piece].GetComponent<SpriteRenderer>();
                        sr.color = new Color(1,1,1);
                    }
                } else if (allies){
                    if (fus.ContainsKey(piece)){
                        SpriteRenderer sr = fus[piece].GetComponent<SpriteRenderer>();
                        sr.color = new Color(1,1,1);
                    }
                } else {
                    Debug.LogWarning("Target not found in either fluct or fus dictionaries.");
                }
            } else Debug.LogWarning("Targeter is null, cannot reset targets' colors.");
        }
        listOfTargets.Clear();
        foreach(Vector3Int tile in attackRange){
            if (TileStateManager.Instance.getState(tile)!=TileState.occupied){
                TileStateManager.Instance.updateState(tile,TileState.basic);
                TileStateManager.Instance.tileMap.RefreshTile(tile);
            }
        }
        attackRange.Clear();
    }

    public bool hasNeighbor<T>(Vector3Int pos, bool player = false, bool isFluct=false) where T : Component { ///If player is true, it will only check the player's pieces
        /** This method checks if there is a neighboring piece of type T at the given position.
         If player is true, it will only check for pieces that belong to the player of the selected piece.
         If isFluct is true, it will check for fluctuomancers, otherwise it will check for fused pieces.
         */
        List<Vector3Int> neighbors = PieceMovement.detectTilesInRange(pos, 1, tileMap);
        foreach(Vector3Int neighbor in neighbors){
            if (isAPiece(neighbor) && neighbor!= pos){
                GameObject piece = getPiece(neighbor);
                if (player){
                    if (piece.GetComponent<PieceMovement>().isFluct==isFluct && piece.GetComponent<T>() != null) {
                        return true; // Found a neighbor piece of type T that belongs to the player
                    }
                } else {
                    if (piece.GetComponent<T>() != null) {
                        return true; // Found a neighbor piece of type T
                    }
                }
            }
        }
        return false; // No neighbor piece of type T found
    }
    public T getNeighborComponent<T> (Vector3Int pos, bool player = false, bool isFluct=false) where T : Component {
        /** This method checks if there is a neighboring piece of type T at the given position.
         If player is true, it will only check for pieces that belong to the player of the selected piece.
         If isFluct is true, it will check for fluctuomancers, otherwise it will check for fused pieces.
         It returns the first found neighbor piece of type T or null if none is found.
         */
        List<Vector3Int> neighbors = PieceMovement.detectTilesInRange(pos, 1, tileMap);
        foreach(Vector3Int neighbor in neighbors){
            if (isAPiece(neighbor) && neighbor!= pos){
                GameObject piece = getPiece(neighbor);
                if (player){
                    if (piece.GetComponent<PieceMovement>().isFluct==isFluct && piece.GetComponent<T>() != null) {
                        return piece.GetComponent<T>(); // Found a neighbor piece of type T that belongs to the player
                    }
                } else {
                    if (piece.GetComponent<T>() != null) {
                        return piece.GetComponent<T>(); // Return the first found neighbor piece of type T
                    }
                }
            }
        } 
        return null; // No neighbor piece of type T found
    }
    public void attack(Vector3Int defender,bool isFluct){
        /** This method handles the attack action.
         It checks if the targeter is not null and if the defender is in the list of targets.
         If so, it calculates the damage based on the attacker's current attack and a random d4 roll (if the attacker has d4 roll atk).
         It then applies the damage to the defender's PieceAttack component.
         Finally, it resets the targets and clears the targeter.
         */
        PieceAttack attacker = targeter.GetComponent<PieceAttack>();
        if (listOfTargets.Contains(defender)){
            int dmg = attacker.curAtk+(attacker.d4Atk*Random.Range(1,5));
            if (isFluct) {
                if (fluct[defender]!=null) fluct[defender].GetComponent<PieceAttack>().damage(dmg);
            }
            else {
                if (fus[defender]!=null) fus[defender].GetComponent<PieceAttack>().damage(dmg);
            }
            attacker.decrCurNbAtk();
            attacker.isAttacking=false;
            PieceStateManager.Instance.updateState(attacker.gameObject,PieceState.basic,attacker.pM.isFluct);
            resetTargets();
            targeter=null;
        }
    }

    public void protect(int dmgReduction){
        /** This method handles the protection action.
         It checks if the targeter is not null and applies the damage reduction to all targeted pieces.
         It iterates through the list of targets and applies the damage reduction to each piece's PieceAttack component.
         */
        foreach(Vector3Int target in listOfTargets){
            if (fluct.ContainsKey(target)){
                fluct[target].GetComponent<PieceAttack>().Protect(dmgReduction);
            } else if (fus.ContainsKey(target)){
                fus[target].GetComponent<PieceAttack>().Protect(dmgReduction);
            }
        }
    }

    public List<GameObject> findName(string name){ /// This method will return a list of GameObjects that have the specified name in their PieceAttributes component
        List<GameObject> foundPieces = new List<GameObject>();
        foreach(GameObject piece in fluct.Values){
            if (piece.GetComponent<PieceAttributes>().pieceName == name){
                foundPieces.Add(piece);
            }
        }
        foreach(GameObject piece in fus.Values){
            if (piece.GetComponent<PieceAttributes>().pieceName == name){
                foundPieces.Add(piece);
            }
        }
        return foundPieces;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance=this;
        initDicts();
        tileMap = TileStateManager.Instance.tileMap;
    }
    void Start(){
        foreach (string troop in GameData.isUnlocked)
        {
            foreach (GameObject piece in findName(troop)){
                piece.SetActive(true); /// Activate all unlocked pieces in the game until now
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
