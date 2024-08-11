using UnityEngine;
using UnityEngine.InputSystem;

public class HandTrackingGrab : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;
    public float pinchThreshold = 0.7f;
    [SerializeField] private OVRGrabbable rightGrabbedObject;
    [SerializeField] private OVRGrabbable leftGrabbedObject;

    [SerializeField] private Transform leftIndexFinger;
    [SerializeField] private Transform rightIndexFinger;

    public InputActionReference rightGrapInput;
    public InputActionReference leftGrapInput;

    private bool rightIsGrabbing = false;
    private bool RightIsGrabbing
    {
        get { return rightIsGrabbing; }
        set
        {
            if (rightIsGrabbing != value)
            {
                rightIsGrabbing = value;

                if (value == true)
                {
                    Debug.Log("right is grabbing");
                }
                else
                {
                    Debug.Log("right is release");
                }
            }
        }
    }

    private bool leftIsGrabbing = false;
    private bool LeftIsGrabbing
    {
        get { return leftIsGrabbing; }
        set
        {
            if (leftIsGrabbing != value)
            {
                leftIsGrabbing = value;
                if (value == true)
                {
                    Debug.Log("left is grabbing");
                }
                else
                {
                    Debug.Log("left is release");
                }
            }
        }
    }


    [SerializeField] private float moveSpeed = 0.01f;

    private void OnEnable()
    {
        leftGrapInput.action.performed += OnGrabObject_Left;
        leftGrapInput.action.canceled += OnReleaseObject_Left;

        rightGrapInput.action.performed += OnGrabObject_Right;
        rightGrapInput.action.canceled += OnReleaseObject_Right;

        rightGrapInput.action.Enable();
        leftGrapInput.action.Enable();
    }



    void Update()
    {
        MoveHand(leftHand, leftIndexFinger, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D);
        MoveHand(rightHand, rightIndexFinger, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
        MoveGrabbableObject();


    }


    private void MoveHand(OVRHand hand, Transform indexFinger, KeyCode up, KeyCode down, KeyCode left, KeyCode right)
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(up))
        {
            movement += Vector3.forward;
        }
        if (Input.GetKey(down))
        {
            movement += Vector3.back;
        }
        if (Input.GetKey(left))
        {
            movement += Vector3.left;
        }
        if (Input.GetKey(right))
        {
            movement += Vector3.right;
        }

        hand.transform.Translate(movement * moveSpeed * Time.deltaTime);

        TryPoke(indexFinger);
        /*
        if (isGrabbing)
        {
            //if (grabObj == null)
            //{
            //    TryGrab(hand, grabObj);
            //}

            Debug.Log(hand.name + " is grabbing.");
        }
        else
        {
            // ���⼭ Release ������ ȣ��
            //Release(isLeft);
            Debug.Log(hand.name + " is releasing.");
        }
        */
    }

    private void OnGrabObject_Left(InputAction.CallbackContext con)
    {
        TryGrab(leftHand, false);
        Debug.Log("Left Grabbed");
    }
    private void OnGrabObject_Right(InputAction.CallbackContext con)
    {
        TryGrab(rightHand, true);
        Debug.Log("Right Grabbed");
    }

    private void OnReleaseObject_Left(InputAction.CallbackContext con)
    {
        Debug.Log("Left Canceled");

        if (leftGrabbedObject != null)
        {
            leftGrabbedObject.transform.SetParent(null);
            leftGrabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            leftGrabbedObject = null;
            Debug.Log("Left Released");
        }
    }
    private void OnReleaseObject_Right(InputAction.CallbackContext con)
    {
        Debug.Log("Right Canceled");

        if (rightGrabbedObject != null)
        {
            rightGrabbedObject.transform.SetParent(null);
            rightGrabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            rightGrabbedObject = null;
            Debug.Log("Right Released");
        }
    }

    private void TryGrab(OVRHand hand, bool grabObjisRight)
    {
        Collider[] colliders = Physics.OverlapSphere(hand.PointerPose.position, 0.05f);


        foreach (var collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out OVRGrabbable grabbable))
            {
                if (grabObjisRight)
                {
                    rightGrabbedObject = grabbable;
                    rightGrabbedObject.transform.SetParent(hand.transform);
                    rightGrabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                }
                else
                {
                    leftGrabbedObject = grabbable;
                    leftGrabbedObject.transform.SetParent(hand.transform);
                    leftGrabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                }

                break;
            }
        }
    }

    private void TryPoke(Transform indexFinger)
    {


        Collider[] indexColliders = Physics.OverlapSphere(indexFinger.position, 0.01f);

        foreach (var collider in indexColliders)
        {
            if (collider.CompareTag("Pokeable"))
            {
                Debug.Log("Index finger poked " + collider.name);
            }
        }

    }

    private void Release(bool isLeft)
    {
        var grabObj = isLeft ? leftGrabbedObject : rightGrabbedObject;
        if (grabObj != null)
        {
            grabObj.transform.SetParent(null);
            grabObj.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void MoveGrabbableObject()
    {
        if (leftGrabbedObject != null)
        {
            leftGrabbedObject.transform.position = new Vector3(leftHand.transform.position.x, leftHand.transform.position.y, 0);
        }
        if (rightGrabbedObject != null)
        {
            rightGrabbedObject.transform.position = new Vector3(rightHand.transform.position.x, rightHand.transform.position.y, 0);
        }
    }

}



/* ���� �ݺ� ��� ����
 
 
 �ݺ� - ����(�̸� ���� ��), �ݺ��� �ൿ.Action()

CodeBlock _block;

void �ݺ���(CodeBlock block)
{
    
    while(����)
    {
        block.Action();
    }
}


���� - ����(�̸� �������� ��), ���� ������ ������ �ൿ

void ���ǹ�(Func<bool> ����, Action ������ �ൿ)
{
    if(����)
    {
        �������ൿ.Invoke();
    }
}

�÷��̾�
OnTriggerEnter(Collision other)
{
    if(other.TryGetComponent(out Monster mon)
    {
        mon
    }
}

 */


/* �ڵ�� �����̳� ��� ����
class �ڵ�� 
{
    �������̳� con;
        
    void OnEnable()
    {
        �ڵ�Ʈ��Ŀ.Intance.OnRelease += OnRelease_CheckContatiner;
    }

    void OnRelease_CheckContainer()
    {
        if(con!=null)
            con.������();
    }

    OnTriggerEnter(collider col)
    {
        if(col.TryGetComponent(out �������̳� con)
        {
            con = this.con;
        }
    }

    (Exit�� �ݴ�� �Ҵ����� con = null;)
}
---------------------------------------------------------
class �������̳� 
{
    public void ������(�ڵ�� block)
    {
        Instantiate(block.gameObject);
    }
}
---------------------------------------------------------
class �ڵ�Ʈ��Ŀ : �̱���
{
    �ڵ�� ����ִº�;

    public event void OnRelease;
    
    void ���±��()
    {
        ���� ����();
        OnRelease.Invoke();
    }
}
 
 */