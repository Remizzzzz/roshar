using UnityEngine;
using System.Collections.Generic;


public enum PieceState {moving,basic}
public class PieceStateManager : MonoBehaviour
{
    public static PieceStateManager Instance;
    public GameObject fusionnes;
    public GameObject fluctuomanciens;
    private Dictionary<PieceMovement, PieceState> statesFus = new();
    private Dictionary<PieceMovement, PieceState> statesFluct = new();

    public void updateState(PieceMovement p,PieceState s, bool isFluct){
        if (isFluct) {
            if (statesFluct.ContainsKey(p)) statesFluct[p]=s;
        }
        else {
            if (statesFus.ContainsKey(p)) statesFus[p]=s;
        }
    }
    public PieceState getState(PieceMovement p, bool isFluct){
        if (isFluct) return statesFluct[p];
        return statesFus[p];
    }
    public bool canMove(bool isFluct){
        if (isFluct) return !statesFluct.ContainsValue(PieceState.moving);
        return !statesFus.ContainsValue(PieceState.moving);    
    }
    private void addPiece(PieceMovement p, bool isFluct){
        if (isFluct){
            if (!statesFluct.ContainsKey(p)) statesFluct.Add(p,PieceState.basic);
        } else {
            if (!statesFus.ContainsKey(p)) statesFus.Add(p,PieceState.basic);
        }
    }
    private void initDicts(){
        PieceMovement[] fus = fusionnes.GetComponentsInChildren<PieceMovement>();
        PieceMovement[] fluct = fluctuomanciens.GetComponentsInChildren<PieceMovement>();
        foreach (PieceMovement piece in fus)
        {
            if (!statesFus.ContainsKey(piece)){
                statesFus.Add(piece,PieceState.basic);

            }
        }
        foreach (PieceMovement piece in fluct){
            if (!statesFluct.ContainsKey(piece)){
                statesFluct.Add(piece,PieceState.basic);
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