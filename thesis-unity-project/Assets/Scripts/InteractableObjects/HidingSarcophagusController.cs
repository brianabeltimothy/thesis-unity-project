using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSarcophagusController : MonoBehaviour, IInteractable
{
    private MummyAI mummyAIScript;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private float duration = 1.0f;

    [SerializeField] private BoxCollider boxCollider; 

    public bool isOpen = false;
    private bool isAnimating = false;
    private Animator animator;
    private bool isMoving = false;
    private AudioSource audioSource;

    private Player player;
    private PlayerController playerController;
    private Transform playerTransform;
    private string instruction = "[E] <br> Hide";

    private void Awake() {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerTransform.GetComponent<Player>();
        playerController = playerTransform.GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }

    public string GivesInstructionText()
    {
        return instruction;
    }

    public void Interact()
    {
        if (!player.isHiding)
        {
            Hide();
            instruction = "[E] <br> Get out";
        }
        else
        {
            Out();
            instruction = "[E] <br> Hide";
        }
    }

    public void Hide()
    {
        playerTransform.position = initialTransform.position;
        if (!isMoving)
        {
            playerController.SetIdleAnimation();
            StartCoroutine(HideCoroutine());
            player.isHiding = true;
            playerController.canMove = false;
        }
    }

    public void Out()
    {
        if (!isMoving)
        {
            StartCoroutine(OutCoroutine());
            player.isHiding = false;
            playerController.canMove = true;
        }
    }

    private IEnumerator HideCoroutine()
    {
        boxCollider.enabled = false;
        OpenDoor();
        yield return new WaitUntil(() => !isAnimating);
        yield return StartCoroutine(MoveToPosition(targetTransform.position, duration));
        CloseDoor();
        boxCollider.enabled = true;
    }

    private IEnumerator OutCoroutine()
    {
        boxCollider.enabled = false;
        OpenDoor();
        yield return new WaitUntil(() => !isAnimating);
        yield return StartCoroutine(MoveToPosition(initialTransform.position, duration));
        CloseDoor();
        boxCollider.enabled = true;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        isMoving = true;
        Vector3 startPosition = playerTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            playerTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerTransform.position = targetPosition;
        isMoving = false;
    }

    private void OpenDoor()
    {
        isOpen = true;
        isAnimating = true;
        animator.ResetTrigger("Open");
        animator.SetTrigger("Open");
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.volume = Random.Range(0.8f, 1.0f);
        audioSource.Play();
    }

    private void CloseDoor()
    {
        isOpen = false;
        isAnimating = true;
        animator.ResetTrigger("Close");
        animator.SetTrigger("Close");
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.volume = Random.Range(0.8f, 1.0f);
        audioSource.Play();
    }

    public void OnAnimationComplete()
    {
        isAnimating = false;
    }
}