using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ControllerInteraction : NetworkBehaviour {
    public Rigidbody attachPoint;
    public float grabbableRange = 0.05f;

    InteractableObject interactedObject;
    SteamVR_TrackedObject trackedObj;
    FixedJoint joint;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void LateUpdate()
    {
        if (!isLocalPlayer) {
            return;
        }

        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            // Finds all Objects containing the script "InteractableObject" and puts them all in an
            // array of InteractableObjects.
            // NOTE: GameObject must be accessed through the InteractableObject.
            InteractableObject[] ios;
            ios = GameObject.FindObjectsOfType(typeof(InteractableObject)) as InteractableObject[];

            InteractableObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (InteractableObject io in ios)
            {
                Vector3 diff = io.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance && curDistance < grabbableRange)
                {
                    closest = io;
                    distance = curDistance;
                }
            }

            if (closest != null)
            {
                // Flag InteractableObject as active.
                interactedObject = closest;
                interactedObject.beingInteracted = true;

                if (closest is InteractablePhysicalObject)
                { // Create joint and attach to it if closest InteractablePhysicalObject is found.
                    InteractablePhysicalObject ipo = closest as InteractablePhysicalObject;

                    ipo.gameObject.transform.position = attachPoint.transform.position + ipo.grabOffsetPosition;
                    ipo.gameObject.transform.rotation = attachPoint.transform.rotation * Quaternion.Euler(ipo.grabOffsetRotation);

                    joint = ipo.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = attachPoint;
                }
                else if (closest is InteractableTriggerObject)
                { // Trigger the closest found if it is a InteractablePhysicalObject.
                    Debug.Log("Detected InteractableTriggerObject");
                }
            }
        }
        else if (joint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (interactedObject != null) // Deactivate InteractableObject if it is one.
            {
                interactedObject.beingInteracted = false;
                var rigidbody = interactedObject.GetComponent<Rigidbody>();
                Object.DestroyImmediate(joint);
                joint = null;

                float velocityMultiplier = 1.0f;
                float angularVelocityMultiplier = 1.0f;

                if (interactedObject is InteractablePhysicalObject)
                {
                    InteractablePhysicalObject ipo = interactedObject as InteractablePhysicalObject;
                    velocityMultiplier = ipo.throwMultiplier;
                    angularVelocityMultiplier = ipo.angularThrowMultiplier;
                }

                // We should probably apply the offset between trackedObj.transform.position
                // and device.transform.pos to insert into the physics sim at the correct
                // location, however, we would then want to predict ahead the visual representation
                // by the same amount we are predicting our render poses.

                var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
                if (origin != null)
                {
                    rigidbody.velocity = origin.TransformVector(device.velocity) * velocityMultiplier;
                    rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity) * angularVelocityMultiplier;
                }
                else
                {
                    rigidbody.velocity = device.velocity * velocityMultiplier;
                    rigidbody.angularVelocity = device.angularVelocity * angularVelocityMultiplier;
                }

                rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
            }
        }
    }
}
