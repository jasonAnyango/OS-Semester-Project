using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ReplacementScript : MonoBehaviour
{
    [Header("Prefabs and Parents")]
    public GameObject pagePrefab;
    public Transform queueParent; // Horizontal layout group for the queue
    public Transform[] slots; // Assign the 4 slot panels in the Inspector

    [Header("Settings")]
    public int totalPages = 15; // Total number of pages to spawn
    public int minPageNumber = 1;
    public int maxPageNumber = 9;

    private Queue<int> pageQueue = new Queue<int>(); // Queue to manage page requests
    private int[] slotPages; // Stores the current page numbers in each slot
    private bool isProcessing = false; // Prevents multiple replacement processes from starting

    void Start()
    {
        slotPages = new int[slots.Length]; // Initialize slot array
        for (int i = 0; i < slotPages.Length; i++)
        {
            slotPages[i] = -1; // -1 means empty slot
        }
    }

    // Button function to spawn pages into the queue
    public void SpawnPages()
    {
        // Prevent spawning multiple sets of pages
        if (pageQueue.Count > 0)
        {
            Debug.LogWarning("Pages already spawned!");
            return;
        }

        float offset = 60f; // Spacing between pages in the queue (adjust as needed)
        Vector3 startPosition = queueParent.position; // Start position for spawning pages

        for (int i = 0; i < totalPages; i++)
        {
            // Create the page prefab and set its parent to queueParent
            GameObject newPage = Instantiate(pagePrefab, queueParent);

            // Calculate the new position based on the offset for horizontal alignment
            RectTransform rectTransform = newPage.GetComponent<RectTransform>();

            // Apply the offset to the position. The offset will be in the x direction based on the index
            rectTransform.anchoredPosition = new Vector2(i * offset, 0); // Add the offset to the position

            // Assign a random page number to the page and update the TMP text
            int pageNumber = Random.Range(minPageNumber, maxPageNumber + 1);
            newPage.GetComponentInChildren<TextMeshProUGUI>().text = pageNumber.ToString();

            // Add the page number to the queue
            pageQueue.Enqueue(pageNumber);
        }

        Debug.Log($"Spawned {totalPages} pages with random numbers and added them to the queue.");
    }

    // Button function to start processing pages
    public void StartReplacement()
    {
        if (isProcessing)
        {
            Debug.LogWarning("Replacement process already running!");
            return;
        }

        if (pageQueue.Count == 0)
        {
            Debug.LogWarning("No pages in the queue. Please spawn pages first.");
            return;
        }

        isProcessing = true;
        StartCoroutine(ProcessPages());
    }

    IEnumerator ProcessPages()
    {
        while (pageQueue.Count > 0)
        {
            int currentPage = pageQueue.Dequeue();
            Destroy(queueParent.GetChild(0).gameObject); // Remove the visual page from the queue
            yield return new WaitForSeconds(1f); // Delay for visualization

            if (IsPageInSlots(currentPage))
            {
                Debug.Log($"Page {currentPage} is already in a slot (Page Hit).");
                continue;
            }

            int emptySlotIndex = FindEmptySlot();

            if (emptySlotIndex != -1)
            {
                // Place the page in an empty slot
                slotPages[emptySlotIndex] = currentPage;
                MovePageToSlot(currentPage, slots[emptySlotIndex]);
                Debug.Log($"Page {currentPage} placed in slot {emptySlotIndex}.");
            }
            else
            {
                // Replace a page using the Optimal algorithm
                int replaceIndex = FindOptimalReplacementIndex();
                Debug.Log($"Replacing page {slotPages[replaceIndex]} with page {currentPage} in slot {replaceIndex}.");
                slotPages[replaceIndex] = currentPage;
                ReplacePageInSlot(currentPage, slots[replaceIndex]);
            }
        }

        Debug.Log("All pages processed.");
        isProcessing = false;
    }

    bool IsPageInSlots(int page)
    {
        return slotPages.Contains(page);
    }

    int FindEmptySlot()
    {
        for (int i = 0; i < slotPages.Length; i++)
        {
            if (slotPages[i] == -1)
                return i;
        }
        return -1;
    }

    int FindOptimalReplacementIndex()
    {
        int replaceIndex = -1;
        int furthestUse = -1;

        for (int i = 0; i < slotPages.Length; i++)
        {
            int nextUse = FindNextUse(slotPages[i]);
            if (nextUse == -1) // Not used again
            {
                return i;
            }
            else if (nextUse > furthestUse)
            {
                furthestUse = nextUse;
                replaceIndex = i;
            }
        }

        return replaceIndex;
    }

    int FindNextUse(int page)
    {
        int index = 0;
        foreach (int queuedPage in pageQueue)
        {
            if (queuedPage == page)
                return index;
            index++;
        }
        return -1; // Not used again
    }

    void MovePageToSlot(int pageNumber, Transform slot)
    {
        GameObject newPage = Instantiate(pagePrefab, slot);
        newPage.GetComponentInChildren<TextMeshProUGUI>().text = pageNumber.ToString();
    }

    void ReplacePageInSlot(int pageNumber, Transform slot)
    {
        foreach (Transform child in slot)
        {
            Destroy(child.gameObject);
        }

        MovePageToSlot(pageNumber, slot);
    }
}
