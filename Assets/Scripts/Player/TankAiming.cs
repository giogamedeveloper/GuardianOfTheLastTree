using UnityEngine;

public class TankAiming : MonoBehaviour
{
    [Header("Aiming")]
    public float camRayLength;

    public LayerMask pointerLayer;
    public Transform aimingPivot;
    public float rotationSpeed = 14f;

    private Camera _mainCamera;

    void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void AimingBehaviour(Vector2 mousePosition)
    {
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit groundHit, camRayLength, pointerLayer))
        {
            Vector3 tempPoint = new Vector3(groundHit.point.x, aimingPivot.position.y, groundHit.point.z);
            Vector3 tempPoint2 = new Vector3(transform.position.x, aimingPivot.position.y, transform.position.z);
            Vector3 dirVector = tempPoint - tempPoint2;
            if (dirVector.magnitude < 2f)
            {
                dirVector.Normalize();
                tempPoint = transform.position + dirVector * 2f;
            }
            tempPoint.y = aimingPivot.position.y;
            aimingPivot.position = tempPoint;
        }
    }

    public void DirectionalAimingBehaviour(Vector3 direction)
    {
        if (direction.magnitude == 0) return;
        Vector3 nextPivotPosition = aimingPivot.position + transform.position + direction.normalized;
        nextPivotPosition.y = aimingPivot.position.y;
        aimingPivot.position = nextPivotPosition;
    }

    public void RotationBehaviour(bool isWalled, bool grounded, bool hasInput)
    {
        if (!hasInput) return;
        Vector3 mouseDirection = aimingPivot.position - transform.position;
        mouseDirection.y = 0;
        if (mouseDirection.magnitude > 0.1f && !isWalled && grounded)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(mouseDirection),
                rotationSpeed * Time.deltaTime);
        }
    }
}
