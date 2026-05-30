using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DaniTech_CameraController : MonoBehaviour
{
    [Header("카메라 이동 설정")]
    [SerializeField] private float _mouseWheelPanSpeed = 2f;
    [SerializeField] private float _touchDragSpeed = 1f;

    [Header("확대 / 축소 설정")]
    [SerializeField] private float _mouseZoomSpeed = 2f;
    [SerializeField] private float _pinchZoomSpeed = 0.01f;
    [SerializeField] private float _minZoomSize = 3f;
    [SerializeField] private float _maxZoomSize = 9f;

    [Header("카메라 이동 제한")]
    [SerializeField] private bool _lockX = true;
    [SerializeField] private float _fixedX = 0f;
    [SerializeField] private float _minY = -3f;
    [SerializeField] private float _maxY = 50f;

    private Camera _camera;
    private Vector3 _lastMouseWorldPosition;
    private bool _isMouseDragging;

    private float _lastTouchDistance;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographic = true;
    }


    private void Update()
    {
        HandleMouseInput();
        HandleTouchInput();
        ClampCameraPosition();

    }

    private void HandleMouseInput()
    {
        float wheelInput = Input.mouseScrollDelta.y;

        if (wheelInput != 0f)
        {
            // Ctrl + 휠 = 확대/축소
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                Zoom(-wheelInput * _mouseZoomSpeed);
            }
            // 그냥 휠 = 상하이동
            else
            {
                Vector3 moveAmount = Vector3.up * (wheelInput * _mouseWheelPanSpeed);
                transform.position += moveAmount;
            }
        }


        // 마우스 드래그로도 카메라 이동 테스트
        if (Input.GetMouseButtonDown(0))
        {
            _isMouseDragging = true;
            _lastMouseWorldPosition = GetMouseWorldPosition();
        }

        if (Input.GetMouseButton(0) && _isMouseDragging)
        {
            Vector3 currentMouseWorldPosition = GetMouseWorldPosition();
            Vector3 delta = _lastMouseWorldPosition - currentMouseWorldPosition;

            transform.position += delta * _touchDragSpeed;
        }

        if(Input.GetMouseButtonUp(0))
        {
            _isMouseDragging = false;
        }


    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 delta = new Vector3(0f, -touch.deltaPosition.y * 0.01f * _touchDragSpeed, 0f);

                transform.position += delta;
            }
        }

        if (Input.touchCount == 2)
        {
            Touch touchA = Input.GetTouch(0);
            Touch touchB = Input.GetTouch(1);

            float currentTouchDistance = Vector2.Distance(touchA.position, touchB.position);

            if (touchA.phase == TouchPhase.Began || touchB.phase == TouchPhase.Began)
            {
                _lastTouchDistance = currentTouchDistance;
                return;
            }

            float distanceDelta = currentTouchDistance - _lastTouchDistance;

            // 두 손가락 사이가 멀어짐 - 확대
            // 두 손가락 사이가 가까워짐 - 축소

            Zoom(-distanceDelta * _pinchZoomSpeed);

            _lastTouchDistance = currentTouchDistance;
        }
    }


    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = - transform.position.z;

        return _camera.ScreenToWorldPoint(mousePosition);
    }

    private void Zoom(float zoomAmount)
    {
        _camera.orthographicSize += zoomAmount;
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, _minZoomSize, _maxZoomSize);
    }




    private void ClampCameraPosition()
    {
        Vector3 position = transform.position;

        if (_lockX)
        {
            position.x = _fixedX;
        }

        position.y = Mathf.Clamp(position.y, _minY, _maxY);
        position.z = -10f;

        transform.position = position;
    }
}
