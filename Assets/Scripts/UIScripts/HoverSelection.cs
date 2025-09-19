using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

public class HoverSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Image im;
    private Color baseColor;
    private Transform child;

    void Start() {
        child = transform.GetChild(0);
        im = GetComponent<Image>();
        baseColor = im.color; // stocke la couleur de départ
    }

    public void OnPointerEnter(PointerEventData eventData) {
        im.color = Color.white; // highlight quand la souris est dessus
        child.gameObject.GetComponent<TextMeshProUGUI>().color = Color.black; // change la couleur du texte
    }

    public void OnPointerExit(PointerEventData eventData) {
        im.color = baseColor; // reset quand la souris sort
        child.gameObject.GetComponent<TextMeshProUGUI>().color = Color.white; // change la couleur du texte
    }
}
