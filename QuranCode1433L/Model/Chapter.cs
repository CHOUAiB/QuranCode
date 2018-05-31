﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Chapter
    {
        private Book book = null;
        public Book Book
        {
            get { return book; }
            set { book = value; }
        }

        private List<Verse> verses = null;
        public List<Verse> Verses
        {
            get { return verses; }
        }

        private int number = 0;
        public int Number
        {
            get { return number; }
        }

        private string name = null;
        public string Name
        {
            get { return name; }
        }

        private string transliterated_name = null;
        public string TransliteratedName
        {
            get { return transliterated_name; }
        }

        private string english_name = null;
        public string EnglishName
        {
            get { return english_name; }
        }

        private RevelationPlace revelation_place = RevelationPlace.Both;
        public RevelationPlace RevelationPlace
        {
            get { return revelation_place; }
        }

        private int revelation_order = 0;
        public int RevelationOrder
        {
            get { return revelation_order; }
        }

        private InitializationType initialization_type = InitializationType.NonInitialized;
        public InitializationType InitializationType
        {
            get { return initialization_type; }
            set { initialization_type = value; }
        }

        private int bowing_count = 0;
        public int BowingCount
        {
            get { return bowing_count; }
        }

        public Word GetWord(int index)
        {
            if (this.verses != null)
            {
                foreach (Verse verse in this.verses)
                {
                    if (index >= verse.Words.Count)
                    {
                        index -= verse.Words.Count;
                    }
                    else
                    {
                        return verse.Words[index];
                    }
                }
            }
            return null;
        }
        public int WordCount
        {
            get
            {
                int word_count = 0;
                if (this.verses != null)
                {
                    foreach (Verse verse in this.verses)
                    {
                        word_count += verse.Words.Count;
                    }
                }
                return word_count;
            }
        }

        public Letter GetLetter(int index)
        {
            if (this.verses != null)
            {
                foreach (Verse verse in this.verses)
                {
                    foreach (Word word in verse.Words)
                    {
                        if (index >= word.Letters.Count)
                        {
                            index -= word.Letters.Count;
                        }
                        else
                        {
                            return word.Letters[index];
                        }
                    }
                }
            }
            return null;
        }
        public int LetterCount
        {
            get
            {
                int letter_count = 0;
                if (this.verses != null)
                {
                    foreach (Verse verse in this.verses)
                    {
                        letter_count += verse.LetterCount;
                    }
                }
                return letter_count;
            }
        }

        private List<char> unique_letters = null;
        public List<char> UniqueLetters
        {
            get
            {
                unique_letters = new List<char>();
                if (this.verses != null)
                {
                    foreach (Verse verse in this.verses)
                    {
                        if (verse.UniqueLetters != null)
                        {
                            foreach (char character in verse.UniqueLetters)
                            {
                                if (!unique_letters.Contains(character))
                                {
                                    unique_letters.Add(character);
                                }
                            }
                        }
                    }
                }
                return unique_letters;
            }
        }
        public int GetLetterFrequency(char character)
        {
            int result = 0;
            if (this.verses != null)
            {
                foreach (Verse verse in this.verses)
                {
                    foreach (Word word in verse.Words)
                    {
                        foreach (Letter letter in word.Letters)
                        {
                            if (letter.Character == character)
                            {
                                result++;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public Chapter(Book book,
                       int number,
                       string name,
                       string transliterated_name,
                       string english_name,
                       RevelationPlace revelation_place,
                       int revelation_order,
                       int bowing_count,
                       List<Verse> verses)
        {
            this.book = book;
            this.number = number;
            this.name = name;
            this.transliterated_name = transliterated_name;
            this.english_name = english_name;
            this.revelation_place = revelation_place;
            this.revelation_order = revelation_order;
            this.bowing_count = bowing_count;
            this.verses = verses;
            if (this.verses != null)
            {
                foreach (Verse verse in this.verses)
                {
                    verse.Chapter = this;
                }
            }
        }

        public string Text
        {
            get
            {
                StringBuilder str = new StringBuilder();
                if (this.verses != null)
                {
                    if (this.verses.Count > 0)
                    {
                        foreach (Verse verse in this.verses)
                        {
                            str.AppendLine(verse.Text);
                        }
                        if (str.Length > 2)
                        {
                            str.Remove(str.Length - 2, 2);
                        }
                    }
                }
                return str.ToString();
            }
        }
        public override string ToString()
        {
            return this.Text;
        }
    }
}
