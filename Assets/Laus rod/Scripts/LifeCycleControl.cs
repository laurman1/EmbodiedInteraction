using System.Collections;
using UnityEngine;
using UnityEngine.Playables;


public class LifeCycleControl : MonoBehaviour
{
    [SerializeField] private GameObject cameraObject;
    private Rigidbody rb;
    [SerializeField] private GameObject playerAttachCube;
    [SerializeField] private GameObject playerAttach;
    private bool startLerp = false;
    private bool isLerping = false;
    private bool attach = false;
    [SerializeField] float lerpDuration = 0.5f; 

    [SerializeField] private AnimationSpeed animSopeedScript;
    [SerializeField] private GameObject animTree;

    //[SerializeField] private AnimationMotionTime motionTimeScript;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayableDirector timeLine;

    [SerializeField] private GameObject extraLeaves;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        StartTimeLine();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        

        if (attach == true)
        {
            transform.position = playerAttachCube.transform.position;
            //transform.rotation = playerAttachCube.transform.rotation;

            if (animSopeedScript.attachStop == true)
            {
                attach = false;
                
            }
        }
    }

    public void FallDown()
    {
        rb.useGravity = true;
    }

    public void Grow()
    {
        anim.enabled = false;
        cameraObject.transform.localRotation = Quaternion.Euler(0, -150f, 0);
        startLerp = true;

        LerptoTree();
        /*
        attach = true;
        animTree.SetActive(true);
        animSopeedScript.IncreaseSpeed();
        Debug.Log("StartGrowing");
        */
    }

    public void EndGame()
    {
        anim.enabled = true;
        anim.Play("GoToBranch");
        Debug.Log("EndTheGame");
        Object.Destroy(extraLeaves);
    }

    public void LerptoTree()
    {
        if (startLerp && !isLerping)
        {
            StartCoroutine(LerpPosition(playerAttachCube.transform.position, lerpDuration));
        }
    }

    public void StartTimeLine()
    {
        timeLine.Play();
    }

    public void triggerGrow()
    {
        timeLine.Stop();
        anim.Play("END");
        Debug.Log("GoToEND");
    }

    private IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        isLerping = true;

        Vector3 startPosition = transform.position;
        float time = 0;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPosition;
        attach = true;
        animTree.SetActive(true);
        animSopeedScript.IncreaseSpeed();
        Debug.Log("StartGrowing");

        isLerping = false;
        startLerp = false;
    }
}
