using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class HTMLHelper {
	private static string SPEAKER_IMG_LINK = "https://firebasestorage.googleapis.com/v0/b/lazeebee-977.appspot.com/o/ic_speaker%403x.png?alt=media&token=2dd7f3ab-a695-4cca-a8b0-81ac6a8fdc96";

	public static string createHTMLForQuestion (WordInfo word) {
		Debug.Log("createHTMLForQuestion");
		string htmlString = "<!DOCTYPE html>\n" +
			"<html>\n" +
			"<head>\n" +
			"<style>\n" +
			"figure {{" +
			"   text-align: center;" +
			"   margin: auto;" +
			"}}" +
			"figure.image img {{" +
			"   width: 100%% !important;" +
			"   height: auto !important;" +
			"}}" +
			"figcaption {{" +
			"   font-size: 10px;" +
			"}}" +
			"a {{" +
			"   margin-top:10px;" +
			"}}" +
			"</style>\n" +
			"<script>" +
			//play the text
			"function playText(content, rate) {{" +
			"   var speaker = new SpeechSynthesisUtterance();" +
			"   speaker.text = content;" +
			"   speaker.lang = 'en-US';" +
			"   speaker.rate = rate;" + //0.1
			"   speaker.pitch = 1.0;" +
			"   speaker.volume = 1.0;" +
			"   speechSynthesis.cancel();" +
			"   speechSynthesis.speak(speaker);" +
			"}}" +
			//cancel speech
			"function cancelSpeech() {{" +
			"   speechSynthesis.pause();" +
			"   speechSynthesis.cancel();" +
			"}}" +
			"</script>" +
			"</head>" +
			"<body>" +
			"<div style='width:100%%'>" +
			"{0}" +  //strWordIconTag
			"</div>\n" +
			"</body>\n" +
			"</html>";

		try {
			float speed = TemporarilyStatus.getInstance().speaking_speed;
			Debug.Log("createHTMLForQuestion 1");
			string strWordIconTag = @"<div style='float:left;width:90%%;text-align: center;'>" +
	                        "<strong style='font-size:18pt;'> {0} </strong>\n" +   //%@ will be replaced by word.question
				"</div>\n" +
				"<div style='float:right;width:10%%'>\n" +
				"<a onclick='playText(\"{1}\", {2});'><img width=100%% src='{3}'/><p>\n" +
				"</div>\n";

			strWordIconTag = String.Format(strWordIconTag, word.word, word.word, speed, SPEAKER_IMG_LINK);
			Debug.Log("createHTMLForQuestion 2 :: " +strWordIconTag);
			Debug.Log("=======================");
			Debug.Log("createHTMLForQuestion htmlString :: " +htmlString);
			htmlString = String.Format(htmlString, strWordIconTag);
			Debug.Log("createHTMLForQuestion 3");
			Debug.Log(htmlString);

		} catch (Exception e) {
			Debug.Log("createHTMLForQuestion :: Exception :; " +e.ToString());
		}

		return htmlString;
	}

	public static string createHTMLForAnswer (WordInfo word) {
		string htmlString 		= "";
		string strExplanation 	= word.common.explain;
		string strExample 		= word.common.example;
		string strMeaning 		= word.common.meaning;

		/* maybe must replace "&nbsp;" by "" */

		//remove html tag, use for playing speech
		string plainExplanation = @"";
		string plainExample = @"";

		if (strExplanation != null) {
			strExplanation = removeNBSP(strExplanation);
			plainExplanation = removeHTML(strExplanation);

		} else {
			strExplanation = "";
		}

		if (strExample != null) {
			strExample = removeNBSP(strExample);
			plainExplanation = removeHTML(strExample);

		} else {
			strExample = "";
		}

		if (strMeaning != null) {
			strMeaning = strMeaning.Replace("<p>", String.Empty);
			strMeaning = strMeaning.Replace("</p>", String.Empty);

		} else {
			strMeaning = "";
		}

		string strPronounciation = word.pronoun;

		if (strPronounciation.Equals("//") == true) {
			strPronounciation = "";
		}

		//check display meaning setting
		if (TemporarilyStatus.getInstance().auto_play_sound == 0) {
			strMeaning = "";
		}

		float speed = TemporarilyStatus.getInstance().speaking_speed;

		string strExplainIconTag 	= @"";
		string strExampleIconTag 	= @"";
		string strWordIconTag 		= @"";
		string strNoteTag 			= @"";

		//create html
		strWordIconTag = "<div style='float:right;width:10%%'>\\n" +
			"<a onclick='playText(\"{0}\", {1});'><img width=100%% src='{2}'/></a>\n" +
			"</div>\n";
		strWordIconTag = String.Format(strWordIconTag, word.word, speed, SPEAKER_IMG_LINK);

		if (strExplanation != null && strExplanation.Length > 0) {
			strExplainIconTag = "<div style=\"float:left;width:90%%; font-size:14pt;\">" +
				"   <em>{0}</em> \n" + //%@ will be replaced by strExplanation
				"</div>\n" +
				"<div style=\"float:right;width:10%%\">\n " +
				"   <p><a onclick='playText(\"{1}\", {2});'><img width=100%% src='{3}'/></a></p>\n" +  //%@ will be replaced by strExplanation
				"</div>\n";
			strExplainIconTag = String.Format(strExplainIconTag, strExplanation, plainExplanation, speed, SPEAKER_IMG_LINK);
		}

		if (strExample != null && strExample.Length > 0) {
			strExampleIconTag = "<div style=\"width:90%%; font-size:12pt;\"><strong>Example: </strong></div>\n" +
			"<div style=\"float:left;width:90%%; font-size:14pt;\">" +
				"   <em>{0}</em> \n" + //%@ will be replaced by strExample
				"</div>\n" +
				"<div style=\"float:right;width:10%%\">\n " +
				"   <p><a onclick='playText(\"{1}\", {2});'><img width=100%% src='{3}'/></a></p>\n" +  //%@ will be replaced by strExample
				"</div>\n";
			strExampleIconTag = String.Format(strExampleIconTag, strExample, plainExample, speed, SPEAKER_IMG_LINK);
		}

		string userNote = word.usernote;

		if (userNote != null && userNote.Length > 0) {			
			userNote = userNote.Replace("\n", "<br>");

			string userNoteLabel = "User note";
			strNoteTag = "<div style=\"width:100%%; font-size:12pt;\"><br><center><hr></center></div>\n" +
				"<div style=\"width:100%%; font-size:12pt;\"><strong>{0}: </strong></div>\n" +
				"<div style=\"width:100%%; font-size:14pt;\">" +
				"   <em>{1}</em> \n" + //%@ will be replaced by word.userNote
				"</div>\n";

			strNoteTag = String.Format(strNoteTag, userNoteLabel, userNote);
		}

		htmlString = @"<html>\n" +
		    "<head>\n" +
			"<meta content=\"width=device-width, initial-scale=1.0, user-scalable=yes\"\n" +
			"name=\"viewport\">\n" +
			"<style>\n" +
			"figure {{" +
			"   text-align: center;" +
			"   margin: auto;" +
			"}}" +
			"figure.image img {{" +
			"   width: 100%% !important;" +
			"   height: auto !important;" +
			"}}" +
			"figcaption {" +
			"   font-size: 10px;" +
			"}}" +
			"a {{" +
			"   margin-top:10px;" +
			"}}" +
			"hr {{" +
			"border: 0;" +
			"border-top: 3px double #8c8c8c;" +
			"text-align:center;" +
			"}}" +
			"</style>\n" +
			"<script>" +
			//play the text
			"function playText(content, rate) {{" +
			"   var speaker = new SpeechSynthesisUtterance();" +
			"   speaker.text = content;" +
			"   speaker.lang = 'en-US';" +
			"   speaker.rate = rate;" +//0.1
			"   speaker.pitch = 1.0;" +
			"   speaker.volume = 1.0;" +
			"   speechSynthesis.cancel();" +
			"   speechSynthesis.speak(speaker);" +
			"}}" +
			//cancel speech
			"function cancelSpeech() {{" +
			"   speechSynthesis.pause();" +
			"   speechSynthesis.cancel();" +
			"}}" +
			"</script>" +
			"</head>" +
			"<body>" +
			"   <div style='width:100%%'>" +
			"       <div style='float:left;width:90%%;text-align: center;'>" +
			"           <strong style='font-size:18pt;'> {0} </strong>" +    //%@ will be replaced by word
			"       </div>" +
			"       {1}\n" +   //%@ will be replaced by strWordIconTag

			"       <div style='width:90%%'>" +
			"           <center><font size='4'> {2} </font></center>" +  //%@ will be replaced by pronunciation
			"       </div>\n" +

			"           <p style=\"text-align: center;\">  </p>" +  //%@ will be replaced by image link, temporary leave it blank

			"       <div style=\"width:100%%\"></div>" +
			"            {3} \n" +     //%@ will be replaced by strExplainIconTag

			"            {4} \n" +    //%@ will be replaced by strExampleIconTag

			"       <div style='width:90%%'>" +
			"           <br><br><br><br><center><font size='4' color='blue'><em style='margin-left: 10px'> {5} </em></font></center>" +    //%@ will be replaced by meaning
			"       </div>" +
			"   </div>" +
			"   {6} " +     //%@ will be replaced by strNoteTag

			"   </body>" +
			"</html>\n";

		htmlString = String.Format(htmlString, word.word, strWordIconTag, strPronounciation, strExplainIconTag, strExampleIconTag, strMeaning, strNoteTag);

		return htmlString;
	}

	private static string removeHTML(string input) {
		return Regex.Replace(input, "<.*?>", String.Empty);
	}

	private static string removeNBSP(string input) {
		return input.Replace("&nbsp;", " ");
	}
}
