using UnityEngine;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;                  
    [HideInInspector] public List<NetworkPlayer> m_Targets;         // tank target

    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;

    public GameObject localPlayer;

    public bool isBossStage = false;

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (localPlayer)
        {
            Move();
            Zoom();
        }

        if(m_Targets.Count == 0)
        {
            updateTargets();
        }
    }


    private void Update()
    {
        if (!localPlayer)
        {
            foreach (NetworkPlayer player in FindObjectsOfType<NetworkPlayer>())
            {
                if (player.photonView.IsMine)
                {
                    localPlayer = player.gameObject;
                    localPlayer.transform.Find("TankRenderers").GetComponentInChildren<TankTurret>().mainCamera = m_Camera;
                }
            }
        }
    }

    private void updateTargets()
    {
        m_Targets = GameManager.gameManager.m_Players;
    }

    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();

        if (isBossStage)
        {
            int numTargets = 0;

            for (int i = 0; i < m_Targets.Count; i++)
            {
                if (!m_Targets[i].gameObject.activeSelf) // activeSelf: The local active state of this GameObject. (Read Only)
                    continue;

                averagePos += m_Targets[i].transform.position;
                numTargets++;
            }

            if (numTargets > 0)
                averagePos /= numTargets;

            averagePos.y = transform.position.y;
        }
        else
        {
            if (localPlayer.activeSelf)
            {
                averagePos = localPlayer.transform.position;
                averagePos.y = transform.position.y;
            }
            else
            {
                foreach(NetworkPlayer player in m_Targets)
                {
                    if(!player.photonView.IsMine)
                    {
                        averagePos = player.transform.position;
                        averagePos.y = transform.position.y;
                        break;
                    }
                }
            }
        }

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        float size = 10.0f;

        if (isBossStage)
        {
            Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); // InverseTransformPoint: world 좌표를 해당 transform 기준 local 좌표로 변환


            for (int i = 0; i < m_Targets.Count; i++)
            {
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].transform.position);

                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos; // 카메라의 중간 지점에서 탱크로 향하는 벡터

                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y)); // 탱크가 화면 안에 나와야 하므로 orthographic의 size(y축)를 설정

                // orthographic의 x축 = size x aspect
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect); // size(y) = x / aspect

            }

            size += m_ScreenEdgeBuffer;

            size = Mathf.Max(size, m_MinSize);
        }

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}