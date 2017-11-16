//WatsonConversation.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;

public class WatsonConversation : MonoBehaviour {

    [SerializeField]
    SpeechToText m_SpeechToText = new SpeechToText();
    TextToSpeech m_TextToSpeech = new TextToSpeech();
    private Conversation m_Conversation = new Conversation();
    private string m_WorkspaceID = "dd2ac19c-dbe9-4c0f-8245-a88c1a7a571d";

    private Animator animator; //ここを追加
    private const string key_isGreet = "isGreet"; //ここを追加
    
    bool point = false;

    // Use this for initialization
    IEnumerator Start() {
        this.animator = GetComponent<Animator>(); //ここを追加
        var audioSource = GetComponent<AudioSource>();

        while (true) {
            // 視点が重なっているときのみ実行
            if (point) {
                yield return RecMic(audioSource);
            } else {
                yield return null;
            }
        }
        yield return null;
    }

    IEnumerator RecMic(AudioSource audioSource) {
        Debug.Log ("Start record");
        audioSource.clip = Microphone.Start(null, true, 4, 44100);
        audioSource.loop = false;
        audioSource.spatialBlend = 0.0f;
        yield return new WaitForSeconds (2.0f);
        Microphone.End (null);
        Debug.Log ("Finish record");

        // 音声の認識言語を日本語に指定
        m_SpeechToText.RecognizeModel = "ja-JP_BroadbandModel";
        // 音声をテキストに変換し、関数：HandleOnRecognize()を呼ぶ
        m_SpeechToText.Recognize(audioSource.clip, HandleOnRecognize);

    }

    void HandleOnRecognize(SpeechRecognitionEvent result){
        if (result != null && result.results.Length > 0) {
            foreach (var res in result.results) {
                foreach (var alt in res.alternatives) {
                    //集音した音声データを文字化したものを text に格納
                    string text = alt.transcript;
                    Debug.Log (string.Format ("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));

                    //text を Conversation Service に送って処理
                    m_Conversation.Message(OnMessage, m_WorkspaceID, text);
                }
            }
        } else {
            //音声ファイルが空あった場合の処理
            Debug.Log ("何も聞き取ってくれないときもある");
        }
    }

    void OnMessage (MessageResponse resp, string customData)
    {
        if (resp != null) {
            foreach (Intent mi in resp.intents)
                Debug.Log ("intent: " + mi.intent + ", confidence: " + mi.confidence);

            if (resp.output.text [0].Contains ("やっほー")) {
                this.animator.SetBool (key_isGreet, true);
            }

            //TextToSpeech の音声タイプを指定
            m_TextToSpeech.Voice = VoiceType.ja_JP_Emi;
            //Conversation Service から返された文字列を TextToSpeech にて発音
            m_TextToSpeech.ToSpeech (resp.output.text[0], HandleToSpeechCallback);

            Debug.Log ("response: " + resp.output.text[0]);
        }
    }

    void HandleToSpeechCallback (AudioClip clip) {
        PlayClip(clip);
    }

    private void PlayClip(AudioClip clip) {
        if (Application.isPlaying && clip != null) {
            GameObject audioObject = new GameObject("AudioObject");
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.spatialBlend = 0.0f;
            source.loop = false;
            source.clip = clip;
            source.Play();

            GameObject.Destroy(audioObject, clip.length);
        }
    }
    
    // Update is called once per frame
    void Update () {

    }
    
    public void pointerEnter(){
        Debug.Log("pointer Enter");
        point = true;
    }

    public void pointerExit(){
        Debug.Log("pointer Exit");
        point = false;
    }
}
