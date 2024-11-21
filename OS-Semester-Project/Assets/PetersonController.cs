using System.Collections;
using UnityEngine;
using TMPro;

public class PetersonController : MonoBehaviour
{
    // UI Elements for flag[] and turn
    public TMP_Text flagText0;
    public TMP_Text flagText1;
    public TMP_Text turnText;

    // GameObjects representing processes and critical section
    public RectTransform process0Panel;
    public RectTransform process1Panel;
    public RectTransform criticalSectionPanel;

    // Animation speed
    public float movementSpeed = 2.0f;

    // Shared variables
    private bool[] flag = new bool[2]; // flag[0] and flag[1]
    private int turn;                  // Turn variable

    // Original positions for processes
    private Vector3 process0OriginalPosition;
    private Vector3 process1OriginalPosition;

    void Start()
    {
        // Store original positions
        process0OriginalPosition = process0Panel.anchoredPosition;
        process1OriginalPosition = process1Panel.anchoredPosition;

        ResetSimulation();
    }

    public void ResetSimulation()
    {
        // Initialize flags and turn
        flag[0] = false;
        flag[1] = false;
        turn = -1;

        // Update UI
        flagText0.text = "Flag[0] = False";
        flagText1.text = "Flag[1] = False";
        turnText.text = "Turn = None";

        // Reset positions
        process0Panel.anchoredPosition = process0OriginalPosition;
        process1Panel.anchoredPosition = process1OriginalPosition;
    }

    public void StartSimulation()
    {
        StartCoroutine(RunProcesses());
    }

    IEnumerator RunProcesses()
    {
        // Alternate execution of Process0 and Process1
        yield return StartCoroutine(ProcessRoutine(0));
        yield return StartCoroutine(ProcessRoutine(1));
    }

    IEnumerator ProcessRoutine(int id)
    {
        // Request access to the critical section
        flag[id] = true;
        if (id == 0) flagText0.text = "Flag[0] = True";
        else flagText1.text = "Flag[1] = True";

        turn = 1 - id; // Set the turn to the other process
        turnText.text = "Turn = " + turn.ToString();

        // Wait until it's safe to enter the critical section
        yield return new WaitUntil(() => CanEnterCriticalSection(id));

        // Move the process to the critical section
        if (id == 0)
        {
            yield return StartCoroutine(MoveToPosition(process0Panel, criticalSectionPanel.anchoredPosition));
        }
        else
        {
            yield return StartCoroutine(MoveToPosition(process1Panel, criticalSectionPanel.anchoredPosition));
        }

        yield return new WaitForSeconds(2.0f); // Simulate time in critical section

        // Exit the critical section
        if (id == 0)
        {
            yield return StartCoroutine(MoveToPosition(process0Panel, process0OriginalPosition));
            flagText0.text = "Flag[0] = False";
        }
        else
        {
            yield return StartCoroutine(MoveToPosition(process1Panel, process1OriginalPosition));
            flagText1.text = "Flag[1] = False";
        }
    }

    bool CanEnterCriticalSection(int id)
    {
        int otherId = 1 - id; // The other process
        return !flag[otherId] || turn != id;
    }

    IEnumerator MoveToPosition(RectTransform process, Vector3 targetPosition)
    {
        Vector3 startPosition = process.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * movementSpeed;
            process.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            yield return null;
        }

        process.anchoredPosition = targetPosition;
    }
}
