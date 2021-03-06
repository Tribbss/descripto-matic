﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Linq;

public class ListCreator : MonoBehaviour 
{
    [System.Serializable]
    public struct Content
    {
		// Transform to parent the created gameobject to
        public Transform Parent;
		// File name to grab the text from
        public string FileName;
    }
    
    [SerializeField]
    private Content[] content; // A array of custom structs to hold the parent transform and the file name 

    [SerializeField]
    private GameObject textPrefab; // GameObject to create for the text item


    #region Individual Lists
	// Lists to hold all of the data from the .txt files
    private List<string> bodyStrings = new List<string>();
    private List<string> descriptionStrings = new List<string>();
    private List<string> thingStrings = new List<string>();
    private List<string> themeStrings = new List<string>();
    private List<string> flairStrings = new List<string>();

	// List of GameObjects to hold the actual object we create
    private List<GameObject> listItems = new List<GameObject>();
    #endregion

	#region List Getters
    public List<string> BodyStrings { get { return bodyStrings; } }
    public List<string> DescriptionStrings { get { return descriptionStrings; } }
    public List<string> ThingStrings { get { return thingStrings; } }
    public List<string> ThemeStrings { get { return themeStrings; } }
    public List<string> FlairStrings { get { return flairStrings; } }
	#endregion

    private void Start () 
	{
		// Initialize the program
        Init();
	}

    private void Init()
    {
		// Clear lists
        ClearLists();
		
		// Destroy all UI list items
		DestroyListUI();
		
	    // for each content we have
        for (int i = 0; i < content.Length; i++)
        {
			// Begin the population of the lists
            StartDescriptor(content[i], i);
        }
    }

	// Clear our lists of all data
    private void ClearLists()
    {
        bodyStrings.Clear();
        descriptionStrings.Clear();
        thingStrings.Clear();
        themeStrings.Clear();
        flairStrings.Clear();
    }

	// Based on the index, populate the correct list
    private void StartDescriptor(Content currentContent, int contentIndex)
    {
        string listFileName = currentContent.FileName;
        string fileContents = ReadFromFile(listFileName);
        List<string> temp = new List<string>();

        switch (contentIndex)
        {
            case 0: // body
                temp = BreakTextByLine(fileContents);
                PopulateList(bodyStrings, temp);
                CreateListUI(bodyStrings, currentContent.Parent);
                WriteToFile(currentContent.FileName, fileContents);
                break;

            case 1: // description
                temp = BreakTextByLine(fileContents);
                PopulateList(descriptionStrings, temp);
                CreateListUI(descriptionStrings, currentContent.Parent);
                WriteToFile(currentContent.FileName, fileContents);
                break;

            case 2: // thing
                temp = BreakTextByLine(fileContents);
                PopulateList(thingStrings, temp);
                CreateListUI(thingStrings, currentContent.Parent);
                WriteToFile(currentContent.FileName, fileContents);
                break;

            case 3: // theme
                temp = BreakTextByLine(fileContents);
                PopulateList(themeStrings, temp);
                CreateListUI(themeStrings, currentContent.Parent);
                WriteToFile(currentContent.FileName, fileContents);
                break;

            case 4: // flair
                temp = BreakTextByLine(fileContents);
                PopulateList(flairStrings, temp);
                CreateListUI(flairStrings, currentContent.Parent);
                WriteToFile(currentContent.FileName, fileContents);
                break;
        }
    }
    
	// Create Listitem
    private void CreateListUI(List<string> list, Transform parent)
    {
        foreach (string s in list)
        {
            GameObject textGO = Instantiate(textPrefab, parent);
            textGO.GetComponent<Text>().text = s;
            listItems.Add(textGO);
        }
    }

	// Destroy listitems
    private void DestroyListUI()
    {
        if (listItems.Count <= 0)
            return;

        foreach (GameObject go in listItems)
        {
            Destroy(go);
        }
        listItems = new List<GameObject>();
    }

	// Populates list a with everything in list b
    private void PopulateList(List<string> a, List<string> b)
    {
        a.AddRange(b);
    }

	// Splits text by comma and returns result as a list
    private List<string> BreakTextByLine(string text)
    {
        string[] result = text.Split(',');
        return result.ToList();
    }
 
    private string ReadFile(string listFileName)
    {
        StringBuilder sb = new StringBuilder();
        TextAsset txt = (TextAsset)Resources.Load(listFileName, typeof(TextAsset));

        sb.Append(txt.text);

        return sb.ToString();
    }

    private string ReadFromFile(string fileName)
    {
        string result = "";
        string filePath = Application.dataPath + "/Lists/" + fileName + ".txt";

        result = File.ReadAllText(filePath);

        return result;
    }

    private List<string> BreakLines(string text)
    {
        return text.Split(',').ToList();
    }

    private void WriteToFile(string fileName, string contents)
    {
        string filePath = Application.dataPath + "/Lists/" + fileName + ".txt";
        StreamWriter sw = File.CreateText(filePath);
        sw.Write(contents);
        sw.Close();
    }

    private void AppendToFile(string fileName, string addition)
    {
        string filePath = Application.dataPath + "/Lists/" + fileName + ".txt";
        string current = ReadFromFile(fileName);
        current += "," + addition;
        StreamWriter sw = File.CreateText(filePath);
        sw.Write(current);
        sw.Close();
        Init();
    }
    
    // This is super bad and I need to clean it all up.
    public void AddTextButton(string filename)
    {
        GameObject btnGO = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        GameObject textHolder = GetSibling(btnGO.transform.parent, btnGO.transform);
        InputField field = FindInputFieldInChildren(textHolder.transform.GetChild(0));
        Text displayTxt = textHolder.transform.GetChild(0).transform.Find("DisplayText").GetComponent<Text>();
        displayTxt.text = "";
        Text txt = FindTextInChilren(textHolder.transform.GetChild(0));
        AddText(txt.text, filename);
        txt.text = "";
        field.text = "";
        displayTxt.text = "";
    }

    private Text FindTextInChilren(Transform current)
    {
        foreach (Transform child in current)
        {
            if (child.name.Contains("InputText"))
            {
                return child.GetComponent<Text>();
            }
        }

        Debug.LogError("Could not find InputText child");
        return null;
    }

    private InputField FindInputFieldInChildren(Transform current)
    {
        return current.GetComponent<InputField>();
    }

    private GameObject GetSibling(Transform parent, Transform current)
    {
        foreach (Transform child in parent)
        {
            if(child.name != current.name)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private void AddText(string input, string fileName)
    {
        AppendToFile(fileName, input);
    }
    
}
