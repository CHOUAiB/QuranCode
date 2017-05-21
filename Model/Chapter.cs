using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public enum ChapterSortMethod { ByCompilation, ByRevelation, ByVerses, ByWords, ByLetters, ByValue }
    public enum ChapterSortOrder { Ascending, Descending }
    public class Chapter : IComparable<Chapter>
    {
        private static ChapterSortMethod s_sort_method = ChapterSortMethod.ByCompilation;
        public static ChapterSortMethod SortMethod
        {
            get { return s_sort_method; }
            set { s_sort_method = value; }
        }
        private static ChapterSortOrder s_sort_order = ChapterSortOrder.Ascending;
        public static ChapterSortOrder SortOrder
        {
            get { return s_sort_order; }
            set { s_sort_order = value; }
        }
        private static bool s_pin_chapter1 = true;
        public static bool PinChapter1
        {
            get { return s_pin_chapter1; }
            set { s_pin_chapter1 = value; }
        }
        public int CompareTo(Chapter obj)
        {
            if (this == obj) return 0;

            // don't pin chapter1 in compilation and revelation orders
            if ((s_sort_method != ChapterSortMethod.ByCompilation) && (s_sort_method != ChapterSortMethod.ByRevelation))
            {
                if ((Chapter.PinChapter1) && (this.CompilationOrder == 1)) return -1;
                if ((Chapter.PinChapter1) && (obj.CompilationOrder == 1)) return 1;
            }

            if (s_sort_order == ChapterSortOrder.Ascending)
            {
                switch (s_sort_method)
                {
                    case ChapterSortMethod.ByCompilation:
                        {
                            if (this.CompilationOrder.CompareTo(obj.CompilationOrder) == 0)
                            {
                                return this.Number.CompareTo(obj.Number);
                            }
                            return this.CompilationOrder.CompareTo(obj.CompilationOrder);
                        }
                    case ChapterSortMethod.ByRevelation:
                        {
                            if (this.RevelationOrder.CompareTo(obj.RevelationOrder) == 0)
                            {
                                return this.Number.CompareTo(obj.Number);
                            }
                            return this.RevelationOrder.CompareTo(obj.RevelationOrder);
                        }
                    case ChapterSortMethod.ByVerses:
                        {
                            if (this.verses.Count.CompareTo(obj.Verses.Count) == 0)
                            {
                                return this.Number.CompareTo(obj.Number);
                            }
                            return this.verses.Count.CompareTo(obj.Verses.Count);
                        }
                    case ChapterSortMethod.ByWords:
                        {
                            if (this.WordCount.CompareTo(obj.WordCount) == 0)
                            {
                                return this.Number.CompareTo(obj.Number);
                            }
                            return this.WordCount.CompareTo(obj.WordCount);
                        }
                    case ChapterSortMethod.ByLetters:
                        {
                            if (this.LetterCount.CompareTo(obj.LetterCount) == 0)
                            {
                                return this.Number.CompareTo(obj.Number);
                            }
                            return this.LetterCount.CompareTo(obj.LetterCount);
                        }
                    case ChapterSortMethod.ByValue:
                        {
                            if (this.Value.CompareTo(obj.Value) == 0)
                            {
                                return this.Number.CompareTo(obj.Number);
                            }
                            return this.Value.CompareTo(obj.Value);
                        }
                    default:
                        {
                            return this.Number.CompareTo(obj.Number);
                        }
                }
            }
            else
            {
                switch (s_sort_method)
                {
                    case ChapterSortMethod.ByCompilation:
                        {
                            if (obj.CompilationOrder.CompareTo(this.CompilationOrder) == 0)
                            {
                                return obj.Number.CompareTo(this.Number);
                            }
                            return obj.CompilationOrder.CompareTo(this.CompilationOrder);
                        }
                    case ChapterSortMethod.ByRevelation:
                        {
                            if (obj.RevelationOrder.CompareTo(this.RevelationOrder) == 0)
                            {
                                return obj.Number.CompareTo(this.Number);
                            }
                            return obj.RevelationOrder.CompareTo(this.RevelationOrder);
                        }
                    case ChapterSortMethod.ByVerses:
                        {
                            if (obj.Verses.Count.CompareTo(this.verses.Count) == 0)
                            {
                                return obj.Number.CompareTo(this.Number);
                            }
                            return obj.Verses.Count.CompareTo(this.verses.Count);
                        }
                    case ChapterSortMethod.ByWords:
                        {
                            if (obj.WordCount.CompareTo(this.WordCount) == 0)
                            {
                                return obj.Number.CompareTo(this.Number);
                            }
                            return obj.WordCount.CompareTo(this.WordCount);
                        }
                    case ChapterSortMethod.ByLetters:
                        {
                            if (obj.LetterCount.CompareTo(this.LetterCount) == 0)
                            {
                                return obj.Number.CompareTo(this.Number);
                            }
                            return obj.LetterCount.CompareTo(this.LetterCount);
                        }
                    case ChapterSortMethod.ByValue:
                        {
                            if (obj.Value.CompareTo(this.Value) == 0)
                            {
                                return obj.Number.CompareTo(this.Number);
                            }
                            return obj.Value.CompareTo(this.Value);
                        }
                    default:
                        {
                            return obj.Number.CompareTo(this.Number);
                        }
                }
            }
        }

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

        private int compilation_order = 0;
        public int CompilationOrder
        {
            get { return compilation_order; }
        }

        private int number = 0;
        public int Number
        {
            get { return number; }
            set { number = value; }
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
            this.compilation_order = number;
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

        // update chapter values for ChapterSortMethod.ByValue
        private long value = 0L;
        public long Value
        {
            set { this.value = value; }
            get { return this.value; }
        }
    }
}
