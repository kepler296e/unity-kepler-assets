using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MousePickUp : MonoBehaviour
{
    public float maxTakeDistance = 5f;
    private float maxSaveDistance = 0.4f;
    private float scrollSpeed = 0.25f;
    private float rotationSpeed = 5f;
    private float mouseSpeed = 0.05f;
    private float moveSpeed = 0.1f;
    public float throwForce = 400f;

    public Transform hand, aimingHand;
    public Vector3 handOriginalPosition;

    private int layerMask;
    private RaycastHit hit;
    private Vector3 forward;

    public List<GameObject> takenObjects = new List<GameObject>();
    public static List<GameObject> savedObjects = new List<GameObject>();
    public GameObject selectedObj, mainObject;

    private MouseLook mouseLook;
    public Image crosshair;
    private float aimSpeed = 0.1f;

    private float scroll;
    private bool taking, aiming, attracting;

    void Start()
    {
        layerMask = LayerMask.GetMask("Objects");
        mouseLook = GetComponent<MouseLook>();
        handOriginalPosition = hand.localPosition;
    }

    void Update()
    {
        forward = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(transform.position, forward, out hit, maxTakeDistance, layerMask))
        {
            // Left click to take the hit object if not already taken
            if (Input.GetButtonDown("Fire1") && !aiming)
            {
                GameObject obj = hit.transform.gameObject;
                if (!takenObjects.Contains(obj)) Take(obj);
            }

            // if (Input.GetKeyDown(KeyCode.E)) Take(hit.transform.gameObject);

            DrawRay(Color.green);
        }
        else DrawRay(Color.red);

        // Hold right click to aim
        if (Input.GetButton("Fire2")) aiming = true;
        else aiming = false;

        if (aiming)
        {
            crosshair.enabled = false;
            // Disable mainObject UI and collisions
            if (mainObject != null)
            {
                if (mainObject.TryGetComponent(out Gun gun)) gun.ammoText.enabled = false;
                mainObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            // hand.position = Vector3.Lerp(hand.position, aimingHand.position, 1000f * Time.deltaTime);
            MoveTo(hand, aimingHand.position);
        }
        else
        {
            crosshair.enabled = true;
            if (mainObject != null)
            {
                if (mainObject.TryGetComponent(out Gun gun)) gun.ammoText.enabled = true;
                mainObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            hand.localPosition = Vector3.Lerp(hand.localPosition, handOriginalPosition, aimSpeed);
        }

        scroll = Input.GetAxis("Mouse ScrollWheel");

        if (takenObjects.Count > 0)
        {
            foreach (GameObject obj in takenObjects) Freeze(obj);

            // Select last object as default
            if (selectedObj == null) selectedObj = takenObjects[takenObjects.Count - 1];

            /* Selected Object System */
            // Left click to release & right click to throw
            if (!taking && Input.GetButtonDown("Fire1")) Release(selectedObj);
            if (Input.GetButtonDown("Fire2")) Throw(selectedObj);

            // Press E to save
            if (Input.GetKeyDown(KeyCode.E)) attracting = true;

            if (attracting) Attract(selectedObj);

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            if (takenObjects.Count == 1)
            {
                /*
                // Move forward/backward
                if (scroll > 0f) selectedObj.transform.position += forward * scrollSpeed;
                if (scroll < 0f) selectedObj.transform.position -= forward * scrollSpeed;
                */
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                // Unpaint last selected object
                if (selectedObj != null) Paint(selectedObj, Color.white);

                // Scroll to select object
                selectedObj = SelectObject(selectedObj, takenObjects);

                // Move selected
                selectedObj.transform.localPosition += new Vector3(mouseX * mouseSpeed, mouseY * mouseSpeed, 0);

                // Paint selected object
                Paint(selectedObj, Color.cyan);

                mouseLook.enabled = false;
            }
            else // if (takenObjects.Count > 1 && scroll != 0f)
            {
                // Move forward/backward
                if (scroll > 0f) selectedObj.transform.position += forward * scrollSpeed;
                if (scroll < 0f) selectedObj.transform.position -= forward * scrollSpeed;

                // Unpaint selected object if exists
                if (selectedObj != null) Paint(selectedObj, Color.white);
            }

            // Rotate selected
            if (Input.GetButton("Fire3") && selectedObj != null)
            {
                selectedObj.transform.localRotation *= Quaternion.Euler(mouseY * rotationSpeed, mouseX * rotationSpeed, 0);
                mouseLook.enabled = false;
            }
            else mouseLook.enabled = true;
            /* Selected Object System END*/
        }

        if (savedObjects.Count > 0)
        {
            // Select last object as default
            if (mainObject == null) mainObject = savedObjects[savedObjects.Count - 1];

            foreach (GameObject obj in savedObjects)
            {
                if (obj == mainObject) obj.SetActive(true);
                else obj.SetActive(false); // Hide(obj, 1f);
                MoveTo(obj.transform, hand.position);
            }

            Freeze(mainObject);
            mainObject.transform.localRotation = Quaternion.Lerp(mainObject.transform.localRotation, Quaternion.identity, moveSpeed);

            if (Input.GetKeyDown(KeyCode.G))
            {
                savedObjects.Remove(mainObject);
                Release(mainObject);
            }

            // Scroll to select main object
            mainObject = SelectObject(mainObject, savedObjects);
        }
    }

    GameObject SelectObject(GameObject obj, List<GameObject> list)
    {
        int objectIndex = list.IndexOf(obj);
        if (scroll < 0f)
            if (objectIndex > 0) obj = list[objectIndex - 1];
            else obj = list[list.Count - 1];
        if (scroll > 0f)
            if (list.Count > objectIndex + 1) obj = list[objectIndex + 1];
            else obj = list[0];
        return obj;
    }

    void DrawRay(Color color) { Debug.DrawRay(transform.position, forward * maxTakeDistance, color); }

    void Take(GameObject obj)
    {
        if (obj.TryGetComponent(out LightObject lightObject)) { lightObject.canTurn = true; };
        if (obj.TryGetComponent(out StopPlayer stopPlayer)) { stopPlayer.enabled = true; };
        if (obj.TryGetComponent(out Skate skate)) { skate.enabled = true; };
        if (obj.TryGetComponent(out Gun gun)) { gun.canUse = true; };

        selectedObj = obj;
        takenObjects.Add(obj);
        obj.GetComponent<Rigidbody>().useGravity = false;
        obj.transform.SetParent(transform);
        taking = true;
        Invoke("FinishTaking", 0.1f);
    }

    void FinishTaking() { taking = false; }

    void Release(GameObject obj)
    {
        if (obj.TryGetComponent(out LightObject lightObject)) { lightObject.canTurn = false; };
        if (obj.TryGetComponent(out StopPlayer stopPlayer)) { stopPlayer.enabled = false; };
        if (obj.TryGetComponent(out Skate skate)) { skate.enabled = false; };
        if (obj.TryGetComponent(out Gun gun)) { gun.canUse = false; };

        if (obj == mainObject)
        {
            savedObjects.Remove(obj);
            mainObject = null;
        }
        else // if (obj == selectedObj)
        {
            takenObjects.Remove(obj);
            ResetSelected();
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.useGravity = true;
        // Add inertia from
        CharacterController cc = transform.parent.GetComponent<CharacterController>();
        rb.velocity = cc.velocity;


        obj.transform.SetParent(null);
    }

    void Freeze(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void Throw(GameObject obj)
    {
        Release(obj);
        obj.GetComponent<Rigidbody>().AddForce(forward * throwForce);
    }

    void Attract(GameObject obj)
    {
        MoveTo(obj.transform, hand.position);
        obj.GetComponent<Rigidbody>().isKinematic = true;
        if (Vector3.Distance(obj.transform.position, hand.position) < maxSaveDistance)
        {
            attracting = false;
            obj.GetComponent<Rigidbody>().isKinematic = false;
            Save(obj);
        }
    }

    void Save(GameObject obj)
    {
        savedObjects.Add(obj);
        takenObjects.Remove(obj);
        ResetSelected();
    }

    void Paint(GameObject obj, Color color) { obj.GetComponent<Renderer>().material.color = color; }

    void MoveTo(Transform t, Vector3 target) { t.position = Vector3.Lerp(t.position, target, moveSpeed); }

    void ResetSelected()
    {
        Paint(selectedObj, Color.white);
        selectedObj = null;
    }
}