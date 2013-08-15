using UnityEngine;
using System.Collections;
using Zenject;

public class FollowMousePosition : MonoBehaviour 
{
    [Inject]
    public Camera _mainCamera { set; private get; }

    public void Update()
    {
        var mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        var mousePos3d = mouseRay.origin;
        mousePos3d.z = 0;

        transform.position = mousePos3d;
    }
}
