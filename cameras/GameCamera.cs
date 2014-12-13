using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	private Camera thisCam;
    private GameObject _gameOver;
    private Player _player;
    private Vector3 _origPos;
    private Vector3 _warpPos = new Vector3(0, 0.6351723f, -1.788481f);

	// Use this for initialization
	void Start ()
	{
        _origPos = new Vector3(0, 1.069077f, -1.327867f);
        _player = GameObject.Find("Ship").GetComponent<Player>();
		thisCam = this.GetComponent<Camera> ();
        _gameOver = this.transform.FindChild("gameOver").gameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{
		buttonInput ();

        if (!_player.paused)
        {
            if (_player.warping)
            {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, _warpPos, Time.deltaTime);
            }
            else
            {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, _origPos, Time.deltaTime * 2);
            }
        }
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
