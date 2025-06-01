using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using utils;
public class PieceInteractionManager : MonoBehaviour
{
    public static PieceInteractionManager Instance;
    public GameObject fusionnes;
    public GameObject fluctuomanciens; 
    public Color targetColor = new Color(165f/255f,165f/255f,165f/255f);
    internal Tilemap tileMap;
    private Dictionary<Vector3Int,GameObject> fluct=new();
    private Dictionary<Vector3Int,GameObject> fus=new();
    private List<Vector3Int> listOfTargets=new();
    private List<Vector3Int> attackRange=new();
    internal GameObject targeter; //Only PieceAttack may access it, it's just to simplify the code, no use of creating a method to delete targeter
    //Debug method (used for the status windows in TileClickUI)
    public bool isAPiece(Vector3Int coor) {return (fluct.ContainsKey(coor) || fus.ContainsKey(coor));}

    //public methods
    public GameObject getPiece(Vector3Int coor) {
        if (fluct.ContainsKey(coor)) return fluct[coor];
        else if (fus.ContainsKey(coor)) return fus[coor];
        return null;
    }
    public bool isATarget(Vector3Int coor) { return listOfTargets.Contains(coor); }
    public void setTargeter(GameObject a){ targeter=a;}
    public void updatePos(GameObject p, Vector3Int pos,bool isFluct){
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
                    //fus[oldKey]=null;
                }
                fus.Add(pos,p);
            }
        }
    }
    public void addPiece(GameObject p, Vector3Int pos, bool isFluct){
        if (isFluct) {
            if (!fluct.ContainsKey(pos)) fluct.Add(pos,p);
            else Debug.LogWarning("Piece already exists at position " + pos + " in fluctuomanciens dictionary.");
        } else {
            if (!fus.ContainsKey(pos)) fus.Add(pos,p);
            else Debug.LogWarning("Piece already exists at position " + pos + " in fusionnes dictionary.");
        }
    }
    private void initDicts(){
        int idCount=-100;
        foreach (Transform piece in fusionnes.transform){
            if (!fus.ContainsValue(piece.gameObject)){
                fus.Add(new Vector3Int(-1,idCount--,0),piece.gameObject);
            }
        }
        idCount=-100;
        foreach (Transform piece in fluctuomanciens.transform){
            if (!fluct.ContainsValue(piece.gameObject)){
                fluct.Add(new Vector3Int(-1,idCount--,0),piece.gameObject);
            }
        }
    }
    public void remove(Vector3Int key){
        if (fluct.ContainsKey(key)) fluct.Remove(key);
        else if (fus.ContainsKey(key)) fus.Remove(key);
    }
    public List<Vector3Int> getAttackTargets(List<Vector3Int> tiles, bool isFluct){
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
                    Debug.Log("Tile " + tile + " is a fus target");
                }
            }
        }
        if (list.Count == 0){
            return null; //No target found
        }
        return list;
    }
    public List<Vector3Int> areTargeted(List<Vector3Int> pieces, bool isFluct){
        Debug.Log(isFluct + "AHHH");
        listOfTargets.Clear();
        foreach(Vector3Int piece in pieces){
            if (isFluct){
                if (fus.ContainsKey(piece)){ //Redundant because already verified with attack target, but in case another type of target appears, I keep it for modularity
                    SpriteRenderer sr = fus[piece].GetComponent<SpriteRenderer>();
                    sr.color = targetColor;
                }
            } else {
                if (fluct.ContainsKey(piece)){
                    SpriteRenderer sr = fluct[piece].GetComponent<SpriteRenderer>();
                    sr.color = targetColor;
                }
            }
            listOfTargets.Add(piece);
        }
        return listOfTargets;
    }
    public void resetTargets(bool allies=false){
        Debug.Log("List of targets size: " + listOfTargets.Count);
        foreach(Vector3Int piece in listOfTargets){
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
    public void attack(Vector3Int defender,bool isFluct){
        PieceAttack attacker = targeter.GetComponent<PieceAttack>();
        if (listOfTargets.Contains(defender)){
            int dmg = attacker.baseAtk+(attacker.d4Atk*Random.Range(1,5));
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance=this;
        initDicts();
        tileMap = TileStateManager.Instance.tileMap;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
