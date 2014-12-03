using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	//private Camera _mainCam;
	private GameObject _gameOver;
	private LevelManager _levelManager;
    private GameObject _warpBar;
    private Component[] children; // Ship's particle system
	
	private float _laneDistance = 1.5f;
	private int _currentLane = 3;
	private GameObject _newLane;
	private bool _changingLane = false;
	private bool _moveLeft;
	private Transform _rotAround;
	private bool _movingForward;
	private bool _turnRight = false;
	private bool _turnLeft = false;
	private Vector3 slerpPos;

	private Ship _ship;

    private bool _warping;
    private bool _warpCooldown;

	private int segs = 5;
    private int _leftTurns;
    private int _rightTurns;
    private const int COOL_DOWN = 5;

    private float _strafeSpeed;
    private float _currentSpeed;

    private bool _dead = false;
	private bool _paused = false;
	public bool paused
	{
		get
		{
			return _paused;
		}
		set
		{
			_paused = value;
		}
	}

    private IEnumerator _wait()
    {
        yield return new WaitForSeconds(3.5f);
        _paused = false;
        
        children = this.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in children)
        {
            ps.Play();
        }
    }

	// Use this for initialization 
	void Start ()
    {
        // Start moving
        _movingForward = true;

        _paused = true;

        // Wait 3.5 seconds for start up animation
        StartCoroutine("_wait");

		//_mainCam = Camera.main;
		_gameOver = GameObject.Find ("gameOver");
		_gameOver.SetActive(false);
		_levelManager = GameObject.Find("GUI Camera").GetComponent<LevelManager> ();
        _levelManager.doReturnToGame(); // 'Unpause' to start count down timer

        _ship = this.GetComponent<Ship>();

        _warpBar = GameObject.Find("warpFront");

		_strafeSpeed = _ship.baseStrafe;
        _warping = false;
        _warpCooldown = false;
		_currentSpeed = _ship.baseSpeed;
	}
	
	// Update is called once per frame
	void Update()
	{
        // Listen for pause button
        if (!_dead && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
            pauseGame();
		
        // Moving forward
		if(!_paused && !_dead)
        {
            if (Input.GetKey(KeyCode.Space) && !_warpCooldown)
            {
                _warping = true;
            }
            else
            {
                _warping = false;
            }

            // Reduce warp when on, replenish when off
            _warpDrive();

            // Move forward
			if(_movingForward)
				_moveForward ();

            // Turn right
			if(_turnRight)
				_doTurnRight();
			
            // Turn left
            if(_turnLeft)
				_doTurnLeft();

            // Changing lanes
			if(!_changingLane)
				_listenForMovement();
			else
				_doChangeLane();
		
            // Speed up from 0 to base
            if (_currentSpeed < _ship.baseSpeed)
		    {
                _currentSpeed = Mathf.Lerp(_currentSpeed, _ship.baseSpeed, Time.deltaTime);

                if (_ship.baseSpeed - _currentSpeed <= 0.09f)
			    {
                    _currentSpeed = _ship.baseSpeed;
			    }
		    }
		}
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "triggerSegment" && other.transform.parent.FindChild("rotAroundObj") == null) // Hit trigger, add a seg
        {
            if (_levelManager.infinite)
                _levelManager.addSegment();

            --segs;

            if (_turnLeft || _turnRight)
            {
                // Convert player position/rotation to friendly values
                _reposition(other.transform.parent);

                this.transform.parent = null;

                // Start forward, stop all other movement
                _turnLeft = false;
                _turnRight = false;
                _movingForward = true;
            }
        }
        else if (other.tag == "soft" && !_warping) // Hit soft object, watch yo self!
        {
            --_ship.playerHealth;
            if (_ship.playerHealth == 0)
            {
                _death();
            }
        }
        else if (other.tag == "rightTurn") // Hit right turn
        {
            if (_levelManager.infinite)
                _levelManager.addSegment();

            --segs;
            _movingForward = false;

            _rotAround = other.transform.parent.FindChild("rotAroundObj").transform;
            this.transform.parent = _rotAround;

            // Start turn, stop all other movement
            _turnRight = true;
            _turnLeft = false;
        }
        else if (other.tag == "leftTurn") // Hit left turn
        {
            if (_levelManager.infinite)
                _levelManager.addSegment();

            --segs;
            _movingForward = false;

            _rotAround = other.transform.parent.FindChild("rotAroundObj").transform;
            this.transform.parent = _rotAround;

            // Start turn, stop all other movement
            _turnLeft = true;
            _turnRight = false;
        }
        else if (other.tag == "finish") // Hit the finish line!
        {
            _death();
        }
    }

	void OnTriggerStay(Collider other)
	{
		if(other.tag == "hard" && !_warping) // Hit hard object, you dead son!
		{
            // Call the death method
            _death();
		}
        else if(other.tag == "hole") // Player fell through the ground
        {
            this.GetComponent<Rigidbody>().useGravity = true;
            Camera.main.transform.parent = null;

            // Invoke the death method after 1 second
            Invoke("_death", 1);
        }

        // Add speed if player passed 5 segments
        if (segs == 0)
        {
            segs = 5;

            if (_currentSpeed <= _ship.maxSpeed)
            {
                _strafeSpeed += 2f;
                _currentSpeed += 2f;
            }
        }
	}

    private void _death()
    {
        _paused = true;
        _dead = true;
        _gameOver.SetActive(true);
        _levelManager.saveTime();
    }

	/*
	 * 
	 */
	private void _moveForward()
	{
		this.transform.position += this.transform.forward * Time.deltaTime * _currentSpeed;
	}

	private void _listenForMovement()
	{
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			if(_currentLane == 0)
			{
				Debug.Log("Can't move left.");
			}
			else
			{
				--_currentLane;
				_moveLeft = true;
				_changingLane = true;

                // Animate left
                _ship.animator.Play("leftTurn");

				// Add a new game object and position it into the new lane
				_newLane = new GameObject();
				_newLane.transform.position = this.transform.position + (this.transform.right * -_laneDistance);
				_newLane.transform.parent = this.transform;

				Debug.Log("Moved left");
			}
		}

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			if(_currentLane == 6)
			{
				Debug.Log("Can't move right.");
			}
			else
			{
				++_currentLane;
				_moveLeft = false;
				_changingLane = true;

                // Animate right
                _ship.animator.Play("rightTurn");

				// Add a new game object and position it into the new lane
				_newLane = new GameObject();
				_newLane.transform.position = this.transform.position + (this.transform.right * _laneDistance);
				_newLane.transform.parent = this.transform;

				Debug.Log("Moved right");
			}
		}

	}
	
	/**
	 * public void _doMovePlayer ()
	 * 
	 * @description
	 * Moves the player left or right.
	 */
	private void _doChangeLane()
    {
		if(_newLane)
		{
			_newLane.transform.parent = null;
	    	if(_moveLeft)
	    	{
				this.transform.position = Vector3.Lerp(this.transform.position, _newLane.transform.position, _strafeSpeed * Time.deltaTime);
			}
			else
			{
				this.transform.position = Vector3.Lerp(this.transform.position, _newLane.transform.position, _strafeSpeed * Time.deltaTime);
			}
			_newLane.transform.parent = this.transform;

			if(Vector3.Distance(this.transform.position, _newLane.transform.position) <= 0.01f)
			{
				this.transform.position = _newLane.transform.position;
				GameObject.Destroy (_newLane);
				_changingLane = false;

                // Animate idle
                _ship.animator.Play("idle");
			}
		}
    }

    private void _warpDrive()
    {
        if (_warping && !_warpCooldown)
        {
            _warpBar.transform.localScale = new Vector3(
                _warpBar.transform.localScale.x,
                _warpBar.transform.localScale.y,
                Mathf.MoveTowards(_warpBar.transform.localScale.z, 0, Time.deltaTime)
            );

            // Disable warping when completely depleted
            if (_warpBar.transform.localScale.z <= 0)
            {
                _warping = false;
                _warpCooldown = true;
            }
        }
        else
        {
            _warpBar.transform.localScale = new Vector3(
                _warpBar.transform.localScale.x,
                _warpBar.transform.localScale.y,
                Mathf.MoveTowards(_warpBar.transform.localScale.z, _ship.maxWarp, 0.25f * Time.deltaTime)
            );

            if (_warpBar.transform.localScale.z >= _ship.maxWarp && _warpCooldown)
            {
                _warpCooldown = false;
            }
        }
    }
	
	private void _doTurnRight()
	{
		_rotAround.Rotate (_rotAround.up * (_currentSpeed * 4 * Time.deltaTime));
	}
	
	private void _doTurnLeft()
    {
        _rotAround.Rotate(-_rotAround.up * (_currentSpeed * 4 * Time.deltaTime));
	}
	
	private void _reposition(Transform track)
	{
        // Match player to origin of track (exluding y)
        this.transform.rotation = track.rotation;
        this.transform.position = new Vector3(track.position.x, this.transform.position.y, track.position.z);
		
		switch(_currentLane)
		{
		case 0:
            if ((int)this.transform.eulerAngles.y == 0 || (int)this.transform.eulerAngles.y == 360)
                this.transform.position -= new Vector3(4.5f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 90 || (int)this.transform.eulerAngles.y == -270)
                this.transform.position += new Vector3(0, 0, 4.5f);
            else if ((int)this.transform.eulerAngles.y == 180 || (int)this.transform.eulerAngles.y == -180)
                this.transform.position += new Vector3(4.5f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 270 || (int)this.transform.eulerAngles.y == -90)
                this.transform.position -= new Vector3(0, 0, 4.5f);
			break;
        case 1:
            if ((int)this.transform.eulerAngles.y == 0 || (int)this.transform.eulerAngles.y == 360)
                this.transform.position -= new Vector3(3f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 90 || (int)this.transform.eulerAngles.y == -270)
                this.transform.position += new Vector3(0, 0, 3f);
            else if ((int)this.transform.eulerAngles.y == 180 || (int)this.transform.eulerAngles.y == -180)
                this.transform.position += new Vector3(3f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 270 || (int)this.transform.eulerAngles.y == -90)
                this.transform.position -= new Vector3(0, 0, 3f);
			break;
        case 2:
            if ((int)this.transform.eulerAngles.y == 0 || (int)this.transform.eulerAngles.y == 360)
                this.transform.position -= new Vector3(1.5f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 90 || (int)this.transform.eulerAngles.y == -270)
                this.transform.position += new Vector3(0, 0, 1.5f);
            else if ((int)this.transform.eulerAngles.y == 180 || (int)this.transform.eulerAngles.y == -180)
                this.transform.position += new Vector3(1.5f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 270 || (int)this.transform.eulerAngles.y == -90)
                this.transform.position -= new Vector3(0, 0, 1.5f);
			break;
        case 3:
			break;
        case 4:
            if ((int)this.transform.eulerAngles.y == 0 || (int)this.transform.eulerAngles.y == 360)
                this.transform.position += new Vector3(1.5f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 90 || (int)this.transform.eulerAngles.y == -270)
                this.transform.position -= new Vector3(0, 0, 1.5f);
            else if ((int)this.transform.eulerAngles.y == 180 || (int)this.transform.eulerAngles.y == -180)
                this.transform.position -= new Vector3(1.5f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 270 || (int)this.transform.eulerAngles.y == -90)
                this.transform.position += new Vector3(0, 0, 1.5f);
			break;
        case 5:
            if ((int)this.transform.eulerAngles.y == 0 || (int)this.transform.eulerAngles.y == 360)
                this.transform.position += new Vector3(3f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 90 || (int)this.transform.eulerAngles.y == -270)
                this.transform.position -= new Vector3(0, 0, 3f);
            else if ((int)this.transform.eulerAngles.y == 180 || (int)this.transform.eulerAngles.y == -180)
                this.transform.position -= new Vector3(3f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 270 || (int)this.transform.eulerAngles.y == -90)
                this.transform.position += new Vector3(0, 0, 3f);
			break;
        case 6:
            if ((int)this.transform.eulerAngles.y == 0 || (int)this.transform.eulerAngles.y == 360)
                this.transform.position += new Vector3(4.5f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 90 || (int)this.transform.eulerAngles.y == -270)
                this.transform.position -= new Vector3(0, 0, 4.5f);
            else if ((int)this.transform.eulerAngles.y == 180 || (int)this.transform.eulerAngles.y == -180)
                this.transform.position -= new Vector3(4.5f, 0, 0);
            else if ((int)this.transform.eulerAngles.y == 270 || (int)this.transform.eulerAngles.y == -90)
                this.transform.position += new Vector3(0, 0, 4.5f);
			break;
        default:
            _currentLane = 3;
			break;
		}

	}
	
    public void pauseGame()
    {
        if (_paused)
        {
            StartCoroutine("_wait");
            _levelManager.doReturnToGame(); // Start count down timer
        }
        else
        {
            _paused = true;

            children = this.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in children)
            {
                ps.Pause();
            }
        }
    }
}
