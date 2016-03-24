using UnityEngine;
using System.Collections;

public class InputHandler : AriaBehaviour
{
    private float touchDuration = 0;
    public Vector3 cameraRelativeDeltaPosition;
    public float touchSensitivity;
    public float tapSpeed;
    public bool move = false;
    public bool tap = false;

    #region Input stuff.
#if UNITY_EDITOR
    Vector2 originMousePosition;
    Vector2 currentMousePosition;
    Vector2 lastMousePosition;
    Vector2 deltaMousePosition;
#endif

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
    Vector2 originTouchPosition;
#endif
    #endregion

    void Start ()
    {

	}

	void Update ()
    {
        #region PC Input
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            touchDuration = 0f;
            originMousePosition = currentMousePosition = lastMousePosition = Input.mousePosition;
            deltaMousePosition = Vector2.zero;
        }
        else if (Input.GetMouseButton(0))
        {
            touchDuration += Time.deltaTime;

            currentMousePosition = Input.mousePosition;
            deltaMousePosition = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            cameraRelativeDeltaPosition = screenXY2CameraXZ(deltaMousePosition);

            if(move == false && Vector2.Distance(currentMousePosition, originMousePosition) >= touchSensitivity)
            {
                move = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(move == false && touchDuration <= tapSpeed)
            {
                tap = true;
            }
        }
        else
        {
            tap = false;
            move = false;
            touchDuration = 0f;
        }
#endif
        #endregion

        #region Mobile Input
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR 
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchDuration = 0f;
                originTouchPosition = touch.position;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                touchDuration += Time.deltaTime;
                cameraRelativeDeltaPosition = screenXY2CameraXZ(touch.deltaPosition);

                if (move == false && Vector2.Distance(touch.position, originTouchPosition) >= touchSensitivity)
                {
                    move = true;
                }
            }
            if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
            {
                if (move == false && touchDuration <= tapSpeed)
                {
                    tap = true;
                }
            }
        }
        else
        {
            tap = false;
            move = false;
            touchDuration = 0f;
        }
#endif
        #endregion
    }

    Vector3 screenXY2CameraXZ(Vector2 deltaPos)
    {
        Vector3 cameraXZ = Vector3.zero;
        deltaPos *= 0.05f;

        Vector3 camRight = Camera.main.transform.TransformDirection(Vector3.right);
        Vector3 camForward = new Vector3(-camRight.z, 0, camRight.x);

        cameraXZ += deltaPos.x * camRight;
        cameraXZ += deltaPos.y * camForward;

        return cameraXZ;
    }
}
