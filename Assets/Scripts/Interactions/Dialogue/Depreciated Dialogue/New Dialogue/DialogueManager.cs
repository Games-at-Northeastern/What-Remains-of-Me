using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private string _name;
    public Text _dialogue_text;
    public Text _name_text;
    public GameObject _textBox;

    // Start is called before the first frame update
    void Start()
    {
        this.sentences = new Queue<string>();
        this._name = "";
        this.gameObject.SetActive(false);
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        this.sentences.Clear();
        this._textBox.SetActive(true);
        foreach (var sentence in dialogue.Sentences)
        {
            this.sentences.Enqueue(sentence);
        }

        this._name = dialogue.Name;

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        this._dialogue_text.text = sentence;
        this._name_text.text = this._name;
    }

    public void EndDialogue()
    {
        this._textBox.SetActive(false);

    }
}
