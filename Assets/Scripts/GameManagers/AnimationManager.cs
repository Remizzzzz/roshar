using UnityEngine;

public enum AnimationCode {stormShape} // Define your animation codes here
public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    public GameObject stormShapeAnimation;


    public GameObject animate(Vector3 position, AnimationCode codeAnimation){
        GameObject fx = null;
        switch (codeAnimation)
        {
            case AnimationCode.stormShape:
                fx = Instantiate(stormShapeAnimation, position, Quaternion.identity); //Quaternion.identity for no rotation
                Destroy(fx, 0.7f); // Duration of the animation
                break;
            // Add more cases for different animations as needed
            default:
                Debug.LogWarning("Animation code not recognized: " + codeAnimation);
                break;
        }
        return fx;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance=this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
