using UnityEngine;
using System.Collections;

public class Flashlight : MonoBehaviour {
    [SerializeField]
    protected bool flashLightOn = false;
    [SerializeField]
    protected Light l;

    [SerializeField]
    protected float maxEnergy = 100.0f;
    [SerializeField]
    protected float drainSpeed = 2.0f;

    protected float oldIntensity;
    protected float maxIntensity;

	// Use this for initialization
	void Start () {
        oldIntensity = l.intensity;
        maxIntensity = l.intensity;
    }
	void ToogleFlashLight()
    {
        if (Input.GetKeyDown(KeyCode.F) && !flashLightOn)
        {
            flashLightOn = true;
            l.intensity = oldIntensity;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F) && flashLightOn)
            {
                flashLightOn = false;
                l.intensity = 0;
            }
        }
    }
	// Update is called once per frame
	void Update () {
        ToogleFlashLight();
        if (flashLightOn)
        {
            if (this.maxEnergy > 0.0f)
            {
                this.maxEnergy -= Time.deltaTime * this.drainSpeed;
                l.intensity = this.maxEnergy * maxEnergy / 100.0f;
            }
        }
    }
}
