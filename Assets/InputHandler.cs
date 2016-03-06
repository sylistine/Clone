using UnityEngine;
using System.Collections;

public class InputHandler : AriaBehaviour
{
    public bool moving = false;
    public Vector3 cameraRelativeDeltaPosition;
    
    #region Input stuff.
#if UNITY_EDITOR
    Vector2 currentMousePosition, lastMousePosition, deltaMousePosition;
#endif
    #endregion

    void Start ()
    {

	}

	void Update ()
    {
        moving = false;
        #region PC Input
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                var entity = hit.collider.transform.GetComponent<Entity>();
                if (entity)
                {
                    entity.health -= 50;
                }
            }

            currentMousePosition = lastMousePosition = Input.mousePosition;
            deltaMousePosition = Vector2.zero;
        }
        if (Input.GetMouseButton(0))
        {
            moving = true;
            currentMousePosition = Input.mousePosition;
            deltaMousePosition = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            cameraRelativeDeltaPosition = screenXY2CameraXZ(deltaMousePosition);
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
                moving = true;
                cameraRelativeDeltaPosition = screenXY2CameraXZ(touch.deltaPosition);
            }
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
