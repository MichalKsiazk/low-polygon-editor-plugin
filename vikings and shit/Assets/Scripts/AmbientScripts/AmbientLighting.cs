using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientLighting : MonoBehaviour {

	public GameObject sun;
	public GameObject moon;

	public GameTime game_time;

	[Range(0f,40f)]
	public float sun_angle_factor;

	float x_distance = 0;


	void Start () {
		game_time = GameObject.Find ("GameTime").GetComponent<GameTime>();
		SetToCurrentDayTime ();
		x_distance += CurrentAngle () * Mathf.Deg2Rad;
	}
		
	void FixedUpdate() {

		float angle = (Time.fixedDeltaTime * 100) / game_time.day_duration * 3.6f * game_time.game_speed;


		if (sun.transform.position.y > 0) {
			x_distance += (angle * 2) * Mathf.Deg2Rad * game_time.game_speed;
		} else {
			x_distance = 0;
		}
		//Debug.Log (x_distance * Mathf.Rad2Deg);
		RotateAround (angle);
	}
		

	void RotateAround(float angle_forward){
		sun.transform.RotateAround (Vector3.zero, Vector3.forward, angle_forward);
		sun.transform.LookAt (Vector3.zero);

		Vector3 last_time_pos = sun.transform.position;

		if (sun.transform.position.y > 0) {
			float distance = Mathf.Sqrt(Mathf.Pow(10000, 2) - Mathf.Pow(sun.transform.position.x, 2)) * sun_angle_factor;
			sun.transform.position = new Vector3 (sun.transform.position.x, sun.transform.position.y, distance);
			Debug.Log (Vector3.Distance (last_time_pos, sun.transform.position));
		} else {
			sun.transform.position = new Vector3 (sun.transform.position.x, sun.transform.position.y, 0);
		}

		moon.transform.RotateAround (Vector3.zero, Vector3.forward, angle_forward);
		moon.transform.LookAt (Vector3.zero);
	}


	public void SetToCurrentDayTime() {
		float angle = CurrentAngle ();
		sun.transform.position = new Vector3 (0, -10000, 0);
		moon.transform.position = new Vector3 (0, 400, 0);
		RotateAround (angle);
	}

	public float CurrentAngle() {
		return (game_time.current_time * 100) / game_time.day_duration * 3.6f;
	}

}
