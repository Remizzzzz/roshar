using UnityEngine;
using TMPro; // Ensure you have TextMeshPro package installed for text animations
public enum AnimationCode {stormShape, damage, death} /// Define your animation codes here
public class AnimationManager : MonoBehaviour
{
    /**
    This class will manages in-game animations.
    */
    public static AnimationManager Instance;
    public GameObject stormShapeAnimation;
    public GameObject damageAnimation;
    public GameObject deathAnimation;

    public GameObject animate(Vector3 position, AnimationCode codeAnimation, int damage = 0 /*optional parameter for damage animations*/){
        /** This method instantiates an animation at the given position based on the provided AnimationCode.
        It returns the instantiated GameObject for further manipulation if needed.
        */
        GameObject fx = null;
        switch (codeAnimation)
        {
            case AnimationCode.stormShape:
                fx = Instantiate(stormShapeAnimation, position, Quaternion.identity); //Quaternion.identity for no rotation
                Destroy(fx, 0.7f); // Duration of the animation
                break;
            case AnimationCode.damage:
                fx = Instantiate(damageAnimation, position, Quaternion.identity); //Quaternion.identity for no rotation
                if (damage <= 1) fx.GetComponent<TextMeshPro>().color = new Color(1,1,1);
                else { 
                    switch (damage){
                        case 2 :
                            fx.GetComponent<TextMeshPro>().color = new Color(1f,1f,0.5f);
                            break;
                        case 3 :
                            fx.GetComponent<TextMeshPro>().color = new Color(1f,1f,0);
                            break;
                        case 4 :
                            fx.GetComponent<TextMeshPro>().color = new Color(1f,0.5f,0);
                            break;
                        case 5 :
                            fx.GetComponent<TextMeshPro>().color = new Color(1f,0.2f,0);
                            break;
                        case 6 :
                            fx.GetComponent<TextMeshPro>().color = new Color(1f,0,0);
                            break;
                        default:
                            fx.GetComponent<TextMeshPro>().color = new Color(0.7f,0,0);
                            break;
                    }
                }
                if (damage >= 0) fx.GetComponent<TextMeshPro>().text = "-" +damage.ToString();
                else fx.GetComponent<TextMeshPro>().text = "+" + (-damage).ToString();
                fx.GetComponent<MeshRenderer>().sortingOrder = 49; // Ensure the text is rendered on top
                fx.GetComponent<AnimationCreator>().Animate();
                Destroy(fx, 1f); // Duration of the animation
                break;
            case AnimationCode.death :
                fx = Instantiate(deathAnimation, position, Quaternion.identity); //Quaternion.identity for no rotation
                Destroy(fx, 1f); // Duration of the animation
                break;
            // Add more cases for different animations as needed
            default:
                Debug.LogWarning("Animation code not recognized: " + codeAnimation);
                break;
        }
        return fx;
    }
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance=this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
