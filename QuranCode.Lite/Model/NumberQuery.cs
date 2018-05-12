using System;
using System.Collections.Generic;

namespace Model
{
    public struct NumberQuery
    {
        /// <summary>
        /// Number of word/verse/chapter or sum of numbers of a Ws/Vs/Cs range
        /// </summary>
        public int Number;
        public int ChapterCount;
        public int VerseCount;
        public int WordCount;
        public int LetterCount;
        public int UniqueLetterCount;
        public long Value;
        public int ValueDigitSum;
        public int ValueDigitalRoot;
        public int NumberRemainder;
        public int ChapterCountRemainder;
        public int VerseCountRemainder;
        public int WordCountRemainder;
        public int LetterCountRemainder;
        public int UniqueLetterCountRemainder;
        public int ValueRemainder;
        public int ValueDigitSumRemainder;
        public int ValueDigitalRootRemainder;
        public NumberType NumberNumberType;
        public NumberType ChapterCountNumberType;
        public NumberType VerseCountNumberType;
        public NumberType WordCountNumberType;
        public NumberType LetterCountNumberType;
        public NumberType UniqueLetterCountNumberType;
        public NumberType ValueNumberType;
        public NumberType ValueDigitSumNumberType;
        public NumberType ValueDigitalRootNumberType;
        public ComparisonOperator NumberComparisonOperator;
        public ComparisonOperator ChapterCountComparisonOperator;
        public ComparisonOperator VerseCountComparisonOperator;
        public ComparisonOperator WordCountComparisonOperator;
        public ComparisonOperator LetterCountComparisonOperator;
        public ComparisonOperator UniqueLetterCountComparisonOperator;
        public ComparisonOperator ValueComparisonOperator;
        public ComparisonOperator ValueDigitSumComparisonOperator;
        public ComparisonOperator ValueDigitalRootComparisonOperator;

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
                case NumbersResultType.WordSets:
                    {
                        return
                        (
                            (WordCount != 1)
                            &&
                            IsValidWordSearch()
                        );
                    }
                case NumbersResultType.VerseRanges:
                case NumbersResultType.VerseSets:
                    {
                        return
                        (
                            (VerseCount != 1)
                            &&
                            IsValidVerseSearch()
                        );
                    }
                case NumbersResultType.ChapterRanges:
                case NumbersResultType.ChapterSets:
                    {
                        return
                        (
                            (ChapterCount != 1)
                            &&
                            IsValidChapterSearch()
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
                    ((LetterCount != 0) && (LetterCount >= WordCount)) ||
                    ((UniqueLetterCount != 0)) ||
                    ((Value != 0) && (Value >= LetterCount)) ||
                    ((ValueDigitSum != 0) && (ValueDigitSum <= Value)) ||
                    ((ValueDigitalRoot != 0) && (ValueDigitalRoot <= Value)) ||

                    (LetterCountNumberType != NumberType.None) ||
                    (UniqueLetterCountNumberType != NumberType.None) ||
                    (ValueNumberType != NumberType.None) ||
                    (ValueDigitSumNumberType != NumberType.None) ||
                    (ValueDigitalRootNumberType != NumberType.None)
                   );
        }
        private bool IsValidVerseSearch()
        {
            return (
                    ((WordCount != 0) && (WordCount >= VerseCount)) ||
                    (WordCountNumberType != NumberType.None) ||
                    IsValidWordSearch()
                   );
        }
        private bool IsValidChapterSearch()
        {
            return (
                    ((VerseCount != 0) && (VerseCount >= ChapterCount)) ||
                    (VerseCountNumberType != NumberType.None) ||
                    IsValidVerseSearch()
                   );
        }
    }
}
