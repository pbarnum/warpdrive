using UnityEngine;
using System.Collections;

public class MenuRotate : MonoBehaviour
{
    private GameObject planet;
    public bool rotate;
    private bool check = false;
    private bool lerp = false;
    private Vector3 lerpTo = new Vector3(-10.93764f, -0.01740861f, 30.38736f);

	// Use this for initialization
	void Start ()
    {
        planet = GameObject.Find("Planet");
        rotate = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(rotate)
        {
            this.transform.RotateAround(planet.transform.position, planet.transform.up, 75 * Time.deltaTime);

            if (this.transform.eulerAngles.y >= 105f && this.transform.eulerAngles.y <= 115f)
            {
                this.transform.position = new Vector3(-10.93764f, -0.01740861f, -8.705765f);
                this.transform.eulerAngles = new Vector3(0, 0, 0);
                rotate = false;
                check = false;
            }
        }

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

    public void snap(bool newObj)
    {
        if(newObj)
        {
            rotate = false;
            check = false;
            this.transform.position = new Vector3(-10.93764f, -0.01740861f, -8.705765f);
            this.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            lerp = false;
            this.transform.position = lerpTo;
        }
    }

    public IEnumerator setLerp()
    {
        yield return new WaitForSeconds(1);
        lerp = true;
    }

}
