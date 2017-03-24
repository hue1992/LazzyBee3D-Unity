using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class HTMLHelper {

	public static string createHTMLForQuestion (WordInfo word) {
		string htmlString = "<!DOCTYPE html>\\n" +
			"<html>\n" +
			"<head>\n" +
			"<style>\n" +
			"figure {" +
			"   text-align: center;" +
			"   margin: auto;" +
			"}" +
			"figure.image img {" +
			"   width: 100%% !important;" +
			"   height: auto !important;" +
			"}" +
			"figcaption {" +
			"   font-size: 10px;" +
			"}" +
			"a {" +
			"   margin-top:10px;" +
			"}" +
			"</style>\n" +
			"<script>" +
			//play the text
			"function playText(content, rate) {" +
			"   var speaker = new SpeechSynthesisUtterance();" +
			"   speaker.text = content;" +
			"   speaker.lang = 'en-US';" +
			"   speaker.rate = rate;" + //0.1
			"   speaker.pitch = 1.0;" +
			"   speaker.volume = 1.0;" +
			"   speechSynthesis.cancel();" +
			"   speechSynthesis.speak(speaker);" +
			"}" +
			//cancel speech
			"function cancelSpeech() {" +
			"   speechSynthesis.pause();" +
			"   speechSynthesis.cancel();" +
			"}" +
			"</script>" +
			"</head>\n" +
			"<body>\n" +
			"<div style='width:100%%'>\n" +
			"{0}\n" +  //strWordIconTag
			"</div>\n" +
			"</body>\n" +
			"</html>";

		float speed = TemporarilyStatus.getInstance().speaking_speed;

		string strWordIconTag = @"<div style='float:left;width:90%%;text-align: center;'>\n" +
                        "<strong style='font-size:18pt;'> {0} </strong>\n" +   //%@ will be replaced by word.question
			"</div>\n" +
			"<div style='float:left;width:10%%'>\n" +
			"<a onclick='playText(\"{1}\", {2});'><img src='ic_speaker.png'/><p>\n" +
			"</div>\n";

		strWordIconTag = String.Format(strWordIconTag, word.word, word.word, speed);

		htmlString = String.Format(htmlString, strWordIconTag);

		Debug.Log(htmlString);

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
		strWordIconTag = "<div style='float:left;width:10%%'>\\n" +
			"<a onclick='playText(\"{0}\", {1});'><img src='ic_speaker.png'/></a>\n" +
			"</div>\n";
		strWordIconTag = String.Format(strWordIconTag, word.word, speed);

		if (strExplanation != null && strExplanation.Length > 0) {
			strExplainIconTag = "<div style=\"float:left;width:90%%; font-size:14pt;\">" +
				"   <em>{0}</em> \n" + //%@ will be replaced by strExplanation
				"</div>\n" +
				"<div style=\"float:left;width:10%%\">\n " +
				"   <p><a onclick='playText(\"{1}\", {2});'><img src='ic_speaker.png'/></a></p>\n" +  //%@ will be replaced by strExplanation
				"</div>\n";
			strExplainIconTag = String.Format(strExplainIconTag, strExplanation, plainExplanation, speed);
		}

		if (strExample != null && strExample.Length > 0) {
			strExampleIconTag = "<div style=\"width:90%%; font-size:12pt;\"><strong>Example: </strong></div>\n" +
			"<div style=\"float:left;width:90%%; font-size:14pt;\">" +
				"   <em>{0}</em> \n" + //%@ will be replaced by strExample
				"</div>\n" +
				"<div style=\"float:left;width:10%%\">\n " +
				"   <p><a onclick='playText(\"{1}\", {2});'><img src='ic_speaker.png'/></a></p>\n" +  //%@ will be replaced by strExample
				"</div>\n";
			strExampleIconTag = String.Format(strExampleIconTag, strExample, plainExample, speed );
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
			"figure {" +
			"   text-align: center;" +
			"   margin: auto;" +
			"}" +
			"figure.image img {" +
			"   width: 100%% !important;" +
			"   height: auto !important;" +
			"}" +
			"figcaption {" +
			"   font-size: 10px;" +
			"}" +
			"a {" +
			"   margin-top:10px;" +
			"}" +
			"hr {" +
			"border: 0;" +
			"border-top: 3px double #8c8c8c;" +
			"text-align:center;" +
			"}" +
			"</style>\n" +
			"<script>" +
			//play the text
			"function playText(content, rate) {" +
			"   var speaker = new SpeechSynthesisUtterance();" +
			"   speaker.text = content;" +
			"   speaker.lang = 'en-US';" +
			"   speaker.rate = rate;" +//0.1
			"   speaker.pitch = 1.0;" +
			"   speaker.volume = 1.0;" +
			"   speechSynthesis.cancel();" +
			"   speechSynthesis.speak(speaker);" +
			"}" +
			//cancel speech
			"function cancelSpeech() {" +
			"   speechSynthesis.pause();" +
			"   speechSynthesis.cancel();" +
			"}" +
			"</script>" +
			"</head>\n" +
			"<body>\n" +
			"   <div style='width:100%%'>\n" +
			"       <div style='float:left;width:90%%;text-align: center;'>\n" +
			"           <strong style='font-size:18pt;'> {0} </strong>\n" +    //%@ will be replaced by word
			"       </div>\n" +
			"       {1}\n" +   //%@ will be replaced by strWordIconTag

			"       <div style='width:90%%'>\n" +
			"           <center><font size='4'> {2} </font></center>\n" +  //%@ will be replaced by pronunciation
			"       </div>\n" +

			"           <p style=\"text-align: center;\">  </p>\n" +  //%@ will be replaced by image link, temporary leave it blank

			"       <div style=\"width:100%%\"></div>\n" +
			"            {3} \n" +     //%@ will be replaced by strExplainIconTag

			"            {4} \n" +    //%@ will be replaced by strExampleIconTag

			"       <div style='width:90%%'>\n" +
			"           <br><br><br><br><center><font size='4' color='blue'><em style='margin-left: 10px'> {5} </em></font></center>\n" +    //%@ will be replaced by meaning
			"       </div>\n" +
			"   </div>\n" +
			"   {6} \n" +     //%@ will be replaced by strNoteTag

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
