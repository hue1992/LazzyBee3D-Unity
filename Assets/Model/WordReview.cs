using UnityEngine;
using System.Collections;

public class WordReview
{
    public string word_id;
    public string last_interver;
    public string factory;
    public string review_count;
    public string queue;
    public string due_date;

    public WordReview()
    {

    }

    public WordReview(string word_id, string last_interver, string factory, string review_count, string queue, string due_date)        
    {
        this.word_id = word_id;
        this.last_interver = last_interver;
        this.factory = factory;
        this.review_count = review_count;
        this.queue = queue;
        this.due_date = due_date;

    }
}
