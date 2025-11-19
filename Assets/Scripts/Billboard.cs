using Unity.Mathematics;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;
    public bool verticalRotation;
    private Vector3 _tempPosition;


    // Update is called once per frame
    void Update()
    {
        _tempPosition = target.position;
        if (!verticalRotation) _tempPosition.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(_tempPosition - transform.position);
    }
}
