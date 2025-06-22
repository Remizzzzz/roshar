using UnityEngine;
using System.Collections.Generic;

public class AnimationCreator : MonoBehaviour
{
    public List<Slide> animations = new List<Slide>(); //list of slides for now, we'll change if we add more animations later
    public bool resetPosition = true; //if true, the position is reset after the animation
    public bool isHiding = true; //if true, the object is hidden when it's not animated

    private Vector3 initialPosition; //initial position of the object, used to reset the position after the animation
    private bool animationStarted = false; //if true, the animation is started
    private int currentAnimationIndex = 0; //index of the current animation

    public void Animate(){
        if (isHiding) gameObject.SetActive(true); //if the object is hidden, we show it
        animations[currentAnimationIndex].StartAnimation(transform); //we start the animation
        animationStarted = true; //we start the animation
    }
    void Start()
    {
        initialPosition = transform.position; //we store the initial position of the object
    }

    void Update(){
        if (!animationStarted) return; //if the animation is not started, we don't do anything
        if (animations[currentAnimationIndex].IsSliding){
            animations[currentAnimationIndex].UpdateAnimation(transform); //we update the current animation
            return; //we don't go to the next animation yet
        }
        currentAnimationIndex++; //we go to the next animation
        if (currentAnimationIndex >= animations.Count) //if we reached the end of the list, we stop the animation
        {
            animationStarted = false;
            currentAnimationIndex = 0; //reset the index
            if (resetPosition) transform.position = initialPosition; //reset the position if needed
            if (isHiding) gameObject.SetActive(false); //hide the object if needed
            return;
        }
        animations[currentAnimationIndex].StartAnimation(transform); //we start the animation
    }
}