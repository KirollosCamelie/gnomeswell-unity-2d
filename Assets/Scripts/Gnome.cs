using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gnome : MonoBehaviour
{
    //The Object that the camera should follow
    public Transform cameraFollowTarget;

    public Rigidbody2D ropeBody;

    public Sprite armHoldingEmty;
    public Sprite armHoldingTreasure;

    public SpriteRenderer holdingArm;

    public GameObject deathPrefab;
    public GameObject flameDeathPrefab;
    public GameObject ghostPrefab;

    public float delayBeforeRemoving = 3.0f;
    public float delayBeforeRelasingGhost = 0.25f;

    public GameObject bloodFountainPrefab;

    bool dead = false;

    bool _holdingTreasure = false;
    public bool holdingTreasure
    {
        get
        {
            return _holdingTreasure;
        }
        set
        {
            if (dead == true)
            {
                return;
            }

            _holdingTreasure = value;

            if (holdingArm != null)
            {
                if (_holdingTreasure)
                {
                    holdingArm.sprite = armHoldingTreasure;
                }
                else
                {
                    holdingArm.sprite = armHoldingEmty;
                }
            }
        }
    }

    public enum DamageType
    {
        Slicing,
        Burning
    }

    public void ShowDamageEffect(DamageType type)
    {
        switch (type)
        {
            case DamageType.Slicing:
                if (deathPrefab != null)
                {
                    Instantiate(deathPrefab,
                                cameraFollowTarget.position,
                                cameraFollowTarget.rotation);
                }
                break;
            case DamageType.Burning:
                if (flameDeathPrefab != null)
                {
                    Instantiate(flameDeathPrefab,
                                cameraFollowTarget.position,
                                cameraFollowTarget.rotation);
                }
                break;
        }
    }

    public void DestroyGnome(DamageType type)
    {
        holdingTreasure = false;

        dead = true;

        //Find all child objects and randomly disconnect their joints
        foreach (BodyPart part in GetComponentsInChildren<BodyPart>())
        {
            switch (type)
            {
                case DamageType.Burning:
                    //1 in 3 chances of burning
                    bool shouldBurn = Random.Range(0, 2) == 0;
                    if (shouldBurn)
                    {
                        part.ApplyDamageSprite(type);
                    }
                    break;
                case DamageType.Slicing:
                    //Slice damage always apply damage on sprite
                    part.ApplyDamageSprite(type);
                    break;
            }
            //1 in 3 chance of seperating from body
            bool shouldDetach = Random.Range(0, 2) == 0;

            if (shouldDetach)
            {
                //Make this object remove its rigidbody and collider after it comes to rest
                part.Detach();

                //If we're seperating and the damage type was slicing, add a blood fountain
                if (type == DamageType.Slicing)
                {
                    if (part.bloodFountainOrigin != null && bloodFountainPrefab != null)
                    {
                        //Attach blood fountain for this detached part
                        GameObject fountain = Instantiate(bloodFountainPrefab,
                                                          part.bloodFountainOrigin.position,
                                                          part.bloodFountainOrigin.rotation
                                                          )as GameObject;

                        fountain.transform.SetParent(this.cameraFollowTarget, false);
                    }
                }

                //Disconnect this object
                var allJoints = part.GetComponentsInChildren<Joint2D>();
                foreach (Joint2D joint in allJoints)
                {
                    Destroy(joint);
                }
            }
        }
        //Add RemoveAfterDelay component to this object
        var remove = gameObject.AddComponent<RemoveAfterDelay>();
        remove.delay = delayBeforeRemoving;

        StartCoroutine(ReleaseGhost());
    }

    IEnumerator ReleaseGhost()
    {
        //No ghost pefab? Bail our.
        if (ghostPrefab == null)
        {
            yield break;
        }

        //Wait for delayBeforeReleasingGhost seconds
        yield return new WaitForSeconds(delayBeforeRelasingGhost);

        //Add the ghost
        Instantiate(
            ghostPrefab,
            transform.position,
            Quaternion.identity);
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
