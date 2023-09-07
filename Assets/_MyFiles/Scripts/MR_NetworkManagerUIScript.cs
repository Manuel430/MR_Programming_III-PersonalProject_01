using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MR_NetworkManagerUIScript : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    [SerializeField] private Button Area_01Button;
    [SerializeField] private Button Area_02Button;
    [SerializeField] private Button Area_03Button;

    private void Awake()
    {
        serverButton.onClick.AddListener(() =>
        {
            Area_01Button.gameObject.SetActive(false);
            Area_02Button.gameObject.SetActive(false);
            Area_03Button.gameObject.SetActive(false);
            NetworkManager.Singleton.StartServer();
        });

        hostButton.onClick.AddListener(() =>
        {
            Area_01Button.gameObject.SetActive(false);
            Area_02Button.gameObject.SetActive(false);
            Area_03Button.gameObject.SetActive(false);
            NetworkManager.Singleton.StartHost();
        });

        clientButton.onClick.AddListener(() =>
        {
            Area_01Button.gameObject.SetActive(false);
            Area_02Button.gameObject.SetActive(false);
            Area_03Button.gameObject.SetActive(false);
            NetworkManager.Singleton.StartClient();
        });
    }
}
