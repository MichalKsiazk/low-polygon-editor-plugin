using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour {

	public float day_duration;
	public float current_time;
	[HideInInspector]
	public float game_speed;
	public bool game_paused;

	[SerializeField]
	[Range(0f,10f)]
	float game_speed_regulator;


	Calendar calendar;

	void Start () {
		calendar = GameObject.FindGameObjectWithTag ("Calendar").GetComponent<Calendar>();
	}
		

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Debug.Log ("space pressed");
			GameObject.Find("AmbientLighting").GetComponent<AmbientLighting>().SetToCurrentDayTime ();
		}

		if (game_paused) {
			game_speed = 0;
		} else {
			game_speed = game_speed_regulator;
		}
			current_time += Time.deltaTime * game_speed;
			if (current_time > day_duration) {
				current_time -= day_duration;
				calendar.NextDay (1);
			}
	}
}
