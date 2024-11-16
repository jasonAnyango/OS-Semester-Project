using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro
using System.Linq;

public class FCFSQueue : MonoBehaviour
{
    public Transform queueArea;
    public Transform processingArea;
    public GameObject jobPrefab;

    public TextMeshProUGUI processTimeText;
    public TextMeshProUGUI averageWTText;
    public TextMeshProUGUI averageTATText;

    private Queue<GameObject> jobQueue = new Queue<GameObject>();
    private GameObject currentJob = null;
    private float timer = 0f;

    private List<float> waitingTimes = new List<float>();
    private List<float> turnAroundTimes = new List<float>();

    public void AddJobToQueue()
    {
        if (jobPrefab != null && queueArea != null)
        {
            GameObject newJob = Instantiate(jobPrefab, queueArea);
            float yOffset = -100f;
            newJob.transform.localPosition = new Vector3(0, yOffset * jobQueue.Count, 0);
            newJob.name = $"Job {jobQueue.Count + 1}";
            jobQueue.Enqueue(newJob);

            JobData jobData = newJob.AddComponent<JobData>();
            jobData.arrivalTime = Time.time;
            jobData.burstTime = Random.Range(2f, 7f);
        }
    }

    private IEnumerator MoveToProcessing(GameObject job)
    {
        Vector3 startPosition = job.transform.position;
        Vector3 targetPosition = processingArea.position;
        float duration = 1f;
        float elapsedTime = 0f;

        SpriteRenderer spriteRenderer = job.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }

        while (elapsedTime < duration)
        {
            job.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        job.transform.position = targetPosition;
    }

    private void Update()
    {
        if (currentJob == null && jobQueue.Count > 0)
        {
            currentJob = jobQueue.Dequeue();
            currentJob.transform.SetParent(processingArea);

            StartCoroutine(MoveToProcessing(currentJob));

            JobData jobData = currentJob.GetComponent<JobData>();
            jobData.startTime = Time.time;

            timer = jobData.burstTime;
        }

        if (currentJob != null)
        {
            timer -= Time.deltaTime;

            if (processTimeText != null)
            {
                processTimeText.text = $"Processing Time: {Mathf.Max(timer, 0f):F2} seconds";
            }

            if (timer <= 0f)
            {
                CompleteJob();
            }
        }
        else if (processTimeText != null)
        {
            processTimeText.text = "No Job Processing";
        }
    }

    private void CompleteJob()
    {
        if (currentJob != null)
        {
            JobData jobData = currentJob.GetComponent<JobData>();
            jobData.completionTime = Time.time;

            float tat = jobData.completionTime - jobData.arrivalTime;
            float wt = jobData.startTime - jobData.arrivalTime;

            turnAroundTimes.Add(tat);
            waitingTimes.Add(wt);

            Debug.Log($"{currentJob.name}: TAT = {tat}, WT = {wt}");

            if (averageWTText != null)
            {
                float averageWT = waitingTimes.Count > 0 ? waitingTimes.Sum() / waitingTimes.Count : 0f;
                averageWTText.text = $"Average WT: {averageWT:F2} seconds";
            }

            if (averageTATText != null)
            {
                float averageTAT = turnAroundTimes.Count > 0 ? turnAroundTimes.Sum() / turnAroundTimes.Count : 0f;
                averageTATText.text = $"Average TAT: {averageTAT:F2} seconds";
            }

            SpriteRenderer spriteRenderer = currentJob.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.green;
            }

            Destroy(currentJob, 0.5f);
            currentJob = null;
        }
    }
}

public class JobData : MonoBehaviour
{
    public float arrivalTime;
    public float startTime;
    public float completionTime;
    public float burstTime;
}
