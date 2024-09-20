using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenAI;
using TMKOC.Sorting;
using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;

public class ChaptGpt_MCQ
{
    public string question;
    public List<string> options;

    public string correctAnswer;
}

public class ChatGptManager : Singleton<ChatGptManager>
{
    [SerializeField] private InputField inputField;
    [SerializeField] private Button button;
    [SerializeField] private ScrollRect scroll;

    [SerializeField] private RectTransform sent;
    [SerializeField] private RectTransform received;

    private float height;
    private OpenAIApi openai = new OpenAIApi();

    private List<ChatMessage> messages = new List<ChatMessage>();
    private string actAsRadomStranger = "Act as a random stranger in a chat room and reply to the questions. Don't break character. Don't ever mention that you are an AI model.";

    private string actAsMCQ = "Act as a multiple choice question and answer creator. Generate multiple choice question on certain topic and generate four answers options";
    private void Start()
    {
        //  button.onClick.AddListener(SendReply);
    }

    // private void AppendMessage(ChatMessage message)
    // {
    //     scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

    //     var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
    //     item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
    //     item.anchoredPosition = new Vector2(0, -height);
    //     LayoutRebuilder.ForceRebuildLayoutImmediate(item);
    //     height += item.sizeDelta.y;
    //     scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    //     scroll.verticalNormalizedPosition = 0;
    // }

    [Button]
    public void AskQuestion(string message)
    {
        AskMCQ(message, (ChaptGpt_MCQ a)=>
        {

        });
    }

    public void AskMCQ(string topic, Action<ChaptGpt_MCQ> callback)
    {
        AskChatGpt(topic,actAsMCQ, (string response)=>
        {
             // Split the input by "Answer:"
        string[] parts = response.Split(new[] { "Answer:" }, StringSplitOptions.None);
        string questionAndOptions = parts[0].Trim();
        string correctAnswer = parts[1].Trim();
        
        // Extract the question (up to the question mark)
        int questionEndIndex = questionAndOptions.IndexOf('?') + 1;
        string question = questionAndOptions.Substring(0, questionEndIndex).Trim();

        // Extract the options (lines after the question mark)
        string optionsPart = questionAndOptions.Substring(questionEndIndex).Trim();
        List<string> options = new List<string>(optionsPart.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));

        // Create the MCQ object
        ChaptGpt_MCQ mcq = new ChaptGpt_MCQ
        {
            question = question,
            options = options,
            correctAnswer = correctAnswer
        };

            Debug.Log("Qeuestion is :" + mcq.question);
            for(int i = 0;i< mcq.options.Count;i++)
            {   
            Debug.Log("options is :" + mcq.options[i]);

            }
            Debug.Log("Answer is :" + mcq.correctAnswer);

        });
    }

   
    private async void AskChatGpt(string message, string actAs,Action<string> callback)
    {
        // Create the new chat message
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = actAs  // Using prompt here
        };

        // Append additional message if necessary
        if (messages.Count == 0) newMessage.Content = newMessage.Content + "\n" + message;

        // Add the message to the list
        messages.Add(newMessage);


        // Send request to OpenAI API
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo",
            Messages = messages
        });

        // Check for valid response
        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            callback?.Invoke(completionResponse.Choices[0].Message.Content);
        }
        else
        {
            callback?.Invoke("No text was generated from this prompt.");
        }

    }
}
