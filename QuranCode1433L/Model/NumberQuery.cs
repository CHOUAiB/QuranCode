﻿using System;
using System.Collections.Generic;

namespace Model
{
    public struct NumberQuery
    {
        public bool WithinVerses;

        /// <summary>
        /// Number of word/verse/chapter or sum of numbers of a Ws/Vs/Cs range/set
        /// </summary>
        public int Number;
        public NumberScope NumberScope;
        public int ChapterCount;
        public int VerseCount;
        public int WordCount;
        public int LetterCount;
        public int UniqueLetterCount;
        public long Value;

        public NumberType NumberNumberType;
        public NumberType ChapterCountNumberType;
        public NumberType VerseCountNumberType;
        public NumberType WordCountNumberType;
        public NumberType LetterCountNumberType;
        public NumberType UniqueLetterCountNumberType;
        public NumberType ValueNumberType;

        public ComparisonOperator NumberComparisonOperator;
        public ComparisonOperator ChapterCountComparisonOperator;
        public ComparisonOperator VerseCountComparisonOperator;
        public ComparisonOperator WordCountComparisonOperator;
        public ComparisonOperator LetterCountComparisonOperator;
        public ComparisonOperator UniqueLetterCountComparisonOperator;
        public ComparisonOperator ValueComparisonOperator;
        public int NumberRemainder;
        public int ChapterCountRemainder;
        public int VerseCountRemainder;
        public int WordCountRemainder;
        public int LetterCountRemainder;
        public int UniqueLetterCountRemainder;
        public int ValueRemainder;

        public int NumberDigitSum;
        public int ChapterCountDigitSum;
        public int VerseCountDigitSum;
        public int WordCountDigitSum;
        public int LetterCountDigitSum;
        public int UniqueLetterCountDigitSum;
        public int ValueDigitSum;

        public int NumberDigitalRoot;
        public int ChapterCountDigitalRoot;
        public int VerseCountDigitalRoot;
        public int WordCountDigitalRoot;
        public int LetterCountDigitalRoot;
        public int UniqueLetterCountDigitalRoot;
        public int ValueDigitalRoot;

        public bool IsValid(NumbersResultType numbers_result_type)
        {
            switch (numbers_result_type)
            {
                case NumbersResultType.Words:
                    {
                        return
                        (
                            IsValidNumberSearch()
                            ||
                            IsValidWordSearch()
                        );
                    }
                case NumbersResultType.Sentences:
                    {
                        return
                        (
                            IsValidVerseSearch()
                        );
                    }
                case NumbersResultType.Verses:
                    {
                        return
                        (
                            IsValidNumberSearch()
                            ||
                            IsValidVerseSearch()
                        );
                    }
                case NumbersResultType.Chapters:
                    {
                        return
                        (
                            IsValidNumberSearch()
                            ||
                            IsValidChapterSearch()
                        );
                    }
                case NumbersResultType.WordRanges:
                    {
                        return
                        (
                            (WordCount != 1)
                        );
                    }
                case NumbersResultType.VerseRanges:
                    {
                        return
                        (
                            (VerseCount != 1)
                        );
                    }
                case NumbersResultType.ChapterRanges:
                    {
                        return
                        (
                            (ChapterCount != 1)
                        );
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        // helper methods
        private bool IsValidNumberSearch()
        {
            return (
                    (Number != 0) ||
                    (NumberNumberType != NumberType.None)
                   );
        }
        private bool IsValidWordSearch()
        {
            return (
                    (LetterCount != 0) ||
                    (UniqueLetterCount != 0) ||
                    (Value != 0) ||
                    (ValueDigitSum != 0) ||
                    (ValueDigitalRoot != 0) ||
                    (LetterCountNumberType != NumberType.None) ||
                    (UniqueLetterCountNumberType != NumberType.None) ||
                    (ValueNumberType != NumberType.None)
                   );
        }
        private bool IsValidVerseSearch()
        {
            return (
                    (WordCount != 0) ||
                    (WordCountNumberType != NumberType.None) ||
                    IsValidWordSearch()
                   );
        }
        private bool IsValidChapterSearch()
        {
            return (
                    (VerseCount != 0) ||
                    (VerseCountNumberType != NumberType.None) ||
                    IsValidVerseSearch()
                   );
        }
    }
}
