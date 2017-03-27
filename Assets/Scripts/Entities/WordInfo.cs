using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class WordInfo  {
	public string word;
	public string category;
	public int level;
	public string pronoun;
	public string subcats;
	public string l_vn;
	public string l_en;
	public string usernote;
	public MeaningInfo common;
	public MeaningInfo toeic;
	public MeaningInfo economy;
	public MeaningInfo it;
	public MeaningInfo ielts;
	public MeaningInfo science;
	public MeaningInfo medicine;

	public WordInfo () {
		word = "";
	}
}