using UnityEngine;
using Vuforia;

public class CameraFocus : MonoBehaviour
{
    void Start()
    {
        // Vuforia başladığında odaklamayı ayarla
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void OnVuforiaStarted()
    {
        // Sürekli otomatik odaklama (Autofocus) modunu aç
        bool focusModeSet = VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(
            FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

        if (!focusModeSet)
        {
            Debug.Log("Otomatik odaklama bu cihazda desteklenmiyor.");
        }
    }
}