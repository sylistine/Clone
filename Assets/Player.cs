using UnityEngine;
using System.Collections;

public class Player : AriaBehaviour
{
    public float cameraAngle;
    public float minCameraAngle;
    public float maxCameraAngle;
    public float currentCameraAngle;
    private float cameraAngleDampVelocity;
    public float cameraDistance;

    private Vector3 lookOffset;
    public float lookOffsetWalkThreshold;
    public float lookOffsetRunThreshold;
    public float maxLookOffsetMagnitude;

    public float playerWalkSpeed;
    public float playerRunSpeed;
    private bool dashing = false;

    #region Camera stuff.
    Camera cam;
    // Used by MoveCamera() every frame.
    Vector3 targetCameraPosition;
    Vector3 cameraVelocity = Vector3.zero;
    // Used by CalculateTargetCameraPosition() every frame.
    Vector3 relativeCameraPosition;
    float relativeCameraXZMagnitude;
    Vector3 relativeTargetCameraPosition;
    #endregion

    #region Input stuff.
    private bool preventCameraAutoAdjust;
#if UNITY_EDITOR
    Vector2 currentMousePosition, lastMousePosition, deltaMousePosition;
#endif
    #endregion

    #region Animation stuff.
    Animator animator;
    float animatorMoveSpeed;
    #endregion

    void Start ()
    {
        cam = Camera.main;
        animator = this.GetComponentInChildren<Animator>();
        
        if (maxCameraAngle < cameraAngle) maxCameraAngle = cameraAngle;
        if (minCameraAngle > cameraAngle) minCameraAngle = cameraAngle;
        currentCameraAngle = cameraAngle;
    }
	
	void Update ()
    {
        preventCameraAutoAdjust = false;

        #region PC Input
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            currentMousePosition = lastMousePosition = Input.mousePosition;
            deltaMousePosition = Vector2.zero;
        }
        if (Input.GetMouseButton(0))
        {
            currentMousePosition = Input.mousePosition;
            deltaMousePosition = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            Vector3 cameraRelativeDeltaPosition = screenXY2CameraXZ(deltaMousePosition);
            lookOffset.x = lookOffset.x - cameraRelativeDeltaPosition.x;
            lookOffset.z = lookOffset.z - cameraRelativeDeltaPosition.z;
        }
#endif
        #endregion

        #region Mobile Input
#if (UNITY_IPHONE || UNITY_ANDROID)
        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 cameraRelativeDeltaPosition = screenXY2CameraXZ(touch.deltaPosition);
                lookOffset.x = lookOffset.x - cameraRelativeDeltaPosition.x;
                lookOffset.z = lookOffset.z - cameraRelativeDeltaPosition.z;
            }

            if(Input.touchCount > 1)
            {
                preventCameraAutoAdjust = true;
                Touch camTouch = Input.GetTouch(1);
                if (camTouch.phase == TouchPhase.Moved)
                {
                    Vector3 relativeCameraPosition = cam.transform.position - this.transform.position;
                    Vector3 relativeCameraXZ = new Vector3(relativeCameraPosition.x, 0, relativeCameraPosition.z);
                    float relativeCameraXZMagnitude = relativeCameraXZ.magnitude;
                    float relativeCameraYaw = Mathf.Atan2(relativeCameraPosition.x, relativeCameraPosition.z) * Mathf.Rad2Deg;

                    relativeCameraYaw -= camTouch.deltaPosition.x * 0.75f;
                    relativeCameraPosition.x = Mathf.Sin(relativeCameraYaw * Mathf.Deg2Rad) * relativeCameraXZMagnitude;
                    relativeCameraPosition.z = Mathf.Cos(relativeCameraYaw * Mathf.Deg2Rad) * relativeCameraXZMagnitude;

                    if ((camTouch.deltaPosition.y > 0 && currentCameraAngle < maxCameraAngle) ||
                        (camTouch.deltaPosition.y < 0 && currentCameraAngle > minCameraAngle))
                    {
                        currentCameraAngle += camTouch.deltaPosition.y * 0.75f;
                    }
                    cam.transform.position = relativeCameraPosition + this.transform.position;
                }
            }
        }
#endif
        #endregion

        if(!preventCameraAutoAdjust && Mathf.Abs(currentCameraAngle - cameraAngle) > 0.01f)
        {
            currentCameraAngle = Mathf.SmoothDamp(currentCameraAngle, cameraAngle, ref cameraAngleDampVelocity, 0.5f);
        }

        animatorMoveSpeed = 0;
        float lookOffsetMagnitude = lookOffset.magnitude;
        float moveSpeed;

        if (lookOffsetMagnitude > lookOffsetWalkThreshold)
        {
            if (lookOffsetMagnitude > lookOffsetRunThreshold) dashing = true;
            moveSpeed = (dashing || lookOffsetMagnitude > lookOffsetRunThreshold) ? playerRunSpeed : playerWalkSpeed;
            this.transform.rotation = Quaternion.LookRotation(lookOffset, Vector3.up);
            Vector3 moveDist = this.transform.TransformDirection(Vector3.forward) * Time.deltaTime * moveSpeed;
            this.transform.position += moveDist;
            lookOffset -= moveDist;
            animatorMoveSpeed = moveSpeed;
        }
        else
        {
            dashing = false;
        }
        if(animator != null)
        {
            animator.SetFloat("Forward Speed", animatorMoveSpeed);
        }

        if (lookOffset.magnitude > maxLookOffsetMagnitude)
        {
            lookOffset = lookOffset.normalized * maxLookOffsetMagnitude;
        }

        UpdateCamera ();
	}

    Vector3 screenXY2CameraXZ(Vector2 touchDeltaPos)
    {
        Vector3 cameraXZ = Vector3.zero;
        touchDeltaPos *= 0.05f;

        Vector3 camRight = cam.transform.TransformDirection(Vector3.right);
        Vector3 camForward = new Vector3(-camRight.z, 0, camRight.x);

        cameraXZ += touchDeltaPos.x * camRight;
        cameraXZ += touchDeltaPos.y * camForward;

        return cameraXZ;
    }

    void UpdateCamera ()
    {
        Vector3 relativeCameraPosition = cam.transform.position - (this.transform.position + lookOffset);
        Vector3 currentCameraXZ = new Vector3(relativeCameraPosition.x, 0, relativeCameraPosition.z);
        float currentCameraPitch = Mathf.Atan2(relativeCameraPosition.y, currentCameraXZ.magnitude) * Mathf.Rad2Deg;
        Vector3 targetCameraPosition = TargetCameraPosition();

        // while (currentCameraPitch > maxCameraAngle)
        while (false)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetCameraPosition, ref cameraVelocity, 0.5f);
            relativeCameraPosition = cam.transform.position - (this.transform.position + lookOffset);
            currentCameraXZ = new Vector3(relativeCameraPosition.x, 0, relativeCameraPosition.z);
            currentCameraPitch = Mathf.Atan2(relativeCameraPosition.y, currentCameraXZ.magnitude) * Mathf.Rad2Deg;
        }
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetCameraPosition, ref cameraVelocity, 0.5f);
        cam.transform.LookAt(this.transform.position + lookOffset);
    }

    Vector3 TargetCameraPosition ()
    {
        // Get player-relative x and z camera position and the magnitude of that vector.
        relativeCameraPosition = cam.transform.position - (this.transform.position + lookOffset);
        relativeCameraPosition.y = 0;
        relativeCameraXZMagnitude = relativeCameraPosition.magnitude;

        // Create new relative target camera position with this data.
        relativeTargetCameraPosition = new Vector3(
            relativeCameraPosition.x,
            Mathf.Tan(Mathf.Deg2Rad * currentCameraAngle) * relativeCameraXZMagnitude,
            relativeCameraPosition.z);
        // Scale to desired zoom level.
        relativeTargetCameraPosition = relativeTargetCameraPosition.normalized * cameraDistance;
        
        return this.transform.position + lookOffset + relativeTargetCameraPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lookOffset + this.transform.position, 1);
    }
}
