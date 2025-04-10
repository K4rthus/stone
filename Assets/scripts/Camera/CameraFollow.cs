using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float minXBoundary;
    private Vector3 _initialPosition;
    private Camera _camera;
    private float _cameraHalfWidth;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _initialPosition = transform.position;

        float aspectRatio = _camera.aspect;
        _cameraHalfWidth = _camera.orthographicSize * aspectRatio;
    }

    void LateUpdate()
    {
        if (_target == null) return;

        float cameraMinX = minXBoundary + _cameraHalfWidth;
        float targetX = _target.position.x;

        float clampedX = Mathf.Max(targetX, cameraMinX);

        transform.position = new Vector3(
            clampedX,
            _initialPosition.y,
            _initialPosition.z
        );
    }
}