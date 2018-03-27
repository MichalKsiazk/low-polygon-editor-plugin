using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : MonoBehaviour {

	public int year_lenght;

	public int current_year;
	public int current_day;
	public Season current_season;

	[SerializeField]
	public Season[] seasons;

	void Start () {
		ChangeSeason ();
	}

	void Update () {
		
	}


	public void NextDay(int days_elapsed){
		current_day += days_elapsed;
		if (current_day > year_lenght) {
			current_year++;
			current_day -= year_lenght;
		}
		ChangeSeason ();
	}

	void ChangeSeason() {
		
		float in_year_percent = (current_day == 0) ? 0 : (float)current_day / (float)year_lenght;
		foreach (Season s in seasons) {
			if (in_year_percent >= s.occurrence && in_year_percent < s.occurrence + s.duration) {
				current_season = s;
			}
		}
	}


}

[System.Serializable]
public class Season {
	public enum season_name
	{
		spring, summer, autumn, winter
	}
	public season_name name;
	public float duration;
	public float occurrence;

}