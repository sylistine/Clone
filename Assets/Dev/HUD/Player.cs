using UnityEngine;
using System.Collections;

public class Player : AriaBehaviour
{
    public float cameraAngle;
    public float cameraDistance;

    public Vector3 playerOffset;
    public float maxOffsetMagnitude;
    public float offsetMagnitudeThreshold;

    Camera cam;
    // Used by MoveCamera() every frame.
    Vector3 targetCameraPosition;
    Vector3 cameraVelocity = Vector3.zero;
    // Used by CalculateTargetCameraPosition() every frame.
    Vector3 relativeCameraPosition;
    float relativeCameraXZMagnitude;
    Vector3 relativeTargetCameraPosition;

    bool handlingInput = false;

	void Start ()
    {
        cam = Camera.main;
	}
	
	void Update ()
    {
        if(!handlingInput)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                handlingInput = true;
                StartCoroutine(HandleTouch());
            }
            else if (Input.GetMouseButtonDown(0))
            {
                handlingInput = true;
                Debug.Log("Click!");
                StartCoroutine(HandleClick());
            }

        if (playerOffset.magnitude > offsetMagnitudeThreshold)
        {
            this.transform.rotation = Quaternion.LookRotation(playerOffset, Vector3.up);
            Vector3 moveDist = this.transform.TransformDirection(Vector3.forward) * Time.deltaTime * 5f;
            this.transform.position += moveDist;
            playerOffset -= moveDist;
        }
        if (playerOffset.magnitude > maxOffsetMagnitude)
        {
            playerOffset = playerOffset.normalized * maxOffsetMagnitude;
        }

        UpdateCamera ();
	}

    IEnumerator HandleClick()
    {
        Debug.Log("HandleClick coroutine entered");
        Vector2 currentMousePosition, lastMousePosition, deltaMousePosition;
        currentMousePosition = lastMousePosition = Input.mousePosition;
        deltaMousePosition = Vector2.zero;

        while(Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            deltaMousePosition = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;
            
            Vector3 cameraRelativeDeltaPosition = screenXY2CameraXZ(deltaMousePosition);
            playerOffset.x = playerOffset.x - cameraRelativeDeltaPosition.x;
            playerOffset.z = playerOffset.z - cameraRelativeDeltaPosition.z;
            
            yield return new WaitForEndOfFrame();
            currentMousePosition = Input.mousePosition;
        }

        handlingInput = false;
        yield return 0;
    }
    IEnumerator HandleTouch()
    {
        Touch touch = Input.GetTouch(0);

        while (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
        {
            if(touch.phase == TouchPhase.Moved)
            {
                Vector3 cameraRelativeDeltaPosition = screenXY2CameraXZ(touch.deltaPosition);
                playerOffset.x = playerOffset.x - cameraRelativeDeltaPosition.x;
                playerOffset.z = playerOffset.z - cameraRelativeDeltaPosition.z;
            }

            yield return new WaitForEndOfFrame();
            touch = Input.GetTouch(0);
        }

        handlingInput = false;
        yield return 0;
    }
    Vector3 screenXY2CameraXZ(Vector2 touchDeltaPos)
    {
        Vector3 cameraXZ = Vector3.zero;
        touchDeltaPos *= 0.05f;

        Vector3 camRight = cam.transform.TransformDirection(Vector3.right);
        Vector3 camForward = new Vector3(-camRight.z, 0, camRight.x);
        Debug.Log(camForward);

        cameraXZ += touchDeltaPos.x * camRight;
        cameraXZ += touchDeltaPos.y * camForward;

        return cameraXZ;
    }

    void UpdateCamera ()
    {
        targetCameraPosition = CalculateTargetCameraPosition();

        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetCameraPosition, ref cameraVelocity, 0.5f);
        cam.transform.LookAt(this.transform.position + playerOffset);
    }

    Vector3 CalculateTargetCameraPosition ()
    {
        // Get player-relative x and z camera position and the magnitude of that vector.
        relativeCameraPosition = cam.transform.position - (this.transform.position + playerOffset);
        relativeCameraPosition.y = 0;
        relativeCameraXZMagnitude = relativeCameraPosition.magnitude;

        // Create new relative target camera position with this data.
        relativeTargetCameraPosition = new Vector3(
            relativeCameraPosition.x,
            Mathf.Tan(Mathf.Deg2Rad * cameraAngle) * relativeCameraXZMagnitude,
            relativeCameraPosition.z);
        // Scale to desired zoom level.
        relativeTargetCameraPosition = relativeTargetCameraPosition.normalized * cameraDistance;
        
        return this.transform.position + playerOffset + relativeTargetCameraPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(playerOffset + this.transform.position, 1);
    }
}
