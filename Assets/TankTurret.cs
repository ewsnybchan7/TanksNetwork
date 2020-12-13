using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankTurret : MonoBehaviour, IPunObservable
{
    public Camera mainCamera;
    public Transform canvas;

    public Transform m_Transform;

    private PhotonView pv;

    private Quaternion curRot;

    private void Awake()
    {
        m_Transform = GetComponent<Transform>();

        pv = GetComponent<PhotonView>();
        pv.Synchronization = ViewSynchronization.Unreliable;

        curRot = m_Transform.localRotation;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
            rotateTowardMouse();
        else
        {
            m_Transform.localRotation = Quaternion.Slerp(m_Transform.localRotation, curRot, Time.deltaTime * 3.0f);
        }

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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_Transform.localRotation);
        }
        else
        {
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
