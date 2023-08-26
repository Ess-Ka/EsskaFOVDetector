
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

public enum FOVDetectMode {
    OnStart = 0,
    Interval = 1,
    Manually = 2
}

/// <summary>
/// Detects the vertical FOV on the main camera.
/// Places a small cube in front of the head with a specific distance and angle.
/// Repeats this with different angles until the cube is inside the view frustum.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FOVDetector : UdonSharpBehaviour {

    #region Constants

    private const int FOV_MIN = 50;
    private const int FOV_MAX = 100;
    private const float DETECT_DISTANCE = 0.2f;

    #endregion

    #region Properies

    /// <summary>
    /// Returns true as long FOV detection runs.
    /// </summary>
    public bool Detecting { get; private set; }

    /// <summary>
    /// Contains the detected FOV as soon the detection has finished.
    /// </summary>
    public int DetectedFOV { get; private set; }

    #endregion

    #region Fields

    [Header("FOV Detection")]
    public FOVDetectMode detectMode;
    [Tooltip("Interval at which FOV detection runs if 'Detect Mode' is set to 'Interval'")]
    [Range(3f, 60f)]
    public float detectInterval = 10f;

    #endregion

    #region Runtime Variables

    private VRCPlayerApi playerApi;
    private MeshRenderer meshRenderer;
    private int detectFOV;

    #endregion

    #region Event Methods

    void Start() {
        playerApi = Networking.LocalPlayer;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        enabled = false;

        if (detectMode != FOVDetectMode.Manually)
            StartDetection();
    }

    public override void PostLateUpdate() {

        if (!Detecting)
            return;

        if (detectFOV == 0)
            detectFOV = FOV_MAX;
        else if (detectFOV > FOV_MIN)
            detectFOV--;
        else {
            Debug.Log($"DETECTION OF FOV FAILED");

            TerminateDetection();
            return;
        }

        VRCPlayerApi.TrackingData head = playerApi.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

        transform.SetPositionAndRotation(head.position, head.rotation);
        transform.Rotate(detectFOV / 2f, 0f, 0f, Space.Self);
        transform.Translate(0f, 0f, DETECT_DISTANCE, Space.Self);
    }

    private void OnWillRenderObject() {

        if (detectFOV > 0) {
            DetectedFOV = detectFOV;
            Debug.Log($"DETECTED FOV IS {detectFOV}°");

            TerminateDetection();
        }
    }

    #endregion

    #region FOV Detection

    /// <summary>
    /// Starts the FOV detection.
    /// </summary>
    public void StartDetection() {

        if (playerApi == null || !playerApi.IsValid() || Detecting)
            return;

        Debug.Log($"START FOV DETECTION");
        Detecting = true;
        DetectedFOV = 0;
        meshRenderer.enabled = true;
        enabled = true;
    }

    private void TerminateDetection() {
        Detecting = false;
        detectFOV = 0;
        meshRenderer.enabled = false;
        enabled = false;

        if (detectMode == FOVDetectMode.Interval)
            SendCustomEventDelayedSeconds(nameof(StartDetection), detectInterval);
    }

    #endregion
}
