using UnityEngine;
using System.Collections;

public enum transformType
{
    slerp,lerp,moveto
}

public class TranslateToPosition : MonoBehaviour {
    public float delayInSeconds = 2;
    public float speed = 0.5f;
    public transformType transformType;
    public GameObject[] targets;
    public bool loop = true;
    public float distanceThreshold = 1.0f;
    private Vector3 initialPosition;
    private bool updateOn = false;
    private int targetIndex = 0;
    

	// Use this for initialization
	void Start () {
        StartCoroutine(TranslateObject());
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (updateOn && targets.Length > 0)
        {
            float step = speed * Time.deltaTime;
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = targets[targetIndex].transform.position;

            // GameObject reaches current target position
            
            if (currentPosition == targetPosition || Vector3.SqrMagnitude(currentPosition - targetPosition) < distanceThreshold)
            {
                ++targetIndex;

                // Reset target to first target stored and teleport object to initial position
                if (loop && targetIndex >= targets.Length)
                {
                    targetIndex = 0;
                    currentPosition = initialPosition;
                    transform.position = currentPosition;
                    transform.LookAt(targets[targetIndex].transform);
                    targetPosition = targets[targetIndex].transform.position;
                    StartCoroutine(TranslateObject());
                } else
                {
                    StopCoroutine(TranslateObject());
                    updateOn = false;
                }
            }

            
            switch(transformType)
            {
                case transformType.slerp:
                    transform.position = Vector3.Slerp(transform.position, targetPosition, step);
                    break;
                case transformType.lerp:
                    transform.position = Vector3.Lerp(transform.position, targetPosition, step);
                    break;
                case transformType.moveto:
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
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
