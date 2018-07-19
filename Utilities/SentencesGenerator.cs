using System;
using System.Text;
using System.Collections.Generic;

public class SentencesGenerator
{
    private List<string> m_words = null;
    private List<string> m_sentences = null;
    public SentencesGenerator(List<string> words)
    {
        m_words = new List<string>(words);
    }

    public List<string> GenerateSentences(int number_of_words, int number_of_letters)
    {
        m_sentences = new List<string>();

        long[] word_lengths = new long[m_words.Count];
        for (int i = 0; i < m_words.Count; i++)
        {
            word_lengths[i] = m_words[i].Length;
        }

        Subsets subsets = new Subsets(word_lengths);
        subsets.Find(number_of_words, number_of_letters, OnSentenceGenerated);

        return m_sentences;
    }
    private void OnSentenceGenerated(Subsets.Item[] items)
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

        StringBuilder str = new StringBuilder();
        foreach (Subsets.Item item in yyy)
        {
            str.Append(m_words[item.Index] + " ");
        }
        if (str.Length > 0)
        {
            str.Remove(str.Length - 1, 1); // " "
        }
        m_sentences.Add(str.ToString());
    }
}
