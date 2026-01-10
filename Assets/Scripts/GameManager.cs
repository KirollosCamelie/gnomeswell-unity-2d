using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    //The location where the GNome should appear
    public GameObject startingPoint;

    //the rope object, which lowers and raises the gnome
    public Rope rope;

    //The follow script which will follow the gnome
    public CameraFollow cameraFollow;

    //the 'current' gnome as opposed to all the dead ones
    Gnome currentGnome;

    //The prefab to instantiate when we need a new game
    public GameObject gnomePrefab;

    //The UI component that contains the 'restart' and 'resume' buttons
    public RectTransform mainMenu;


    //The UI component that contains the 'Up', 'Down', and 'menu' buttons
    public RectTransform gameplayMenu;

    //The UI component that contains the 'you win' screen
    public RectTransform gameOverMenu;

    //If true ignore all damage but still show damage effect.
    //the 'get; set;'
    //make this a property to make it show up in the list of methods in the inspector for Unity Events
    public bool gnomeInvincible {  get; set; }

    // How long to wait after dying to create a new gnome
    public float delayAfterDeath = 1.0f;

    //the sound to play when the gnome dies
    public AudioClip gnomeDiedSound;

    //the soud to play when the game is won
    public AudioClip gameOverSound;

    void Start()
    {
        //When the game starts call Reset to set up the gnome
        Reset();
        mainMenu.gameObject.SetActive(false);

    }

    //Reset the entire game
    public void Reset()
    {
        //Turn off the menu turn on the gameplay UI
        if (gameOverMenu)
        {
            gameOverMenu.gameObject.SetActive(false);
        }
        if (mainMenu)
        {
            mainMenu.gameObject.SetActive(true);
        }
        if (gameplayMenu)
        {
            gameplayMenu.gameObject.SetActive(true);
        }
        //Find all Resettable componnents and tell them to reset
        var resetObjects = FindObjectsOfType<Resettable>();
        foreach (Resettable r in resetObjects)
        {
            r.Reset();
        }
        //Make a new game
        CreateNewGnome();

        //Un-pause the game
        Time.timeScale = 1.0f;
    }

    void CreateNewGnome()
    {
        //Remove the current gnome if there is on
        RemovGnome();

        //Create a new Gnome object and make it be our currentGnome
        GameObject newGnome = (GameObject)Instantiate(gnomePrefab, 
                                                      startingPoint.transform.position,
                                                      Quaternion.identity);
        currentGnome = newGnome.GetComponent<Gnome>();

        //Make the rope visible
        rope.gameObject.SetActive(true);

        //Connect the rope's trailling end to whichever rigidbody the gnome object Wants
        //(e.g, the foot)
        rope.connectedObject = currentGnome.ropeBody;

        //Reset the rope length to the default
        rope.ResetLength();

        //Tell the Camera Follow to start tracking the new gnome
        cameraFollow.target = currentGnome.cameraFollowTarget;
    }

    void RemovGnome()
    {
        //Don't actually do anything if the gnome is invincible
        if (gnomeInvincible)
        {
            return;
        }

        //Hide the rope
        rope.gameObject.SetActive (false);

        //Stop tracking the gnome
        cameraFollow.target = null;

        //If we have a current gnome, make that no longer be the player
        if (currentGnome != null)
        {
            //This gnome is no longer holding treasure
            currentGnome.holdingTreasure = false;

            //Mark this object as not the player
            //(so that colliders won't report when the object hits them)
            currentGnome.gameObject.tag = "Untagged";

            //Find everything that's currently tagged "Player" and remove that tag
            foreach (Transform child in currentGnome.transform)
            {
                child.gameObject.tag = "Untagged";
            }
            //Mark ourselves as not currently having a gnome
            currentGnome = null;
        }

    }

    //Kills the gnome
    void KillGnome(Gnome.DamageType damageType)
    {
        //If we have an audio source, play "gnome died" sound
        var audio = GetComponent<AudioSource>();
        if (audio)
        {
            audio.PlayOneShot(this.gnomeDiedSound);
        }

        //Show the damage effect
        currentGnome.ShowDamageEffect(damageType);

        //If we're not invincible,
        //reset the game and make the gnome not be the current player
        if (gnomeInvincible == false)
        {
            //Tell the gnome that it died
            currentGnome.DestroyGnome(damageType);

            //Remove the Gnome
            RemovGnome();

            //Reset the game
            StartCoroutine(ResetAfterDelay());
        }
    }

    //Called when the gnome dies
    IEnumerator ResetAfterDelay()
    {
        //Wait for delayAfterDeath seconds, then call reset
        yield return new WaitForSeconds(delayAfterDeath);
        Reset();
    }

    //Called when the player touches a trap
    public void TrapTouched()
    {
        KillGnome(Gnome.DamageType.Slicing);
    }

    //Called when the player touches a fire trap
    public void FireTrapTouched()
    {
        KillGnome(Gnome.DamageType.Burning);
    }

    //Called when the gnome picks up the treasure
    public void TreasureCollected()
    {
        // tell the cuurentGnome that it should have the treasure
        currentGnome.holdingTreasure = true;
    }

    //Called when the player touches the exit
    public void ExitReached()
    {
        //If we have a player and that player is holding treasure, game over!
        if (currentGnome != null && currentGnome.holdingTreasure == true)
        {
            //If we an audio source, play the "game over" sound
            var audio = GetComponent<AudioSource>();
            if (audio)
            {
                audio.PlayOneShot(this.gameOverSound);
            }
            //Pause the game
            Time.timeScale = 0.0f;

            //Turn off Game Over menu, and turn on the "game over" screen!
            if (gameOverMenu)
            {
                gameOverMenu.gameObject.SetActive(true);
            }

            if (gameplayMenu)
            {
                gameplayMenu.gameObject.SetActive(false);
            }
        }
    }

    //Called Called when the Menu button is tapped, 
    //and when the Resume game button is tapped.
    public void SetPaused(bool paused)
    {

        //If we're paused, stop time and enable the menu (and disable the game overlay)
        if (paused)
        {
            Time.timeScale = 0.0f;
            mainMenu.gameObject.SetActive(true);
            gameplayMenu.gameObject.SetActive(false);
        }
        else
        {
            //If we're not paused,
            //resume time and disable the menu (and enable the game overlay)
            Time.timeScale = 1.0f;
            mainMenu.gameObject.SetActive(false);
            gameplayMenu.gameObject.SetActive(true);
        }
    }

    //Called when the restart button is tapped
    public void RestartGame()
    {
        //Immediately remove the gnome (instead of killing it)
        Destroy(currentGnome.gameObject);
        currentGnome = null;

        //Now reset the game to create anew gnome
        Reset();
        mainMenu.gameObject.SetActive(false);
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
