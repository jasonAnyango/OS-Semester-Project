using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Required for Linq methods
using TMPro; // For UI elements

public class SJFController : MonoBehaviour
{
    public Transform queueArea;
    public Transform processingArea;
    public GameObject jobPrefab;

    public TMP_Text avgWaitingTimeText;
    public TMP_Text avgTATText;
    public TMP_Text processTimerText;

    private List<GameObject> jobQueue = new List<GameObject>();
    private GameObject currentJob = null;

    private float timer = 0f;
    private List<float> waitingTimes = new List<float>();
    private List<float> turnaroundTimes = new List<float>();

    private Color originalColor = Color.white; // Default job color
    private Color processingColor = Color.green; // Color for the active process

    // Add a job to the queue
    public void AddJobToQueue()
    {
        if (jobPrefab != null && queueArea != null)
        {
            GameObject newJob = Instantiate(jobPrefab, queueArea);
            float xOffset = 100f; // Horizontal spacing
            newJob.transform.localPosition = new Vector3(xOffset * jobQueue.Count, 0, 0);
            newJob.name = $"Job {jobQueue.Count + 1}";

            // Add job data
            SJFJobData jobData = newJob.AddComponent<SJFJobData>();
            jobData.arrivalTime = Time.time;
            jobData.burstTime = Random.Range(2f, 8f); // Assign random burst times
            jobData.remainingTime = jobData.burstTime;

            jobQueue.Add(newJob);
        }
    }

    // Processing logic
    private void Update()
    {
        if (jobQueue.Count > 0)
        {
            // Find the job with the shortest remaining time
            GameObject shortestJob = jobQueue.OrderBy(job => job.GetComponent<SJFJobData>().remainingTime).First();

            if (currentJob == null || shortestJob != currentJob)
            {
                // Preempt the current job
                if (currentJob != null)
                {
                    Debug.Log($"{currentJob.name} preempted. Remaining time: {currentJob.GetComponent<SJFJobData>().remainingTime}");
                    ResetJobColor(currentJob); // Restore original color
                }

                currentJob = shortestJob;
                currentJob.transform.SetParent(processingArea);
                StartCoroutine(MoveToProcessing(currentJob));

                // Change job color to indicate processing
                ChangeJobColor(currentJob, processingColor);
            }

            // Process the job
            ProcessJob();
        }
    }

    private void ProcessJob()
    {
        if (currentJob != null)
        {
            SJFJobData jobData = currentJob.GetComponent<SJFJobData>();
            timer += Time.deltaTime;

            // Update process timer UI
            processTimerText.text = $"Processing Time: {jobData.remainingTime:F1}s";

            // Reduce remaining time
            jobData.remainingTime -= Time.deltaTime;

            // Check if processing is complete
            if (jobData.remainingTime <= 0f)
            {
                jobData.completionTime = Time.time;

                // Calculate TAT and WT
                float tat = jobData.completionTime - jobData.arrivalTime;
                float wt = tat - jobData.burstTime;

                turnaroundTimes.Add(tat);
                waitingTimes.Add(wt);

                Debug.Log($"{currentJob.name}: TAT = {tat}, WT = {wt}");

                // Update averages
                UpdateUI();

                // Remove the job from the queue and destroy it
                jobQueue.Remove(currentJob);
                Destroy(currentJob);
                currentJob = null;

                // Reset process timer UI
                processTimerText.text = "Processing Time: --";
            }
        }
    }

    private void UpdateUI()
    {
        float avgTAT = turnaroundTimes.Average();
        float avgWT = waitingTimes.Average();

        avgTATText.text = $"Avg TAT: {avgTAT:F2}s";
        avgWaitingTimeText.text = $"Avg WT: {avgWT:F2}s";
    }

    // Coroutine to handle smooth movement
    private IEnumerator MoveToProcessing(GameObject job)
    {
        Vector3 startPosition = job.transform.position;
        Vector3 targetPosition = processingArea.position;
        float duration = 1f; // Animation duration
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            job.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        job.transform.position = targetPosition;
    }

    // Change job color
    private void ChangeJobColor(GameObject job, Color color)
    {
        SpriteRenderer renderer = job.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = color;
        }
    }

    // Reset job color
    private void ResetJobColor(GameObject job)
    {
        SpriteRenderer renderer = job.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = originalColor;
        }
    }
}

// JobData Component
public class SJFJobData : MonoBehaviour
{
    public float arrivalTime;
    public float burstTime;
    public float remainingTime;
    public float completionTime;
}
