using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatModeButton : MonoBehaviour {
    private string combatText = "End of Turn";
    private string moveText = "Next : Summon Phase";
    private string summoningText = "Next : Combat Phase";
    private Color colorSummoning = new Color (60f/255f,100f/255f,1f);
    private Color colorCombat = new Color(182f / 255f, 64f / 255f, 59f / 255f);
    private Color colorMove = new Color(92f / 255f, 201f / 255f, 221f / 255f);
    void Update() {
        TMP_Text textComponent = GetComponentInChildren<TMP_Text>();
        Image backgroundImage = GetComponentInChildren<Image>();
        if (PhaseManager.Instance.MovementPhase()){
            if (textComponent != null) textComponent.text = moveText;
            if (backgroundImage != null) backgroundImage.color = colorMove;
        }
        else if (PhaseManager.Instance.CombatPhase()) {
            if (textComponent != null) textComponent.text = combatText;
            if (backgroundImage != null) backgroundImage.color = colorCombat;
        }
        else if (PhaseManager.Instance.SummoningPhase()){
            if (textComponent != null) textComponent.text = summoningText;
            if (backgroundImage != null) backgroundImage.color = colorSummoning;
        }

    }
}
