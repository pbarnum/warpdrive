using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	private Camera thisCam;
    private GameObject _gameOver;

	// Use this for initialization
	void Start ()
	{
		thisCam = GetComponent<Camera> ();
        _gameOver = this.transform.FindChild("gameOver").gameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{
		buttonInput ();
	}

	private void buttonInput()
	{
		if (Input.GetMouseButtonDown(0))//Input.touchCount > 0)
		{
			RaycastHit hit;
			Ray ray = thisCam.ScreenPointToRay(/*Input.GetTouch(0).position*/Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider != null)
				{
					Debug.Log(hit.transform.name);
					switch(hit.transform.name)
					{
					case "returnToMenuButton":
						Application.LoadLevel("main");
						break;
					case "retryButton":
                        Application.LoadLevel(Application.loadedLevelName);
						break;
					default: // Something went wrong
						Debug.Log("Update switch default activated");
						break;
					}
				}
				else
				{
					Debug.Log("Player tapped screen but not a button");
				}
			}
		}

        if(_gameOver.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Application.LoadLevel(Application.loadedLevelName);
            }
        }
        
	}
}
