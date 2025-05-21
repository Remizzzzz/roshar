using UnityEngine;
using TMPro;

[ExecuteAlways] // Script done by ChatGPT to gain time, it's only an utility script
public class ButtonLabelEditor : MonoBehaviour
{
    [TextArea]
    public string label = "Nouveau texte";
    private string lastLabel;

    void Update()
    {
        if (!Application.isPlaying) {
            if (label != lastLabel) {
                TMP_Text textComponent = GetComponentInChildren<TMP_Text>();
                if (textComponent != null)
                {
                    textComponent.text = label;
                    lastLabel = label;
                }
            }
        }
    }
}
