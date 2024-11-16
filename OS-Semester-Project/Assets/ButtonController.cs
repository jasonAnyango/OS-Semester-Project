using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonController : MonoBehaviour
{
    // Load the CPU Scheduler scene
    public void loadCPUScheduler()
    {
        SceneManager.LoadScene("CPUSchedulerScene");
    }

    // Load the Process Synchronization scene
    public void loadProcessSynchronization()
    {
        SceneManager.LoadScene("ProcessSynchronizationScene");
    }

    //  Load the Memory Management scene
    public void loadMemoryManagement()
    {
        SceneManager.LoadScene("MemoryManagementScene");
    }
    // Method to go back to the main menu scene
    public void backToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Method to go to FCFS
    public void goToFCFS()
    {
        SceneManager.LoadScene("FCFS");
    }

    // Method to go to SJF(Preemptive)
    public void goToSJF()
    {
        SceneManager.LoadScene("SJF");
    }
}
