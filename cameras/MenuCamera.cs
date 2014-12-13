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

	private menu[] _menuList = new menu[6];
	private int _currentMenu;
    private int _lastMenu;
    private GameObject _ship;
    private Material[] textures;
    private int selTex;
    private int oldTex;
    private bool noSplash;

	// Use this for initialization
	void Start ()
	{
        noSplash = false;

        // Play ship's idle animation
        _ship = GameObject.Find("LiftOff:Ship");
        GameObject.Find("Ship").GetComponent<Animator>().Play("idle");

        textures = Resources.LoadAll<Material>("ship");
        if (PlayerPrefs.HasKey("material"))
        {
            _ship.renderer.material = Resources.Load<Material>("ship/" + PlayerPrefs.GetString("material"));
            for(int i = 0; i < textures.Length; ++i)
            {
                if (textures[i].name == PlayerPrefs.GetString("material"))
                    selTex = i;
            }
            string tmp = textures[selTex].name.ToString();
            tmp = tmp.Substring(tmp.IndexOf('_') + 1);
            GameObject.Find("colorText").GetComponent<TextMesh>().text = tmp;
        }
        else
        {
            _ship.renderer.material = Resources.Load<Material>("ship/ShipDiffuse_Red");
            selTex = 5;
            string tmp = textures[selTex].name.ToString();
            tmp = tmp.Substring(tmp.IndexOf('_') + 1);
            GameObject.Find("colorText").GetComponent<TextMesh>().text = tmp;
        }

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
        _menuList[5] = new menu(GameObject.Find("shipObject"), 1);

	}
	
	// Update is called once per frame
	void Update()
	{
		buttonInput ();

        if(noSplash)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, Vector3.zero, Time.deltaTime * 2);

            if(Vector3.Distance(this.transform.position, Vector3.zero) <= 0.01f)
            {
                noSplash = false;
            }
        }
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
                    case "splash":
                        noSplash = true;
                        break;
                    case "playButton":
                        _currentMenu = 2;
                        _moveMenu();
                        break;
                    case "shipButton":
                        _currentMenu = 5;
                        oldTex = selTex; // Save the old texture
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
						//doBackMenuLevel();
						Application.Quit();
						break;
					case "cancelButton":
                        selTex = oldTex;
                        _ship.renderer.material = textures[selTex];
                        string tmp = textures[selTex].name.ToString();
                        tmp = tmp.Substring(tmp.IndexOf('_') + 1);
                        GameObject.Find("colorText").GetComponent<TextMesh>().text = tmp;
						doBackMenuLevel();
                        break;
                    case "saveButton":
                        doBackMenuLevel();
                        break;
                    case "leftButton":
                        _changeTex(false);
                        break;
                    case "rightButton":
                        _changeTex(true);
                        break;
					case "levelsButton":
                        _currentMenu = 4;
                        _moveMenu();
						break;
					case "infiniteButton":
                        PlayerPrefs.SetString("material", textures[selTex].name.ToString());
						Application.LoadLevel("infinity");
                        break;
                    case "tutorialButton":
                        PlayerPrefs.SetString("material", textures[selTex].name.ToString());
						Application.LoadLevel("tutorial");
                        break;
                    case "level1Button":
                        PlayerPrefs.SetString("material", textures[selTex].name.ToString());
                        Application.LoadLevel("level_01");
                        break;
                    case "level2Button":
                        PlayerPrefs.SetString("material", textures[selTex].name.ToString());
                        Application.LoadLevel("level_02");
                        break;
                    case "level3Button":
                        PlayerPrefs.SetString("material", textures[selTex].name.ToString());
                        Application.LoadLevel("level_03");
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

        // Lerp new
        _menuList[_currentMenu].go.GetComponent<MenuRotate>().lerp = true;
    }
    
    private void _changeTex(bool right)
    {
        if(right)
        {
            if (selTex == textures.Length - 1)
            {
                selTex = 0;
                _ship.renderer.material = textures[0];
            }
            else
            {
                _ship.renderer.material = textures[++selTex];
            }
        }
        else
        {
            if (selTex == 0)
            {
                selTex = textures.Length - 1;
                _ship.renderer.material = textures[textures.Length - 1];
            }
            else
            {
                _ship.renderer.material = textures[--selTex];
            }
        }

        string tmp = textures[selTex].name.ToString();
        tmp = tmp.Substring(tmp.IndexOf('_') + 1);
        GameObject.Find("colorText").GetComponent<TextMesh>().text = tmp;
    }

    public int getCurrentMenu()
    {
    	return _currentMenu;
    }
}
