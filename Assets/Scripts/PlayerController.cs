using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour
{
    //스피드 조정 변수
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;
    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    //상태변수
    private bool isRun = false;
    private bool isGround = true;

    //땅 착지 여부
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private float lookSensitivity; //카메라 민감도

    [SerializeField]
    private float cameraRotationLimit; //상하 카메라 각도 제한
    private float currentCameraRotationX = 0; //상하 카메라 회전


    //필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>(); //인스펙터 창의 rigidbody 가져오기
        applySpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        TryJump();
        TryRun();
        Move();
        CameraRotation();
        CharacterRotation();
    }

    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        myRigid.velocity = transform.up * jumpForce;
    }

    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }


    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");  //좌우
        float _moveDirZ = Input.GetAxisRaw("Vertical");  //상하

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    //좌우 캐릭터회전
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxis("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        //구한 벡터값을 Quaternion값으로 변환 후 myRigid.rotation과 곱
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    //상하 카메라 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = (_xRotation * lookSensitivity);
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); //가두기

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    
}
