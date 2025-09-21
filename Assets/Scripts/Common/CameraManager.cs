using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 相机管理器类，用于相机移动逻辑
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera;

        [SerializeField] private float scrollSpeed = 10f;
        [SerializeField] private float edgeThreshold = 10f;
        [SerializeField] private float zoomSpeed = 20f;

        [Header("缩放设置")] [SerializeField] private float minZoomDistance = 5f;
        [SerializeField] private float maxZoomDistance = 50f;

        [Header("鼠标中键拖拽设置")] [SerializeField] private float dragSpeed = 2f;

        private Vector3 zoomVelocity = Vector3.zero;
        private Vector3 edgeMoveVelocity = Vector3.zero;
        private Vector3 dragVelocity = Vector3.zero;

        private Vector3 lastMousePosition;
        private bool isDragging = false;

        private float screenWidth;
        private float screenHeight;


        private void Awake()
        {
            // 缓存屏幕尺寸
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        /// <summary>
        /// 每帧更新方法，处理相机移动逻辑
        /// </summary>
        private void Update()
        {
            HandleEdgeMovement();
            HandleZoom();
            HandleMouseDrag();
        }

        /// <summary>
        /// 鼠标放在屏幕边缘控制镜头移动
        /// </summary>
        private void HandleEdgeMovement()
        {
            if (isDragging)
                return;
            
            Vector3 targetPosition = mainCamera.transform.position;
            Vector3 mousePosition = Input.mousePosition;
            Vector3 flatForward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;

            // 检测鼠标是否接近屏幕边缘，实现边缘滚动效果
            if (mousePosition.x > screenWidth - edgeThreshold)
                targetPosition += mainCamera.transform.right * (scrollSpeed * Time.deltaTime);

            if (mousePosition.x < edgeThreshold)
                targetPosition -= mainCamera.transform.right * (scrollSpeed * Time.deltaTime);

            if (mousePosition.y > screenHeight - edgeThreshold)
                targetPosition += flatForward * (scrollSpeed * Time.deltaTime);

            if (mousePosition.y < edgeThreshold)
                targetPosition -= flatForward * (scrollSpeed * Time.deltaTime);

            // 使用SmoothDamp实现平滑移动
            mainCamera.transform.position = Vector3.SmoothDamp(
                mainCamera.transform.position,
                targetPosition,
                ref edgeMoveVelocity,
                0.1f
            );
        }

        /// <summary>
        /// 鼠标滚轮控制缩放
        /// </summary>
        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                // 沿相机朝向进行缩放
                Vector3 zoomDirection = mainCamera.transform.forward * (scroll * zoomSpeed);
                Vector3 targetPosition = mainCamera.transform.position + zoomDirection;

                // 限制Y轴范围（基于世界坐标）
                targetPosition.y = Mathf.Clamp(targetPosition.y, minZoomDistance, maxZoomDistance);

                // 使用SmoothDamp实现平滑缩放
                mainCamera.transform.position = Vector3.SmoothDamp(
                    mainCamera.transform.position,
                    targetPosition,
                    ref zoomVelocity,
                    0.1f
                );
            }
        }

        /// <summary>
        /// 鼠标中键拖拽控制相机移动
        /// </summary>
        private void HandleMouseDrag()
        {
            // 检测鼠标中键按下
            if (Input.GetMouseButtonDown(2))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
                return;
            }

            // 检测鼠标中键释放
            if (Input.GetMouseButtonUp(2))
            {
                isDragging = false;
                return;
            }

            // 处理拖拽逻辑
            if (isDragging)
            {
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 deltaMousePosition = currentMousePosition - lastMousePosition;

                // 计算拖拽移动（反转鼠标移动方向以匹配直观操作）
                Vector3 rightMovement =
                    mainCamera.transform.right * (-deltaMousePosition.x * dragSpeed * Time.deltaTime);
                Vector3 forwardMovement = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized *
                                          (-deltaMousePosition.y * dragSpeed * Time.deltaTime);

                // 只在水平面上移动
                rightMovement.y = 0;
                forwardMovement.y = 0;

                Vector3 targetPosition = mainCamera.transform.position + rightMovement + forwardMovement;

                // 使用SmoothDamp实现平滑拖拽
                mainCamera.transform.position = Vector3.SmoothDamp(
                    mainCamera.transform.position,
                    targetPosition,
                    ref dragVelocity,
                    0.1f
                );

                lastMousePosition = currentMousePosition;
            }
        }
    }
}
