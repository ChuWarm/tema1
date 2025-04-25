using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private float cameraSpeed = 5f;
    private Transform _targetPlayer;
    private CameraObstacleHandler _obstacleHandler;
    private Vector3 _offset = new Vector3(-15f, 15f, -15f);
    private float _angleX = 50f;
    private float _angleY = 45f;

    private void Start()
    {
        _obstacleHandler = GetComponent<CameraObstacleHandler>();
    }

    private void LateUpdate()
    {
        if (!_targetPlayer) return;
        Vector3 targetPos = _targetPlayer.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraSpeed);
        transform.rotation = Quaternion.Euler(_angleX, _angleY, 0f);

        _obstacleHandler.CheckObstacles(_targetPlayer);
    }
    
    public void CameraInit()
    {
        var playerObj = GameManager.Instance.FindPlayer();
        if (playerObj != null)
        {
            _targetPlayer = playerObj.transform;
        }
    }
}