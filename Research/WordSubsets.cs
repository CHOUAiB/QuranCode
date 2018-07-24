using System;
using System.Text;
using System.Collections.Generic;
using Model;

public class WordSubsets
{
    private List<Word> m_words = null;
    private List<List<Word>> m_sentences = null;
    public WordSubsets(List<Word> words)
    {
        m_words = new List<Word>(words);
    }

    public List<List<Word>> Find(int number_of_words, int number_of_letters)
    {
        m_sentences = new List<List<Word>>();

        long[] word_lengths = new long[m_words.Count];
        for (int i = 0; i < m_words.Count; i++)
        {
            string simplified_word_text = m_words[i].Text;
            if (m_words[i].Text.IsArabicWithDiacritics())
            {
                simplified_word_text = simplified_word_text.Simplify29();
            }
            word_lengths[i] = simplified_word_text.Length;
        }

        Subsets subsets = new Subsets(word_lengths);
        subsets.Find(number_of_words, number_of_letters, OnFound);

        return m_sentences;
    }
    
    private void OnFound(Subsets.Item[] items)
    {
        // sort items in ascending order
        List<Subsets.Item> xxx = new List<Subsets.Item>(items);
        List<Subsets.Item> yyy = new List<Subsets.Item>();
        while (xxx.Count > 0)
        {
            int min = 0;
            for (int i = 0; i < xxx.Count; i++)
            {
                if (xxx[min].Index > xxx[i].Index)
                {
                    min = i;
                }
            }
            yyy.Add(xxx[min]);
            xxx.Remove(xxx[min]);
        };

        List<Word> sentence = new List<Word>();
        foreach (Subsets.Item item in yyy)
        {
            sentence.Add(m_words[item.Index]);
        }
        m_sentences.Add(sentence);
    }
}
