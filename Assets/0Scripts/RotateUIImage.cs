using UnityEngine;
using System.Collections;

public class RotateUIImage : MonoBehaviour {
    public int sign;
    public float speed = 80.0f;
	// Use this for initialization
	void Start () {
	
	}

    Vector3 rotationEuler;
    // Update is called once per frame
    void Update()
    {
        rotationEuler += Vector3.forward * speed * Time.deltaTime * sign;
        transform.rotation = Quaternion.Euler(0.0f, 180.0f, rotationEuler.z);
    }
}
