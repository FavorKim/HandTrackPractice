using UnityEngine;
using UnityEngine.InputSystem;

public class HandTrackingGrab : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;
    public float pinchThreshold = 0.7f;
    [SerializeField] private OVRGrabbable rightGrabbedObject;
    [SerializeField] private OVRGrabbable leftGrabbedObject;

    public InputActionReference rightGrapInput;
    public InputActionReference leftGrapInput;

    private bool rightIsGrabbing = false;
    private bool leftIsGrabbing = false;


    [SerializeField] private float moveSpeed = 0.01f;

    private void OnEnable()
    {
        leftGrapInput.action.performed += context => { leftIsGrabbing = true; };
        leftGrapInput.action.canceled += context => { leftIsGrabbing = false; };

        rightGrapInput.action.performed += context => { rightIsGrabbing = true; };
        rightGrapInput.action.canceled += context => { rightIsGrabbing = false; };

        rightGrapInput.action.Enable();
        leftGrapInput.action.Enable();
    }


    void Update()
    {
        SimulateHandMovement(leftHand, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, leftIsGrabbing, leftGrabbedObject, true);
        SimulateHandMovement(rightHand, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, rightIsGrabbing, rightGrabbedObject, false);
    }


    private void SimulateHandMovement(OVRHand hand, KeyCode up, KeyCode down, KeyCode left, KeyCode right, bool isGrabbing, OVRGrabbable grabObj, bool isLeft)
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

        if (isGrabbing)
        {
            if (grabObj == null)
            {
                TryGrab(hand, grabObj);
            }

            Debug.Log(hand.name + " is grabbing.");
        }
        else
        {
            // ���⼭ Release ������ ȣ��
            Release(isLeft);
            Debug.Log(hand.name + " is releasing.");
        }
    }

    private void TryGrab(OVRHand hand, OVRGrabbable grabObj)
    {
        Collider[] colliders = Physics.OverlapSphere(hand.PointerPose.position, 0.05f);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out OVRGrabbable grabbable))
            {
                grabObj = grabbable;
                grabObj.transform.SetParent(hand.transform);
                grabObj.GetComponent<Rigidbody>().isKinematic = true;
                break;
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
}


/*

view - model << �갡 ��� �ٲ��� �Ѵٸ�?
UI�� �Ŵ����� ������ ���������ϱ�.

_Character = �Ŵ���.ĳ�����ּ���(Aĳ����);
��� : _Character = Aĳ����;

public Character ĳ�����ּ���(string ĳ�����̸�)
{
    foreach(ĳ���� c in ĳ���� ���)
    {
        if(c.�̸� == ĳ�����̸�)
        return c;
    }
}


Model = ����
ViewModel = ����
View = ����


�Ű��� ĳ���� �̸��� �����ؼ�
�Ŵ����� �� ĳ���� �̸��� ��ȯ�� �ϰ�?
��ȯ�� ĳ���͸� ���� ������ �ɰ��ִ� ĳ���Ϳ� ����.
*/