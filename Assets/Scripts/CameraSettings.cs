using UnityEngine;
using Cinemachine;

public class CameraSettings : MonoBehaviour
{
    PlayerMovement movementInstance;

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachineBasicMultiChannelPerlin virtualCameraPerlinNoise;

    [Header("Intense value")]
    [SerializeField] float amplitudeValue; 
    [SerializeField] float frequencyvalue;

    [SerializeField] bool intenseCamera = false;

    private void Start()
    {
        movementInstance = PlayerMovement.instance;
        virtualCameraPerlinNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (!intenseCamera)
            return;

        if (movementInstance.playerPhase == PlayerMovement.PlayerPhase.Running)
            IsRunning();
        else 
            NotRunning();
    }

    void IsRunning()
    {
        virtualCameraPerlinNoise.m_AmplitudeGain = amplitudeValue;
        virtualCameraPerlinNoise.m_FrequencyGain = frequencyvalue;
    }

    void NotRunning()
    {
        virtualCameraPerlinNoise.m_AmplitudeGain = 1f;
        virtualCameraPerlinNoise.m_FrequencyGain = 1f;
    }
}
