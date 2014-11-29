using UnityEngine;
using System.Collections;

public class MenuCamera : MonoBehaviour
{
	public struct menu
	{
        public GameObject go;
		public int levelUp;

		public menu(GameObject g, int l)
		{
            this.go = g; this.levelUp = l;
		}
	};

	private menu[] _menuList = new menu[5];
	private int _currentMenu;
    private int _lastMenu;

	// Use this for initialization
	void Start ()
	{
        // Play ship's idle animation
        GameObject.Find("Ship").GetComponent<Animator>().Play("idle");

		_initMenu ();
        _currentMenu = 1;
	}

	private void _initMenu()
    {
		_menuList[0] = new menu(GameObject.Find("confirmObject"), 1);
        _menuList[1] = new menu(GameObject.Find("mainObject"), 0);
        _menuList[2] = new menu(GameObject.Find("playObject"), 1);
        _menuList[3] = new menu(GameObject.Find("settingsObject"), 1);
        _menuList[4] = new menu(GameObject.Find("levelsObject"), 2);

	}
	
	// Update is called once per frame
	void Update()
	{
		buttonInput ();
    }

	private void buttonInput()
	{
		if (Input.GetMouseButtonDown(0))//Input.touchCount > 0)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(/*Input.GetTouch(0).position*/Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider != null)
				{
                    Debug.Log(hit.transform.name);
                    _lastMenu = _currentMenu;
					switch(hit.transform.name)
					{
					case "playButton":
						_currentMenu = 2;
                        _moveMenu();
						break;
					case "infoButton":
						_currentMenu = 3;
						_moveMenu();
						break;
					case "exitButton":
						_currentMenu = 0;
						_moveMenu();
						break;
					case "confirmButton":
						Debug.Log("Game has been quit (but not for this demo)");
						doBackMenuLevel();
						//Application.Quit();
						break;
					case "cancelButton":
						doBackMenuLevel();
						break;
					case "saveButton":
						Debug.Log("Saved Settings (but not really)");
						doBackMenuLevel();
						break;
					case "levelsButton":
                        _currentMenu = 4;
                        _moveMenu();
						break;
					case "infiniteButton":
						Application.LoadLevel("infinity");
                        break;
                    case "level1Button":
                        Application.LoadLevel("level_01");
                        break;
                    case "level2Button":
                        Application.LoadLevel("level_02");
                        break;
					/*case "retryButton":
                        Application.LoadLevel("infinity");
						break;*/
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
		
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            _lastMenu = _currentMenu;
			doBackMenuLevel();
		}
	}

    // Go back one menu level
    private void doBackMenuLevel()
    {
        _currentMenu = _menuList[_currentMenu].levelUp;
        _moveMenu();
    }

    private void _moveMenu()
    {
        // Snap old and new to prevent weird bugs
        _menuList[_lastMenu].go.GetComponent<MenuRotate>().snap(false);
        _menuList[_currentMenu].go.GetComponent<MenuRotate>().snap(true);

        // Rotate old
        _menuList[_lastMenu].go.GetComponent<MenuRotate>().rotate = true;

        // Rotate new
        StartCoroutine(_menuList[_currentMenu].go.GetComponent<MenuRotate>().setLerp());
    }

    public int getCurrentMenu()
    {
    	return _currentMenu;
    }
}
