using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
	private Queue<GameObject> _segmentsQ = new Queue<GameObject>();
	private GameObject _lastQueued;
	private int _direction;
    private List<Object> _prefabs = new List<Object>();

	private float _timer = 0f;
	private float _best = 0f;
    private float _countdown = 4.5f;
    private bool _countdownBool = false;

	public Player player;
	public TextMesh onScreenTimer;
	public TextMesh bestTime;
    public TextMesh pTimer;

    public bool infinite;

	// Use this for initialization 
	void Start ()
	{
		_direction = 0;
		player = GameObject.Find("Ship").GetComponent<Player> ();

		onScreenTimer = GameObject.Find ("onScreenTimer").GetComponent<TextMesh> ();
        bestTime = GameObject.Find("bestTime").GetComponent<TextMesh>();
        pTimer = GameObject.Find("pauseTimer").GetComponent<TextMesh>();
        pTimer.gameObject.SetActive(true);

        _prefabs.AddRange(Resources.LoadAll("straight"));
        _prefabs.AddRange(Resources.LoadAll("right"));
        _prefabs.AddRange(Resources.LoadAll("left"));

		if(System.IO.File.Exists(System.IO.Path.GetTempPath() + "WarpDrive_"+ Application.loadedLevelName +".dat"))
		{
            System.IO.StreamReader sr = System.IO.File.OpenText(System.IO.Path.GetTempPath() + "WarpDrive_" + Application.loadedLevelName + ".dat");
			string line;
			while((line = sr.ReadLine()) != null)
			{
				break;
			}
			sr.Close();
			bestTime.text = line;
			_best = float.Parse(line.Remove(0, 6));
		}

        // If current level is 'infinity' - create level segments
        infinite = (Application.loadedLevelName == "infinity") ? true : false;

        if(infinite)
        {
		    for(int i = 0; i < 3; ++i)
		    {
			    addSegment();
		    }
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!player.paused)
			_timer += Time.deltaTime;

		if(_timer > _best)
			bestTime.text = "Best: "+ _timer.ToString("F2");

        if(_countdownBool)
        {
            _countdown -= Time.deltaTime;

            if(_countdown >= 3)
            {
                pTimer.text = "3";
                pTimer.color = Color.red;
            }
            else if(_countdown >= 2)
            {
                pTimer.text = "2";
                pTimer.color = new Color(1, 0.64f, 0, 1); // Orange
            }
            else if(_countdown >= 1)
            {
                pTimer.text = "1";
                pTimer.color = Color.yellow;
            }
            else if (_countdown >= 0)
            {
                pTimer.text = "GO!";
                pTimer.color = new Color(0, 128, 0, _countdown); // Green
            }
            else if (_countdown >= -1)
            {
                _countdownBool = false;
                _countdown = 4.5f;
                pTimer.gameObject.SetActive(false);
            }
        }


		onScreenTimer.text = "Timer: " + _timer.ToString ("F2");
	}

	public void addSegment()
	{
		Vector3 pos;
		Vector3 rot;

		if(_segmentsQ.Count <= 0)
		{
			_segmentsQ.Enqueue(GameObject.Find ("initSeg"));
			_lastQueued = GameObject.Find ("initSeg");
		}

		int rand = Random.Range (0, _prefabs.Count);
		GameObject nextSeg = (GameObject)Instantiate (_prefabs[rand]);
        Transform connector = _lastQueued.transform.FindChild("connector").transform;

		// Use last segment's direction
		switch(_direction)
		{
		case 0: // North
			if(nextSeg.name.StartsWith("straight"))
			{
                pos = connector.position;
                rot = new Vector3(0, 0, 0);
			}
            else if (nextSeg.name.StartsWith("right"))
            {
                _direction = 1; // Now heading east

                pos = connector.position;
                rot = new Vector3(0, 180, 0);
            }
            else // Left
            {
                _direction = 3; // Now heading west

                pos = connector.position;
                rot = new Vector3(0, -90, 0);
            }
			break;
		case 1: // East
			if(nextSeg.name.StartsWith("straight"))
            {
                pos = connector.position;
                rot = new Vector3(0, 90, 0);
			}
			else if(nextSeg.name.StartsWith("right"))
            {
                _direction = 2; // Now heading south

                pos = connector.position;
                rot = new Vector3(0, -90, 0);
			}
            else // Left
            {
                _direction = 0; // Now heading north

                pos = connector.position;
                rot = new Vector3(0, 0, 0);
            }
			break;
        case 2: // South
            if (nextSeg.name.StartsWith("straight"))
            {
                pos = connector.position;
                rot = new Vector3(0, 180, 0);
            }
            else if (nextSeg.name.StartsWith("right"))
            {
                _direction = 3; // Now heading west

                pos = connector.position;
                rot = new Vector3(0, 0, 0);
            }
            else // Left
            {
                _direction = 1; // Now heading east

                pos = connector.position;
                rot = new Vector3(0, 90, 0);
            }
			break;
        case 3: // West
            if (nextSeg.name.StartsWith("straight"))
            {
                pos = connector.position;
                rot = new Vector3(0, -90, 0);
            }
            else if (nextSeg.name.StartsWith("right"))
            {
                _direction = 0; // Now heading north

                pos = connector.position;
                rot = new Vector3(0, 90, 0);
            }
            else // Left
            {
                _direction = 2; // Now heading south

                pos = connector.position;
                rot = new Vector3(0, 180, 0);
            }
			break;
		default: // Unknown direction
            pos = connector.position;
            rot = connector.eulerAngles;
			Debug.Log("What direction are you heading!?!");
			break;
		}

		nextSeg.transform.position = pos;
		nextSeg.transform.eulerAngles = rot;

		_segmentsQ.Enqueue (nextSeg);
		_lastQueued = nextSeg;

		if(_segmentsQ.Count > 5)
			_removeSegment();
	}

	private void _removeSegment()
	{
		Destroy (_segmentsQ.Dequeue ());
	}

	public float getTimer()
	{
		return _timer;
	}

	private void setBest()
	{
		if(_timer > _best)
		{
			_best = _timer;
			bestTime.text = "Best: " + _best.ToString ("F2");
		}
	}

	public void doReturnToGame()
	{
        _countdownBool = true;

        if(pTimer == null)
            pTimer = GameObject.Find("pauseTimer").GetComponent<TextMesh>();

        pTimer.gameObject.SetActive(true);
	}

	public void saveTime()
	{
		setBest ();
		string text = bestTime.text;
        System.IO.File.WriteAllText(System.IO.Path.GetTempPath() + "WarpDrive_" + Application.loadedLevelName + ".dat", text);
	}

	public Transform getCurrentSeg()
	{
		return _lastQueued.transform;
	}
}
