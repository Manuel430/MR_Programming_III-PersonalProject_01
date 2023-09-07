using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MR_AreaDecider : NetworkBehaviour
{
    [Header ("Buttons")]
    [SerializeField] private Button arenaButton_01;
    [SerializeField] private Button arenaButton_02;
    [SerializeField] private Button arenaButton_03;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button ownerButton;
    [SerializeField] private Button clientButton;

    [Header("Arenas")]
    public GameObject Arena_01;
    public GameObject Arena_02;
    public GameObject Arena_03;

    [Header("WorldSet")]
    public int worldChoice;
    public int ownerChoice;
    public int clientChoice;

    private void Awake()
    {
        arenaButton_01.onClick.AddListener(() =>
        {
            worldChoice = 1;
            SetWorld();
        });

        arenaButton_02.onClick.AddListener(() =>
        {
            worldChoice = 2;
            SetWorld();
        });

        arenaButton_03.onClick.AddListener(() =>
        {
            worldChoice = 3;
            SetWorld();
        });

        ownerButton.onClick.AddListener(() =>
        {
            FinalChoice();

        });
    }
    private void SetWorld()
    {
        if (worldChoice == 1)
        {
            Arena_01.gameObject.SetActive(true);
            Arena_02.gameObject.SetActive(false);
            Arena_03.gameObject.SetActive(false);
        }
        else if(worldChoice == 2)
        {
            Arena_01.gameObject.SetActive(false);
            Arena_02.gameObject.SetActive(true);
            Arena_03.gameObject.SetActive(false);
        }
        else
        {
            Arena_01.gameObject.SetActive(false);
            Arena_02.gameObject.SetActive(false);
            Arena_03.gameObject.SetActive(true);
        }
    }

    private void FinalChoice()
    {
        if (!IsHost)
        {
            ownerChoice = worldChoice;
        }
        else
        {
            clientChoice = worldChoice;
        }

        if(ownerChoice == clientChoice)
        {
            return;
        }
        else
        {
            worldChoice = ownerChoice;
            SetWorld();
        }
    }
}
