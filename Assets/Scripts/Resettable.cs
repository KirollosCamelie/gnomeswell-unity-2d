using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Contains a UnityEvent that can be used to reset the state of this object
public class Resettable : MonoBehaviour
{
    //In the editor, connects this event to methods that should run when the game resets.
    public UnityEvent onReset;

    //Called by the GameManager when the game rests.
    public void Reset()
    {
        //Kicks off the event, which calls of the connected methods.
        onReset.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
