using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("UI References")]

    public Text DialogText;

    public GameObject OneResponseOption;

    public GameObject TwoResponseOptions;

    public GameObject ThreeResponseOptions;

    public GameObject Highlight;

    public GameObject DialogBackground;


    [Header("Settings")]

    [Range(-1f, 1f)]
    public float HighlightVerticalOffset = 0;
    [Range(-1f, 1f)]
    public float HighlightHorizontalOffset = 0;

    public bool TypewriterEffect = false;

    [HideInInspector]
    public float TypewriterSpeed;

    public string SavePath;

    private Sentence _currentNode;

    // References to the Text objects that make up the response options
    private Text responseOneOne;
    private Text responseTwoOne;
    private Text responseTwoTwo;
    private Text responseThreeOne;
    private Text responseThreeTwo;
    private Text responseThreeThree;

    //private float[][] highlightVerticalPositions = new float[][] {
    //    new float[] { -0.02f },
    //    new float[] { 0.72f, -0.4f },
    //    new float[] { 0.73f, -0.02f, -0.77f }
    //};
    private float[][] highlightVerticalPositions = new float[][] {
        new float[] { 0 },
        new float[] { 0,0 },
        new float[] { 0,0,0 }
    };

    private int numResponses = 1;   // 1, 2, or 3

    private int currentSelection = 0;   // index of response that is currently highlighted

    private bool isListeningForResponse = false;  // true when a user can submit a dialog response, false when the dialog of an npc is being displayed

    private bool endOfConversation = false; // true when the conversation is about to end

    private List<Sentence> DialogGraph;

    private IConversable Speaker;   // Reference to the NPC the player is talking to

    private bool isWindowHidden = false;  // initialize to false so the HideDialogWindow call on start actually hides the window
    
    private void Start()
    {
        Instance = this;    // save singleton reference
        // Get references to the Text objects for the responses
        GameObject text11 = OneResponseOption.transform.GetChild(0).gameObject;
        GameObject text21 = TwoResponseOptions.transform.GetChild(0).gameObject;
        GameObject text22 = TwoResponseOptions.transform.GetChild(1).gameObject;
        GameObject text31 = ThreeResponseOptions.transform.GetChild(0).gameObject;
        GameObject text32 = ThreeResponseOptions.transform.GetChild(1).gameObject;
        GameObject text33 = ThreeResponseOptions.transform.GetChild(2).gameObject;
        responseOneOne = text11.GetComponent<Text>();
        responseTwoOne = text21.GetComponent<Text>();
        responseTwoTwo = text22.GetComponent<Text>();
        responseThreeOne = text31.GetComponent<Text>();
        responseThreeTwo = text32.GetComponent<Text>();
        responseThreeThree = text33.GetComponent<Text>();
        highlightVerticalPositions[0][0] = text11.transform.position.y + HighlightVerticalOffset;
        highlightVerticalPositions[1][0] = text21.transform.position.y + HighlightVerticalOffset;
        highlightVerticalPositions[1][1] = text22.transform.position.y + HighlightVerticalOffset;
        highlightVerticalPositions[2][0] = text31.transform.position.y + HighlightVerticalOffset;
        highlightVerticalPositions[2][1] = text32.transform.position.y + HighlightVerticalOffset;
        highlightVerticalPositions[2][2] = text33.transform.position.y + HighlightVerticalOffset;
        Vector3 initHighlightPos = Highlight.transform.position;
        Highlight.transform.position = new Vector3(initHighlightPos.x + HighlightHorizontalOffset, initHighlightPos.y, initHighlightPos.z);
        HideDialogWindow();
    }

    private void Update()
    {
        if (!isWindowHidden)
        {
            if (isListeningForResponse)
            {
                ListenForResponse();
            }
            else
            {
                DisplayCharacterDialog();
            }
        }
    }

    /// <summary>
    /// Starts the conversation.
    /// </summary>
    /// <param name="Conversation">The dialog graph that will be interpreted.</param>
    /// <param name="NPC">Reference to the NPC that the player is going to talk with.</param>
    public void StartConversation(List<Sentence> Conversation, IConversable NPC)
    {
        this.Speaker = NPC;
        this.DialogGraph = Conversation;
        UpdateDialog(this.DialogGraph[0]);
        currentSelection = 0;
        endOfConversation = false;
        isWindowHidden = false;
        DialogBackground.SetActive(true);
    }

    /// <summary>
    /// Updates the dialog UI with data from a DialogNode instance.
    /// </summary>
    /// <param name="Dialog">Dialog.</param>
    public void UpdateDialog(Sentence Dialog)
    {
        isListeningForResponse = false;
        _currentNode = Dialog;

        // Hide the dialog reponses, show the dialog text for the character's dialog output
        DialogText.gameObject.SetActive(true);
        OneResponseOption.SetActive(false);
        TwoResponseOptions.SetActive(false);
        ThreeResponseOptions.SetActive(false);
        Highlight.SetActive(false);

        if (_currentNode.Responses.Count == 0)
        {
            endOfConversation = true;
        }
        else
        {
            numResponses = _currentNode.Responses.Count;
        }

        if (TypewriterEffect)
        {
            DisplayTextWithTypewritterEffect(DialogText, _currentNode.Text);
        }
        else
        {
            DialogText.text = _currentNode.Text;
        }
    }

    // Navigate through dialog options with the arrow keys
    // Select dialog option when enter key is pressed
    private void ListenForResponse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelection--;
            if (currentSelection < 0)
            {
                currentSelection = numResponses - 1;
            }
            UpdateHighlightPosition();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelection++;
            if (currentSelection == numResponses)
            {
                currentSelection = 0;
            }
            UpdateHighlightPosition();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            // Proceed to the next dialog node, and invoke a function if this dialog path specifies it via the 
            // selected DialogResponse instance's triggerFunctionName property.
            // If triggerFunctionName is an empty string, no function will be invoked.
            Response selectedResponse = _currentNode.Responses[currentSelection];
            if (selectedResponse.Trigger != "")
            {
                // Invoke can only be called on a MonoBehaviour instance, so get the MonoBehaviour from the same instance of the IConversable instance 
                (this.Speaker as MonoBehaviour).Invoke(selectedResponse.Trigger, 0);
            }
            UpdateDialog(DialogGraph[selectedResponse.NextId]);
        }
    } // end ListenForResponse method

    // Shows the character's dialog, and prepares text responses for user
    private void DisplayCharacterDialog()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // If presseing enter, the typewritter effect is enabled, and the sentence has not been finished, finish it
            if (TypewriterEffect && indexInSentence < fullText.Length)
            {
                indexInSentence = fullText.Length;
                textElementRef.text = fullText;
            }
            // Otherwise, show the response options for the dialog prompt
            else
            {
                if (endOfConversation)
                {
                    HideDialogWindow();
                }
                else
                {
                    // Set and show the reponses the user can submit for the dialog prompt
                    switch (numResponses)
                    {
                        case 1:
                            // If there is only one response and its string is empty,
                            // this is an indication that no user response is necessary, 
                            // so proceed immediately to the next dialog prompt.
                            if (_currentNode.Responses[0].Text == "")
                            {
                                UpdateDialog(DialogGraph[_currentNode.Responses[0].NextId]);
                                return;
                            }
                            else
                            {
                                responseOneOne.text = _currentNode.Responses[0].Text;
                                OneResponseOption.SetActive(true);
                                TwoResponseOptions.SetActive(false);
                                ThreeResponseOptions.SetActive(false);
                            }
                            break;
                        case 2:
                            OneResponseOption.SetActive(false);
                            TwoResponseOptions.SetActive(true);
                            ThreeResponseOptions.SetActive(false);
                            responseTwoOne.text = _currentNode.Responses[0].Text;
                            responseTwoTwo.text = _currentNode.Responses[1].Text;
                            break;
                        case 3:
                            OneResponseOption.SetActive(false);
                            TwoResponseOptions.SetActive(false);
                            ThreeResponseOptions.SetActive(true);
                            responseThreeOne.text = _currentNode.Responses[0].Text;
                            responseThreeTwo.text = _currentNode.Responses[1].Text;
                            responseThreeThree.text = _currentNode.Responses[2].Text;
                            break;
                    }
                    Highlight.SetActive(true);
                    DialogText.gameObject.SetActive(false);
                    currentSelection = 0;
                    isListeningForResponse = true;  // Changes central behaviour in the Update method
                    UpdateHighlightPosition();
                }
            }
        }
    } // end DisplayCharacterDialog method

    // Moves the text response highlight to position next to the current selection
    private void UpdateHighlightPosition()
    {
        Vector3 initHighlightPos = Highlight.transform.position;
        Highlight.transform.position = new Vector3(initHighlightPos.x, highlightVerticalPositions[numResponses - 1][currentSelection], initHighlightPos.z);
    }

    public void HideDialogWindow()
    {
        if (!isWindowHidden)
        {
            SetUIElementsActive(false);
            isWindowHidden = true;
        }
    }

    public void ShowDialogWindow()
    {
        if (isWindowHidden)
        {
            SetUIElementsActive(true);
            isWindowHidden = false;
        }
    }

    private void SetUIElementsActive(bool active)
    {
        this.DialogText.gameObject.SetActive(active);
        this.OneResponseOption.SetActive(active);
        this.TwoResponseOptions.SetActive(active);
        this.ThreeResponseOptions.SetActive(active);
        this.Highlight.SetActive(active);
        this.DialogBackground.SetActive(active);
    }

    // Stuff for the typewritter text effect:
    private string fullText = "";

    private string currentText = "";

    private Text textElementRef;

    private int indexInSentence = 0;

    private void DisplayTextWithTypewritterEffect(Text textElement, string sentence)
    {
        this.fullText = sentence;
        this.textElementRef = textElement;
        StartCoroutine(TypewriterText());
    }

    private IEnumerator TypewriterText()
    {
        for (indexInSentence = 0; indexInSentence <= fullText.Length; indexInSentence++)
        {
            currentText = fullText.Substring(0, indexInSentence);
            textElementRef.text = currentText;
            yield return new WaitForSeconds(1 / TypewriterSpeed);
        }
    }

} // end DialogManager class
