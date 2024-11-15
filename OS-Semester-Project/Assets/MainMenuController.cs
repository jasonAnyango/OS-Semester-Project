using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Methods to control actions of buttons in Main Menu

    // 1. Load the CPU Scheduler scene
    public void loadCPUScheduler()
    {
        SceneManager.LoadScene("CPUSchedulerScene");
    }
    
    // 2. Load the Process Synchronization scene
    public void loadProcessSynchronization()
    {
        SceneManager.LoadScene("ProcessSynchronizationScene");
    }

    // 3. Load the Memory Management scene
    public void loadMemoryManagement()
    {
        SceneManager.LoadScene("MemoryManagementScene");
    }
}
