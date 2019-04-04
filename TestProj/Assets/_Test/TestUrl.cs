using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class IndexC
{
    public int startIndex;
    public int endIndex;
}

public class TestUrl : MonoBehaviour {

    public string result = "";
    public string regex = "(?<=\\《)[^\\》]+";
    [ContextMenu("Go")]
	void Start () {

        Regex rg = new Regex(regex, RegexOptions.Multiline | RegexOptions.Singleline);
        List<IndexC> indexList = new List<IndexC>();
        Match match = rg.Match(result);
        Debug.LogError(match.Value);
        while (match.Success)
        {
            IndexC icc = new IndexC();
            icc.startIndex = match.Index;
            icc.endIndex = match.Index + match.Length;
            indexList.Add(icc);
            match = match.NextMatch();
        }
        for (int i = 0, maxi = indexList.Count; i < maxi; ++i)
        {
            Debug.LogError("S_index : " + indexList[i].startIndex + "  E_index" + indexList[i].endIndex);
        }
    }

    [ContextMenu("Go1")]
    void Find_po()
    {
        string text = @"嗨！我是[25833B]{0}[-]，初次见面，我们加个好友吧！[035BC8][url=742:{1}][加为好友][/url][-]";
        string pattern = @"\b([url)\S*(])\b";
        MatchCollection matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase
        | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
        for (int i = 0, maxi = matches.Count; i < maxi; ++i)
        {
            Debug.LogError(matches[i].Value);
        }
    }
}
