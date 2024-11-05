using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Text interactionText;
    [SerializeField] private Transform holdPosition;
    [SerializeField] private GameObject Reticle;

    public float interactDistance = 2f;
    public float moveSpeed = 5f;

    private GameObject heldObject;
    private bool isMovingObject = false;

    private Outline currentOutline;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (interactionText == null)
        {
            Debug.LogError("interactionText не назначен в инспекторе");
        }

        if (holdPosition == null)
        {
            Debug.LogError("holdPosition не назначен в инспекторе");
        }

        Reticle.SetActive(true);
    }

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        if (heldObject == null)
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.transform.tag == "Interactable")
                {
                    interactionText.text = "Используйте ЛКМ чтобы взять";
                    Outline outline = hit.transform.GetComponent<Outline>();

                    if (outline != null)
                    {
                        if (currentOutline != null && currentOutline != outline)
                        {
                            currentOutline.enabled = false;
                        }
                        outline.enabled = true;
                        currentOutline = outline;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (outline != null)
                        {
                            outline.enabled = false;
                        }
                        PickUpObject(hit.transform.gameObject);
                    }
                }
                else
                {
                    if (currentOutline != null)
                    {
                        currentOutline.enabled = false;
                        currentOutline = null;
                    }
                    interactionText.text = "";
                }
            }
            else
            {
                if (currentOutline != null)
                {
                    currentOutline.enabled = false;
                    currentOutline = null;
                }
                interactionText.text = "";
            }
        }
        else
        {
            interactionText.text = "Используйте ЛКМ чтобы отпустить";

            if (isMovingObject)
            {
                heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, holdPosition.position, Time.deltaTime * moveSpeed);

                if (Vector3.Distance(heldObject.transform.position, holdPosition.position) < 0.1f)
                {
                    heldObject.transform.position = holdPosition.position;
                    heldObject.transform.parent = holdPosition;
                    isMovingObject = false;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                ReleaseObject();
            }
        }
    }

    private void PickUpObject(GameObject obj)
    {
        heldObject = obj;
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
        heldObject.GetComponent<Collider>().enabled = false; 
        isMovingObject = true;
        Reticle.SetActive(false);
    }

    private void ReleaseObject()
    {
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.GetComponent<Collider>().enabled = true; 
        heldObject.transform.parent = null;
        heldObject = null;
        isMovingObject = false;
        Reticle.SetActive(true);
    }
}
