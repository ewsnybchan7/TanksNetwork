using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SmoothRotataion : MonoBehaviour, IPunObservable
{
    private Transform m_Transform;

    private PhotonView pv;

    private Quaternion curRot = Quaternion.identity;

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
        { }
        else
        {
            m_Transform.localRotation = Quaternion.Slerp(m_Transform.localRotation, curRot, Time.deltaTime * 3.0f);
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
