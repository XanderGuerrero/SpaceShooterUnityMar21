using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using System;

public class MenuManager : MonoBehaviour
{
    [Header("Join Now")]
    [SerializeField]
    private GameObject joinContainer;

    [SerializeField]
    private Button QueueButton;

    [Header("Sign In")]
    [SerializeField]
    private GameObject signInContainer;

    private string username;
    private string password;



    //where all events are stored per socket connection
    private SocketIOComponent socketReference;
    //retrun the socket reference
    private SocketIOComponent SocketReference
    {
        get
        {
            return socketReference = (socketReference == null) ? FindObjectOfType<NetworkClient>() : socketReference;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //womt accept button input
        QueueButton.interactable = false;
        signInContainer.SetActive(false);
        joinContainer.SetActive(false);

        NetworkClient.OnSignInComplete += OnSignInComplete;

        SceneManagementManager.Instance.LoadLevel(SceneList.ONLINE, (levelName) => {
            signInContainer.SetActive(true);
            joinContainer.SetActive(false);
            QueueButton.interactable = true;
        });
    }

    // Update is called once per frame
    public void OnQueue()
    {
        Debug.Log("Joining the game!!!!!!!");
        //call the servers join game event
        SocketReference.Emit("joinGame");
    }

    public void OnSignIn()
    {
        SocketReference.Emit("signIn", new JSONObject(JsonUtility.ToJson(new SignInData()
        {
            username = username,
            password = password
        })));
    }

    public void OnSignInComplete()
    {
        signInContainer.SetActive(false);
        joinContainer.SetActive(true);
        QueueButton.interactable = true;
    }


    public void OnCreateAccount()
    {
        SocketReference.Emit("createAccount", new JSONObject(JsonUtility.ToJson(new SignInData()
        {
            username = username,
            password = password
        })));
    }

    public void EditUsername(string text)
    {
        username = text;
    }


    public void EditPassword(string text)
    {
        password = text;
    }

}


[Serializable]
public class SignInData
{
    public string username;
    public string password;
}