using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class UserLearning {
	public WordInfo wordInfo;
	public WordProgress wordProgress;

	public UserLearning () {
		wordInfo = null;
		wordProgress = null;
	}
}
