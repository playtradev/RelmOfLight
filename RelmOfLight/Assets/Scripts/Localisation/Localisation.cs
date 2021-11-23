using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LocalisationSystem
{
    static LocalisationReader _LocReader = null;
    static LocalisationReader LocReader
    {
        get
        {
            if (_LocReader != null)
                return _LocReader;
            _LocReader = new LocalisationReader("Localisation");
            return _LocReader;
        }
    }

    public enum LanguageType { ENG = 0, ITA = 1 }
    public static LanguageType LocalisationLanguage = LanguageType.ENG;

    static Dictionary<LanguageType, Dictionary<string, string>> localisedDictionaries = new Dictionary<LanguageType, Dictionary<string, string>>();
    static Dictionary<string, string> LocalisedDictionary
    {
        get
        {
            if (!localisedDictionaries.ContainsKey(LocalisationLanguage))
                localisedDictionaries.Add(LocalisationLanguage, LocReader.GetLocalisedValues(LocalisationLanguage.ToString()));
            return localisedDictionaries[LocalisationLanguage];
        }
    }

    public static string GetLocalisedValue(string textKey)
    {
        string res = textKey;
        if(!LocalisedDictionary.TryGetValue(textKey, out res))
        {
            Debug.LogError("Missing localisation for: <b>" + textKey + "</b> - <b>" + LocalisationLanguage.ToString().ToUpper() + "</b>");
            return textKey;
        }
        return res;
    }
}

public class LocalisationReader
{
    public TextAsset file;
    private const char lineIdentifier = '\n';
    private const char textIdentifier = '"';
    private string[] fieldIdentifier = new string[] { "\",\"" };
    /// <summary>
    /// Where the language titles are set in the csv file, with the data starting after this line
    /// </summary>
    private const int headerLine = 1; 

    public LocalisationReader(string fileName)
    {
        file = Resources.Load<TextAsset>(fileName);
    }

    public Dictionary<string, string> GetLocalisedValues(string languageCode)
    {
        languageCode = "lang_" + languageCode.ToLower();
        Dictionary<string, string> res = new Dictionary<string, string>();
        string[] lines = file.text.Split(lineIdentifier);

        int localisationIndex = -1;
        string[] headers = lines[headerLine].Split(fieldIdentifier, System.StringSplitOptions.None);
        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Contains(languageCode))
            {
                localisationIndex = i;
                break;
            }
        }

        Regex parser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        string line = "";
        string[] fields = new string[0];
        for (int i = headerLine + 1; i < lines.Length; i++)
        {
            line = lines[i];
            fields = parser.Split(line);
            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].TrimStart(' ', textIdentifier);
                fields[j] = fields[j].TrimEnd();
            }

            if (fields.Length > localisationIndex)
            {
                if (res.ContainsKey(fields[0])) 
                    continue;
                res.Add(fields[0], fields[localisationIndex]);
            }
        }
        return res;
    }
}
