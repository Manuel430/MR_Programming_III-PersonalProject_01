using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MR_NetworkManagerUIScript : NetworkBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    public override void OnNetworkSpawn()
    {
        joinCodeText.text = MR_TestRelayScript.Instance.JoinCode;
    }

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            hostButton.gameObject.SetActive(false);
            clientButton.gameObject.SetActive(false);
            joinCodeInput.gameObject.SetActive(false);

            MR_TestRelayScript.Instance.CreateRelay();

            joinCodeText.gameObject.SetActive(true);

        });

     clientButton.onClick.AddListener(() =>
        {
            if(string.IsNullOrEmpty(joinCodeInput.text))
            {
                return;
            }

            hostButton.gameObject.SetActive(false);
            clientButton.gameObject.SetActive(false);
            joinCodeInput.gameObject.SetActive(false);

            MR_TestRelayScript.Instance.JoinRelay(joinCodeInput.text);
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
