using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float CamSpeed = 30.0f;
    public float speedmovewheel_cam = 1000.0f;
    public float speedRotate_cam = 200.0f;
    public float speedaccelaratemove_cam = 5.0f;
    public bool do_accerlatemove_cam = false;

    protected Vector2 DFLT_V2ROT_CAM = new Vector2(0.0f, 0.0f);
    protected Vector2 _v2MousePoint_previous = new Vector2(0.0f, 0.0f);

    void Start()
    {
        DFLT_V2ROT_CAM.x = Camera.main.transform.eulerAngles.y;
        DFLT_V2ROT_CAM.y = Camera.main.transform.eulerAngles.x;
    }
    void Update()
    {
        Update_FpsCamera(Time.deltaTime);
        Update_KeyboardInput(Time.deltaTime);
    }

    void Update_FpsCamera(float ftimedeltatime)
    {
        float fTimeDelta = ftimedeltatime;

        if (Input.GetMouseButton(1))
        {
            float fSpeedRotCurr = speedRotate_cam * fTimeDelta;

            DFLT_V2ROT_CAM.x += Input.GetAxis("Mouse X") * fSpeedRotCurr;
            DFLT_V2ROT_CAM.y -= Input.GetAxis("Mouse Y") * fSpeedRotCurr;

            Camera.main.transform.rotation = Quaternion.Euler(DFLT_V2ROT_CAM.y, DFLT_V2ROT_CAM.x, 0);
        }

        if (Input.GetMouseButton(2))
        {
            Camera.main.transform.Translate(-(Input.GetAxis("Mouse X")), -(Input.GetAxis("Mouse Y")), 0);
        } // if (Input.GetMouseButton(0))

        float fInputHori = Input.GetAxis("Horizontal");
        float fInputVert = Input.GetAxis("Vertical");
        float fWeightMoveSpeed = 1.0f;

        if (true == Input.GetKey(KeyCode.LeftShift))
        {
            fWeightMoveSpeed = fWeightMoveSpeed * speedaccelaratemove_cam;
        }

        if (fInputHori + fInputVert != 0.0f)
        {
            Vector3 v3PosCam = Camera.main.transform.position;
            Vector3 v3DirCam = Camera.main.transform.forward;
            Vector3 v3RightCam = Camera.main.transform.right;
            float fSpeedCamCurr = CamSpeed * fTimeDelta * fWeightMoveSpeed;

            v3PosCam += (v3RightCam * (fInputHori * fSpeedCamCurr));
            v3PosCam += (v3DirCam * (fInputVert * fSpeedCamCurr));


            Camera.main.transform.position = v3PosCam;
        }
        //@ Zoom In Out Move
        float fWheelMoveCurr = Input.GetAxis("Mouse ScrollWheel");
        if (fWheelMoveCurr != 0.0f)
        {
            float fDistanceWheelCurr = fWheelMoveCurr * speedmovewheel_cam * fTimeDelta * fWeightMoveSpeed;

            Vector3 v3PosCam = Camera.main.transform.position;
            Vector3 v3DirCam = Camera.main.transform.forward;

            v3PosCam += v3DirCam * fDistanceWheelCurr;

            Camera.main.transform.position = v3PosCam;
        }
        _v2MousePoint_previous.x = Input.GetAxis("Mouse X");
        _v2MousePoint_previous.y = Input.GetAxis("Mouse Y");
    } // void Update_FpsCamera(float ftimedeltatime)

    void Update_KeyboardInput(float ftimedeltatime)
    {
        
    }

} // public class ProcessInput : MonoBehaviour
