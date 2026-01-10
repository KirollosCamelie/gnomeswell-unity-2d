using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Swap out a sprite for another.
//For example:
//the switches from 'treasure present' to 'treasure not present'.
public class SpriteSwapper : MonoBehaviour
{
    //The sprite that should be displayed
    public Sprite spriteToUse;

    //The sprite renderer that should use the new sprite
    public SpriteRenderer spriteRenderer;

    //The original sprite. Used when ResetSprite is used.
    private Sprite originalSprite;

    //Swap out the sprite
    public void SwapSprite()
    {
        //If this sprite is different than the current sprite...
        if (spriteToUse != spriteRenderer.sprite)
        {
            //store the previous sprite in the orignalSprite.
            originalSprite = spriteRenderer.sprite;

            //Make the sprite Renderer use the new sprite
            spriteRenderer.sprite = spriteToUse;
        }
    }

    //Reverts back to the old sprite
    public void ResetSprite()
    {
        //If we have a previous sprite...
        if (originalSprite != null)
        {
            //...make the sprite renderer use it.
            spriteRenderer.sprite = originalSprite;
        }
    }

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
