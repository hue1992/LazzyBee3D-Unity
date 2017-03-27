using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class WordProgress {
	public int due;
	public int e_fact;
	public int last_ivl;
	public int queue;
	public int rev_count;

	public WordProgress () {
		due 		= 0;
		e_fact 		= CommonDefine.DEFAULT_EFACTOR;
		last_ivl 	= 0;
		queue 		= 0;
		rev_count 	= 0;
	}
}
