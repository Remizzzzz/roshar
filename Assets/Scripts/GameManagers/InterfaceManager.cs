using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro; // ou UnityEngine.UI si tu utilises le Text classique

public class InterfaceManager : MonoBehaviour
{
    /** This class manages the user interface of the game.
     It updates the phase, turn, stormlight, and voidlight counts,
     and displays the status of pieces and tiles when the right mouse button is clicked.
    */
    public Tilemap tilemap;
    public Camera cam;
    public TextMeshProUGUI phaseText; 
    public TextMeshProUGUI statusText; 
    public TextMeshProUGUI phaseAnnounceText; 
    public TextMeshProUGUI StormlightCount;
    public TextMeshProUGUI VoidlightCount;

    void Update()
    {
        /** This method updates the user interface elements every frame.
         It retrieves the current phase, turn number, stormlight, and voidlight counts,
         and updates the corresponding text elements.
         It also handles right-click interactions to display piece or tile status.
        */
        StormlightCount.text = Ability.Stormlight.ToString();
        VoidlightCount.text = Ability.Voidlight.ToString();
        
        phaseText.text = "Phase : "+PhaseManager.Instance.getPhase().ToString()+"\nTurn : "+ (!TurnManager.Instance.getPlayerTurn() ? "Fluctuomancers " : "Fused ")+TurnManager.Instance.getTurnNumber().ToString();
        phaseAnnounceText.text = PhaseManager.Instance.CombatPhase()? "Fight !": (PhaseManager.Instance.MovementPhase() ? "Move !" : "Summon !");
        if (Input.GetMouseButtonDown(1)) /// clic droit
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
                statusText.text = piece.GetComponent<PieceAttributes>().pieceName + "\nLp : "+p.getCurLp().ToString() +"\nAttack left : "+p.getCurNbAtk().ToString() + "\nMovement left : "+p.pM.getCurMov().ToString() + "\nDamage Reduction : "+p.dmgReduc.ToString()+"\nDamage : "+(p.d4Atk!=0 ? p.d4Atk.ToString()+" d4 + ":"") + p.baseAtk.ToString() + (p.pM.isSpecial ? "\nAbility Cost : "+piece.GetComponent<Ability>().abilityCost.ToString() : "");
            }
        }
    }
}
