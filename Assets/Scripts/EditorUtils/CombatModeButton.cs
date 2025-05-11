using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatModeButton : MonoBehaviour {
    private string combatMode = "Combat Mode";
    private string moveMode = "Movement Mode";
    private Color colorCombat = new Color(182f / 255f, 64f / 255f, 59f / 255f);
    private Color colorMove = new Color(92f / 255f, 201f / 255f, 221f / 255f);
    void Update() {
        TMP_Text textComponent = GetComponentInChildren<TMP_Text>();
        Image backgroundImage = GetComponentInChildren<Image>();
        if (PieceInteractionManager.Instance.combatModeEnabled()) {
            if (textComponent != null) textComponent.text = combatMode;
            if (backgroundImage != null) backgroundImage.color = colorCombat;
        } else {
            if (textComponent != null) textComponent.text = moveMode;
            if (backgroundImage != null) backgroundImage.color = colorMove;
        }
    }
}
