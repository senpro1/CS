using UnityEngine;

public class CursorLock : MonoBehaviour
{
    private void Start()     //when player loads, cursor is invisible and locked to center of screen
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
