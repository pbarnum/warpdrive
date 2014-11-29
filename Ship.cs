using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
	//private mesh
	//private texture
	//private indicator

    public int playerHealth = 5;
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
	
        //switch()
        //{
        //case 0:
        //    break;
        //case 1:
        //    break;
        //case 2:
        //    break;
        //case 3:
        //    break;
        //default:
        //    break;
        //}
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
	
}
