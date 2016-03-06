using UnityEngine;
using System.Collections;

public class MovementController : AriaBehaviour
{
    public Projector projector;

    public Vector2 _cameraXZPosition;
    private Vector2 cameraXZPosition;
    public float cameraPitch;
    public float cameraDistance;

    private Vector3 playerOffset;
    public float playerOffsetWalkDist;
    public float playerOffsetRunDist;
    public float playerOffsetMaxDist;

    public float playerWalkSpeed;
    public float playerRunSpeed;
    private bool dashing = false;

    private InputHandler input;

    #region Camera stuff.
    Transform mainCamera;
    // Used by CalculateTargetCameraPosition() every frame.
    Vector3 relativeCameraPosition;
    float relativeCameraXZMagnitude;
    Vector3 relativeTargetCameraPosition;
    #endregion

    #region Animation stuff.
    Animator animator;
    float animatorMoveSpeed;
    #endregion

    void Start ()
    {
        input = this.GetComponent<InputHandler>();
        projector.enabled = false;

        mainCamera = Camera.main.transform;
        animator = this.GetComponentInChildren<Animator>();

        // Set Default Camera Position
        float cameraPositionY = Mathf.Sin(cameraPitch * Mathf.Deg2Rad) * cameraDistance;
        cameraXZPosition = _cameraXZPosition.normalized * Mathf.Cos(cameraPitch * Mathf.Deg2Rad) * cameraDistance;
        Vector3 cameraPosition = new Vector3(cameraXZPosition.x, cameraPositionY, cameraXZPosition.y);
        mainCamera.position = cameraPosition - this.transform.position - playerOffset;
        mainCamera.LookAt(this.transform.position - playerOffset);
    }
	
	void Update ()
    {
        if(input.moving)
        {
            playerOffset.x = playerOffset.x - input.cameraRelativeDeltaPosition.x;
            playerOffset.z = playerOffset.z - input.cameraRelativeDeltaPosition.z;
        }

        animatorMoveSpeed = 0;
        float moveSpeed;

        if (playerOffset.magnitude > playerOffsetWalkDist)
        {
            if (playerOffset.magnitude > playerOffsetRunDist) dashing = true;
            moveSpeed = (dashing || playerOffset.magnitude > playerOffsetRunDist) ? playerRunSpeed : playerWalkSpeed;
            this.transform.rotation = Quaternion.LookRotation(playerOffset, Vector3.up);
            Vector3 moveDist = this.transform.TransformDirection(Vector3.forward) * Time.deltaTime * moveSpeed;
            this.transform.position += moveDist;
            playerOffset -= moveDist;
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

        if(projector != null)
        {
            projector.enabled = animatorMoveSpeed > 0 ? true : false;
        }

        if (playerOffset.magnitude > playerOffsetMaxDist)
        {
            playerOffset = playerOffset.normalized * playerOffsetMaxDist;
        }

        UpdateCamera ();
    }

    void UpdateCamera ()
    {
        float cameraPositionY = Mathf.Sin(cameraPitch * Mathf.Deg2Rad) * cameraDistance;
        cameraXZPosition = _cameraXZPosition.normalized * Mathf.Cos(cameraPitch * Mathf.Deg2Rad) * cameraDistance;
        Vector3 cameraPosition = new Vector3(cameraXZPosition.x, cameraPositionY, cameraXZPosition.y);
        mainCamera.position = cameraPosition + this.transform.position + playerOffset;
        mainCamera.LookAt(this.transform.position + playerOffset);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(playerOffset + this.transform.position, 1);
    }
}
