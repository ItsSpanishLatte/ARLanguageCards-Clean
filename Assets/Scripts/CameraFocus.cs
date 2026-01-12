using UnityEngine;
using Vuforia;

public class CameraFocus : MonoBehaviour
{
    void Start()
    {
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void OnVuforiaStarted()
    {
        bool focusModeSet = VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(
            FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

        if (!focusModeSet)
        {
            Debug.Log("Otomatik odaklama bu cihazda desteklenmiyor.");
        }
    }
}