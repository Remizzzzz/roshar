using UnityEngine;
using TMPro;

[ExecuteAlways] // Script done by ChatGPT to gain time, it's only an utility script
public class ButtonLabelEditor : MonoBehaviour
{
    /** This class is used to edit the label of a button in the editor.
     * It will update the text of the button with the label defined in the inspector.
     * It is used to directly personalize the button's label.
     * It is not used in the game, only in the editor.
     */
    [TextArea]
    public string label = "Nouveau texte";
    public float size= 20f;
    private string lastLabel;
    public TMP_FontAsset fontAsset;

    void Update()
    {
        if (!Application.isPlaying) {
            TMP_Text textComponent = GetComponentInChildren<TMP_Text>();
            if (label != lastLabel) {
                if (textComponent != null)
                {
                    textComponent.text = label;
                    lastLabel = label;
                }
            }
            textComponent.fontSize = size;
            textComponent.font = fontAsset;
        }
    }
}
