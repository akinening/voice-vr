//SampleConversation.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;

public class SampleConversation : MonoBehaviour {

    private Conversation m_Conversation = new Conversation();
    private string m_WorkspaceID = "dd2ac19c-dbe9-4c0f-8245-a88c1a7a571d"; //各自変更してください
    private string m_Input = "おはよう";

    // Use this for initialization
    void Start () {
        Debug.Log("User: " + m_Input);
        m_Conversation.Message(OnMessage, m_WorkspaceID, m_Input);
    }

    void OnMessage (MessageResponse resp, string customData)
    {
        foreach(Intent mi in resp.intents)
            Debug.Log("intent: " + mi.intent + ", confidence: " + mi.confidence);

        Debug.Log("response: " + resp.output.text[0]);
    }

    // Update is called once per frame
    void Update () {

    }
}
