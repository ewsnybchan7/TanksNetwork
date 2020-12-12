using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankTurret : MonoBehaviour
{
    public Camera mainCamera;
    public Transform canvas;

    private PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponentInParent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine)
            rotateTowardMouse();
    }

    private void rotateTowardMouse()
    {
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane GroupPlane = new Plane(Vector3.up, Vector3.zero);

        float rayLength;

        if (GroupPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);
            Vector3 lookPoint = new Vector3(pointTolook.x, transform.position.y, pointTolook.z);
            transform.LookAt(lookPoint);
        }
    }
}
