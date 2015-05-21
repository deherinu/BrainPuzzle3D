using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour {

	// Camera and foco
	private Transform _camera;
	private GameObject foco;
	//Game object list with puzzle pieces
	private List<GameObject> piece;
	private List<GameObject> matchPiece;
	private List<GameObject> namePieceR;
	private List<GameObject> namePieceW;
	private List<GameObject> leftHemisphere;
	//Array list of audio clips
	private ArrayList clips;	
	//Get position and rotation of the current and matching object
	private Vector3 currentPos;
	private Quaternion currentRot;
	private Vector3 matchPosObj;
	private Quaternion matchRotObj;
	//Delta value for rotation and posicion matching
	private float deltaRot;
	private float deltaPos;
	//Camera position and rotation
	private Vector3 cam_Pos;
	private Quaternion cam_Rot; 
	private Vector3 last_cam_Pos;
	//Auxiliars
	public bool HorizonLock = true;
	private int focoId;
	private GameObject matchObj;
	private int counter;
	//Hand Gameobject
	private GameObject hand;
	public AudioSource sonido;
	private Vector3 newhandPos;
	private bool ejecutar;
	//Time
	private float startTime;
	private float ellapsed;
	//Temporal position
	private Vector3 defPos;


	public void Start () {

		//Pieces of the puzzle
		piece = new List<GameObject>();
		piece.Add (GameObject.Find("Pieces/Subcortical"));
		piece.Add (GameObject.Find("Pieces/CorpusCallosum"));
		piece.Add (GameObject.Find("Pieces/Occipital"));
		piece.Add (GameObject.Find("Pieces/Temporal"));
		piece.Add (GameObject.Find("Pieces/Parietal"));
		piece.Add (GameObject.Find("Pieces/Frontal"));
		piece.Add (GameObject.Find ("Pieces/Cingulate"));
		piece.Add (GameObject.Find("Pieces/RightHemisphere"));
		piece.Add (GameObject.Find("Congratz"));
		foreach (GameObject pieceDeact in piece){
			pieceDeact.SetActive(false);
		}
		//Match pieces of the puzzle
		matchPiece = new List<GameObject>();
		matchPiece.Add (GameObject.Find("Match/MatchRightSubcortical"));
		matchPiece.Add (GameObject.Find("Match/MatchCorpuscallosum"));
		matchPiece.Add (GameObject.Find("Match/MatchSegmentedRightHemisphere/MatchOccipital"));
		matchPiece.Add (GameObject.Find("Match/MatchSegmentedRightHemisphere/MatchTemporal"));
		matchPiece.Add (GameObject.Find("Match/MatchSegmentedRightHemisphere/MatchParietal"));
		matchPiece.Add (GameObject.Find("Match/MatchSegmentedRightHemisphere/MatchFrontal"));
		matchPiece.Add (GameObject.Find("Match/MatchSegmentedRightHemisphere/MatchCingulate"));
		matchPiece.Add (GameObject.Find("Match/MatchFullRightHemisphere"));
		matchPiece.Add (GameObject.Find("Congratz2"));
		//Disable match objects
		foreach (GameObject matchDeac in matchPiece){
			matchDeac.SetActive(false);
		}
		//Pieces names of the puzzle
		namePieceR = new List<GameObject>();
		namePieceR.Add (GameObject.Find("Board/Names/SubR"));
		namePieceR.Add (GameObject.Find("Board/Names/CCR"));
		namePieceR.Add (GameObject.Find("Board/Names/OccR"));
		namePieceR.Add (GameObject.Find("Board/Names/TempR"));
		namePieceR.Add (GameObject.Find("Board/Names/ParR"));
		namePieceR.Add (GameObject.Find("Board/Names/FrontR"));
		namePieceR.Add (GameObject.Find("Board/Names/CinR"));
		namePieceR.Add (GameObject.Find("Board/Names/RHR"));
		namePieceR.Add (GameObject.Find("Board/Names/RHR"));
		//Disable name objects
		foreach (GameObject namesR in namePieceR){
			namesR.SetActive(false);
		}

		//Pieces names of the puzzle
		namePieceW = new List<GameObject>();
		namePieceW.Add (GameObject.Find("Board/Names/SubW"));
		namePieceW.Add (GameObject.Find("Board/Names/CCW"));
		namePieceW.Add (GameObject.Find("Board/Names/OccW"));
		namePieceW.Add (GameObject.Find("Board/Names/TempW"));
		namePieceW.Add (GameObject.Find("Board/Names/ParW"));
		namePieceW.Add (GameObject.Find("Board/Names/FrontW"));
		namePieceW.Add (GameObject.Find("Board/Names/CinW"));
		namePieceW.Add (GameObject.Find("Board/Names/RHW"));
		namePieceW.Add (GameObject.Find("Board/Names/RHW"));

		//Pieces of the left hemisphere
		leftHemisphere = new List<GameObject>();
		leftHemisphere.Add (GameObject.FindWithTag("subLeft"));
		leftHemisphere.Add (GameObject.FindWithTag("ccLeft"));
		leftHemisphere.Add (GameObject.FindWithTag("occLeft"));
		leftHemisphere.Add (GameObject.FindWithTag("temptLeft"));
		leftHemisphere.Add (GameObject.FindWithTag("parLeft"));
		leftHemisphere.Add (GameObject.FindWithTag("frontLeft"));
		leftHemisphere.Add (GameObject.FindWithTag("cingLeft"));

		//List of AudioClips
		clips = new ArrayList ();
		clips.Add ("Bienvenido");
		clips.Add ("Subcorticals");
		clips.Add ("CCallosum");
		clips.Add ("Occipital");
		clips.Add ("Temporal");
		clips.Add ("Parietal");
		clips.Add ("Frontal");
		clips.Add ("Cingular");
		clips.Add ("Hemisferios");
		clips.Add ("Ganaste");

		//Hand game object
		hand = GameObject.Find ("Board/Hand");

		// First piece active focus
		foco = piece[0];
		foco.SetActive (true);

		//Starting object and next object
		matchObj = matchPiece[0];
		matchObj.SetActive (true);

		// Delta for rotation matching
		deltaRot = 0.3f;
		deltaPos = 30f;

		//Set default Camera Position and Rotation
		cam_Pos=new Vector3(0,330,43);
		cam_Rot=new Quaternion(0,0,0,1);
		last_cam_Pos=new Vector3(0,0,-10);

		//Auxiliars
		focoId = 1;
		counter = 0;
		ejecutar = false;

		//Sounds objects, play the introduce audio
		sonido = GameObject.Find ("SoundObject").AddComponent<AudioSource>();
		sonido.clip = Resources.Load (clips[0].ToString()) as AudioClip;
		sonido.Play ();

		//Set default piece position
		defPos = new Vector3 (109, -14, -14);

	}

	//Show the buttons and time
	void OnGUI(){
		// Make the first button. If it is pressed, Application.Loadlevel (0) will be executed
		if(GUI.Button(new Rect(500,40,80,20), "Reiniciar")) {
			Application.LoadLevel(0);
		}
		// Start the game
		if(GUI.Button(new Rect(600,40,80,20), "Comenzar!")) {
			iniciar();
		}
		// Show Time
		GUI.Label (new Rect (800, 40, 80, 30), "<color=white><size=20>"+ ellapsed + " </size></color>");
	}
	
	// Start the game
	public void Update () {
		if (ejecutar == true) {
			run ();
		}
	}

	//Start the game
	public void iniciar(){
		// Initialize time
		startTime = Time.time;
		ejecutar = true;
		//Start first piece sound
		sonido.Stop();
		sonido = GameObject.Find ("SoundObject").AddComponent<AudioSource>();
		sonido.clip = Resources.Load (clips[1].ToString()) as AudioClip;
		sonido.Play ();
	}

	public void run(){

		//Time
		ellapsed = Mathf.RoundToInt(Time.time-startTime);

		//Variable for manipulate Main Camera
		_camera = Camera.main.transform;

		//Space Navigator Controller

		//Set device sensitivity : Translation and Rotation
		SpaceNavigator.SetRotationSensitivity (1);
		SpaceNavigator.SetTranslationSensitivity (2);

		if (HorizonLock) {
			//Translate objects with SpaceNavigator
			foco.transform.Translate(_camera.transform.TransformDirection(SpaceNavigator.Translation), Space.World);

			// Perform yaw in local coordinates
			foco.transform.Rotate(Vector3.forward, SpaceNavigator.Rotation.Yaw() * 10 , Space.Self);
									
			// Perform pitch in local coordinates
			foco.transform.Rotate(Vector3.right, SpaceNavigator.Rotation.Pitch() * 10 , Space.Self);
			
			// Perform roll in local coordinates
			foco.transform.Rotate (Vector3.down,SpaceNavigator.Rotation.Roll() * 10, Space.Self);
		}
		else {
			_camera.transform.RotateAround(foco.transform.position, Vector3.up, SpaceNavigator.Rotation.Yaw () * 10);
			_camera.transform.RotateAround(foco.transform.position, Vector3.right, SpaceNavigator.Rotation.Pitch() * 10);
			_camera.transform.RotateAround(foco.transform.position, Vector3.forward, SpaceNavigator.Rotation.Roll() * 10);	
		}
		//End Space Navigator Controller
		
		//Keyboard Control (camera movement, reset camera, reset object)

		if (Input.GetKeyDown(KeyCode.Tab)) {
			HorizonLock = !HorizonLock;
		}

		if (Input.GetKey (KeyCode.Space)) {
			iTween.MoveTo (_camera.gameObject, iTween.Hash ("position", cam_Pos, "islocal", true, "time", 1));
			_camera.transform.localRotation = cam_Rot;
			_camera.transform.Rotate (90, 0, 0);
		}

		if (Input.GetKey (KeyCode.RightControl)) {
			iTween.MoveTo (foco.gameObject, iTween.Hash ("position", defPos, "islocal", true, "time", 1));
			foco.transform.localRotation = cam_Rot;
			foco.transform.Rotate (0, 0, 0);
		}

		//End Keyboard controller

		// Temporal position and rotation

		currentPos = foco.transform.position;
		currentRot = foco.transform.rotation;
		matchPosObj = matchObj.transform.position;
		matchRotObj = matchObj.transform.rotation;

		// Matching validation
		if (counter <= 7) {
			if (Mathf.Abs(currentRot.x) < Mathf.Abs(matchRotObj.x) + deltaRot && Mathf.Abs(currentRot.x) > Mathf.Abs(matchRotObj.x) - deltaRot && 
			    Mathf.Abs(currentRot.y) < Mathf.Abs(matchRotObj.y) + deltaRot && Mathf.Abs(currentRot.y) > Mathf.Abs(matchRotObj.y) - deltaRot &&
			    Mathf.Abs(currentRot.z) < Mathf.Abs(matchRotObj.z) + deltaRot && Mathf.Abs(currentRot.z) > Mathf.Abs(matchRotObj.z) - deltaRot &&
			    Mathf.Abs(currentPos.x) < Mathf.Abs(matchPosObj.x) + deltaPos && Mathf.Abs(currentPos.x) > Mathf.Abs(matchPosObj.x) - deltaPos && 
			    Mathf.Abs(currentPos.y) < Mathf.Abs(matchPosObj.y) + deltaPos && Mathf.Abs(currentPos.y) > Mathf.Abs(matchPosObj.y) - deltaPos &&
			    Mathf.Abs(currentPos.z) < Mathf.Abs(matchPosObj.z) + deltaPos && Mathf.Abs(currentPos.z) > Mathf.Abs(matchPosObj.z) - deltaPos) {
				// Matching objects (move and rotation)
				foco.transform.position = matchObj.transform.position;
				foco.transform.rotation = matchObj.transform.rotation;
				//Change camera parent
				_camera.transform.parent = null;
				//Change name color's board
				namePieceW[counter].SetActive(false);
				namePieceR[counter].SetActive(true);
				// Set next object
				foco = piece [focoId];
				foco.SetActive (true);
				// Move camera to the new object
				_camera.transform.parent = foco.transform;
				iTween.MoveTo (_camera.gameObject, iTween.Hash ("position", cam_Pos, "islocal", true, "time", 1));
				_camera.transform.localRotation = cam_Rot;
				_camera.transform.Rotate (90,0,0);
				// Play the next sound
				sonido.clip = Resources.Load (clips[counter+2].ToString()) as AudioClip;
				sonido.Play ();
				//Set new match object
				matchObj = matchPiece [focoId];
				matchObj.SetActive (true);
				//Change hand board position
				hand.transform.position = new Vector3 (hand.transform.position.x, namePieceW[counter].transform.position.y - 700, hand.transform.position.z);
				// Increase auxiliars
				focoId++;
				counter++;
			}
		}	// Game finished
			else if (counter == 8){
			// Move the camera to the last rotation
			_camera.transform.parent = null;
			_camera.transform.parent = foco.transform;
			iTween.MoveTo (_camera.gameObject, iTween.Hash ("position", last_cam_Pos, "islocal", true, "time", 1));
			_camera.transform.localRotation = cam_Rot;
			//Stop and play the last audio
			sonido.Stop();
			sonido.clip = Resources.Load (clips[9].ToString()) as AudioClip;
			sonido.Play ();
			hand.transform.position = newhandPos;
			//Auxiliar
			counter++;
			// Stop timer
			//Time.timeScale = 0.0f;
		}
	}
	}



