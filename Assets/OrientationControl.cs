using UnityEngine;
using System.Collections;

public class OrientationControl : MonoBehaviour
{

    public bool TorsoSimetric;
    public bool ArmsSimetric;
    public bool LegsSimetric;

    public float Orientation;
    private Animator animator;

    SpriteRenderer TorsoSprite, LegsSprite, LarmSprite, RarmSprite;


    // Use this for initialization
    void Start()
    {

        animator = this.GetComponent<Animator>();


        LegsSimetric = true;


        var ChildrenSprite = this.GetComponentsInChildren<SpriteRenderer>();


        foreach (var sprite in ChildrenSprite)
        {
            if (sprite.name == "Torso")
            {
                TorsoSprite = sprite;
            } else if (sprite.name == "Legs")
            {
                LegsSprite = sprite;
            } else if (sprite.name == "Larm")
            {
                LarmSprite = sprite;
            } else if (sprite.name == "Rarm")
            {
                RarmSprite = sprite;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Orientation = animator.GetFloat("Orientation");

        if (LegsSimetric && Orientation > 16 && !LegsSprite.flipX)
        {
            LegsSprite.flipX = true;

        } else if (Orientation < 17 && LegsSprite.flipX)
        {
            LegsSprite.flipX = false;
        }

        if (ArmsSimetric && Orientation > 16 && !LarmSprite.flipX)
        {
            LarmSprite.flipX = true;
            RarmSprite.flipX = true;

        } else if (Orientation < 17 && LarmSprite.flipX)
        {
            LarmSprite.flipX = false;
            RarmSprite.flipX = false;
        }

        if (TorsoSimetric && Orientation > 16 && !TorsoSprite.flipX)
        {
            TorsoSprite.flipX = true;

        } else if (Orientation < 17 && TorsoSprite.flipX)
        {
            TorsoSprite.flipX = false;
        }

    }
}
