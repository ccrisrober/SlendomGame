using UnityEngine;
using System.Collections;

public class ExitGame : MonoBehaviour {
	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
