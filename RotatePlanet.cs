using UnityEngine;
using System.Collections;

public class RotatePlanet : MonoBehaviour
{
    GameObject moon;

	// Use this for initialization
	void Start ()
    {
        moon = GameObject.Find("Moon");

        // Dynamically change object's texture
        //this.transform.renderer.material.mainTexture = (Texture)Resources.Load("planets/" + Random.Range(1, 6));
        //moon.transform.renderer.material.mainTexture = (Texture)Resources.Load("planets/" + Random.Range(0, 6));
	}
	
	// Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 2 * Time.deltaTime);

        moon.transform.RotateAround(this.transform.position, this.transform.up, 2 * Time.deltaTime);
        moon.transform.Rotate(Vector3.up * 3 * Time.deltaTime);
    }
}
