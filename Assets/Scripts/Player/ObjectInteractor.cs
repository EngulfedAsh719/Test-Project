using UnityEngine;
using UnityEngine.UI;

public class CandleInteractor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Text interactionText;
    [SerializeField] private Transform holdPosition;
    [SerializeField] private GameObject Reticle;

    public float interactDistance = 2f;
    public float moveSpeed = 5f;
    public float windDuration = 5f;
    private float originalLightRange;
    private float windTimer = 0f;

    private GameObject heldObject;
    private ParticleSystem candleParticleSystem;
    private Light candleLight;
    private bool isMovingObject = false;
    private bool isCovered = false;
    private bool isExtinguished = false;
    private bool isInWind = false;

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

        if (Input.GetKeyDown(KeyCode.E) && heldObject != null)
        {
            CoverCandle(!isCovered);
            Debug.Log(isCovered ? "Свеча закрыта" : "Свеча открыта");
        }

        if (!isCovered && heldObject != null && !isExtinguished)
        {
            if (isInWind)
            {
                windTimer += Time.deltaTime;
                Debug.Log("Свеча в зоне действия ветра. Таймер: " + windTimer);
                if (windTimer >= windDuration)
                {
                    ExtinguishCandle("Свеча потухла из-за ветра");
                }
            }
            else
            {
                windTimer = 0f;
                Debug.Log("Таймер сброшен.");
            }
        }
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
                    Debug.Log("Объект найден: " + hit.transform.name);

                    if (Input.GetMouseButtonDown(0))
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
                else
                {
                    interactionText.text = "";
                }
            }
            else
            {
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
        heldObject.transform.rotation = Quaternion.identity;
        candleParticleSystem = heldObject.GetComponentInChildren<ParticleSystem>();
        candleLight = heldObject.GetComponentInChildren<Light>();

        if (candleLight != null)
        {
            originalLightRange = candleLight.range;
        }

        isMovingObject = true;
        isExtinguished = false;
        Reticle.SetActive(false);
        Debug.Log("Объект поднят: " + heldObject.name);
    }

    private void ReleaseObject()
    {
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.GetComponent<Collider>().enabled = true;
        heldObject.transform.parent = null;
        heldObject = null;
        candleParticleSystem = null;
        candleLight = null;
        isMovingObject = false;
        isExtinguished = false;
        Reticle.SetActive(true);
        Debug.Log("Объект отпущен");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") && !isCovered)
        {
            ExtinguishCandle("Свеча потухла из-за воды");
        }

        if (other.CompareTag("Wind") && !isCovered)
        {
            isInWind = true;
            Debug.Log("Свеча в зоне действия ветра.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wind"))
        {
            isInWind = false;
            windTimer = 0f;
            Debug.Log("Таймер сброшен.");
        }
    }

    private void ExtinguishCandle(string reason)
    {
        if (candleParticleSystem != null)
        {
            candleParticleSystem.gameObject.SetActive(false);
        }
        if (candleLight != null)
        {
            candleLight.enabled = false;
        }
        isExtinguished = true;
        Debug.Log(reason);
    }

    public void CoverCandle(bool cover)
    {
        isCovered = cover;
        if (isCovered)
        {
            candleLight.range = originalLightRange / 2;
        }
        else
        {
            candleLight.range = originalLightRange;
        }
    }
}
