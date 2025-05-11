using UnityEngine;
using System.Collections.Generic;
using utils;
public class PieceInteractionManager : MonoBehaviour
{
    public static PieceInteractionManager Instance;
    private bool combatMode=false;
    public GameObject fusionnes;
    public GameObject fluctuomanciens; 
    public Color targetColor = new Color(165f/255f,165f/255f,165f/255f);
    private Dictionary<Vector3Int,GameObject> fluct=new();
    private Dictionary<Vector3Int,GameObject> fus=new();
    private List<Vector3Int> listOfTargets=new();
    public bool combatModeEnabled() {return combatMode;}
    public void setCombatMode(bool mode) {combatMode=mode;}
    public void toggleCombatMode() {combatMode=!combatMode;}
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
    private void initDicts(){
        int idCount=-1;
        foreach (Transform piece in fusionnes.transform){
            if (!fus.ContainsValue(piece.gameObject)){
                fus.Add(new Vector3Int(-1,idCount--,0),piece.gameObject);
            }
        }
        idCount=-1;
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
        }
        return targets;
    }
    public void areTargeted(List<Vector3Int> pieces, bool isFluct){
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
    }
    public void resetTargets(List<Vector3Int> pieces, bool isFluct){
        foreach(Vector3Int piece in pieces){
            if (isFluct){
                if (fus.ContainsKey(piece)){
                    SpriteRenderer sr = fus[piece].GetComponent<SpriteRenderer>();
                    sr.color = new Color(255f,255f,255f);
                }
            } else {
                if (fluct.ContainsKey(piece)){
                    SpriteRenderer sr = fluct[piece].GetComponent<SpriteRenderer>();
                    sr.color = new Color(255f,255f,255f);
                }
            }
        }
    }
    public void attack(Vector3Int attacker, Vector3Int defender, bool isFluct){
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance=this;
        initDicts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
