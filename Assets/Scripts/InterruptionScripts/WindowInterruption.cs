using UnityEngine;
using TMPro; // ou UnityEngine.UI si tu utilises le Text classique

public class WindowInterruption : MonoBehaviour
{
    public TextMeshPro windowText; // ou public Text phaseText;
    public GameObject button;
    private GameObject caller; // The GameObject that called the interruption
    private string callerName;

    public void setCallerName(string name)
    {
        callerName = name;
    }
    public void setCaller(GameObject Caller)
    {
        caller = Caller;
    }
    private void SetOrderInLayer(GameObject go, int order)
    {
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = order;
        }

        // If the GameObject has children with SpriteRenderers, set their sorting order as well
        foreach (SpriteRenderer childSr in go.GetComponentsInChildren<SpriteRenderer>())
        {
            childSr.sortingOrder = order;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 50; // Set the sorting order of the window
        windowText.sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
        windowText.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;


        if (caller != null){
            if (caller.GetComponent<DeepestAbility>()!=null) {
                windowText.text = "Do you want to activate the Deepest unit ability ?";
                // Get the order in layer of the parent SpriteRenderer to set the order of the buttons
                int baseOrder = GetComponent<SpriteRenderer>().sortingOrder;

                // Button "No"
                GameObject noButton = Instantiate(button);
                noButton.transform.SetParent(transform, false);
                noButton.transform.localPosition = new Vector3(-0.37f, -0.3f, 0);
                SetOrderInLayer(noButton, baseOrder + 1);

                noButton.GetComponent<ButtonForInterruption>().setText("No");
                noButton.GetComponent<ButtonForInterruption>().setColor(Color.red);
                noButton.GetComponent<ButtonForInterruption>().setOnClickAction(() => {
                    caller.GetComponent<DeepestAbility>().cancelActivation();
                    InterruptionManager.Instance.endInterruption();
                    InterruptionManager.Instance.destroyWindows(); // Destroy all interruption windows
                });

                // Button "Yes"
                GameObject yesButton = Instantiate(button);
                yesButton.transform.SetParent(transform, false);
                yesButton.transform.localPosition = new Vector3(0.37f, -0.3f, 0);
                SetOrderInLayer(yesButton, baseOrder + 1);

                yesButton.GetComponent<ButtonForInterruption>().setText("Yes");
                noButton.GetComponent<ButtonForInterruption>().setColor(Color.green);
                yesButton.GetComponent<ButtonForInterruption>().setOnClickAction(() => {
                    caller.GetComponent<DeepestAbility>().acceptActivation();
                    InterruptionManager.Instance.endInterruption();
                    InterruptionManager.Instance.destroyWindows(); // Destroy all interruption windows
                });
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
