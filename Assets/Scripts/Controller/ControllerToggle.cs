using UnityEngine;
using System.Collections;
using Wacki;

public class ControllerToggle : MonoBehaviour {
    private ViveUILaserPointer scriptLaserPointer;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    
    private SteamVR_Controller.Device device { get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    private SteamVR_TrackedObject trackedObject;

    public GameObject menuPrefab;
    bool dPadPressed = false;
    bool laserOn;
    bool laserNotInitialized;
    bool turnMenuOn;
    bool menuOn;
    Object newMenu;

    // Use this for initialization
    void Start () {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        laserNotInitialized = true;
        laserOn = false;
        menuOn = true;
	}
	
	// Update is called once per frame
	void Update () {
        scriptLaserPointer = GetComponent<ViveUILaserPointer>();
        // List of Buttons to Booleans
        if (device.GetPressDown(gripButton) && laserOn == false)
        {
            Transform t = trackedObject.gameObject.transform;
            Vector3 euler = t.rotation.eulerAngles;
            euler.z = 0;
            euler.x = 0;
            t.rotation = Quaternion.Euler(euler);

            Vector3 pos = new Vector3(t.forward.x*3, t.forward.y + 1.5f, t.forward.z*3);
            newMenu = Instantiate(menuPrefab, pos, t.rotation);
            laserOn = true;
            trackedObject.gameObject.AddComponent<ViveUILaserPointer>();  
        }
        else if (device.GetPressDown(gripButton) && laserOn == true)
        {
            laserOn = false;
            Destroy(newMenu);
            Destroy(scriptLaserPointer.hitPoint);
            Destroy(scriptLaserPointer.pointer);
            Destroy(GetComponent<ViveUILaserPointer>());
        }

    }
}
