using System.Collections.Generic;
using UnityEngine;

public class GuidePageNavigation : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;
    
    private int currentPage;

    void Start()
    {
        // Find which page is currently active
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i].activeSelf)
            {
                currentPage = i;
                break;
            }
        }
    }

    // void OnEnable() // Re-check active page each time, when enabled. Replace Start() if this dont use, DONT DESTORY ON LOAD
    // {
    //     for (int i = 0; i < pages.Count; i++)
    //     {
    //         if (pages[i].activeSelf)
    //         {
    //             currentPage = i;
    //             break;
    //         }
    //     }
    // }

    public void Next()
    {
        pages[currentPage].SetActive(false);

        currentPage = (currentPage + 1) % pages.Count;

        pages[currentPage].SetActive(true);
    }

    public void Previous()
    {
        pages[currentPage].SetActive(false);

        currentPage = (currentPage - 1 + pages.Count) % pages.Count;

        pages[currentPage].SetActive(true);
    }
}
