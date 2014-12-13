using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
    public Material texture;

    public int playerHealth = 3;
    public float maxWarp = 2.05f;
    public float baseSpeed = 10f;
    public float baseStrafe = 20f;
    public float maxSpeed = 30f;

    public Animator animator;

	// Use this for initialization
	void Start ()
    {
        // Start ship in grounded animation
        animator = this.GetComponent<Animator>();
        animator.Play("grounded");

        if(PlayerPrefs.HasKey("material"))
        {
            GameObject.Find("LiftOff:Ship").renderer.material = Resources.Load<Material>("ship/" + PlayerPrefs.GetString("material"));
        }
        else
        {
            GameObject.Find("LiftOff:Ship").renderer.material = Resources.Load<Material>("ship/ShipDiffuse_Red");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
	
}
