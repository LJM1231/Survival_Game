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
    [SerializeField]
    private float crouchSpeed;

    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    //상태변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    //움직임 체크 변수
    private Vector3 lastPos;

    //앉았을 때 얼마나 앉을지 결정하는 함수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

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
    private GunController theGunController;
    private Crosshair theCrosshair;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>(); //인스펙터 창의 rigidbody 가져오기
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();

        //초기화
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        MoveCheck();
        CameraRotation();
        CharacterRotation();
    }

    //앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    //앉기 동작
    private void Crouch()
    {
        isCrouch = !isCrouch;
        theCrosshair.CrouchingAnimation(isCrouch);

        if (isCrouch) //앉았을 때 상태
        {
            applySpeed = crouchSpeed; 
            applyCrouchPosY = crouchPosY;
        }
        else //앉았을 때 상태 해제
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    //부드러운 앉기 동작 실행
    IEnumerator CrouchCoroutine() //코루틴
    {
        float _PosY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_PosY != applyCrouchPosY) //목적지와 _PosY의 값이 같게 되면 while문 빠져나옴
        {
            count++;
            _PosY = Mathf.Lerp(_PosY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _PosY, 0);
            if (count > 15)
                break; //count가 15번 넘으면 while문 빠져나오기
            yield return null; //한프레임 대기
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0);
    }

    //지면 체크
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        theCrosshair.RunningAnimation(!isGround);
    }

    //점프 시도
    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    //점프 실행
    private void Jump()
    {
        //앉은 상태에서 점프시 앉은 상태 해제
        if (isCrouch)
            Crouch();

        myRigid.velocity = transform.up * jumpForce;
    }

    //달리기 시도
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

    //달리기 실행
    private void Running()
    {
        //앉은 상태에서 달리기 실행시 앉은 상태 해제
        if (isCrouch)
            Crouch();

        theGunController.CancelFineSight();

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = runSpeed;
    }

    //달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }

    //움직임 실행
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");  //좌우
        float _moveDirZ = Input.GetAxisRaw("Vertical");  //상하

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    //움직임 체크
    private void MoveCheck()
    {
        if (!isRun && !isCrouch && isGround)
        {
            if (Vector3.Distance(lastPos, transform.position) >= 0.01f)
                isWalk = true;
            else
                isWalk = false;

            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
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
