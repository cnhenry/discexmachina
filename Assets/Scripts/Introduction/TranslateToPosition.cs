using UnityEngine;
using System.Collections;

public enum transformType
{
    slerp,lerp,moveto
}

public class TranslateToPosition : MonoBehaviour {
    public float delayInSeconds = 2;
    public float speed = 0.5f;
    public GameObject finalPositionLocation;
    public transformType transformType;
    bool updateOn = false;

	// Use this for initialization
	void Start () {
        StartCoroutine(TranslateObject());
    }

    // Update is called once per frame
    void Update () {
        if (updateOn)
        {
            float step = speed * Time.deltaTime;
            Vector3 finalPosition = finalPositionLocation.transform.position;

            switch(transformType)
            {
                case transformType.slerp:
                    transform.position = Vector3.Slerp(transform.position, finalPosition, step);
                    break;
                case transformType.lerp:
                    transform.position = Vector3.Lerp(transform.position, finalPosition, step);
                    break;
                case transformType.moveto:
                    transform.position = Vector3.MoveTowards(transform.position, finalPosition, step);
                    break;
                default:
                    Debug.Log("Shouldn't have been here...");
                    break;
            }
        }
        
    }

    IEnumerator TranslateObject()
    {
        yield return new WaitForSeconds(delayInSeconds);
        updateOn = true;
    }
}
