using UnityEngine;
using System.Collections.Generic;


public enum PieceState {moving,basic,attacking,casting,locked}
public class PieceStateManager : MonoBehaviour
{
    public static PieceStateManager Instance;
    public GameObject fusionnes;
    public GameObject fluctuomanciens;
    private Dictionary<GameObject, PieceState> statesFus = new(); //The piece state is linked to GameObject because of tiny error in implementation, it's too late to change it now
    private Dictionary<GameObject, PieceState> statesFluct = new();

    public void updateState(GameObject p,PieceState s, bool isFluct){
        if (isFluct) {
            if (statesFluct.ContainsKey(p)) statesFluct[p]=s;
        }
        else {
            if (statesFus.ContainsKey(p)) statesFus[p]=s;
        }
    }
    public PieceState getState(GameObject p, bool isFluct){
        if (isFluct) return statesFluct[p];
        return statesFus[p];
    }
    public void remove(GameObject g){
        if (statesFluct.ContainsKey(g)) statesFluct.Remove(g);
        else if (statesFus.ContainsKey(g)) statesFus.Remove(g);
    }
    public bool canMove(bool isFluct){
        if (isFluct) return !statesFluct.ContainsValue(PieceState.moving);
        return !statesFus.ContainsValue(PieceState.moving);    
    }
    public bool isAttacked(bool isFluct){ //For now, not very useful, but for safety and modularity we keep it
        if (isFluct) return statesFus.ContainsValue(PieceState.attacking);
        return statesFluct.ContainsValue(PieceState.attacking);    
    }
    public bool isCasting(){
        return (statesFus.ContainsValue(PieceState.casting) || statesFluct.ContainsValue(PieceState.casting));
    }
    private void addPiece(GameObject p, bool isFluct){
        if (isFluct){
            if (!statesFluct.ContainsKey(p)) statesFluct.Add(p,PieceState.basic);
        } else {
            if (!statesFus.ContainsKey(p)) statesFus.Add(p,PieceState.basic);
        }
    }
    private void initDicts(){
        foreach (Transform piece in fusionnes.transform)
        {
            if (!statesFus.ContainsKey(piece.gameObject)){
                statesFus.Add(piece.gameObject,PieceState.basic);

            }
        }
        foreach (Transform piece in fluctuomanciens.transform){
            if (!statesFluct.ContainsKey(piece.gameObject)){
                statesFluct.Add(piece.gameObject,PieceState.basic);
            }
        }
    }
    void Awake(){
        Instance=this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initDicts();
    }

    // Update is called once per frame
    void Update()
    {
    }
}