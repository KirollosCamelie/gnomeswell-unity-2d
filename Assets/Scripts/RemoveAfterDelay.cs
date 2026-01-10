using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Removes an object after  a certain delay
public class RemoveAfterDelay : MonoBehaviour
{
    //How many seconds to wait before removing
    public float delay = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Kick off the 'Remove' coroutine
        StartCoroutine("Remove");
    }

    IEnumerator Remove()
    {
        //Wait 'delay' seconds, and then destroy the game object attached to this object.
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);

        //Don't say Destroy(this) that just Destroy RemoveAfterDelay script.
    }
    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
