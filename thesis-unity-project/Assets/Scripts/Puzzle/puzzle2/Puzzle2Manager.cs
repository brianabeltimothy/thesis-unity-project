using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle2Manager : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject camPos;
    // private AudioSource audioSource;

    private GameObject playerObject;
    private PlayerController playerController;
    private GameObject cam;
    private BoxCollider boxCollider;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private bool isInteracting = false;
    private Animator animator;

    
    private int[] result, correctCombination;

    private void Awake() 
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera"); 
        playerObject = GameObject.FindGameObjectWithTag("Player"); 
        playerController = playerObject.GetComponent<PlayerController>();
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // audioSource = stoneCover.GetComponent<AudioSource>();
        result = new int[]{1, 1, 1};
        correctCombination = new int[] {1, 2, 3};
        Puzzle2Piece.Rotated += CheckResults;
    }

    private void Update() 
    {
        if(isInteracting && Input.GetKeyDown(KeyCode.Q))
        {
            ExitInteracting();
        }
    }

    public void Interact()
    {
        StartCoroutine(Interacting());
    }

    private IEnumerator Interacting()
    {
        //lock player movement
        //lock camera movement
        //disable collider
        //move camera 
        //make cursor visible
        //unlock the cursor
        isInteracting = true;
        initialPos = cam.transform.position;
        initialRot = cam.transform.rotation;
        playerController.canMove = false;
        playerController.canMoveCam = false;
        boxCollider.enabled = false;
        yield return StartCoroutine(MoveToPositionAndRotation(camPos.transform.position, camPos.transform.rotation, 2f));
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //display instruction
    }

    private void ExitInteracting()
    {
        playerController.canMove = true;
        playerController.canMoveCam = true;
        boxCollider.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam.transform.position = initialPos;
        cam.transform.rotation = initialRot;
        isInteracting = false;
    }

    private IEnumerator MoveToPositionAndRotation(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Vector3 startPosition = cam.transform.position;
        Quaternion startRotation = cam.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            cam.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            cam.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = targetPosition;
        cam.transform.rotation = targetRotation;
    }

    private void CheckResults(int pieceId, int numberShown)
    {   
        switch (pieceId)
        {
            case 1:
                result[0] = numberShown;
                break;
            case 2:
                result[1] = numberShown;
                break;
            case 3:
                result[2] = numberShown;
                break;
        }

        if (result[0] == correctCombination[0] && 
            result[1] == correctCombination[1] &&
            result[2] == correctCombination[2]) 
        {
            ExitInteracting();
            boxCollider.enabled = false;
            animator.SetTrigger("Open");
            //play sound
        }
    }

    private void OnDestroy()
    {
        SliderController.Rotated += CheckResults;
    }
}