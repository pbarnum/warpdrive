using UnityEngine;
using System.Collections;

public class MenuRotate : MonoBehaviour
{
    private GameObject planet;
    public bool rotate;
    public bool lerp;
    private Vector3 lerpTo = new Vector3(-10.93764f, -0.01740861f, 30.38736f);

	// Use this for initialization
	void Start ()
    {
        planet = GameObject.Find("Planet");
        rotate = false;
        lerp = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // If old menu, rotate around planet
	    if(rotate)
        {
            this.transform.RotateAround(planet.transform.position, planet.transform.up, 75 * Time.deltaTime);

            if (this.transform.eulerAngles.y >= 105f && this.transform.eulerAngles.y <= 115f)
            {
                rotate = false;
                this.transform.position = new Vector3(-10.93764f, -0.01740861f, -35.42116f);
                this.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }

        // If new menu, lerp it into view 
        if(lerp)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, lerpTo, 3 * Time.deltaTime);

            if (Vector3.Distance(this.transform.position, lerpTo) <= 0.1f)
            {
                this.transform.position = lerpTo;
                lerp = false;
            }
        }
	}

    // Snap the menu into place before moving it.
    public void snap(bool newObj)
    {
        if(newObj)
        {
            rotate = false;
            this.transform.position = new Vector3(-10.93764f, -0.01740861f, -8.705765f);
            this.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            lerp = false;
            this.transform.position = lerpTo;
        }
    }

}
