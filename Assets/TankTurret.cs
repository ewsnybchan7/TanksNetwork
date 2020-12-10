using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTurret : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        //Vector3 screenTurretPos = Camera.main.
        float theta = Vector2.Dot(Vector3.forward, (worldMousePos - this.transform.position).normalized);
        this.transform.rotation = new Quaternion(0, Mathf.Acos(theta), 0, 0);
        //Debug.Log(mousePos);
        Debug.Log(worldMousePos);
        Debug.Log(Mathf.Acos(theta));
    }
}
