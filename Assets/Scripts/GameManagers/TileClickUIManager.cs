using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro; // ou UnityEngine.UI si tu utilises le Text classique

public class TileClickUI : MonoBehaviour
{
    public Tilemap tilemap;
    public Camera cam;
    public TextMeshProUGUI phaseText; // ou public Text phaseText;
    public TextMeshProUGUI statusText; // ou public Text phaseText;

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
        phaseText.text = PhaseManager.Instance.getPhase().ToString();
        if (Input.GetMouseButtonDown(1)) // clic droit
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
            TileBase tile = tilemap.GetTile(cellPos);
            if (!PieceInteractionManager.Instance.isAPiece(cellPos)){
                if (tile != null)
                {
                    statusText.text = "Coordonnées : " + cellPos.ToString() + "\nState : "+TileStateManager.Instance.getState(cellPos).ToString();
                }
                else
                {
                    statusText.text = "nbFluct"+WinCondition.Instance.fluctOnMap.ToString() + "\n" + "nbFus"+WinCondition.Instance.fusOnMap.ToString();
                }
            } else {
                PieceAttack p = PieceInteractionManager.Instance.getPiece(cellPos).GetComponent<PieceAttack>();
                statusText.text = "Lp : "+p.getCurLp().ToString() +"\nCurNbAtk : "+p.getCurNbAtk().ToString() + "\nCurMov : "+p.pM.getCurMov().ToString() + "\nCanSummon : "+ TurnManager.Instance.isSummonable(p.pM.isSpecial, p.pM.isFluct).ToString() + "\nDmgReduc : "+p.dmgReduc.ToString();
            }
        }
    }
}
