using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkUi : MonoBehaviour
{
    public Button joinButton;
    public Button hostButton;
    public GameObject hostPanel;
    public Text ipAddressText;

    public void HostGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ReceiveGameBroadcast()
    {
        CustomNetworkDiscovery.Instance.ReceiveBroadcast();
    }

    public void JoinGame()
    {
        NetworkManager.singleton.networkAddress = ipAddressText.text;
        NetworkManager.singleton.StartClient();
        CustomNetworkDiscovery.Instance.StopBroadcasting();
    }

    public void OnReceiveBroadcast(string fromIp, string data)
    {
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        ipAddressText.text = fromIp;
        hostPanel.SetActive(true);
    }

    void Start()
    {
        CustomNetworkDiscovery.Instance.onServerDetected += OnReceiveBroadcast;
    }

    void OnDestroy()
    {
        CustomNetworkDiscovery.Instance.onServerDetected -= OnReceiveBroadcast;
    }
}
