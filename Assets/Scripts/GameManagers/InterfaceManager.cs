using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro; // ou UnityEngine.UI si tu utilises le Text classique

public class InterfaceManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Camera cam;
    public TextMeshProUGUI phaseText; 
    public TextMeshProUGUI statusText; 
    public TextMeshProUGUI phaseAnnounceText; 

    void Update()
    {
        /*if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
            TileBase tile = tilemap.GetTile(cellPos);
            if (!PieceInteractionManager.Instance.isAPiece(cellPos)){
                if (tile != null)
                {
                    phaseText.text = "Coordonnées : " + cellPos.ToString() + "\nState : "+TileStateManager.Instance.getState(cellPos).ToString();
                }
                else
                {
                    phaseText.text = "No tile here";
                }
            } else {
                PieceAttack p = PieceInteractionManager.Instance.getPiece(cellPos);
                phaseText.text = "Lp : "+p.getCurLp().ToString() + "\nSelection : "+(!PieceStateManager.Instance.isAttacked(p.pM.isFluct) && !PieceStateManager.Instance.isAttacked(!(p.pM.isFluct)))+"\nCurNbAtk : "+p.getCurNbAtk().ToString();
            }
        }*/
        phaseText.text = "Phase : "+PhaseManager.Instance.getPhase().ToString()+"\nTurn : "+ (!TurnManager.Instance.getPlayerTurn() ? "Fluctuomanciens " : "Fusionnés ")+TurnManager.Instance.getTurnNumber().ToString();
        phaseAnnounceText.text = PhaseManager.Instance.CombatPhase()? "Fight !": (PhaseManager.Instance.MovementPhase() ? "Move !" : "Summon !");
        if (Input.GetMouseButtonDown(1)) // clic droit
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
            TileBase tile = tilemap.GetTile(cellPos);
            if (!PieceInteractionManager.Instance.isAPiece(cellPos)){
                if (tile != null)
                {
                    statusText.text = "\n\nCoordinates : " + cellPos.ToString() + "\nState : "+TileStateManager.Instance.getState(cellPos).ToString();
                }
                else
                {
                    statusText.text = "\n\nLeft-click to check status of a piece or tile.";
                }
            } else {
                GameObject piece = PieceInteractionManager.Instance.getPiece(cellPos);
                PieceAttack p = PieceInteractionManager.Instance.getPiece(cellPos).GetComponent<PieceAttack>();
                statusText.text = piece.GetComponent<PieceAttributes>().pieceName + "\nLp : "+p.getCurLp().ToString() +"\nAttack left : "+p.getCurNbAtk().ToString() + "\nMovement left : "+p.pM.getCurMov().ToString() + "\nDamage Reduction : "+p.dmgReduc.ToString()+"\nDamage : "+(p.d4Atk!=0 ? p.d4Atk.ToString()+" d4 + ":"") + p.baseAtk.ToString();
            }
        }
    }
}
