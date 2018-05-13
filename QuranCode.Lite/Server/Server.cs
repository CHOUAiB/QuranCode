using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Model;

public class Server
{
    public const string DEFAULT_EMLAAEI_TEXT = "ar.emlaaei";
    public const string DEFAULT_TRANSLATION = "en.qarai";
    public const string DEFAULT_TRANSLITERATION = "en.transliteration";
    public const string DEFAULT_WORD_MEANINGS = "en.wordbyword";
    public const string DEFAULT_RECITATION = "Alafasy_64kbps";

    static Server()
    {
        if (!Directory.Exists(Globals.STATISTICS_FOLDER))
        {
            Directory.CreateDirectory(Globals.STATISTICS_FOLDER);
        }

        if (!Directory.Exists(Globals.RULES_FOLDER))
        {
            Directory.CreateDirectory(Globals.RULES_FOLDER);
        }

        if (!Directory.Exists(Globals.VALUES_FOLDER))
        {
            Directory.CreateDirectory(Globals.VALUES_FOLDER);
        }

        if (!Directory.Exists(Globals.HELP_FOLDER))
        {
            Directory.CreateDirectory(Globals.HELP_FOLDER);
        }

        // load simplification systems
        LoadSimplificationSystems();

        // load numerology systems
        LoadNumerologySystems();

        // load help messages
        LoadHelpMessages();
    }

    // the book [DYNAMIC]
    private static Book s_book = null;
    public static Book Book
    {
        get { return s_book; }
    }

    // loaded simplification systems [STATIC]
    private static Dictionary<string, SimplificationSystem> s_loaded_simplification_systems = null;
    public static Dictionary<string, SimplificationSystem> LoadedSimplificationSystems
    {
        get { return s_loaded_simplification_systems; }
    }
    private static void LoadSimplificationSystems()
    {
        if (s_loaded_simplification_systems == null)
        {
            s_loaded_simplification_systems = new Dictionary<string, SimplificationSystem>();
        }

        if (s_loaded_simplification_systems != null)
        {
            s_loaded_simplification_systems.Clear();

            string path = Globals.RULES_FOLDER;
            DirectoryInfo folder = new DirectoryInfo(path);
            if (folder != null)
            {
                FileInfo[] files = folder.GetFiles("*.txt");
                if ((files != null) && (files.Length > 0))
                {
                    foreach (FileInfo file in files)
                    {
                        string text_mode = file.Name.Remove(file.Name.Length - 4, 4);
                        if (!String.IsNullOrEmpty(text_mode))
                        {
                            LoadSimplificationSystem(text_mode);
                        }
                    }

                    // start with default simplification system
                    if (s_loaded_simplification_systems.ContainsKey(SimplificationSystem.DEFAULT_NAME))
                    {
                        s_simplification_system = new SimplificationSystem(s_loaded_simplification_systems[SimplificationSystem.DEFAULT_NAME]);
                    }
                    else
                    {
                        //throw new Exception("ERROR: No default simplification system was found.");
                    }
                }
            }
        }
    }
    // simplification system [DYNAMIC]
    private static SimplificationSystem s_simplification_system = null;
    public static SimplificationSystem SimplificationSystem
    {
        get { return s_simplification_system; }
    }
    public static void LoadSimplificationSystem(string text_mode)
    {
        if (String.IsNullOrEmpty(text_mode)) return;

        if (s_loaded_simplification_systems != null)
        {
            // remove and rebuild on the fly without restarting application
            if (s_loaded_simplification_systems.ContainsKey(text_mode))
            {
                s_loaded_simplification_systems.Remove(text_mode);
            }

            string filename = Globals.RULES_FOLDER + "/" + text_mode + ".txt";
            if (File.Exists(filename))
            {
                List<string> lines = DataAccess.LoadLines(filename);

                SimplificationSystem simplification_system = new SimplificationSystem(text_mode);
                if (simplification_system != null)
                {
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('\t');
                        if (parts.Length == 2)
                        {
                            SimplificationRule rule = new SimplificationRule(parts[0], parts[1]);
                            if (rule != null)
                            {
                                simplification_system.Rules.Add(rule);
                            }
                        }
                        else
                        {
                            //throw new Exception(filename + " file format must be:\r\n\tText TAB Replacement");
                        }
                    }
                }

                try
                {
                    // add to dictionary
                    s_loaded_simplification_systems.Add(simplification_system.Name, simplification_system);
                }
                catch
                {
                    // silence error
                }
            }

            // set current simplification system
            if (s_loaded_simplification_systems.ContainsKey(text_mode))
            {
                s_simplification_system = s_loaded_simplification_systems[text_mode];
            }
        }
    }
    public static void BuildSimplifiedBook(string text_mode, bool with_bism_Allah, bool waw_as_word, bool shadda_as_letter)
    {
        if (!String.IsNullOrEmpty(text_mode))
        {
            if (s_loaded_simplification_systems != null)
            {
                if (s_loaded_simplification_systems.ContainsKey(text_mode))
                {
                    s_simplification_system = s_loaded_simplification_systems[text_mode];

                    // reload original Quran text
                    List<string> original_verse_texts = DataAccess.LoadVerseTexts();
                    List<Stopmark> verse_stopmarks = DataAccess.LoadVerseStopmarks();

                    // remove bismAllah from 112 chapters
                    if (!with_bism_Allah)
                    {
                        string bimsAllah_text1 = "بِسْمِ ٱللَّهِ ٱلرَّحْمَٰنِ ٱلرَّحِيمِ ";
                        string bimsAllah_text2 = "بِّسْمِ ٱللَّهِ ٱلرَّحْمَٰنِ ٱلرَّحِيمِ ";
                        for (int i = 0; i < original_verse_texts.Count; i++)
                        {
                            if (original_verse_texts[i].StartsWith(bimsAllah_text1))
                            {
                                original_verse_texts[i] = original_verse_texts[i].Replace(bimsAllah_text1, "");
                            }
                            else if (original_verse_texts[i].StartsWith(bimsAllah_text2))
                            {
                                original_verse_texts[i] = original_verse_texts[i].Replace(bimsAllah_text2, "");
                            }
                        }
                    }

                    // replace shedda with previous letter before any simplification
                    if (shadda_as_letter)
                    {
                        for (int i = 0; i < original_verse_texts.Count; i++)
                        {
                            StringBuilder str = new StringBuilder(original_verse_texts[i]);
                            for (int j = 1; j < str.Length; j++)
                            {
                                if (str[j] == 'ّ')
                                {
                                    str[j] = str[j - 1];
                                }
                            }
                            original_verse_texts[i] = str.ToString();
                        }
                    }

                    // simplify verse texts
                    List<string> verse_texts = new List<string>();
                    foreach (string original_verse_text in original_verse_texts)
                    {
                        string verse_text = s_simplification_system.Simplify(original_verse_text);
                        verse_texts.Add(verse_text);
                    }

                    // buid verses
                    List<Verse> verses = new List<Verse>();
                    for (int i = 0; i < verse_texts.Count; i++)
                    {
                        Verse verse = new Verse(i + 1, verse_texts[i], verse_stopmarks[i]);
                        if (verse != null)
                        {
                            verses.Add(verse);
                            verse.ApplyWordStopmarks(original_verse_texts[i]);
                        }
                    }

                    s_book = new Book(text_mode, verses);
                    if (s_book != null)
                    {
                        s_book.WithBismAllah = with_bism_Allah;
                        s_book.WawAsWord = waw_as_word;
                        s_book.ShaddaAsLetter = shadda_as_letter;

                        // build words before DataAccess.Loads
                        if (waw_as_word)
                        {
                            SplitWawWords(s_book);
                        }
                        DataAccess.LoadRecitationInfos(s_book);
                        DataAccess.LoadTranslationInfos(s_book);
                        DataAccess.LoadTranslations(s_book);
                        DataAccess.LoadWordMeanings(s_book);
                        DataAccess.LoadRootWords(s_book);
                    }
                }
            }
        }
    }
    private static void SplitWawWords(Book book)
    {
        if (book != null)
        {
            if (File.Exists(Globals.DATA_FOLDER + "/" + "non-waw-words.txt"))
            {
                List<string> non_waw_words = DataAccess.LoadLines(Globals.DATA_FOLDER + "/" + "non-waw-words.txt");
                if (non_waw_words != null)
                {
                    foreach (Verse verse in book.Verses)
                    {
                        StringBuilder str = new StringBuilder();
                        if (verse.Words.Count > 0)
                        {
                            for (int i = 0; i < verse.Words.Count; i++)
                            {
                                if (verse.Words[i].Text.StartsWith("و"))
                                {
                                    if (!non_waw_words.Contains(verse.Words[i].Text))
                                    {
                                        str.Append(verse.Words[i].Text.Insert(1, " ") + " ");
                                    }
                                    else // cases where sometimes waw-word and sometimes not
                                    {
                                        // و رد الله الذين كفروا vs ولما ورد ماء مدين
                                        if ((verse.Words[i].Text == "ورد") && (verse.Words[i + 1].Text == "الله"))
                                        {
                                            str.Append(verse.Words[i].Text.Insert(1, " ") + " ");
                                        }
                                        else
                                        {
                                            str.Append(verse.Words[i].Text + " ");
                                        }
                                    }
                                }
                                else
                                {
                                    str.Append(verse.Words[i].Text + " ");
                                }
                            }
                            if (str.Length > 1)
                            {
                                str.Remove(str.Length - 1, 1); // " "
                            }
                        }

                        // re-create new Words with word stopmarks
                        verse.RecreateWordsApplyStopmarks(str.ToString());
                    }
                }
            }
        }
    }

    // loaded numerology systems [STATIC]
    private static Dictionary<string, NumerologySystem> s_loaded_numerology_systems = null;
    public static Dictionary<string, NumerologySystem> LoadedNumerologySystems
    {
        get { return s_loaded_numerology_systems; }
    }
    private static void LoadNumerologySystems()
    {
        if (s_loaded_numerology_systems == null)
        {
            s_loaded_numerology_systems = new Dictionary<string, NumerologySystem>();
        }

        if (s_loaded_numerology_systems != null)
        {
            s_loaded_numerology_systems.Clear();

            string path = Globals.VALUES_FOLDER;
            DirectoryInfo folder = new DirectoryInfo(path);
            if (folder != null)
            {
                FileInfo[] files = folder.GetFiles("*.txt");
                if ((files != null) && (files.Length > 0))
                {
                    foreach (FileInfo file in files)
                    {
                        string numerology_system_name = file.Name.Remove(file.Name.Length - 4, 4);
                        if (!String.IsNullOrEmpty(numerology_system_name))
                        {
                            string[] parts = numerology_system_name.Split('_');
                            if (parts.Length == 3)
                            {
                                LoadNumerologySystem(numerology_system_name);
                            }
                            else
                            {
                                // skip invalid filename
                                //throw new Exception("ERROR: " + file.FullName + " must contain 3 parts separated by \"_\".");
                            }
                        }
                    }

                    // start with default numerology system
                    if (s_loaded_numerology_systems.ContainsKey(NumerologySystem.DEFAULT_NAME))
                    {
                        s_numerology_system = new NumerologySystem(s_loaded_numerology_systems[NumerologySystem.DEFAULT_NAME]);
                    }
                    else
                    {
                        //throw new Exception("ERROR: No default numerology system was found.");
                    }
                }
            }
        }
    }
    // numerology system [DYNAMIC]
    private static NumerologySystem s_numerology_system = null;
    public static NumerologySystem NumerologySystem
    {
        get { return s_numerology_system; }
        set { s_numerology_system = value; }
    }
    public static void LoadNumerologySystem(string numerology_system_name)
    {
        if (String.IsNullOrEmpty(numerology_system_name)) return;

        if (s_loaded_numerology_systems != null)
        {
            // remove and rebuild on the fly without restarting application
            if (s_loaded_numerology_systems.ContainsKey(numerology_system_name))
            {
                s_loaded_numerology_systems.Remove(numerology_system_name);
            }

            string filename = Globals.VALUES_FOLDER + "/" + numerology_system_name + ".txt";
            if (File.Exists(filename))
            {
                List<string> lines = DataAccess.LoadLines(filename);

                NumerologySystem numerology_system = new NumerologySystem(numerology_system_name);
                if (numerology_system != null)
                {
                    numerology_system.LetterValues.Clear();
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('\t');
                        if (parts.Length == 2)
                        {
                            try
                            {
                                numerology_system.LetterValues.Add(parts[0][0], long.Parse(parts[1]));
                            }
                            catch
                            {
                                //throw new Exception(filename + " file format must be:\r\n\tLetter TAB Value");
                            }
                        }
                        else
                        {
                            //throw new Exception(filename + " file format must be:\r\n\tLetter TAB Value");
                        }
                    }
                }

                try
                {
                    // add to dictionary
                    s_loaded_numerology_systems.Add(numerology_system.Name, numerology_system);
                }
                catch
                {
                    // silence error
                }
            }

            // set current numerology system
            if (s_loaded_numerology_systems.ContainsKey(numerology_system_name))
            {
                s_numerology_system = new NumerologySystem(s_loaded_numerology_systems[numerology_system_name]);

                // update chapter values for ChapterSortMethod.ByValue
                if (s_numerology_system != null)
                {
                    if (s_book != null)
                    {
                        foreach (Chapter chapter in s_book.Chapters)
                        {
                            CalculateValue(chapter);
                        }
                    }
                }
            }
        }
    }
    public static void SaveNumerologySystem(string numerology_system_name)
    {
        if (String.IsNullOrEmpty(numerology_system_name)) return;

        if (s_loaded_numerology_systems != null)
        {
            if (s_loaded_numerology_systems.ContainsKey(numerology_system_name))
            {
                NumerologySystem numerology_system = s_loaded_numerology_systems[numerology_system_name];
                if (numerology_system != null)
                {
                    string filename = Globals.VALUES_FOLDER + "/" + numerology_system.Name + ".txt";
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                        {
                            foreach (char key in numerology_system.Keys)
                            {
                                writer.WriteLine(key + "\t" + numerology_system[key].ToString());
                            }
                        }
                    }
                    catch
                    {
                        // silence IO error in case running from read-only media (CD/DVD)
                    }
                }
            }
        }
    }
    public static void UpdateNumerologySystem(string dynamic_text)
    {
        if (String.IsNullOrEmpty(dynamic_text)) return;

        if (s_numerology_system != null)
        {
            dynamic_text = dynamic_text.SimplifyTo(s_numerology_system.TextMode);
            dynamic_text = dynamic_text.Replace("\r", "");
            dynamic_text = dynamic_text.Replace("\n", "");
            dynamic_text = dynamic_text.Replace("\t", "");
            dynamic_text = dynamic_text.Replace("_", "");
            dynamic_text = dynamic_text.Replace(" ", "");
            dynamic_text = dynamic_text.Replace(Verse.OPEN_BRACKET, "");
            dynamic_text = dynamic_text.Replace(Verse.CLOSE_BRACKET, "");
            foreach (char character in Constants.INDIAN_DIGITS)
            {
                dynamic_text = dynamic_text.Replace(character.ToString(), "");
            }

            BuildLetterStatistics(dynamic_text);

            BuildNumerologySystem(dynamic_text);

            if (s_book != null)
            {
                if (s_book.Verses != null)
                {
                    foreach (Verse verse in s_book.Verses)
                    {
                        CalculateValue(verse);
                    }
                }
            }
        }
    }
    private static void BuildNumerologySystem(string dynamic_text)
    {
        if (String.IsNullOrEmpty(dynamic_text)) return;

        if (s_loaded_numerology_systems != null)
        {
            if (s_numerology_system != null)
            {
                // build letter_order using letters in dynamic_text only
                string numerology_system_name = s_numerology_system.Name;
                if (s_loaded_numerology_systems.ContainsKey(numerology_system_name))
                {
                    NumerologySystem loaded_numerology_system = s_loaded_numerology_systems[numerology_system_name];

                    // re-generate the letter_order
                    List<char> letter_order = new List<char>();
                    if (dynamic_text.Length < s_book.Text.Length) //??? TODO: ??? restore Scope to NumerologySystem as this is too slow
                    {
                        switch (s_numerology_system.LetterOrder)
                        {
                            case "Alphabet":
                            case "Alphabet▲":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByLetter;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Alphabet▼":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByLetter;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Descending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Appearance":
                            case "Appearance▲":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByOrder;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Appearance▼":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByOrder;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Descending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Frequency▲":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByFrequency;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Frequency▼":
                            case "Frequency":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByFrequency;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Descending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            default: // use static numerology system
                                {
                                    foreach (char letter in loaded_numerology_system.LetterValues.Keys)
                                    {
                                        if (dynamic_text.Contains(letter.ToString()))
                                        {
                                            letter_order.Add(letter);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    else // use static numerology system
                    {
                        foreach (char letter in loaded_numerology_system.LetterValues.Keys)
                        {
                            if (dynamic_text.Contains(letter.ToString()))
                            {
                                letter_order.Add(letter);
                            }
                        }
                    }

                    // re-generate the letter_values
                    if (letter_order.Count > 0)
                    {
                        List<long> letter_values = new List<long>();

                        if (s_numerology_system.Name.EndsWith("Linear"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                letter_values.Add(i + 1L);
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("Primes"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.Primes.Count)
                                {
                                    letter_values.Add(Numbers.Primes[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("AdditivePrimes"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.AdditivePrimes.Count)
                                {
                                    letter_values.Add(Numbers.AdditivePrimes[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("PurePrimes"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.PurePrimes.Count)
                                {
                                    letter_values.Add(Numbers.PurePrimes[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("Primes2"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.Primes.Count)
                                {
                                    letter_values.Add(Numbers.Primes[i + 1]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("AdditivePrimes2"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.AdditivePrimes.Count)
                                {
                                    letter_values.Add(Numbers.AdditivePrimes[i + 1]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("PurePrimes2"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.PurePrimes.Count)
                                {
                                    letter_values.Add(Numbers.PurePrimes[i + 1]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("Composites"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.Composites.Count)
                                {
                                    letter_values.Add(Numbers.Composites[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("AdditiveComposites"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.AdditiveComposites.Count)
                                {
                                    letter_values.Add(Numbers.AdditiveComposites[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("PureComposites"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.PureComposites.Count)
                                {
                                    letter_values.Add(Numbers.PureComposites[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("MersennePrimes"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.MersennePrimes.Count)
                                {
                                    letter_values.Add(Numbers.MersennePrimes[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("Fibonacci"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.Fibonaccis.Count)
                                {
                                    letter_values.Add(Numbers.Fibonaccis[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("Frequency▲"))
                        {
                            // letter-frequency mismacth: different letters for different frequencies
                            LetterStatistic.SortMethod = StatisticSortMethod.ByFrequency;
                            LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
                            s_letter_statistics.Sort();
                            foreach (LetterStatistic letter_statistic in s_letter_statistics)
                            {
                                letter_values.Add(letter_statistic.Frequency);
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("Frequency"))
                        {
                            letter_order.Clear();
                            foreach (LetterStatistic letter_statistic in s_letter_statistics)
                            {
                                letter_order.Add(letter_statistic.Letter);
                                letter_values.Add(letter_statistic.Frequency);
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("Frequency▼"))
                        {
                            // letter-frequency mismacth: different letters for different frequencies
                            LetterStatistic.SortMethod = StatisticSortMethod.ByFrequency;
                            LetterStatistic.SortOrder = StatisticSortOrder.Descending;
                            s_letter_statistics.Sort();
                            foreach (LetterStatistic letter_statistic in s_letter_statistics)
                            {
                                letter_values.Add(letter_statistic.Frequency);
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("Gematria"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.Gematria.Count)
                                {
                                    letter_values.Add(Numbers.Gematria[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else if (s_numerology_system.Name.EndsWith("QuranNumbers"))
                        {
                            for (int i = 0; i < letter_order.Count; i++)
                            {
                                if (i < Numbers.QuranNumbers.Count)
                                {
                                    letter_values.Add(Numbers.QuranNumbers[i]);
                                }
                                else
                                {
                                    letter_values.Add(0L);
                                }
                            }
                        }
                        else // if not defined in Numbers
                        {
                            // use loadeded numerology system instead
                            foreach (long value in loaded_numerology_system.LetterValues.Values)
                            {
                                letter_values.Add(value);
                            }
                        }

                        // finally
                        // rebuild the current numerology system
                        s_numerology_system.Clear();
                        for (int i = 0; i < letter_order.Count; i++)
                        {
                            s_numerology_system.Add(letter_order[i], letter_values[i]);
                        }
                    }
                }
            }
        }
    }

    // letter statistics [DYNAMIC]
    private static List<LetterStatistic> s_letter_statistics = new List<LetterStatistic>();
    private static void BuildLetterStatistics(string dynamic_text)
    {
        if (String.IsNullOrEmpty(dynamic_text)) return;

        if (s_letter_statistics != null)
        {
            s_letter_statistics.Clear();
            for (int i = 0; i < dynamic_text.Length; i++)
            {
                // calculate letter frequency
                bool is_found = false;
                for (int j = 0; j < s_letter_statistics.Count; j++)
                {
                    if (dynamic_text[i] == s_letter_statistics[j].Letter)
                    {
                        s_letter_statistics[j].Frequency++;
                        is_found = true;
                        break;
                    }
                }

                // add entry into dictionary
                if (!is_found)
                {
                    LetterStatistic letter_statistic = new LetterStatistic();
                    letter_statistic.Order = s_letter_statistics.Count + 1;
                    letter_statistic.Letter = dynamic_text[i];
                    letter_statistic.Frequency++;
                    s_letter_statistics.Add(letter_statistic);
                }
            }
        }
    }

    // used for non-Quran text
    public static long CalculateValue(char user_char)
    {
        if (user_char == '\0') return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            result = s_numerology_system.CalculateValue(user_char);
        }
        return result;
    }
    public static long CalculateValue(string user_text)
    {
        if (string.IsNullOrEmpty(user_text)) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            result = s_numerology_system.CalculateValue(user_text);
        }
        return result;
    }
    // used for Quran text only
    public static long CalculateValue(Letter letter)
    {
        if (letter == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            result += s_numerology_system.CalculateValue(letter.Character);
        }
        return result;
    }
    public static long CalculateValue(Word word)
    {
        if (word == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Letter letter in word.Letters)
            {
                result += s_numerology_system.CalculateValue(letter.Character);
            }
        }
        return result;
    }
    public static long CalculateValue(Verse verse)
    {
        if (verse == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Word word in verse.Words)
            {
                foreach (Letter letter in word.Letters)
                {
                    result += s_numerology_system.CalculateValue(letter.Character);
                }
            }
        }
        return result;
    }
    public static long CalculateValue(Sentence sentence)
    {
        if (sentence == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            List<Word> words = GetCompleteWords(sentence);
            if (words != null)
            {
                foreach (Word word in words)
                {
                    foreach (Letter letter in word.Letters)
                    {
                        result += s_numerology_system.CalculateValue(letter.Character);
                    }
                }
            }
        }
        return result;
    }
    private static List<Word> GetCompleteWords(Sentence sentence)
    {
        if (sentence == null) return null;

        List<Word> result = new List<Word>();
        if (sentence.FirstVerse.Number == sentence.LastVerse.Number)
        {
            foreach (Word word in sentence.FirstVerse.Words)
            {
                if ((word.Position >= sentence.StartPosition) && (word.Position < sentence.EndPosition))
                {
                    result.Add(word);
                }
            }
        }
        else // multi-verse
        {
            // first verse
            foreach (Word word in sentence.FirstVerse.Words)
            {
                if (word.Position >= sentence.StartPosition)
                {
                    result.Add(word);
                }
            }

            // middle verses
            int after_first_index = (sentence.FirstVerse.Number + 1) - 1;
            int before_last_index = (sentence.LastVerse.Number - 1) - 1;
            if (after_first_index <= before_last_index)
            {
                for (int i = after_first_index; i <= before_last_index; i++)
                {
                    result.AddRange(sentence.FirstVerse.Book.Verses[i].Words);
                }
            }

            // last verse
            foreach (Word word in sentence.LastVerse.Words)
            {
                if (word.Position < sentence.EndPosition) // not <= because EndPosition is after the start of the last word in the sentence
                {
                    result.Add(word);
                }
            }
        }
        return result;
    }
    private static List<Verse> GetCompleteVerses(Sentence sentence)
    {
        if (sentence == null) return null;

        List<Verse> result = new List<Verse>();
        if (sentence.FirstVerse.Number == sentence.LastVerse.Number)
        {
            if ((sentence.StartPosition == 0) && (sentence.EndPosition == sentence.Text.Length - 1))
            {
                result.Add(sentence.FirstVerse);
            }
        }
        else // multi-verse
        {
            // first verse
            if (sentence.StartPosition == 0)
            {
                result.Add(sentence.FirstVerse);
            }

            // middle verses
            int after_first_index = (sentence.FirstVerse.Number + 1) - 1;
            int before_last_index = (sentence.LastVerse.Number - 1) - 1;
            if (after_first_index <= before_last_index)
            {
                for (int i = after_first_index; i <= before_last_index; i++)
                {
                    result.Add(sentence.FirstVerse.Book.Verses[i]);
                }
            }

            // last verse
            if (sentence.EndPosition == sentence.LastVerse.Text.Length - 1)
            {
                result.Add(sentence.LastVerse);
            }
        }
        return result;
    }
    public static long CalculateValue(List<Verse> verses)
    {
        if (verses == null) return 0L;
        if (verses.Count == 0) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Verse verse in verses)
            {
                result += CalculateValue(verse);
            }
        }
        return result;
    }
    public static long CalculateValue(Chapter chapter)
    {
        if (chapter == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            result = CalculateValue(chapter.Verses);
            chapter.Value = result; // update chapter values for ChapterSortMethod.ByValue
        }
        return result;
    }
    public static long CalculateValue(Book book)
    {
        if (book == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Chapter chapter in book.Chapters)
            {
                result += CalculateValue(chapter.Verses);
            }
        }
        return result;
    }
    public static long CalculateValue(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2)
    {
        if (verses == null) return 0L;
        if (verses.Count == 0) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (verses.Count == 1)
            {
                result += CalculateMiddlePartValue(verses[0], letter_index_in_verse1, letter_index_in_verse2);
            }
            else if (verses.Count == 2)
            {
                result += CalculateEndPartValue(verses[0], letter_index_in_verse1);
                result += CalculateBeginningPartValue(verses[1], letter_index_in_verse2);
            }
            else //if (verses.Count > 2)
            {
                result += CalculateEndPartValue(verses[0], letter_index_in_verse1);

                // middle verses
                for (int i = 1; i < verses.Count - 1; i++)
                {
                    result += CalculateValue(verses[i]);
                }

                result += CalculateBeginningPartValue(verses[verses.Count - 1], letter_index_in_verse2);
            }
        }
        return result;
    }
    private static List<Chapter> GetCompleteChapters(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2)
    {
        if (verses == null) return null;
        if (verses.Count == 0) return null;

        List<Chapter> result = new List<Chapter>();
        List<Verse> complete_verses = new List<Verse>(verses); // make a copy so we don't change the passed verses

        if (complete_verses != null)
        {
            if (complete_verses.Count > 0)
            {
                Verse first_verse = complete_verses[0];
                if (first_verse != null)
                {
                    if (letter_index_in_verse1 != 0)
                    {
                        complete_verses.Remove(first_verse);
                    }
                }

                if (complete_verses.Count > 0) // check again after maybe removing a verse
                {
                    Verse last_verse = complete_verses[complete_verses.Count - 1];
                    if (last_verse != null)
                    {
                        if (letter_index_in_verse2 != last_verse.LetterCount - 1)
                        {
                            complete_verses.Remove(last_verse);
                        }
                    }
                }

                if (complete_verses.Count > 0) // check again after maybe removing a verse
                {
                    foreach (Chapter chapter in s_book.Chapters)
                    {
                        bool include_chapter = true;
                        foreach (Verse v in chapter.Verses)
                        {
                            if (!complete_verses.Contains(v))
                            {
                                include_chapter = false;
                                break;
                            }
                        }

                        if (include_chapter)
                        {
                            result.Add(chapter);
                        }
                    }
                }
            }
        }

        return result;
    }
    private static long CalculateBeginningPartValue(Verse verse, int to_letter_index)
    {
        return CalculateMiddlePartValue(verse, 0, to_letter_index);
    }
    private static long CalculateMiddlePartValue(Verse verse, int from_letter_index, int to_letter_index)
    {
        if (verse == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            int word_index = -1;   // in verse
            int letter_index = -1; // in verse
            bool done = false;
            foreach (Word word in verse.Words)
            {
                word_index++;
                if (word.Letters != null)
                {
                    foreach (Letter letter in word.Letters)
                    {
                        letter_index++;

                        if (letter_index < from_letter_index) continue;
                        if (letter_index > to_letter_index)
                        {
                            done = true;
                            break;
                        }

                        result += s_numerology_system.CalculateValue(letter.Character);
                    }
                }
                if (done) break;
            }
        }
        return result;
    }
    private static long CalculateEndPartValue(Verse verse, int from_letter_index)
    {
        return CalculateMiddlePartValue(verse, from_letter_index, verse.LetterCount - 1);
    }

    // helper methods for finds
    public static List<Verse> GetSourceVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result)
    {
        List<Verse> result = new List<Verse>();
        if (s_book != null)
        {
            if (search_scope == SearchScope.Book)
            {
                result = s_book.Verses;
            }
            else if (search_scope == SearchScope.Selection)
            {
                result = current_selection.Verses;
            }
            else if (search_scope == SearchScope.Result)
            {
                if (previous_result != null)
                {
                    result = new List<Verse>(previous_result);
                }
            }
        }
        return result;
    }
    public static List<Verse> GetVerses(List<Phrase> phrases)
    {
        List<Verse> result = new List<Verse>();
        if (phrases != null)
        {
            foreach (Phrase phrase in phrases)
            {
                if (phrase != null)
                {
                    if (!result.Contains(phrase.Verse))
                    {
                        result.Add(phrase.Verse);
                    }
                }
            }
        }
        return result;
    }
    public static List<Phrase> BuildPhrases(Verse verse, MatchCollection matches)
    {
        List<Phrase> result = new List<Phrase>();
        foreach (Match match in matches)
        {
            foreach (Capture capture in match.Captures)
            {
                string text = capture.Value;
                int position = capture.Index;
                Phrase phrase = new Phrase(verse, position, text);
                if (phrase != null)
                {
                    result.Add(phrase);
                }
            }
        }
        return result;
    }
    private static List<Phrase> BuildPhrasesAndOriginify(Verse verse, MatchCollection matches)
    {
        List<Phrase> result = new List<Phrase>();
        foreach (Match match in matches)
        {
            foreach (Capture capture in match.Captures)
            {
                string text = capture.Value;
                int position = capture.Index;
                Phrase phrase = new Phrase(verse, position, text);
                if (phrase != null)
                {
                    if (s_numerology_system != null)
                    {
                        if (s_numerology_system.TextMode == "Original")
                        {
                            phrase = OriginifyPhrase(phrase);
                        }
                        if (phrase != null)
                        {
                            result.Add(phrase);
                        }
                    }
                }
            }
        }
        return result;
    }
    private static Phrase OriginifyPhrase(Phrase phrase)
    {
        if (phrase != null)
        {
            // simple phrase
            Verse verse = phrase.Verse;
            string text = phrase.Text;
            int position = phrase.Position;

            // convert to original
            if (verse != null)
            {
                int start = 0;
                for (int i = 0; i < verse.Text.Length; i++)
                {
                    char character = verse.Text[i];

                    if ((character == ' ') || (Constants.ARABIC_LETTERS.Contains(character)))
                    {
                        start++;
                    }
                    else if ((Constants.STOPMARKS.Contains(character)) || (Constants.QURANMARKS.Contains(character)))
                    {
                        start--; // ignore space after stopmark
                        if (start < 0)
                        {
                            start = 0;
                        }
                    }

                    // i has reached phrase start
                    if (start > position)
                    {
                        int phrase_length = text.Trim().Length;
                        StringBuilder str = new StringBuilder();

                        int length = 0;
                        for (int j = i; j < verse.Text.Length; j++)
                        {
                            character = verse.Text[j];
                            str.Append(character);

                            if ((character == ' ') || (Constants.ARABIC_LETTERS.Contains(character)))
                            {
                                length++;
                            }
                            else if ((Constants.STOPMARKS.Contains(character)) || (Constants.QURANMARKS.Contains(character)))
                            {
                                length--; // ignore space after stopmark
                                if (length < 0)
                                {
                                    length = 0;
                                }
                            }

                            // j has reached phrase end
                            if (length == phrase_length)
                            {
                                return new Phrase(verse, i, str.ToString());
                            }
                        }
                    }
                }
            }
        }
        return null;
    }
    public static Phrase SwitchTextMode(Phrase phrase, string to_text_mode)
    {
        if (phrase != null)
        {
            if (to_text_mode == "Original")
            {
                // simple phrase
                Verse phrase_verse = phrase.Verse;
                string phrase_text = phrase.Text.Trim();
                int phrase_position = phrase.Position;
                int phrase_length = phrase_text.Length;

                // convert to original
                if (phrase_verse != null)
                {
                    int letter_count = 0;
                    int position = 0;
                    for (int i = 0; i < phrase_verse.Text.Length; i++)
                    {
                        char character = phrase_verse.Text[i];
                        if ((character == ' ') || (Constants.ARABIC_LETTERS.Contains(character)))
                        {
                            letter_count++;
                        }
                        else if (Constants.STOPMARKS.Contains(character))
                        {
                            letter_count--; // decrement space after stopmark as it will be incremented above
                            if (letter_count < 0)
                            {
                                letter_count = 0;
                            }
                        }
                        else if (Constants.QURANMARKS.Contains(character))
                        {
                            letter_count--; // decrement space after stopmark as it will be incremented above
                        }

                        // check if finished
                        if (letter_count == phrase_position)
                        {
                            position = i;
                            break;
                        }
                    }

                    letter_count = 0;
                    StringBuilder str = new StringBuilder();
                    for (int i = position; i < phrase_verse.Text.Length; i++)
                    {
                        char character = phrase_verse.Text[i];
                        str.Append(character);

                        if ((character == ' ') || (Constants.ARABIC_LETTERS.Contains(character)))
                        {
                            letter_count++;
                        }
                        else if (Constants.STOPMARKS.Contains(character))
                        {
                            letter_count--; // decrement space after stopmark as it will be incremented above
                            if (letter_count < 0)
                            {
                                letter_count = 0;
                            }
                        }
                        else if (Constants.QURANMARKS.Contains(character))
                        {
                            letter_count--; // decrement space after quranmark as it will be incremented above
                        }

                        // check if finished
                        if (letter_count == phrase_length)
                        {
                            // skip any non-letter at start
                            int index = position;
                            if ((index > 0) && (index < phrase_verse.Text.Length))
                            {
                                character = phrase_verse.Text[index];
                                if (!Constants.ARABIC_LETTERS.Contains(character))
                                {
                                    position++;
                                    str.Append(" "); // increment length
                                }
                            }

                            // skip any non-letter at end
                            index = position + str.Length - 1;
                            if ((index > 0) && (position + str.Length < phrase_verse.Text.Length))
                            {
                                character = phrase_verse.Text[index];
                                if (!Constants.ARABIC_LETTERS.Contains(character))
                                {
                                    str.Append(" "); // increment length
                                }
                            }

                            return new Phrase(phrase_verse, position, str.ToString());
                        }
                    }
                }
            }
            else // simplify phrase
            {
                //??? TODO: ConvertPhrase to simplified still not complete:  ONLY builds first phrase occurrence in verse

                Verse verse = phrase.Verse;
                int position = phrase.Position;
                string text = phrase.Text;

                // simplifiy text
                text = text.SimplifyTo(to_text_mode);
                text = text.Trim();
                if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                {
                    // simplifiy position
                    string verse_text = verse.Text.SimplifyTo(to_text_mode);
                    position = verse_text.IndexOf(text);  // will ONLY build first phrase occurrence in verse

                    // build simplified phrase
                    return new Phrase(verse, position, text);
                }
            }
        }
        return null;
    }

    // find by text - Exact
    public static List<Phrase> FindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, LanguageType language_type, string translation, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, bool case_sensitive, TextWordness wordness, int multiplicity, bool with_diacritics)
    {
        List<Phrase> result = new List<Phrase>();

        if (language_type == LanguageType.RightToLeft)
        {
            result = DoFindPhrases(search_scope, current_selection, previous_result, text, language_type, translation, text_location_in_verse, text_location_in_word, case_sensitive, wordness, multiplicity, with_diacritics, true);
        }
        else if (language_type == LanguageType.LeftToRight)
        {
            if (s_book != null)
            {
                if (s_book.Verses != null)
                {
                    if (s_book.Verses.Count > 0)
                    {
                        foreach (string key in s_book.Verses[0].Translations.Keys)
                        {
                            List<Phrase> new_phrases = DoFindPhrases(search_scope, current_selection, previous_result, text, language_type, key, text_location_in_verse, text_location_in_word, case_sensitive, wordness, multiplicity, with_diacritics, false);

                            result.AddRange(new_phrases);
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, LanguageType language_type, string translation, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, bool case_sensitive, TextWordness wordness, int multiplicity, bool with_diacritics, bool try_emlaaei_if_nothing_found)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        if (language_type == LanguageType.RightToLeft)
        {
            return DoFindPhrases(source, search_scope, current_selection, previous_result, text, text_location_in_verse, text_location_in_word, wordness, multiplicity, with_diacritics, try_emlaaei_if_nothing_found);
        }
        else //if (language_type == FindByTextLanguageType.LeftToRight)
        {
            return DoFindPhrases(translation, source, search_scope, current_selection, previous_result, text, text_location_in_verse, text_location_in_word, case_sensitive, wordness, multiplicity);
        }
    }
    private static List<Phrase> DoFindPhrases(List<Verse> source, SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness wordness, int multiplicity, bool with_diacritics, bool try_emlaaei_if_nothing_found)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

                    RegexOptions regex_options = RegexOptions.IgnoreCase | RegexOptions.RightToLeft;

                    try
                    {
                        if (with_diacritics)
                        {
                            text = BuildPattern(text, text_location_in_verse, text_location_in_word, wordness);
                            if (!String.IsNullOrEmpty(text))
                            {
                                foreach (Verse verse in source)
                                {
                                    string verse_text = verse.Text;
                                    MatchCollection matches = Regex.Matches(verse_text, text, regex_options);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            result.AddRange(BuildPhrases(verse, matches));
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (matches.Count >= multiplicity)
                                        {
                                            if (matches.Count > 0)
                                            {
                                                result.AddRange(BuildPhrases(verse, matches));
                                            }
                                            else
                                            {
                                                result.Add(new Phrase(verse, 0, ""));
                                            }
                                        }
                                    }
                                } // end for
                            }
                        }
                        else // if without diacritics
                        {
                            if (s_numerology_system != null)
                            {
                                text = text.SimplifyTo(s_numerology_system.TextMode);
                                if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                {
                                    text = text.Simplify29();
                                }
                                if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                                {
                                    text = BuildPattern(text, text_location_in_verse, text_location_in_word, wordness);
                                    if (!String.IsNullOrEmpty(text))
                                    {
                                        foreach (Verse verse in source)
                                        {
                                            string verse_text = verse.Text.SimplifyTo(s_numerology_system.TextMode);
                                            if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                            {
                                                verse_text = verse_text.Simplify29();
                                            }
                                            MatchCollection matches = Regex.Matches(verse_text, text, regex_options);
                                            if (multiplicity == -1) // without multiplicity
                                            {
                                                if (matches.Count > 0)
                                                {
                                                    result.AddRange(BuildPhrasesAndOriginify(verse, matches));
                                                }
                                            }
                                            else // with multiplicity
                                            {
                                                if (matches.Count >= multiplicity)
                                                {
                                                    if (matches.Count > 0)
                                                    {
                                                        result.AddRange(BuildPhrasesAndOriginify(verse, matches));
                                                    }
                                                    else
                                                    {
                                                        result.Add(new Phrase(verse, 0, ""));
                                                    }
                                                }
                                            }
                                        } // end for
                                    }
                                }
                            }
                        }

                        // if nothing found
                        if ((multiplicity != 0) && (result.Count == 0))
                        {
                            //  search in emlaaei
                            if (try_emlaaei_if_nothing_found)
                            {
                                if (s_numerology_system != null)
                                {
                                    text = text.SimplifyTo(s_numerology_system.TextMode);
                                    if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                    {
                                        text = text.Simplify29();
                                    }

                                    if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                                    {
                                        if ((source != null) && (source.Count > 0))
                                        {
                                            foreach (Verse verse in source)
                                            {
                                                string emlaaei_text = verse.Translations[DEFAULT_EMLAAEI_TEXT].SimplifyTo(s_numerology_system.TextMode);
                                                if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                                {
                                                    emlaaei_text = emlaaei_text.Simplify29();
                                                }

                                                while (emlaaei_text.Contains("  "))
                                                {
                                                    emlaaei_text = emlaaei_text.Replace("  ", " ");
                                                }

                                                MatchCollection matches = Regex.Matches(emlaaei_text, text, regex_options);
                                                if (multiplicity == -1) // without multiplicity
                                                {
                                                    if (matches.Count > 0)
                                                    {
                                                        // don't colorize matches in emlaaei text
                                                        //result.AddRange(BuildPhrases(verse, matches));
                                                        result.Add(new Phrase(verse, 0, ""));
                                                    }
                                                }
                                                else // with multiplicity
                                                {
                                                    if (matches.Count >= multiplicity)
                                                    {
                                                        // don't colorize matches in emlaaei text
                                                        //result.AddRange(BuildPhrases(verse, matches));
                                                        result.Add(new Phrase(verse, 0, ""));
                                                    }
                                                }
                                            } // end for
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(string translation, List<Verse> source, SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, bool case_sensitive, TextWordness wordness, int multiplicity)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

                    RegexOptions regex_options = case_sensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    if (text.IsArabic()) // Arabic letters in translation (Emlaaei, Urdu, Farsi, etc.) 
                    {
                        regex_options |= RegexOptions.RightToLeft;
                    }

                    try
                    {
                        string pattern_empty_line = @"^$";
                        string pattern_whole_line = "(" + @"^" + text + @"$" + ")";

                        string pattern_any_with_prefix = "(" + @"\S+?" + text + ")";
                        string pattern_any_with_prefix_and_suffix = "(" + @"\S+?" + text + @"\S+?" + ")";
                        string pattern_any_with_suffix = "(" + text + @"\S+?" + ")";

                        string pattern_word_with_prefix = "(" + pattern_any_with_prefix + @"\b" + ")";
                        string pattern_word_with_prefix_and_suffix = "(" + pattern_any_with_prefix_and_suffix + ")";
                        string pattern_word_with_suffix = "(" + @"\b" + pattern_any_with_suffix + ")";
                        string pattern_word_with_any_fixes = "(" + pattern_word_with_prefix + "|" + pattern_word_with_prefix_and_suffix + "|" + pattern_any_with_suffix + ")";

                        // Any == Whole word | Part of word
                        string pattern_any_at_start = "(" + pattern_whole_line + "|" + @"^" + text + ")";
                        string pattern_any_at_middle = "(" + pattern_whole_line + "|" + @"(?<!^)" + text + @"(?!$)" + ")";
                        string pattern_any_at_end = "(" + pattern_whole_line + "|" + text + @"$" + ")";
                        string pattern_any_anywhere = text;

                        // Part of word
                        string pattern_part_word_at_start = "(" + @"^" + pattern_word_with_any_fixes + ")";
                        string pattern_part_word_at_middle = "(" + @"(?<!^)" + pattern_word_with_any_fixes + @"(?!$)" + ")";
                        string pattern_part_word_at_end = "(" + pattern_word_with_any_fixes + @"$" + ")";
                        string pattern_part_word_anywhere = "(" + pattern_part_word_at_start + "|" + pattern_part_word_at_middle + "|" + pattern_part_word_at_end + ")";

                        // Whole word
                        string pattern_whole_word_at_start = "(" + pattern_whole_line + "|" + @"^" + text + @"\b" + ")";
                        string pattern_whole_word_at_middle = "(" + pattern_whole_line + "|" + @"(?<!^)" + @"\b" + text + @"\b" + @"(?!$)" + ")";
                        string pattern_whole_word_at_end = "(" + pattern_whole_line + "|" + @"\b" + text + @"$" + ")";
                        string pattern_whole_word_anywhere = "(" + pattern_whole_line + "|" + @"\b" + text + @"\b" + ")";

                        string pattern = null;

                        switch (text_location_in_verse)
                        {
                            case TextLocationInVerse.Anywhere:
                                {
                                    if (wordness == TextWordness.Any)
                                    {
                                        pattern += pattern_any_anywhere;
                                    }
                                    else if (wordness == TextWordness.PartOfWord)
                                    {
                                        pattern += pattern_part_word_anywhere;
                                    }
                                    else if (wordness == TextWordness.WholeWord)
                                    {
                                        pattern += pattern_whole_word_anywhere;
                                    }
                                    else
                                    {
                                        pattern += pattern_empty_line;
                                    }
                                }
                                break;
                            case TextLocationInVerse.AtStart:
                                {
                                    if (wordness == TextWordness.Any)
                                    {
                                        pattern += pattern_any_at_start;
                                    }
                                    else if (wordness == TextWordness.PartOfWord)
                                    {
                                        pattern += pattern_part_word_at_start;
                                    }
                                    else if (wordness == TextWordness.WholeWord)
                                    {
                                        pattern += pattern_whole_word_at_start;
                                    }
                                    else
                                    {
                                        pattern += pattern_empty_line;
                                    }
                                }
                                break;
                            case TextLocationInVerse.AtMiddle:
                                {
                                    if (wordness == TextWordness.Any)
                                    {
                                        pattern += pattern_any_at_middle;
                                    }
                                    else if (wordness == TextWordness.PartOfWord)
                                    {
                                        pattern += pattern_part_word_at_middle;
                                    }
                                    else if (wordness == TextWordness.WholeWord)
                                    {
                                        pattern += pattern_whole_word_at_middle;
                                    }
                                    else
                                    {
                                        pattern += pattern_empty_line;
                                    }
                                }
                                break;
                            case TextLocationInVerse.AtEnd:
                                {
                                    if (wordness == TextWordness.Any)
                                    {
                                        pattern += pattern_any_at_end;
                                    }
                                    else if (wordness == TextWordness.PartOfWord)
                                    {
                                        pattern += pattern_part_word_at_end;
                                    }
                                    else if (wordness == TextWordness.WholeWord)
                                    {
                                        pattern += pattern_whole_word_at_end;
                                    }
                                    else
                                    {
                                        pattern += pattern_empty_line;
                                    }
                                }
                                break;
                            default:
                                {
                                    return new List<Phrase>();
                                }
                        }

                        switch (text_location_in_word)
                        {
                            case TextLocationInWord.Anywhere:
                                {
                                    // do noting
                                }
                                break;
                            case TextLocationInWord.AtStart:
                                {
                                    pattern = @"(" + @"(?<=\b)" + pattern + @")"; // positive lookbehind
                                }
                                break;
                            case TextLocationInWord.AtMiddle:
                                {
                                    pattern = @"(" + @"(?<!\s)" + pattern + @"(?!\s)" + @")"; // positive lookbehind and lookahead
                                }
                                break;
                            case TextLocationInWord.AtEnd:
                                {
                                    pattern = @"(" + pattern + @"(?=\b)" + @")"; // positive lookahead
                                }
                                break;
                        }

                        // do actual search
                        foreach (Verse verse in source)
                        {
                            MatchCollection matches = Regex.Matches(verse.Translations[translation], pattern, regex_options);
                            if (multiplicity == -1) // without multiplicity
                            {
                                if (matches.Count > 0)
                                {
                                    // don't colorize non-Arabic matches in Quran text
                                    //result.AddRange(BuildPhrasesAndOriginify(verse, matches));
                                    result.Add(new Phrase(verse, 0, ""));
                                }
                            }
                            else // with multiplicity
                            {
                                if (matches.Count >= multiplicity)
                                {
                                    if (matches.Count > 0)
                                    {
                                        // don't colorize non-Arabic matches in Quran text
                                        //result.AddRange(BuildPhrasesAndOriginify(verse, matches));
                                        result.Add(new Phrase(verse, 0, ""));
                                    }
                                    else
                                    {
                                        result.Add(new Phrase(verse, 0, ""));
                                    }
                                }
                            }
                        } // end for
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }
        return result;
    }
    private static string BuildPattern(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness wordness)
    {
        string pattern = null;

        if (String.IsNullOrEmpty(text)) return text;
        text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any
        //// comment out to allow end of words // text = text.Trim();

        /*
        =====================================================================
        Regular Expressions (RegEx)
        =====================================================================
        Best Reference: http://www.regular-expressions.info/
        =====================================================================
        Matches	Characters 
        x	character x 
        \\	backslash character 
        \0n	character with octal value 0n (0 <= n <= 7) 
        \0nn	character with octal value 0nn (0 <= n <= 7) 
        \0mnn	character with octal value 0mnn (0 <= m <= 3, 0 <= n <= 7) 
        \xhh	character with hexadecimal value 0xhh 
        \uhhhh	character with hexadecimal value 0xhhhh 
        \t	tab character ('\u0009') 
        \n	newline (line feed) character ('\u000A') 
        \r	carriage-return character ('\u000D') 
        \f	form-feed character ('\u000C') 
        \a	alert (bell) character ('\u0007') 
        \e	escape character ('\u001B') 
        \cx	control character corresponding to x 
                                  
        Character Classes 
        [abc]		    a, b, or c				                    (simple class) 
        [^abc]		    any character except a, b, or c		        (negation) 
        [a-zA-Z]	    a through z or A through Z, inclusive	    (range) 
        [a-d[m-p]]	    a through d, or m through p: [a-dm-p]	    (union) 
        [a-z&&[def]]	d, e, or f				                    (intersection) 
        [a-z&&[^bc]]	a through z, except for b and c: [ad-z]	    (subtraction) 
        [a-z&&[^m-p]]	a through z, and not m through p: [a-lq-z]  (subtraction) 
                                  
        Predefined 
        .	any character (inc line terminators) except newline 
        \d	digit				            [0-9] 
        \D	non-digit			            [^0-9] 
        \s	whitespace character		    [ \t\n\x0B\f\r] 
        \S	non-whitespace character	    [^\s] 
        \w	word character (alphanumeric)	[a-zA-Z_0-9] 
        \W	non-word character		        [^\w] 

        Boundary Matchers 
        ^	beginning of a line	(in Multiline)
        $	end of a line  		(in Multiline)
        \b	word boundary 
        \B	non-word boundary 
        \A	beginning of the input 
        \G	end of the previous match 
        \Z	end of the input but for the final terminator, if any 
        \z	end of the input

        Greedy quantifiers 
        X?	X, once or not at all 
        X*	X, zero or more times 
        X+	X, one or more times 
        X{n}	X, exactly n times 
        X{n,}	X, at least n times 
        X{n,m}	X, at least n but not more than m times 
                                  
        Reluctant quantifiers 
        X??	X, once or not at all 
        X*?	X, zero or more times 
        X+?	X, one or more times 
        X{n}?	X, exactly n times 
        X{n,}?	X, at least n times 
        X{n,m}?	X, at least n but not more than m times 
                                  
        Possessive quantifiers 
        X?+	X, once or not at all 
        X*+	X, zero or more times 
        X++	X, one or more times 
        X{n}+	X, exactly n times 
        X{n,}+	X, at least n times 
        X{n,m}+	X, at least n but not more than m times 

        positive lookahead	(?=text)
        negative lookahead	(?!text)
        // eg: not at end of line 	    (?!$)
        positive lookbehind	(?<=text)
        negative lookbehind	(?<!text)
        // eg: not at start of line 	(?<!^)
        =====================================================================
        */

        string pattern_empty_line = @"^$";
        string pattern_whole_line = "(" + @"^" + text + @"$" + ")";

        string pattern_any_with_prefix = "(" + @"\S+?" + text + ")";
        string pattern_any_with_prefix_and_suffix = "(" + @"\S+?" + text + @"\S+?" + ")";
        string pattern_any_with_suffix = "(" + text + @"\S+?" + ")";

        string pattern_word_with_prefix = "(" + pattern_any_with_prefix + @"\b" + ")";
        string pattern_word_with_prefix_and_suffix = "(" + pattern_any_with_prefix_and_suffix + ")";
        string pattern_word_with_suffix = "(" + @"\b" + pattern_any_with_suffix + ")";
        string pattern_word_with_any_fixes = "(" + pattern_word_with_prefix + "|" + pattern_word_with_prefix_and_suffix + "|" + pattern_any_with_suffix + ")";

        // Any == Whole word | Part of word
        string pattern_any_at_start = "(" + pattern_whole_line + "|" + @"^" + text + ")";
        string pattern_any_at_middle = "(" + pattern_whole_line + "|" + @"(?<!^)" + text + @"(?!$)" + ")";
        string pattern_any_at_end = "(" + pattern_whole_line + "|" + text + @"$" + ")";
        string pattern_any_anywhere = text;

        // Part of word
        string pattern_part_word_at_start = "(" + @"^" + pattern_word_with_any_fixes + ")";
        string pattern_part_word_at_middle = "(" + @"(?<!^)" + pattern_word_with_any_fixes + @"(?!$)" + ")";
        string pattern_part_word_at_end = "(" + pattern_word_with_any_fixes + @"$" + ")";
        string pattern_part_word_anywhere = "(" + pattern_part_word_at_start + "|" + pattern_part_word_at_middle + "|" + pattern_part_word_at_end + ")";

        // Whole word
        string pattern_whole_word_at_start = "(" + pattern_whole_line + "|" + @"^" + text + @"\b" + ")";
        string pattern_whole_word_at_middle = "(" + pattern_whole_line + "|" + @"(?<!^)" + @"\b" + text + @"\b" + @"(?!$)" + ")";
        string pattern_whole_word_at_end = "(" + pattern_whole_line + "|" + @"\b" + text + @"$" + ")";
        string pattern_whole_word_anywhere = "(" + pattern_whole_line + "|" + @"\b" + text + @"\b" + ")";

        switch (text_location_in_verse)
        {
            case TextLocationInVerse.Anywhere:
                {
                    if (wordness == TextWordness.Any)
                    {
                        pattern += pattern_any_anywhere;
                    }
                    else if (wordness == TextWordness.PartOfWord)
                    {
                        pattern += pattern_part_word_anywhere;
                    }
                    else if (wordness == TextWordness.WholeWord)
                    {
                        pattern += pattern_whole_word_anywhere;
                    }
                    else
                    {
                        pattern += pattern_empty_line;
                    }
                }
                break;
            case TextLocationInVerse.AtStart:
                {
                    if (wordness == TextWordness.Any)
                    {
                        pattern += pattern_any_at_start;
                    }
                    else if (wordness == TextWordness.PartOfWord)
                    {
                        pattern += pattern_part_word_at_start;
                    }
                    else if (wordness == TextWordness.WholeWord)
                    {
                        pattern += pattern_whole_word_at_start;
                    }
                    else
                    {
                        pattern += pattern_empty_line;
                    }
                }
                break;
            case TextLocationInVerse.AtMiddle:
                {
                    if (wordness == TextWordness.Any)
                    {
                        pattern += pattern_any_at_middle;
                    }
                    else if (wordness == TextWordness.PartOfWord)
                    {
                        pattern += pattern_part_word_at_middle;
                    }
                    else if (wordness == TextWordness.WholeWord)
                    {
                        pattern += pattern_whole_word_at_middle;
                    }
                    else
                    {
                        pattern += pattern_empty_line;
                    }
                }
                break;
            case TextLocationInVerse.AtEnd:
                {
                    if (wordness == TextWordness.Any)
                    {
                        pattern += pattern_any_at_end;
                    }
                    else if (wordness == TextWordness.PartOfWord)
                    {
                        pattern += pattern_part_word_at_end;
                    }
                    else if (wordness == TextWordness.WholeWord)
                    {
                        pattern += pattern_whole_word_at_end;
                    }
                    else
                    {
                        pattern += pattern_empty_line;
                    }
                }
                break;
        }

        switch (text_location_in_word)
        {
            case TextLocationInWord.Anywhere:
                {
                    // do noting
                }
                break;
            case TextLocationInWord.AtStart:
                {
                    pattern = @"(" + @"(?<=\b)" + pattern + @")"; // positive lookbehind
                }
                break;
            case TextLocationInWord.AtMiddle:
                {
                    pattern = @"(" + @"(?<!\s)" + pattern + @"(?!\s)" + @")"; // positive lookbehind and lookahead
                }
                break;
            case TextLocationInWord.AtEnd:
                {
                    pattern = @"(" + pattern + @"(?=\b)" + @")"; // positive lookahead
                }
                break;
        }

        return pattern;
    }
    // find by text - Proximity
    public static List<Phrase> FindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, LanguageType language_type, string translation, ProximitySearchType proximity_search_type, bool case_sensitive, TextWordness wordness, bool with_diacritics)
    {
        List<Phrase> result = new List<Phrase>();

        if (language_type == LanguageType.RightToLeft)
        {
            result = DoFindPhrases(search_scope, current_selection, previous_result, text, language_type, translation, proximity_search_type, case_sensitive, wordness, with_diacritics, true);
        }
        else if (language_type == LanguageType.LeftToRight)
        {
            if (s_book != null)
            {
                if (s_book.Verses != null)
                {
                    if (s_book.Verses.Count > 0)
                    {
                        foreach (string key in s_book.Verses[0].Translations.Keys)
                        {
                            List<Phrase> new_phrases = DoFindPhrases(search_scope, current_selection, previous_result, text, language_type, key, proximity_search_type, case_sensitive, wordness, with_diacritics, false);

                            result.AddRange(new_phrases);
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, LanguageType language_type, string translation, ProximitySearchType proximity_search_type, bool case_sensitive, TextWordness wordness, bool with_diacritics, bool try_emlaaei_if_nothing_found)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        if (language_type == LanguageType.RightToLeft)
        {
            return DoFindPhrases(source, search_scope, current_selection, previous_result, text, proximity_search_type, wordness, with_diacritics, try_emlaaei_if_nothing_found);
        }
        else //if (language_type == FindByTextLanguageType.LeftToRight)
        {
            return DoFindPhrases(translation, source, search_scope, current_selection, previous_result, text, proximity_search_type, case_sensitive, wordness);
        }
    }
    private static List<Phrase> DoFindPhrases(List<Verse> source, SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, ProximitySearchType proximity_search_type, TextWordness wordness, bool with_diacritics, bool try_emlaaei_if_nothing_found)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

                    List<string> unsigned_words = null;
                    List<string> positive_words = null;
                    List<string> negative_words = null;

                    try
                    {
                        if (with_diacritics)
                        {
                            BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);
                            foreach (Verse verse in source)
                            {
                                /////////////////////////
                                // process negative_words
                                /////////////////////////
                                if (negative_words.Count > 0)
                                {
                                    bool found = false;
                                    foreach (string negative_word in negative_words)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            string word_text = word.Text;
                                            if (wordness == TextWordness.Any)
                                            {
                                                if (word_text.Contains(negative_word))
                                                {
                                                    found = true; // next verse
                                                    break;
                                                }
                                            }
                                            else if (wordness == TextWordness.PartOfWord)
                                            {
                                                if ((word_text.Contains(negative_word)) && (word_text.Length > negative_word.Length))
                                                {
                                                    found = true; // next verse
                                                    break;
                                                }
                                            }
                                            else if (wordness == TextWordness.WholeWord)
                                            {
                                                if (word_text == negative_word)
                                                {
                                                    found = true; // next verse
                                                    break;
                                                }
                                            }
                                        }
                                        if (found)
                                        {
                                            break;
                                        }
                                    }
                                    if (found) continue; // next verse
                                }

                                /////////////////////////
                                // process positive_words
                                /////////////////////////
                                if (positive_words.Count > 0)
                                {
                                    int match_count = 0;
                                    foreach (string positive_word in positive_words)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            string word_text = word.Text;
                                            if (wordness == TextWordness.Any)
                                            {
                                                if (word_text.Contains(positive_word))
                                                {
                                                    match_count++;
                                                    break; // next positive_word
                                                }
                                            }
                                            else if (wordness == TextWordness.PartOfWord)
                                            {
                                                if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                {
                                                    match_count++;
                                                    break; // next positive_word
                                                }
                                            }
                                            else if (wordness == TextWordness.WholeWord)
                                            {
                                                if (word_text == positive_word)
                                                {
                                                    match_count++;
                                                    break; // next positive_word
                                                }
                                            }
                                        }
                                    }

                                    // verse failed test, so skip it
                                    if (match_count < positive_words.Count)
                                    {
                                        continue; // next verse
                                    }
                                }

                                //////////////////////////////////////////////////////
                                // both negative and positive conditions have been met
                                //////////////////////////////////////////////////////

                                /////////////////////////
                                // process unsigned_words
                                /////////////////////////
                                //////////////////////////////////////////////////////////
                                // FindByText WORDS All
                                //////////////////////////////////////////////////////////
                                if (proximity_search_type == ProximitySearchType.AllWords)
                                {
                                    int match_count = 0;
                                    foreach (string unsigned_word in unsigned_words)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            string word_text = word.Text;
                                            if (wordness == TextWordness.Any)
                                            {
                                                if (word_text.Contains(unsigned_word))
                                                {
                                                    match_count++;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                            else if (wordness == TextWordness.PartOfWord)
                                            {
                                                if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                {
                                                    match_count++;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                            else if (wordness == TextWordness.WholeWord)
                                            {
                                                if (word_text == unsigned_word)
                                                {
                                                    match_count++;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                        }
                                    }

                                    if (match_count == unsigned_words.Count)
                                    {
                                        ///////////////////////////////////////////////////////////////
                                        // all negative, positive and unsigned conditions have been met
                                        ///////////////////////////////////////////////////////////////

                                        // add positive matches
                                        foreach (string positive_word in positive_words)
                                        {
                                            foreach (Word word in verse.Words)
                                            {
                                                string word_text = word.Text;
                                                if (wordness == TextWordness.Any)
                                                {
                                                    if (word_text.Contains(positive_word))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (wordness == TextWordness.PartOfWord)
                                                {
                                                    if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (wordness == TextWordness.WholeWord)
                                                {
                                                    if (word_text == positive_word)
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                            }
                                        }

                                        // add unsigned matches
                                        foreach (string unsigned_word in unsigned_words)
                                        {
                                            foreach (Word word in verse.Words)
                                            {
                                                string word_text = word.Text;
                                                if (wordness == TextWordness.Any)
                                                {
                                                    if (word_text.Contains(unsigned_word))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (wordness == TextWordness.PartOfWord)
                                                {
                                                    if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (wordness == TextWordness.WholeWord)
                                                {
                                                    if (word_text == unsigned_word)
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else // verse failed test, so skip it
                                    {
                                        continue; // next verse
                                    }
                                }
                                //////////////////////////////////////////////////////////
                                // FindByText WORDS Any
                                //////////////////////////////////////////////////////////
                                else if (proximity_search_type == ProximitySearchType.AnyWord)
                                {
                                    bool found = false;
                                    foreach (string unsigned_word in unsigned_words)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            string word_text = word.Text;
                                            if (wordness == TextWordness.Any)
                                            {
                                                if (word_text.Contains(unsigned_word))
                                                {
                                                    found = true;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                            else if (wordness == TextWordness.PartOfWord)
                                            {
                                                if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                {
                                                    found = true;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                            else if (wordness == TextWordness.WholeWord)
                                            {
                                                if (word_text == unsigned_word)
                                                {
                                                    found = true;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                        }
                                        if (found)
                                        {
                                            break;
                                        }
                                    }

                                    if (found) // found 1 unsigned word in verse, which is enough
                                    {
                                        ///////////////////////////////////////////////////////////////
                                        // all negative, positive and unsigned conditions have been met
                                        ///////////////////////////////////////////////////////////////

                                        // add positive matches
                                        foreach (string positive_word in positive_words)
                                        {
                                            foreach (Word word in verse.Words)
                                            {
                                                string word_text = word.Text;
                                                if (wordness == TextWordness.Any)
                                                {
                                                    if (word_text.Contains(positive_word))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (wordness == TextWordness.PartOfWord)
                                                {
                                                    if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (wordness == TextWordness.WholeWord)
                                                {
                                                    if (word_text == positive_word)
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                            }
                                        }

                                        // add unsigned matches
                                        foreach (string unsigned_word in unsigned_words)
                                        {
                                            foreach (Word word in verse.Words)
                                            {
                                                string word_text = word.Text;
                                                if (wordness == TextWordness.Any)
                                                {
                                                    if (word_text.Contains(unsigned_word))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (wordness == TextWordness.PartOfWord)
                                                {
                                                    if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (wordness == TextWordness.WholeWord)
                                                {
                                                    if (word_text == unsigned_word)
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else // verse failed test, so skip it
                                    {
                                        continue; // next verse
                                    }
                                }
                            } // end for
                        }
                        else // if without diacritics
                        {
                            if (s_numerology_system != null)
                            {
                                text = text.SimplifyTo(s_numerology_system.TextMode);
                                if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                {
                                    text = text.Simplify29();
                                }
                                if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                                {
                                    BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);
                                    foreach (Verse verse in source)
                                    {
                                        /////////////////////////
                                        // process negative_words
                                        /////////////////////////
                                        if (negative_words.Count > 0)
                                        {
                                            bool found = false;
                                            foreach (string negative_word in negative_words)
                                            {
                                                foreach (Word word in verse.Words)
                                                {
                                                    string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                    if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                                    {
                                                        word_text = word_text.Simplify29();
                                                    }

                                                    if (wordness == TextWordness.Any)
                                                    {
                                                        if (word_text.Contains(negative_word))
                                                        {
                                                            found = true; // next verse
                                                            break;
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((word_text.Contains(negative_word)) && (word_text.Length > negative_word.Length))
                                                        {
                                                            found = true; // next verse
                                                            break;
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.WholeWord)
                                                    {
                                                        if (word_text == negative_word)
                                                        {
                                                            found = true; // next verse
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (found)
                                                {
                                                    break;
                                                }
                                            }
                                            if (found) continue; // next verse
                                        }

                                        /////////////////////////
                                        // process positive_words
                                        /////////////////////////
                                        if (positive_words.Count > 0)
                                        {
                                            int match_count = 0;
                                            foreach (string positive_word in positive_words)
                                            {
                                                foreach (Word word in verse.Words)
                                                {
                                                    // simplify all text_modes
                                                    string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                    if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                                    {
                                                        word_text = word_text.Simplify29();
                                                    }

                                                    if (wordness == TextWordness.Any)
                                                    {
                                                        if (word_text.Contains(positive_word))
                                                        {
                                                            match_count++;
                                                            break; // next positive_word
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                        {
                                                            match_count++;
                                                            break; // next positive_word
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.WholeWord)
                                                    {
                                                        if (word_text == positive_word)
                                                        {
                                                            match_count++;
                                                            break; // next positive_word
                                                        }
                                                    }
                                                }
                                            }

                                            // verse failed test, so skip it
                                            if (match_count < positive_words.Count)
                                            {
                                                continue; // next verse
                                            }
                                        }

                                        //////////////////////////////////////////////////////
                                        // both negative and positive conditions have been met
                                        //////////////////////////////////////////////////////

                                        /////////////////////////
                                        // process unsigned_words
                                        /////////////////////////
                                        //////////////////////////////////////////////////////////
                                        // FindByText WORDS All
                                        //////////////////////////////////////////////////////////
                                        if (proximity_search_type == ProximitySearchType.AllWords)
                                        {
                                            int match_count = 0;
                                            foreach (string unsigned_word in unsigned_words)
                                            {
                                                foreach (Word word in verse.Words)
                                                {
                                                    // simplify all text_modes
                                                    string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                    if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                                    {
                                                        word_text = word_text.Simplify29();
                                                    }

                                                    if (wordness == TextWordness.Any)
                                                    {
                                                        if (word_text.Contains(unsigned_word))
                                                        {
                                                            match_count++;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                        {
                                                            match_count++;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.WholeWord)
                                                    {
                                                        if (word_text == unsigned_word)
                                                        {
                                                            match_count++;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                }
                                            }

                                            if (match_count == unsigned_words.Count)
                                            {
                                                ///////////////////////////////////////////////////////////////
                                                // all negative, positive and unsigned conditions have been met
                                                ///////////////////////////////////////////////////////////////

                                                // add positive matches
                                                foreach (string positive_word in positive_words)
                                                {
                                                    foreach (Word word in verse.Words)
                                                    {
                                                        // simplify all text_modes
                                                        string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                        if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                                        {
                                                            word_text = word_text.Simplify29();
                                                        }

                                                        if (wordness == TextWordness.Any)
                                                        {
                                                            if (word_text.Contains(positive_word))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (wordness == TextWordness.PartOfWord)
                                                        {
                                                            if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (wordness == TextWordness.WholeWord)
                                                        {
                                                            if (word_text == positive_word)
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                    }
                                                }

                                                // add unsigned matches
                                                foreach (string unsigned_word in unsigned_words)
                                                {
                                                    foreach (Word word in verse.Words)
                                                    {
                                                        // simplify all text_modes
                                                        string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                        if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                                        {
                                                            word_text = word_text.Simplify29();
                                                        }

                                                        if (wordness == TextWordness.Any)
                                                        {
                                                            if (word_text.Contains(unsigned_word))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (wordness == TextWordness.PartOfWord)
                                                        {
                                                            if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (wordness == TextWordness.WholeWord)
                                                        {
                                                            if (word_text == unsigned_word)
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else // verse failed test, so skip it
                                            {
                                                continue; // next verse
                                            }
                                        }
                                        //////////////////////////////////////////////////////////
                                        // FindByText WORDS Any
                                        //////////////////////////////////////////////////////////
                                        else if (proximity_search_type == ProximitySearchType.AnyWord)
                                        {
                                            bool found = false;
                                            foreach (string unsigned_word in unsigned_words)
                                            {
                                                foreach (Word word in verse.Words)
                                                {
                                                    // simplify all text_modes
                                                    string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                    if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                                    {
                                                        word_text = word_text.Simplify29();
                                                    }

                                                    if (wordness == TextWordness.Any)
                                                    {
                                                        if (word_text.Contains(unsigned_word))
                                                        {
                                                            found = true;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                        {
                                                            found = true;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.WholeWord)
                                                    {
                                                        if (word_text == unsigned_word)
                                                        {
                                                            found = true;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                }
                                                if (found)
                                                {
                                                    break;
                                                }
                                            }

                                            if (found) // found 1 unsigned word in verse, which is enough
                                            {
                                                ///////////////////////////////////////////////////////////////
                                                // all negative, positive and unsigned conditions have been met
                                                ///////////////////////////////////////////////////////////////

                                                // add positive matches
                                                foreach (string positive_word in positive_words)
                                                {
                                                    foreach (Word word in verse.Words)
                                                    {
                                                        // simplify all text_modes
                                                        string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                        if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                                        {
                                                            word_text = word_text.Simplify29();
                                                        }

                                                        if (wordness == TextWordness.Any)
                                                        {
                                                            if (word_text.Contains(positive_word))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (wordness == TextWordness.PartOfWord)
                                                        {
                                                            if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (wordness == TextWordness.WholeWord)
                                                        {
                                                            if (word_text == positive_word)
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                    }
                                                }

                                                // add unsigned matches
                                                foreach (string unsigned_word in unsigned_words)
                                                {
                                                    foreach (Word word in verse.Words)
                                                    {
                                                        // simplify all text_modes
                                                        string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                        if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                                        {
                                                            word_text = word_text.Simplify29();
                                                        }

                                                        if (wordness == TextWordness.Any)
                                                        {
                                                            if (word_text.Contains(unsigned_word))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (wordness == TextWordness.PartOfWord)
                                                        {
                                                            if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (wordness == TextWordness.WholeWord)
                                                        {
                                                            if (word_text == unsigned_word)
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else // verse failed test, so skip it
                                            {
                                                continue; // next verse
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }

                        // if nothing found, try emlaaei
                        if (result.Count == 0)
                        {
                            //  search in emlaaei
                            if (try_emlaaei_if_nothing_found)
                            {
                                if (s_numerology_system != null)
                                {
                                    if ((source != null) && (source.Count > 0))
                                    {
                                        foreach (Verse verse in source)
                                        {
                                            //foreach (Word word in verse.Words)
                                            //{
                                            string verse_emlaaei_text = verse.Translations[DEFAULT_EMLAAEI_TEXT].SimplifyTo(s_numerology_system.TextMode);
                                            if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editions
                                            {
                                                verse_emlaaei_text = verse_emlaaei_text.Simplify29();
                                            }

                                            if (proximity_search_type == ProximitySearchType.AllWords)
                                            {
                                                bool found = false;
                                                foreach (string negative_word in negative_words)
                                                {
                                                    if (wordness == TextWordness.Any)
                                                    {
                                                        if (verse_emlaaei_text.Contains(negative_word))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((verse_emlaaei_text.Contains(negative_word)) && (verse_emlaaei_text.Length > negative_word.Length))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.WholeWord)
                                                    {
                                                        if (verse_emlaaei_text == negative_word) //??? we need word_emlaaei_text not verse_emlaaei_text
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                }
                                                if (found) continue;

                                                foreach (string positive_word in positive_words)
                                                {
                                                    if (wordness == TextWordness.Any)
                                                    {
                                                        if (!verse_emlaaei_text.Contains(positive_word))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.PartOfWord)
                                                    {
                                                        if (!(verse_emlaaei_text.Contains(positive_word)) || !(verse_emlaaei_text.Length > positive_word.Length))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.WholeWord)
                                                    {
                                                        if (verse_emlaaei_text != positive_word) //??? we need word_emlaaei_text not verse_emlaaei_text
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                }
                                                if (found) continue;

                                                if (
                                                     (unsigned_words.Count == 0) ||
                                                     (verse_emlaaei_text.ContainsWordsOf(unsigned_words))
                                                   )
                                                {
                                                    result.Add(new Phrase(verse, 0, ""));
                                                }
                                            }
                                            else if (proximity_search_type == ProximitySearchType.AnyWord)
                                            {
                                                bool found = false;
                                                foreach (string negative_word in negative_words)
                                                {
                                                    if (wordness == TextWordness.Any)
                                                    {
                                                        if (verse_emlaaei_text.Contains(negative_word))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((verse_emlaaei_text.Contains(negative_word)) && (verse_emlaaei_text.Length > negative_word.Length))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.WholeWord)
                                                    {
                                                        if (verse_emlaaei_text == negative_word) //??? we need word_emlaaei_text not verse_emlaaei_text
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                }
                                                if (found) continue;

                                                foreach (string positive_word in positive_words)
                                                {
                                                    if (wordness == TextWordness.Any)
                                                    {
                                                        if (!verse_emlaaei_text.Contains(positive_word))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.PartOfWord)
                                                    {
                                                        if (!(verse_emlaaei_text.Contains(positive_word)) || !(verse_emlaaei_text.Length > positive_word.Length))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (wordness == TextWordness.WholeWord)
                                                    {
                                                        if (verse_emlaaei_text != positive_word) //??? we need word_emlaaei_text not verse_emlaaei_text
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                }
                                                if (found) continue;

                                                if (
                                                     (negative_words.Count > 0) ||
                                                     (positive_words.Count > 0) ||
                                                     (
                                                       (unsigned_words.Count == 0) ||
                                                       (verse_emlaaei_text.ContainsWordOf(unsigned_words))
                                                     )
                                                   )
                                                {
                                                    result.Add(new Phrase(verse, 0, ""));
                                                }
                                            }
                                        } // end for
                                        //}
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(string translation, List<Verse> source, SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, ProximitySearchType proximity_search_type, bool case_sensitive, TextWordness wordness)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

                    RegexOptions regex_options = case_sensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    if (text.IsArabic()) // Arabic letters in translation (Emlaaei, Urdu, Farsi, etc.) 
                    {
                        regex_options |= RegexOptions.RightToLeft;
                    }

                    try
                    {
                        List<string> negative_words = new List<string>();
                        List<string> positive_words = new List<string>();
                        List<string> unsigned_words = new List<string>();

                        BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);
                        string[] text_words = text.Split();
                        foreach (string text_word in text_words)
                        {
                            if (text_word.StartsWith("-"))
                            {
                                negative_words.Add(text_word.Substring(1));
                            }
                            else if (text_word.EndsWith("-"))
                            {
                                negative_words.Add(text_word.Substring(0, text_word.Length - 1));
                            }
                            else if (text_word.StartsWith("+"))
                            {
                                positive_words.Add(text_word.Substring(1));
                            }
                            else if (text_word.EndsWith("+"))
                            {
                                positive_words.Add(text_word.Substring(0, text_word.Length - 1));
                            }
                            else
                            {
                                unsigned_words.Add(text_word);
                            }
                        }

                        // do actual search
                        foreach (Verse verse in source)
                        {
                            if (proximity_search_type == ProximitySearchType.AllWords)
                            {
                                bool found = false;
                                foreach (string negative_word in negative_words)
                                {
                                    if (wordness == TextWordness.Any)
                                    {
                                        if (verse.Translations[translation].Contains(negative_word))
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                    else if (wordness == TextWordness.PartOfWord)
                                    {
                                        if ((verse.Translations[translation].Contains(negative_word)) && (verse.Translations[translation].Length > negative_word.Length))
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                    else if (wordness == TextWordness.WholeWord)
                                    {
                                        if (verse.Translations[translation] == negative_word)
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                }
                                if (found) continue;

                                foreach (string positive_word in positive_words)
                                {
                                    if (wordness == TextWordness.Any)
                                    {
                                        if (!verse.Translations[translation].Contains(positive_word))
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                    else if (wordness == TextWordness.PartOfWord)
                                    {
                                        if (!(verse.Translations[translation].Contains(positive_word)) || !(verse.Translations[translation].Length > positive_word.Length))
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                    else if (wordness != TextWordness.WholeWord)
                                    {
                                        if (verse.Translations[translation] == positive_word)
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                }
                                if (found) continue;

                                if (
                                     (unsigned_words.Count == 0) ||
                                     (verse.Translations[translation].ContainsWordsOf(unsigned_words))
                                   )
                                {
                                    result.Add(new Phrase(verse, 0, ""));
                                }
                            }
                            else if (proximity_search_type == ProximitySearchType.AnyWord)
                            {
                                bool found = false;
                                foreach (string negative_word in negative_words)
                                {
                                    if (wordness == TextWordness.Any)
                                    {
                                        if (verse.Translations[translation].Contains(negative_word))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (wordness == TextWordness.PartOfWord)
                                    {
                                        if ((verse.Translations[translation].Contains(negative_word)) && (verse.Translations[translation].Length > negative_word.Length))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (wordness == TextWordness.WholeWord)
                                    {
                                        if (verse.Translations[translation] == negative_word)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                if (found) continue;

                                foreach (string positive_word in positive_words)
                                {
                                    if (wordness == TextWordness.Any)
                                    {
                                        if (!verse.Translations[translation].Contains(positive_word))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (wordness == TextWordness.PartOfWord)
                                    {
                                        if (!(verse.Translations[translation].Contains(positive_word)) || !(verse.Translations[translation].Length > positive_word.Length))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (wordness != TextWordness.WholeWord)
                                    {
                                        if (verse.Translations[translation] == positive_word)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                if (found) continue;

                                if (
                                     (negative_words.Count > 0) ||
                                     (positive_words.Count > 0) ||
                                     (
                                       (unsigned_words.Count == 0) ||
                                       (verse.Translations[translation].ContainsWordOf(unsigned_words))
                                     )
                                   )
                                {
                                    result.Add(new Phrase(verse, 0, ""));
                                }
                            }
                        } // end for
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }
        return result;
    }
    private static void BuildWordLists(string text, out List<string> unsigned_words, out List<string> positive_words, out List<string> negative_words)
    {
        unsigned_words = new List<string>();
        positive_words = new List<string>();
        negative_words = new List<string>();

        if (String.IsNullOrEmpty(text)) return;
        text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any
        text = text.Trim();

        string[] text_words = text.Split();
        foreach (string text_word in text_words)
        {
            if (text_word.StartsWith("-"))
            {
                negative_words.Add(text_word.Substring(1));
            }
            else if (text_word.EndsWith("-"))
            {
                negative_words.Add(text_word.Substring(0, text_word.Length - 1));
            }
            else if (text_word.StartsWith("+"))
            {
                positive_words.Add(text_word.Substring(1));
            }
            else if (text_word.EndsWith("+"))
            {
                positive_words.Add(text_word.Substring(0, text_word.Length - 1));
            }
            else
            {
                unsigned_words.Add(text_word);
            }
        }
    }
    // find by text - Root
    public static List<Phrase> FindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string roots, int multiplicity, bool with_diacritics)
    {
        if (String.IsNullOrEmpty(roots)) return null;

        List<Phrase> result = new List<Phrase>();
        List<Verse> found_verses = previous_result;

        while (roots.Contains("  "))
        {
            roots = roots.Replace("  ", " ");
        }

        string[] parts = roots.Split();
        if (parts.Length > 0) // enable nested searches
        {
            List<Phrase> partial_result = null;

            List<string> negative_words = new List<string>();
            List<string> positive_words = new List<string>();
            List<string> unsigned_words = new List<string>();
            BuildWordLists(roots, out unsigned_words, out positive_words, out negative_words);

            foreach (string negative_word in negative_words)
            {
                partial_result = DoFindPhrases(search_scope, current_selection, found_verses, negative_word, 0, with_diacritics); // multiplicity = 0 for exclude
                AddPartialResult(partial_result, ref search_scope, ref found_verses, ref result);
            }

            foreach (string positive_word in positive_words)
            {
                partial_result = DoFindPhrases(search_scope, current_selection, found_verses, positive_word, multiplicity, with_diacritics);
                AddPartialResult(partial_result, ref search_scope, ref found_verses, ref result);
            }

            foreach (string unsigned_word in unsigned_words)
            {
                partial_result = DoFindPhrases(search_scope, current_selection, found_verses, unsigned_word, multiplicity, with_diacritics);
                AddPartialResult(partial_result, ref search_scope, ref found_verses, ref result);
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string root, int multiplicity, bool with_diacritics)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindPhrases(source, search_scope, current_selection, previous_result, root, multiplicity, with_diacritics);
    }
    private static List<Phrase> DoFindPhrases(List<Verse> source, SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string root, int multiplicity, bool with_diacritics)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (root.Length > 0)
                {
                    try
                    {
                        if (s_book != null)
                        {
                            Dictionary<string, List<Word>> root_words_dictionary = s_book.RootWords;
                            if (root_words_dictionary != null)
                            {
                                List<Word> root_words = null;
                                if (root_words_dictionary.ContainsKey(root))
                                {
                                    // get all pre-identified root_words
                                    root_words = root_words_dictionary[root];
                                }
                                else // if no such root, search for the matching root_word by its verse position and get its root and then get all root_words
                                {
                                    string new_root = s_book.GetBestRoot(root, with_diacritics);
                                    if (!String.IsNullOrEmpty(new_root))
                                    {
                                        // get all pre-identified root_words for new root
                                        root_words = root_words_dictionary[new_root];
                                    }
                                }

                                if (root_words != null)
                                {
                                    result = GetPhrasesWithRootWords(source, root_words, multiplicity, with_diacritics);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> GetPhrasesWithRootWords(List<Verse> source, List<Word> root_words, int multiplicity, bool with_diacritics)
    {
        //with_diacritics is not used. may not be needed.
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    Dictionary<Verse, int> multiplicity_dictionary = new Dictionary<Verse, int>();
                    foreach (Word word in root_words)
                    {
                        Verse verse = s_book.Verses[word.Verse.Number - 1];
                        if (source.Contains(verse))
                        {
                            if (multiplicity_dictionary.ContainsKey(verse))
                            {
                                multiplicity_dictionary[verse]++;
                            }
                            else // first find
                            {
                                multiplicity_dictionary.Add(verse, 1);
                            }
                        }
                    }

                    if (multiplicity == 0) // verses not containg word
                    {
                        foreach (Verse verse in source)
                        {
                            if (!multiplicity_dictionary.ContainsKey(verse))
                            {
                                Phrase phrase = new Phrase(verse, 0, "");
                                result.Add(phrase);
                            }
                        }
                    }
                    else // add only matching multiplicity or wildcard (-1)
                    {
                        foreach (Word root_word in root_words)
                        {
                            int verse_index = root_word.Verse.Number - 1;
                            if ((verse_index >= 0) && (verse_index < s_book.Verses.Count))
                            {
                                Verse verse = s_book.Verses[verse_index];
                                if ((multiplicity == -1) || (multiplicity_dictionary[verse] >= multiplicity))
                                {
                                    if (source.Contains(verse))
                                    {
                                        int word_index = root_word.NumberInVerse - 1;
                                        if ((word_index >= 0) && (word_index < verse.Words.Count))
                                        {
                                            Word verse_word = verse.Words[word_index];
                                            string word_text = verse_word.Text;
                                            int word_position = verse_word.Position;
                                            Phrase phrase = new Phrase(verse, word_position, word_text);
                                            result.Add(phrase);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static void AddPartialResult(List<Phrase> partial_result, ref SearchScope search_scope, ref List<Verse> found_verses, ref List<Phrase> previous_result)
    {
        if (partial_result != null)
        {
            List<Verse> partial_result_verses = new List<Verse>(GetVerses(partial_result));

            // if first result
            if (found_verses.Count == 0)
            {
                // fill it up with a copy of the first root search result
                previous_result = new List<Phrase>(partial_result);

                // prepare for subsequent nested searches
                search_scope = SearchScope.Result;
            }
            else // subsequent nested search
            {
                List<Phrase> intersected_phrases = new List<Phrase>(partial_result);
                if (previous_result != null)
                {
                    foreach (Phrase phrase in previous_result)
                    {
                        if (phrase != null)
                        {
                            if (partial_result_verses.Contains(phrase.Verse))
                            {
                                intersected_phrases.Add(phrase);
                            }
                        }
                    }
                }
                previous_result = intersected_phrases;
            }

            // in all cases
            // fill it up with verses of partial result
            found_verses = new List<Verse>(partial_result_verses);
        }
    }

    // find by similarity - phrases similar to given text
    public static List<Phrase> FindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, double similarity_percentage)
    {
        List<Phrase> result = new List<Phrase>();
        List<Verse> found_verses = previous_result;

        while (text.Contains("  "))
        {
            text = text.Replace("  ", " ");
        }
        while (text.Contains("+"))
        {
            text = text.Replace("+", "");
        }
        while (text.Contains("-"))
        {
            text = text.Replace("-", "");
        }

        string[] word_texts = text.Split();
        if (word_texts.Length == 0)
        {
            return result;
        }
        else if (word_texts.Length == 1)
        {
            return DoFindPhrases(search_scope, current_selection, previous_result, text, similarity_percentage);
        }
        else if (word_texts.Length > 1) // enable nested searches
        {
            if (text.Length > 1) // enable nested searches
            {
                List<Phrase> phrases = null;
                List<Verse> verses = null;

                foreach (string word_text in word_texts)
                {
                    phrases = DoFindPhrases(search_scope, current_selection, found_verses, word_text, similarity_percentage);
                    verses = new List<Verse>(GetVerses(phrases));

                    // if first result
                    if (found_verses == null)
                    {
                        // fill it up with a copy of the first similar word search result
                        result = new List<Phrase>(phrases);
                        found_verses = new List<Verse>(verses);

                        // prepare for nested search by search
                        search_scope = SearchScope.Result;
                    }
                    else // subsequent search result
                    {
                        found_verses = new List<Verse>(verses);

                        List<Phrase> union_phrases = new List<Phrase>(phrases);
                        foreach (Phrase phrase in result)
                        {
                            if (phrase != null)
                            {
                                if (verses.Contains(phrase.Verse))
                                {
                                    union_phrases.Add(phrase);
                                }
                            }
                        }
                        result = union_phrases;
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, double similarity_percentage)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindPhrases(source, search_scope, current_selection, previous_result, text, similarity_percentage);
    }
    private static List<Phrase> DoFindPhrases(List<Verse> source, SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, double similarity_percentage)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    if (!String.IsNullOrEmpty(text))
                    {
                        try
                        {
                            foreach (Verse verse in source)
                            {
                                foreach (Word word in verse.Words)
                                {
                                    if (word.Text.IsSimilarTo(text, similarity_percentage))
                                    {
                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // log exception
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by similarity - verse similar to given verse
    public static List<Verse> FindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, Verse verse, SimilarityMethod similarity_method, double similarity_percentage)
    {
        return DoFindVerses(search_scope, current_selection, previous_result, verse, similarity_method, similarity_percentage);
    }
    private static List<Verse> DoFindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, Verse verse, SimilarityMethod similarity_method, double similarity_percentage)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindVerses(source, search_scope, current_selection, previous_result, verse, similarity_method, similarity_percentage);
    }
    private static List<Verse> DoFindVerses(List<Verse> source, SearchScope search_scope, Selection current_selection, List<Verse> previous_result, Verse verse, SimilarityMethod find_similarity_method, double similarity_percentage)
    {
        List<Verse> result = new List<Verse>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (verse != null)
                {
                    switch (find_similarity_method)
                    {
                        case SimilarityMethod.SimilarText:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.IsSimilarTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarWords:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarWordsTo(source[j].Text, (int)Math.Round((Math.Min(verse.Words.Count, source[j].Words.Count) * similarity_percentage)), 1.0))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarFirstHalf:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarFirstHalfTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarLastHalf:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarLastHalfTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarFirstWord:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarFirstWordTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarLastWord:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarLastWordTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        return result;
    }
    // find by similarity - all similar verses to each other throughout the book
    public static List<List<Verse>> FindVersess(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, SimilarityMethod similarity_method, double similarity_percentage)
    {
        return DoFindVersess(search_scope, current_selection, previous_result, similarity_method, similarity_percentage);
    }
    private static List<List<Verse>> DoFindVersess(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, SimilarityMethod similarity_method, double similarity_percentage)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindVersess(source, search_scope, current_selection, previous_result, similarity_method, similarity_percentage);
    }
    private static List<List<Verse>> DoFindVersess(List<Verse> source, SearchScope search_scope, Selection current_selection, List<Verse> previous_result, SimilarityMethod find_similarity_method, double similarity_percentage)
    {
        List<List<Verse>> result = new List<List<Verse>>();
        Dictionary<Verse, List<Verse>> verse_ranges = new Dictionary<Verse, List<Verse>>(); // need dictionary to check if key exist
        bool[] already_compared = new bool[source.Count];
        if (source != null)
        {
            if (source.Count > 0)
            {
                switch (find_similarity_method)
                {
                    case SimilarityMethod.SimilarText:
                        {
                            for (int i = 0; i < source.Count - 1; i++)
                            {
                                for (int j = i + 1; j < source.Count; j++)
                                {
                                    if (!already_compared[j])
                                    {
                                        if (source[i].Text.IsSimilarTo(source[j].Text, similarity_percentage))
                                        {
                                            if (!verse_ranges.ContainsKey(source[i])) // first time matching verses found
                                            {
                                                List<Verse> similar_verses = new List<Verse>();
                                                verse_ranges.Add(source[i], similar_verses);
                                                similar_verses.Add(source[i]);
                                                similar_verses.Add(source[j]);
                                                already_compared[i] = true;
                                                already_compared[j] = true;
                                            }
                                            else // matching verses already exists
                                            {
                                                List<Verse> similar_verses = verse_ranges[source[i]];
                                                similar_verses.Add(source[j]);
                                                already_compared[j] = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case SimilarityMethod.SimilarWords:
                        {
                            for (int i = 0; i < source.Count - 1; i++)
                            {
                                for (int j = i + 1; j < source.Count; j++)
                                {
                                    if (!already_compared[j])
                                    {
                                        if (source[i].Text.HasSimilarWordsTo(source[j].Text, (int)Math.Round((Math.Min(source[i].Words.Count, source[j].Words.Count) * similarity_percentage)), 1.0))
                                        {
                                            if (!verse_ranges.ContainsKey(source[i])) // first time matching verses found
                                            {
                                                List<Verse> similar_verses = new List<Verse>();
                                                verse_ranges.Add(source[i], similar_verses);
                                                similar_verses.Add(source[i]);
                                                similar_verses.Add(source[j]);
                                                already_compared[i] = true;
                                                already_compared[j] = true;
                                            }
                                            else // matching verses already exists
                                            {
                                                List<Verse> similar_verses = verse_ranges[source[i]];
                                                similar_verses.Add(source[j]);
                                                already_compared[j] = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case SimilarityMethod.SimilarFirstWord:
                        {
                            for (int i = 0; i < source.Count - 1; i++)
                            {
                                for (int j = i + 1; j < source.Count; j++)
                                {
                                    if (!already_compared[j])
                                    {
                                        if (source[j].Text.HasSimilarFirstWordTo(source[j].Text, similarity_percentage))
                                        {
                                            if (!verse_ranges.ContainsKey(source[i])) // first time matching verses found
                                            {
                                                List<Verse> similar_verses = new List<Verse>();
                                                verse_ranges.Add(source[i], similar_verses);
                                                similar_verses.Add(source[i]);
                                                similar_verses.Add(source[j]);
                                                already_compared[i] = true;
                                                already_compared[j] = true;
                                            }
                                            else // matching verses already exists
                                            {
                                                List<Verse> similar_verses = verse_ranges[source[i]];
                                                similar_verses.Add(source[j]);
                                                already_compared[j] = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case SimilarityMethod.SimilarLastWord:
                        {
                            for (int i = 0; i < source.Count - 1; i++)
                            {
                                for (int j = i + 1; j < source.Count; j++)
                                {
                                    if (!already_compared[j])
                                    {
                                        if (source[i].Text.HasSimilarLastWordTo(source[j].Text, similarity_percentage))
                                        {
                                            if (!verse_ranges.ContainsKey(source[i])) // first time matching verses found
                                            {
                                                List<Verse> similar_verses = new List<Verse>();
                                                verse_ranges.Add(source[i], similar_verses);
                                                similar_verses.Add(source[i]);
                                                similar_verses.Add(source[j]);
                                                already_compared[i] = true;
                                                already_compared[j] = true;
                                            }
                                            else // matching verses already exists
                                            {
                                                List<Verse> similar_verses = verse_ranges[source[i]];
                                                similar_verses.Add(source[j]);
                                                already_compared[j] = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        // copy dictionary to list of list
        if (verse_ranges.Count > 0)
        {
            foreach (List<Verse> verse_range in verse_ranges.Values)
            {
                result.Add(verse_range);
            }
        }
        return result;
    }

    // find by numbers - helper methods
    private static bool Compare(Word word, NumberQuery query)
    {
        if (word != null)
        {
            long value = 0L;

            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Any))
            {
                if (query.Number > 0)
                {
                    if (!Numbers.Compare(word.NumberInChapter, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(word.NumberInChapter, query.NumberNumberType))
                {
                    return false;
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Any))
            {
                if (query.LetterCount > 0)
                {
                    if (!Numbers.Compare(word.Letters.Count, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(word.Letters.Count, query.LetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Any))
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(word.UniqueLetters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(word.UniqueLetters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Any))
            {
                if (query.Value > 0)
                {
                    if (value == 0L) { value = CalculateValue(word); }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(word); }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitSumNumberType == NumberType.None) || (query.ValueDigitSumNumberType == NumberType.Any))
            {
                if (query.ValueDigitSum > 0)
                {
                    if (value == 0L) { value = CalculateValue(word); }
                    if (!Numbers.Compare(Numbers.DigitSum(value), query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(word); }
                if (!Numbers.IsNumberType(Numbers.DigitSum(value), query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitalRootNumberType == NumberType.None) | (query.ValueDigitalRootNumberType == NumberType.Any))
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (value == 0L) { value = CalculateValue(word); }
                    if (!Numbers.Compare(Numbers.DigitalRoot(value), query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(word); }
                if (!Numbers.IsNumberType(Numbers.DigitalRoot(value), query.ValueDigitalRootNumberType))
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(List<Word> words, NumberQuery query)
    {
        if (words != null)
        {
            int sum = 0;
            long value = 0L;

            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Any))
            {
                if (query.Number > 0)
                {
                    sum = 0;
                    foreach (Word word in words)
                    {
                        sum += word.NumberInChapter;
                    }
                    if (!Numbers.Compare(sum, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                sum = 0;
                foreach (Word word in words)
                {
                    sum += word.NumberInChapter;
                }
                if (!Numbers.IsNumberType(sum, query.NumberNumberType))
                {
                    return false;
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Any))
            {
                if (query.LetterCount > 0)
                {
                    sum = 0;
                    foreach (Word word in words)
                    {
                        sum += word.Letters.Count;
                    }
                    if (!Numbers.Compare(sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                sum = 0;
                foreach (Word word in words)
                {
                    sum += word.Letters.Count;
                }
                if (!Numbers.IsNumberType(sum, query.LetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Any))
            {
                if (query.UniqueLetterCount > 0)
                {
                    List<char> unique_letters = new List<char>();
                    foreach (Word word in words)
                    {
                        foreach (char character in word.UniqueLetters)
                        {
                            if (!unique_letters.Contains(character))
                            {
                                unique_letters.Add(character);
                            }
                        }
                    }
                    if (!Numbers.Compare(unique_letters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                List<char> unique_letters = new List<char>();
                foreach (Word word in words)
                {
                    foreach (char character in word.UniqueLetters)
                    {
                        if (!unique_letters.Contains(character))
                        {
                            unique_letters.Add(character);
                        }
                    }
                }
                if (!Numbers.IsNumberType(unique_letters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Any))
            {
                if (query.Value > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Word word in words)
                        {
                            value += CalculateValue(word);
                        }
                    }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Word word in words)
                    {
                        value += CalculateValue(word);
                    }
                }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitSumNumberType == NumberType.None) || (query.ValueDigitSumNumberType == NumberType.Any))
            {
                if (query.ValueDigitSum > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Word word in words)
                        {
                            value += CalculateValue(word);
                        }
                    }
                    if (!Numbers.Compare(Numbers.DigitSum(value), query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Word word in words)
                    {
                        value += CalculateValue(word);
                    }
                }
                if (!Numbers.IsNumberType(Numbers.DigitSum(value), query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitalRootNumberType == NumberType.None) || (query.ValueDigitalRootNumberType == NumberType.Any))
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Word word in words)
                        {
                            value += CalculateValue(word);
                        }
                    }
                    if (!Numbers.Compare(Numbers.DigitalRoot(value), query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Word word in words)
                    {
                        value += CalculateValue(word);
                    }
                }
                if (!Numbers.IsNumberType(Numbers.DigitalRoot(value), query.ValueDigitalRootNumberType))
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(Sentence sentence, NumberQuery query)
    {
        if (sentence != null)
        {
            long value = 0L;

            if ((query.WordCountNumberType == NumberType.None) || (query.WordCountNumberType == NumberType.Any))
            {
                if (query.WordCount > 0)
                {
                    if (!Numbers.Compare(sentence.WordCount, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(sentence.WordCount, query.WordCountNumberType))
                {
                    return false;
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Any))
            {
                if (query.LetterCount > 0)
                {
                    if (!Numbers.Compare(sentence.LetterCount, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(sentence.LetterCount, query.LetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Any))
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(sentence.UniqueLetterCount, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(sentence.UniqueLetterCount, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Any))
            {
                if (query.Value > 0)
                {
                    if (value == 0L) { value = CalculateValue(sentence); }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(sentence); }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitSumNumberType == NumberType.None) || (query.ValueDigitSumNumberType == NumberType.Any))
            {
                if (query.ValueDigitSum > 0)
                {
                    if (value == 0L) { value = CalculateValue(sentence); }
                    if (!Numbers.Compare(Numbers.DigitSum(value), query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(sentence); }
                if (!Numbers.IsNumberType(Numbers.DigitSum(value), query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitalRootNumberType == NumberType.None) || (query.ValueDigitalRootNumberType == NumberType.Any))
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (value == 0L) { value = CalculateValue(sentence); }
                    if (!Numbers.Compare(Numbers.DigitalRoot(value), query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(sentence); }
                if (!Numbers.IsNumberType(Numbers.DigitalRoot(value), query.ValueDigitalRootNumberType))
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(Verse verse, NumberQuery query)
    {
        if (verse != null)
        {
            long value = 0L;

            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Any))
            {
                if (query.Number > 0)
                {
                    if (!Numbers.Compare(verse.NumberInChapter, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(verse.NumberInChapter, query.NumberNumberType))
                {
                    return false;
                }
            }

            if ((query.WordCountNumberType == NumberType.None) || (query.WordCountNumberType == NumberType.Any))
            {
                if (query.WordCount > 0)
                {
                    if (!Numbers.Compare(verse.Words.Count, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(verse.Words.Count, query.WordCountNumberType))
                {
                    return false;
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Any))
            {
                if (query.LetterCount > 0)
                {
                    if (!Numbers.Compare(verse.LetterCount, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(verse.LetterCount, query.LetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Any))
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(verse.UniqueLetters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(verse.UniqueLetters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Any))
            {
                if (query.Value > 0)
                {
                    if (value == 0L) { value = CalculateValue(verse); }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(verse); }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitSumNumberType == NumberType.None) || (query.ValueDigitSumNumberType == NumberType.Any))
            {
                if (query.ValueDigitSum > 0)
                {
                    if (value == 0L) { value = CalculateValue(verse); }
                    if (!Numbers.Compare(Numbers.DigitSum(value), query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(verse); }
                if (!Numbers.IsNumberType(Numbers.DigitSum(value), query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitalRootNumberType == NumberType.None) || (query.ValueDigitalRootNumberType == NumberType.Any))
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (value == 0L) { value = CalculateValue(verse); }
                    if (!Numbers.Compare(Numbers.DigitalRoot(value), query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(verse); }
                if (!Numbers.IsNumberType(Numbers.DigitalRoot(value), query.ValueDigitalRootNumberType))
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(List<Verse> verses, NumberQuery query)
    {
        if (verses != null)
        {
            int sum = 0;
            long value = 0L;

            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Any))
            {
                if (query.Number > 0)
                {
                    sum = 0;
                    foreach (Verse verse in verses)
                    {
                        sum += verse.NumberInChapter;
                    }
                    if (!Numbers.Compare(sum, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                sum = 0;
                foreach (Verse verse in verses)
                {
                    sum += verse.NumberInChapter;
                }
                if (!Numbers.IsNumberType(sum, query.NumberNumberType))
                {
                    return false;
                }
            }

            if ((query.WordCountNumberType == NumberType.None) || (query.WordCountNumberType == NumberType.Any))
            {
                if (query.WordCount > 0)
                {
                    sum = 0;
                    foreach (Verse verse in verses)
                    {
                        sum += verse.Words.Count;
                    }
                    if (!Numbers.Compare(sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                sum = 0;
                foreach (Verse verse in verses)
                {
                    sum += verse.Words.Count;
                }
                if (!Numbers.IsNumberType(sum, query.WordCountNumberType))
                {
                    return false;
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Any))
            {
                if (query.LetterCount > 0)
                {
                    sum = 0;
                    foreach (Verse verse in verses)
                    {
                        sum += verse.LetterCount;
                    }
                    if (!Numbers.Compare(sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                sum = 0;
                foreach (Verse verse in verses)
                {
                    sum += verse.LetterCount;
                }
                if (!Numbers.IsNumberType(sum, query.LetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Any))
            {
                if (query.UniqueLetterCount > 0)
                {
                    List<char> unique_letters = new List<char>();
                    foreach (Verse verse in verses)
                    {
                        foreach (char character in verse.UniqueLetters)
                        {
                            if (!unique_letters.Contains(character))
                            {
                                unique_letters.Add(character);
                            }
                        }
                    }
                    if (!Numbers.Compare(unique_letters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                List<char> unique_letters = new List<char>();
                foreach (Verse verse in verses)
                {
                    foreach (char character in verse.UniqueLetters)
                    {
                        if (!unique_letters.Contains(character))
                        {
                            unique_letters.Add(character);
                        }
                    }
                }
                if (!Numbers.IsNumberType(unique_letters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Any))
            {
                if (query.Value > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Verse verse in verses)
                        {
                            value += CalculateValue(verse);
                        }
                    }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Verse verse in verses)
                    {
                        value += CalculateValue(verse);
                    }
                }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitSumNumberType == NumberType.None) || (query.ValueDigitSumNumberType == NumberType.Any))
            {
                if (query.ValueDigitSum > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Verse verse in verses)
                        {
                            value += CalculateValue(verse);
                        }
                    }
                    if (!Numbers.Compare(Numbers.DigitSum(value), query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Verse verse in verses)
                    {
                        value += CalculateValue(verse);
                    }
                }
                if (!Numbers.IsNumberType(Numbers.DigitSum(value), query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitalRootNumberType == NumberType.None) || (query.ValueDigitalRootNumberType == NumberType.Any))
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Verse verse in verses)
                        {
                            value += CalculateValue(verse);
                        }
                    }
                    if (!Numbers.Compare(Numbers.DigitalRoot(value), query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Verse verse in verses)
                    {
                        value += CalculateValue(verse);
                    }
                }
                if (!Numbers.IsNumberType(Numbers.DigitalRoot(value), query.ValueDigitalRootNumberType))
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(Chapter chapter, NumberQuery query)
    {
        if (chapter != null)
        {
            long value = 0L;

            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Any))
            {
                if (query.Number > 0)
                {
                    if (!Numbers.Compare(chapter.Number, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(chapter.Number, query.NumberNumberType))
                {
                    return false;
                }
            }

            if ((query.VerseCountNumberType == NumberType.None) || (query.VerseCountNumberType == NumberType.Any))
            {
                if (query.VerseCount > 0)
                {
                    if (!Numbers.Compare(chapter.Verses.Count, query.VerseCount, query.VerseCountComparisonOperator, query.VerseCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(chapter.Verses.Count, query.VerseCountNumberType))
                {
                    return false;
                }
            }

            if ((query.WordCountNumberType == NumberType.None) || (query.WordCountNumberType == NumberType.Any))
            {
                if (query.WordCount > 0)
                {
                    if (!Numbers.Compare(chapter.WordCount, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(chapter.WordCount, query.WordCountNumberType))
                {
                    return false;
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Any))
            {
                if (query.LetterCount > 0)
                {
                    if (!Numbers.Compare(chapter.LetterCount, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(chapter.LetterCount, query.LetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Any))
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(chapter.UniqueLetters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(chapter.UniqueLetters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Any))
            {
                if (query.Value > 0)
                {
                    if (value == 0L) { value = CalculateValue(chapter); }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(chapter); }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitSumNumberType == NumberType.None) || (query.ValueDigitSumNumberType == NumberType.Any))
            {
                if (query.ValueDigitSum > 0)
                {
                    if (value == 0L) { value = CalculateValue(chapter); }
                    if (!Numbers.Compare(Numbers.DigitSum(value), query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(chapter); }
                if (!Numbers.IsNumberType(Numbers.DigitSum(value), query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitalRootNumberType == NumberType.None) || (query.ValueDigitalRootNumberType == NumberType.Any))
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (value == 0L) { value = CalculateValue(chapter); }
                    if (!Numbers.Compare(Numbers.DigitalRoot(value), query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(chapter); }
                if (!Numbers.IsNumberType(Numbers.DigitalRoot(value), query.ValueDigitalRootNumberType))
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(List<Chapter> chapters, NumberQuery query)
    {
        if (chapters != null)
        {
            int sum = 0;
            long value = 0L;

            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Any))
            {
                if (query.Number > 0)
                {
                    sum = 0;
                    foreach (Chapter chapter in chapters)
                    {
                        sum += chapter.Number;
                    }
                    if (!Numbers.Compare(sum, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                sum = 0;
                foreach (Chapter chapter in chapters)
                {
                    sum += chapter.Number;
                }
                if (!Numbers.IsNumberType(sum, query.NumberNumberType))
                {
                    return false;
                }
            }

            if ((query.VerseCountNumberType == NumberType.None) || (query.VerseCountNumberType == NumberType.Any))
            {
                if (query.VerseCount > 0)
                {
                    sum = 0;
                    foreach (Chapter chapter in chapters)
                    {
                        sum += chapter.Verses.Count;
                    }
                    if (!Numbers.Compare(sum, query.VerseCount, query.VerseCountComparisonOperator, query.VerseCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                sum = 0;
                foreach (Chapter chapter in chapters)
                {
                    sum += chapter.Verses.Count;
                }
                if (!Numbers.IsNumberType(sum, query.VerseCountNumberType))
                {
                    return false;
                }
            }

            if ((query.WordCountNumberType == NumberType.None) || (query.WordCountNumberType == NumberType.Any))
            {
                if (query.WordCount > 0)
                {
                    sum = 0;
                    foreach (Chapter chapter in chapters)
                    {
                        sum += chapter.WordCount;
                    }
                    if (!Numbers.Compare(sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                sum = 0;
                foreach (Chapter chapter in chapters)
                {
                    sum += chapter.WordCount;
                }
                if (!Numbers.IsNumberType(sum, query.WordCountNumberType))
                {
                    return false;
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Any))
            {
                if (query.LetterCount > 0)
                {
                    sum = 0;
                    foreach (Chapter chapter in chapters)
                    {
                        sum += chapter.LetterCount;
                    }
                    if (!Numbers.Compare(sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                sum = 0;
                foreach (Chapter chapter in chapters)
                {
                    sum += chapter.LetterCount;
                }
                if (!Numbers.IsNumberType(sum, query.LetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Any))
            {
                if (query.UniqueLetterCount > 0)
                {
                    List<char> unique_letters = new List<char>();
                    foreach (Chapter chapter in chapters)
                    {
                        foreach (char character in chapter.UniqueLetters)
                        {
                            if (!unique_letters.Contains(character))
                            {
                                unique_letters.Add(character);
                            }
                        }
                    }
                    if (!Numbers.Compare(unique_letters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                List<char> unique_letters = new List<char>();
                foreach (Chapter chapter in chapters)
                {
                    foreach (char character in chapter.UniqueLetters)
                    {
                        if (!unique_letters.Contains(character))
                        {
                            unique_letters.Add(character);
                        }
                    }
                }
                if (!Numbers.IsNumberType(unique_letters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Any))
            {
                if (query.Value > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Chapter chapter in chapters)
                        {
                            value += CalculateValue(chapter);
                        }
                    }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Chapter chapter in chapters)
                    {
                        value += CalculateValue(chapter);
                    }
                }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitSumNumberType == NumberType.None) || (query.ValueDigitSumNumberType == NumberType.Any))
            {
                if (query.ValueDigitSum > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Chapter chapter in chapters)
                        {
                            value += CalculateValue(chapter);
                        }
                    }
                    if (!Numbers.Compare(Numbers.DigitSum(value), query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Chapter chapter in chapters)
                    {
                        value += CalculateValue(chapter);
                    }
                }
                if (!Numbers.IsNumberType(Numbers.DigitSum(value), query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueDigitalRootNumberType == NumberType.None) || (query.ValueDigitalRootNumberType == NumberType.Any))
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Chapter chapter in chapters)
                        {
                            value += CalculateValue(chapter);
                        }
                    }
                    if (!Numbers.Compare(Numbers.DigitalRoot(value), query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Chapter chapter in chapters)
                    {
                        value += CalculateValue(chapter);
                    }
                }
                if (!Numbers.IsNumberType(Numbers.DigitalRoot(value), query.ValueDigitalRootNumberType))
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    // find by numbers - Words
    public static List<Word> FindWords(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindWords(search_scope, current_selection, previous_result, query);
    }
    private static List<Word> DoFindWords(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindWords(source, query);
    }
    private static List<Word> DoFindWords(List<Verse> source, NumberQuery query)
    {
        List<Word> result = new List<Word>();
        if (source != null)
        {
            if (query.WordCount <= 1) // ensure no range search
            {
                foreach (Verse verse in source)
                {
                    foreach (Word word in verse.Words)
                    {
                        if (Compare(word, query))
                        {
                            result.Add(word);
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - WordRanges
    public static List<List<Word>> FindWordRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindWordRanges(search_scope, current_selection, previous_result, query);
    }
    private static List<List<Word>> DoFindWordRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindWordRanges(source, query);
    }
    private static List<List<Word>> DoFindWordRanges(List<Verse> source, NumberQuery query)
    {
        List<List<Word>> result = new List<List<Word>>();
        if (source != null)
        {
            int range_length = query.WordCount;
            if (range_length == 1) return null;

            List<Word> words = new List<Word>();
            foreach (Verse verse in source)
            {
                words.AddRange(verse.Words);
            }

            if (range_length == 0) // non-specified range length
            {
                // limit range length to minimum
                int word_count = 0;
                foreach (Verse verse in source)
                {
                    word_count += verse.Words.Count;
                }
                int limit = word_count - 1;
                if ((query.LetterCount > 0) && (query.LetterCount < word_count))
                {
                    limit = query.LetterCount;
                }
                else if ((query.Value > 0L) && (query.Value < word_count))
                {
                    int min_letters_per_word = 1; // Waw in WawAsWord
                    limit = (int)(query.Value / min_letters_per_word);
                }

                for (int r = 1; r <= limit; r++) // try all possible range lengths
                {
                    for (int i = 0; i < words.Count - r + 1; i++)
                    {
                        // build required range
                        List<Word> range = new List<Word>();
                        for (int j = i; j < i + r; j++)
                        {
                            range.Add(words[j]);
                        }

                        // check range
                        if (Compare(range, query))
                        {
                            result.Add(range);
                        }
                    }
                }
            }
            else // specified range length
            {
                int r = range_length;
                for (int i = 0; i < words.Count - r + 1; i++)
                {
                    // build required range
                    List<Word> range = new List<Word>();
                    for (int j = i; j < i + r; j++)
                    {
                        range.Add(words[j]);
                    }

                    // check range
                    if (Compare(range, query))
                    {
                        result.Add(range);
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - WordSets
    public static List<List<Word>> FindWordSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindWordSets(search_scope, current_selection, previous_result, query);
    }
    private static List<List<Word>> DoFindWordSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindWordSets(source, query);
    }
    private static List<List<Word>> DoFindWordSets(List<Verse> source, NumberQuery query)
    {
        List<List<Word>> result = new List<List<Word>>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    List<Word> words = new List<Word>();
                    if (words != null)
                    {
                        int set_size = query.WordCount;
                        if (set_size == 1) return null;

                        foreach (Verse verse in source)
                        {
                            words.AddRange(verse.Words);
                        }

                        if (set_size == 0) // non-specified set size
                        {
                            // limit range length to minimum
                            int word_count = 0;
                            foreach (Verse verse in source)
                            {
                                word_count += verse.Words.Count;
                            }
                            int limit = word_count - 1;
                            if ((query.LetterCount > 0) && (query.LetterCount < word_count))
                            {
                                limit = query.LetterCount;
                            }
                            else if ((query.Value > 0L) && (query.Value < word_count))
                            {
                                int min_letters_per_word = 1; // Waw in WawAsWord
                                limit = (int)(query.Value / min_letters_per_word);
                            }

                            for (int i = 0; i < limit; i++) // try all possible set sizes
                            {
                                int size = i + 1;
                                Combinations<Word> sets = new Combinations<Word>(words, size, GenerateOption.WithoutRepetition);
                                foreach (List<Word> set in sets)
                                {
                                    // check set against query
                                    if (Compare(set, query))
                                    {
                                        result.Add(set);
                                    }
                                }
                            }
                        }
                        else // specified set size
                        {
                            Combinations<Word> sets = new Combinations<Word>(words, set_size, GenerateOption.WithoutRepetition);
                            foreach (List<Word> set in sets)
                            {
                                // check set against query
                                if (Compare(set, query))
                                {
                                    result.Add(set);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - Sentences
    public static List<Sentence> FindSentences(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindSentences(search_scope, current_selection, previous_result, query);
    }
    private static List<Sentence> DoFindSentences(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindSentences(source, query);
    }
    private static List<Sentence> DoFindSentences(List<Verse> source, NumberQuery query)
    {
        List<Sentence> result = new List<Sentence>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                List<Word> words = new List<Word>();
                foreach (Verse verse in source)
                {
                    words.AddRange(verse.Words);
                }

                // scan linearly for sequence of words with total Text matching query
                bool done_MustContinue = false;
                for (int i = 0; i < words.Count - 1; i++)
                {
                    StringBuilder str = new StringBuilder();

                    // start building word sequence
                    str.Append(words[i].Text);

                    string stopmark_text = StopmarkHelper.GetStopmarkText(words[i].Stopmark);

                    // 1-word sentence
                    if (
                         (words[i].Stopmark != Stopmark.None) &&
                         (words[i].Stopmark != Stopmark.CanStopAtEither) &&
                         (words[i].Stopmark != Stopmark.MustPause) //&&
                        //(words[i].Stopmark != Stopmark.MustContinue)
                       )
                    {
                        Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[i].Verse, words[i].Position + words[i].Text.Length, str.ToString());
                        if (sentence != null)
                        {
                            if (Compare(sentence, query))
                            {
                                result.Add(sentence);
                            }
                        }
                    }
                    else // multi-word sentence
                    {
                        // mark the start of 1-to-m MustContinue stopmarks
                        int backup_i = i;

                        // continue building with next words until a stopmark
                        bool done_CanStopAtEither = false;
                        for (int j = i + 1; j < words.Count; j++)
                        {
                            str.Append(" " + words[j].Text);

                            if (words[j].Stopmark == Stopmark.None)
                            {
                                continue; // continue building longer senetence
                            }
                            else // there is a real stopmark
                            {
                                if (s_numerology_system != null)
                                {
                                    if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                    {
                                        if (s_numerology_system.TextMode == "Original")
                                        {
                                            str.Append(" " + stopmark_text);
                                        }
                                    }
                                }

                                if (words[j].Stopmark == Stopmark.MustContinue)
                                {
                                    // TEST Stopmark.MustContinue
                                    //----1 2 3 4 sentences
                                    //1268
                                    //4153
                                    //1799
                                    //2973
                                    //----1 12 123 1234 sentences
                                    //1268
                                    //5421
                                    //7220
                                    //10193
                                    //-------------
                                    //ERRORS
                                    //# duplicate 1
                                    //# short str
                                    //  in 123 1234
                                    //-------------
                                    //// not needed yet
                                    //// multi-mid sentences
                                    //5952
                                    //4772

                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        if (Compare(sentence, query))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    if (done_MustContinue)
                                    {
                                        done_MustContinue = false;
                                        continue; // get all overlapping long sentence
                                    }

                                    StringBuilder k_str = new StringBuilder();
                                    for (int k = j + 1; k < words.Count; k++)
                                    {
                                        k_str.Append(words[k].Text + " ");

                                        if (words[k].Stopmark == Stopmark.None)
                                        {
                                            continue; // next k
                                        }
                                        else // there is a stopmark
                                        {
                                            if (s_numerology_system != null)
                                            {
                                                if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                                {
                                                    if (s_numerology_system.TextMode == "Original")
                                                    {
                                                        stopmark_text = StopmarkHelper.GetStopmarkText(words[k].Stopmark);
                                                        k_str.Append(stopmark_text + " ");
                                                    }
                                                }
                                            }
                                            if (k_str.Length > 0)
                                            {
                                                k_str.Remove(k_str.Length - 1, 1);
                                            }

                                            sentence = new Sentence(words[j + 1].Verse, words[j + 1].Position, words[k].Verse, words[k].Position + words[k].Text.Length, k_str.ToString());
                                            if (sentence != null)
                                            {
                                                if (Compare(sentence, query))
                                                {
                                                    result.Add(sentence);
                                                }
                                            }

                                            if (
                                                 (words[k].Stopmark == Stopmark.ShouldContinue) ||
                                                 (words[k].Stopmark == Stopmark.CanStop) ||
                                                 (words[k].Stopmark == Stopmark.ShouldStop)
                                               )
                                            {
                                                done_MustContinue = true;   // restart from beginning skipping any MustContinue
                                            }
                                            else
                                            {
                                                done_MustContinue = false;   // keep building ever-longer multi-MustContinue sentence
                                            }

                                            j = k;
                                            break; // next j
                                        }
                                    }

                                    if (done_MustContinue)
                                    {
                                        i = backup_i - 1;  // start new sentence from beginning
                                        break; // next i
                                    }
                                    else
                                    {
                                        continue; // next j
                                    }
                                }
                                else if (
                                     (words[j].Stopmark == Stopmark.ShouldContinue) ||
                                     (words[j].Stopmark == Stopmark.CanStop) ||
                                     (words[j].Stopmark == Stopmark.ShouldStop)
                                   )
                                {
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        if (Compare(sentence, query))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    i = j; // start new sentence after j
                                    break; // next i
                                }
                                else if (words[j].Stopmark == Stopmark.MustPause)
                                {
                                    if (
                                         (words[j].Text.Simplify29() == "مَنْ".Simplify29()) ||
                                         (words[j].Text.Simplify29() == "بَلْ".Simplify29())
                                       )
                                    {
                                        continue; // continue building longer senetence
                                    }
                                    else if (
                                              (words[j].Text.Simplify29() == "عِوَجَا".Simplify29()) ||
                                              (words[j].Text.Simplify29() == "مَّرْقَدِنَا".Simplify29()) ||
                                              (words[j].Text.Simplify29() == "مَالِيَهْ".Simplify29())
                                            )
                                    {
                                        Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                        if (sentence != null)
                                        {
                                            if (Compare(sentence, query))
                                            {
                                                result.Add(sentence);
                                            }
                                        }

                                        i = j; // start new sentence after j
                                        break; // next i
                                    }
                                    else // unknown case
                                    {
                                        throw new Exception("Unknown paused Quran word.");
                                    }
                                }
                                // first CanStopAtEither found at j
                                else if ((!done_CanStopAtEither) && (words[j].Stopmark == Stopmark.CanStopAtEither))
                                {
                                    // ^ ذَٰلِكَ ٱلْكِتَٰبُ لَا رَيْبَ
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        if (Compare(sentence, query))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    int kk = -1; // start after ^ (e.g. هُدًۭى)
                                    StringBuilder kk_str = new StringBuilder();
                                    StringBuilder kkk_str = new StringBuilder();
                                    for (int k = j + 1; k < words.Count; k++)
                                    {
                                        str.Append(" " + words[k].Text);
                                        if (kkk_str.Length > 0) // skip first k loop
                                        {
                                            kk_str.Append(" " + words[k].Text);
                                        }
                                        kkk_str.Append(" " + words[k].Text);

                                        if (words[k].Stopmark == Stopmark.None)
                                        {
                                            continue; // next k
                                        }
                                        else // there is a stopmark
                                        {
                                            if (s_numerology_system != null)
                                            {
                                                if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                                {
                                                    if (s_numerology_system.TextMode == "Original")
                                                    {
                                                        str.Append(" " + stopmark_text);
                                                        if (kk_str.Length > 0)
                                                        {
                                                            kk_str.Append(" " + stopmark_text);
                                                        }
                                                        kkk_str.Append(" " + stopmark_text);
                                                    }
                                                }
                                            }

                                            // second CanStopAtEither found at k
                                            if (words[k].Stopmark == Stopmark.CanStopAtEither)
                                            {
                                                // ^ ذَٰلِكَ ٱلْكِتَٰبُ لَا رَيْبَ ۛ^ فِيهِ
                                                sentence = new Sentence(words[i].Verse, words[i].Position, words[k].Verse, words[k].Position + words[k].Text.Length, str.ToString());
                                                if (sentence != null)
                                                {
                                                    if (Compare(sentence, query))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                kk = k + 1; // backup k after second ^
                                                continue; // next k
                                            }
                                            else // non-CanStopAtEither stopmark
                                            {
                                                // kkk_str   فِيهِ ۛ^ هُدًۭى لِّلْمُتَّقِينَ
                                                sentence = new Sentence(words[j + 1].Verse, words[j + 1].Position, words[k].Verse, words[k].Position + words[k].Text.Length, kkk_str.ToString());
                                                if (sentence != null)
                                                {
                                                    if (Compare(sentence, query))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                // kk_str   هُدًۭى لِّلْمُتَّقِينَ
                                                sentence = new Sentence(words[kk].Verse, words[kk].Position, words[k].Verse, words[k].Position + words[k].Text.Length, kk_str.ToString());
                                                if (sentence != null)
                                                {
                                                    if (Compare(sentence, query))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                // skip the whole surrounding non-CanStopAtEither sentence
                                                j = k;
                                                break; // next j
                                            }
                                        }
                                    }

                                    // restart from last
                                    str.Length = 0;
                                    j = i - 1; // will be j++ by reloop
                                    done_CanStopAtEither = true;
                                }
                                else if (words[j].Stopmark == Stopmark.MustStop)
                                {
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        if (Compare(sentence, query))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    i = j; // start new sentence after j
                                    break; // next i
                                }
                                else // unknown case
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - Verses
    public static List<Verse> FindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindVerses(search_scope, current_selection, previous_result, query);
    }
    private static List<Verse> DoFindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindVerses(source, query);
    }
    private static List<Verse> DoFindVerses(List<Verse> source, NumberQuery query)
    {
        List<Verse> result = new List<Verse>();
        if (source != null)
        {
            if (query.VerseCount <= 1) // ensure no range search
            {
                foreach (Verse verse in source)
                {
                    if (Compare(verse, query))
                    {
                        result.Add(verse);
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - VerseRanges
    public static List<List<Verse>> FindVerseRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindVerseRanges(search_scope, current_selection, previous_result, query);
    }
    private static List<List<Verse>> DoFindVerseRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindVerseRanges(source, query);
    }
    private static List<List<Verse>> DoFindVerseRanges(List<Verse> source, NumberQuery query)
    {
        List<List<Verse>> result = new List<List<Verse>>();
        if (source != null)
        {
            int range_length = query.VerseCount;
            if (range_length == 1) return null;

            if (range_length == 0) // non-specified range length
            {
                // limit range length to minimum
                int limit = source.Count - 1;
                if ((query.WordCount > 0) && (query.WordCount < source.Count))
                {
                    limit = query.WordCount;
                }
                else if ((query.LetterCount > 0) && (query.LetterCount < source.Count))
                {
                    limit = query.LetterCount;
                }
                else if ((query.Value > 0L) && (query.Value < source.Count))
                {
                    int min_letters_per_verse = 2; // HM in if without BismAllah
                    limit = (int)(query.Value / min_letters_per_verse);
                }

                for (int r = 1; r <= limit; r++) // try all possible range lengths
                {
                    for (int i = 0; i < source.Count - r + 1; i++)
                    {
                        // build required range
                        List<Verse> range = new List<Verse>();
                        for (int j = i; j < i + r; j++)
                        {
                            range.Add(source[j]);
                        }

                        // check range
                        if (Compare(range, query))
                        {
                            result.Add(range);
                        }
                    }
                }
            }
            else // specified range length
            {
                int r = range_length;
                for (int i = 0; i < source.Count - r + 1; i++)
                {
                    // build required range
                    List<Verse> range = new List<Verse>();
                    for (int j = i; j < i + r; j++)
                    {
                        range.Add(source[j]);
                    }

                    // check range
                    if (Compare(range, query))
                    {
                        result.Add(range);
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - VerseSets
    public static List<List<Verse>> FindVerseSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindVerseSets(search_scope, current_selection, previous_result, query);
    }
    private static List<List<Verse>> DoFindVerseSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindVerseSets(source, query);
    }
    private static List<List<Verse>> DoFindVerseSets(List<Verse> source, NumberQuery query)
    {
        List<List<Verse>> result = new List<List<Verse>>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    List<Verse> verses = source;
                    if (verses != null)
                    {
                        int set_size = query.VerseCount;
                        if (set_size == 1) return null;

                        if (set_size == 0) // non-specified set size
                        {
                            // limit set size to minimum
                            int limit = source.Count - 1;
                            if ((query.WordCount > 0) && (query.WordCount < source.Count))
                            {
                                limit = query.WordCount;
                            }
                            else if ((query.LetterCount > 0) && (query.LetterCount < source.Count))
                            {
                                limit = query.LetterCount;
                            }
                            else if ((query.Value > 0L) && (query.Value < source.Count))
                            {
                                int min_letters_per_verse = 2; // HM in if without BismAllah
                                limit = (int)(query.Value / min_letters_per_verse);
                            }

                            for (int i = 0; i < limit; i++) // try all possible set sizes
                            {
                                int size = i + 1;
                                Combinations<Verse> sets = new Combinations<Verse>(verses, size, GenerateOption.WithoutRepetition);
                                foreach (List<Verse> set in sets)
                                {
                                    // check set against query
                                    if (Compare(set, query))
                                    {
                                        result.Add(set);
                                    }
                                }
                            }
                        }
                        else // specified set size
                        {
                            Combinations<Verse> sets = new Combinations<Verse>(verses, set_size, GenerateOption.WithoutRepetition);
                            foreach (List<Verse> set in sets)
                            {
                                // check set against query
                                if (Compare(set, query))
                                {
                                    result.Add(set);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - Chapters
    public static List<Chapter> FindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindChapters(search_scope, current_selection, previous_result, query);
    }
    private static List<Chapter> DoFindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindChapters(source, query);
    }
    private static List<Chapter> DoFindChapters(List<Verse> source, NumberQuery query)
    {
        List<Chapter> result = new List<Chapter>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    List<Chapter> chapters = s_book.GetCompleteChapters(source);
                    if (chapters != null)
                    {
                        if (query.ChapterCount <= 1) // ensure no range search
                        {
                            foreach (Chapter chapter in chapters)
                            {
                                if (Compare(chapter, query))
                                {
                                    result.Add(chapter);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - ChapterRanges
    public static List<List<Chapter>> FindChapterRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindChapterRanges(search_scope, current_selection, previous_result, query);
    }
    private static List<List<Chapter>> DoFindChapterRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindChapterRanges(source, query);
    }
    private static List<List<Chapter>> DoFindChapterRanges(List<Verse> source, NumberQuery query)
    {
        List<List<Chapter>> result = new List<List<Chapter>>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    List<Chapter> chapters = s_book.GetCompleteChapters(source);
                    if (chapters != null)
                    {
                        int range_length = query.ChapterCount;
                        if (range_length == 1) return null;

                        if (range_length == 0) // non-specified range length
                        {
                            // limit range length to minimum
                            int limit = chapters.Count - 1;
                            if ((query.VerseCount > 0) && (query.VerseCount < chapters.Count))
                            {
                                limit = query.VerseCount;
                            }
                            else if ((query.WordCount > 0) && (query.WordCount < chapters.Count))
                            {
                                limit = query.WordCount;
                            }
                            else if ((query.LetterCount > 0) && (query.LetterCount < chapters.Count))
                            {
                                limit = query.LetterCount;
                            }
                            else if ((query.Value > 0L) && (query.Value < chapters.Count))
                            {
                                int min_letters_per_chapter = 61; // chapter #108 Al-Kawthar
                                limit = (int)(query.Value / min_letters_per_chapter);
                            }

                            for (int r = 1; r <= limit; r++) // try all possible range lengths
                            {
                                for (int i = 0; i < chapters.Count - r + 1; i++)
                                {
                                    // build required range
                                    List<Chapter> range = new List<Chapter>();
                                    for (int j = i; j < i + r; j++)
                                    {
                                        range.Add(chapters[j]);
                                    }

                                    // check range
                                    if (Compare(range, query))
                                    {
                                        result.Add(range);
                                    }
                                }
                            }
                        }
                        else // specified range length
                        {
                            int r = range_length;
                            for (int i = 0; i < chapters.Count - r + 1; i++)
                            {
                                // build required range
                                List<Chapter> range = new List<Chapter>();
                                for (int j = i; j < i + r; j++)
                                {
                                    range.Add(chapters[j]);
                                }

                                // check range
                                if (Compare(range, query))
                                {
                                    result.Add(range);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - ChapterSets
    public static List<List<Chapter>> FindChapterSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        return DoFindChapterSets(search_scope, current_selection, previous_result, query);
    }
    private static List<List<Chapter>> DoFindChapterSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindChapterSets(source, query);
    }
    private static List<List<Chapter>> DoFindChapterSets(List<Verse> source, NumberQuery query)
    {
        List<List<Chapter>> result = new List<List<Chapter>>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    List<Chapter> chapters = s_book.GetCompleteChapters(source);
                    if (chapters != null)
                    {
                        int set_size = query.ChapterCount;
                        if (set_size == 1) return null;

                        if (set_size == 0) // non-specified set size
                        {
                            // limit range length to minimum
                            int limit = chapters.Count - 1;
                            if ((query.VerseCount > 0) && (query.VerseCount < chapters.Count))
                            {
                                limit = query.VerseCount;
                            }
                            else if ((query.WordCount > 0) && (query.WordCount < chapters.Count))
                            {
                                limit = query.WordCount;
                            }
                            else if ((query.LetterCount > 0) && (query.LetterCount < chapters.Count))
                            {
                                limit = query.LetterCount;
                            }
                            else if ((query.Value > 0L) && (query.Value < chapters.Count))
                            {
                                int min_letters_per_chapter = 61; // chapter #108 Al-Kawthar
                                limit = (int)(query.Value / min_letters_per_chapter);
                            }

                            for (int i = 0; i < limit; i++) // try all possible set sizes
                            {
                                int size = i + 1;
                                Combinations<Chapter> sets = new Combinations<Chapter>(chapters, size, GenerateOption.WithoutRepetition);
                                foreach (List<Chapter> set in sets)
                                {
                                    // check set against query
                                    if (Compare(set, query))
                                    {
                                        result.Add(set);
                                    }
                                }
                            }
                        }
                        else // specified set size
                        {
                            Combinations<Chapter> sets = new Combinations<Chapter>(chapters, set_size, GenerateOption.WithoutRepetition);
                            foreach (List<Chapter> set in sets)
                            {
                                // check set against query
                                if (Compare(set, query))
                                {
                                    result.Add(set);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }

    // find by revelation place
    public static List<Chapter> FindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, RevelationPlace revelation_place)
    {
        return DoFindChapters(search_scope, current_selection, previous_result, revelation_place);
    }
    private static List<Chapter> DoFindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, RevelationPlace revelation_place)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindChapters(source, revelation_place);
    }
    private static List<Chapter> DoFindChapters(List<Verse> source, RevelationPlace revelation_place)
    {
        List<Chapter> result = new List<Chapter>();
        List<Verse> result_verses = new List<Verse>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                foreach (Verse verse in source)
                {
                    if (verse.Chapter != null)
                    {
                        if (verse.Chapter.RevelationPlace == revelation_place)
                        {
                            result_verses.Add(verse);
                        }
                    }
                }
            }
        }

        int current_chapter_number = -1;
        foreach (Verse verse in result_verses)
        {
            if (verse.Chapter != null)
            {
                if (current_chapter_number != verse.Chapter.Number)
                {
                    current_chapter_number = verse.Chapter.Number;
                    result.Add(verse.Chapter);
                }
            }
        }
        return result;
    }

    // find by prostration type
    public static List<Verse> FindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, ProstrationType prostration_type)
    {
        return DoFindVerses(search_scope, current_selection, previous_result, prostration_type);
    }
    private static List<Verse> DoFindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, ProstrationType prostration_type)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindVerses(source, prostration_type);
    }
    private static List<Verse> DoFindVerses(List<Verse> source, ProstrationType prostration_type)
    {
        List<Verse> result = new List<Verse>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                foreach (Verse verse in source)
                {
                    if ((verse.ProstrationType & prostration_type) > 0)
                    {
                        result.Add(verse);
                    }
                }
            }
        }
        return result;
    }

    // find by frequency - helper methods   
    private static bool Compare(int number1, int number2, NumberType number_type, ComparisonOperator comparison_operator, int remainder)
    {
        if ((number_type == NumberType.None) || (number_type == NumberType.Any))
        {
            if (Numbers.Compare(number1, number2, comparison_operator, remainder))
            {
                return true;
            }
        }
        else
        {
            if (Numbers.IsNumberType(number1, number_type))
            {
                return true;
            }
        }

        return false;
    }
    public static int CalculateLetterFrequencySum(string text, string phrase, FrequencySearchType frequency_search_type)
    {
        if (String.IsNullOrEmpty(phrase)) return 0;

        int result = 0;
        if (s_numerology_system != null)
        {
            text = text.SimplifyTo(s_numerology_system.TextMode);
            if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editios
            {
                text = text.Simplify29();
            }
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            text = text.Replace("\t", "");
            text = text.Replace("_", "");
            text = text.Replace(" ", "");
            text = text.Replace(Verse.OPEN_BRACKET, "");
            text = text.Replace(Verse.CLOSE_BRACKET, "");
            foreach (char character in Constants.INDIAN_DIGITS)
            {
                text = text.Replace(character.ToString(), "");
            }

            if (!String.IsNullOrEmpty(text))
            {
                phrase = phrase.SimplifyTo(s_numerology_system.TextMode);
                if (s_numerology_system.TextMode == "Original") // force simplify Original even in Standard or Grammar Editios
                {
                    phrase = phrase.Simplify29();
                }
                phrase = phrase.Replace("\r", "");
                phrase = phrase.Replace("\n", "");
                phrase = phrase.Replace("\t", "");
                phrase = phrase.Replace("_", "");
                phrase = phrase.Replace(" ", "");
                phrase = phrase.Replace(Verse.OPEN_BRACKET, "");
                phrase = phrase.Replace(Verse.CLOSE_BRACKET, "");
                foreach (char character in Constants.INDIAN_DIGITS)
                {
                    phrase = phrase.Replace(character.ToString(), "");
                }

                if (frequency_search_type == FrequencySearchType.UniqueLetters)
                {
                    phrase = phrase.RemoveDuplicates();
                }

                if (!String.IsNullOrEmpty(phrase))
                {
                    for (int i = 0; i < phrase.Length; i++)
                    {
                        int frequency = 0;
                        for (int j = 0; j < text.Length; j++)
                        {
                            if (phrase[i] == text[j])
                            {
                                frequency++;
                            }
                        }

                        if (frequency > 0)
                        {
                            result += frequency;
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by frequency - Words
    public static List<Word> FindWords(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        return DoFindWords(search_scope, current_selection, previous_result, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type);
    }
    private static List<Word> DoFindWords(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindWords(source, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type);
    }
    private static List<Word> DoFindWords(List<Verse> source, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        List<Word> result = new List<Word>();
        if (!string.IsNullOrEmpty(phrase))
        {
            if (source != null)
            {
                if (source.Count > 0)
                {
                    if (!String.IsNullOrEmpty(phrase))
                    {
                        foreach (Verse verse in source)
                        {
                            if (verse != null)
                            {
                                foreach (Word word in verse.Words)
                                {
                                    string text = word.Text;
                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                    {
                                        result.Add(word);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by frequency - Sentences
    public static List<Sentence> FindSentences(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        return DoFindSentences(search_scope, current_selection, previous_result, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type);
    }
    private static List<Sentence> DoFindSentences(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindSentences(source, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type);
    }
    private static List<Sentence> DoFindSentences(List<Verse> source, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        List<Sentence> result = new List<Sentence>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                List<Word> words = new List<Word>();
                foreach (Verse verse in source)
                {
                    words.AddRange(verse.Words);
                }

                // scan linearly for sequence of words with total Text matching query
                bool done_MustContinue = false;
                for (int i = 0; i < words.Count - 1; i++)
                {
                    StringBuilder str = new StringBuilder();

                    // start building word sequence
                    str.Append(words[i].Text);

                    string stopmark_text = StopmarkHelper.GetStopmarkText(words[i].Stopmark);

                    // 1-word sentence
                    if (
                         (words[i].Stopmark != Stopmark.None) &&
                         (words[i].Stopmark != Stopmark.CanStopAtEither) &&
                         (words[i].Stopmark != Stopmark.MustPause) //&&
                        //(words[i].Stopmark != Stopmark.MustContinue)
                       )
                    {
                        Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[i].Verse, words[i].Position + words[i].Text.Length, str.ToString());
                        if (sentence != null)
                        {
                            string text = sentence.ToString();
                            int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                            if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                            {
                                result.Add(sentence);
                            }
                        }
                    }
                    else // multi-word sentence
                    {
                        // mark the start of 1-to-m MustContinue stopmarks
                        int backup_i = i;

                        // continue building with next words until a stopmark
                        bool done_CanStopAtEither = false;
                        for (int j = i + 1; j < words.Count; j++)
                        {
                            str.Append(" " + words[j].Text);

                            if (words[j].Stopmark == Stopmark.None)
                            {
                                continue; // continue building longer senetence
                            }
                            else // there is a real stopmark
                            {
                                if (s_numerology_system != null)
                                {
                                    if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                    {
                                        if (s_numerology_system.TextMode == "Original")
                                        {
                                            str.Append(" " + stopmark_text);
                                        }
                                    }
                                }

                                if (words[j].Stopmark == Stopmark.MustContinue)
                                {
                                    // TEST Stopmark.MustContinue
                                    //----1 2 3 4 sentences
                                    //1268
                                    //4153
                                    //1799
                                    //2973
                                    //----1 12 123 1234 sentences
                                    //1268
                                    //5421
                                    //7220
                                    //10193
                                    //-------------
                                    //ERRORS
                                    //# duplicate 1
                                    //# short str
                                    //  in 123 1234
                                    //-------------
                                    //// not needed yet
                                    //// multi-mid sentences
                                    //5952
                                    //4772

                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        string text = sentence.ToString();
                                        int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                        if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    if (done_MustContinue)
                                    {
                                        done_MustContinue = false;
                                        continue; // get all overlapping long sentence
                                    }

                                    StringBuilder k_str = new StringBuilder();
                                    for (int k = j + 1; k < words.Count; k++)
                                    {
                                        k_str.Append(words[k].Text + " ");

                                        if (words[k].Stopmark == Stopmark.None)
                                        {
                                            continue; // next k
                                        }
                                        else // there is a stopmark
                                        {
                                            if (s_numerology_system != null)
                                            {
                                                if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                                {
                                                    if (s_numerology_system.TextMode == "Original")
                                                    {
                                                        stopmark_text = StopmarkHelper.GetStopmarkText(words[k].Stopmark);
                                                        k_str.Append(stopmark_text + " ");
                                                    }
                                                }
                                            }
                                            if (k_str.Length > 0)
                                            {
                                                k_str.Remove(k_str.Length - 1, 1);
                                            }

                                            sentence = new Sentence(words[j + 1].Verse, words[j + 1].Position, words[k].Verse, words[k].Position + words[k].Text.Length, k_str.ToString());
                                            if (sentence != null)
                                            {
                                                string text = sentence.ToString();
                                                int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                                if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                                {
                                                    result.Add(sentence);
                                                }
                                            }

                                            if (
                                                 (words[k].Stopmark == Stopmark.ShouldContinue) ||
                                                 (words[k].Stopmark == Stopmark.CanStop) ||
                                                 (words[k].Stopmark == Stopmark.ShouldStop)
                                               )
                                            {
                                                done_MustContinue = true;   // restart from beginning skipping any MustContinue
                                            }
                                            else
                                            {
                                                done_MustContinue = false;   // keep building ever-longer multi-MustContinue sentence
                                            }

                                            j = k;
                                            break; // next j
                                        }
                                    }

                                    if (done_MustContinue)
                                    {
                                        i = backup_i - 1;  // start new sentence from beginning
                                        break; // next i
                                    }
                                    else
                                    {
                                        continue; // next j
                                    }
                                }
                                else if (
                                     (words[j].Stopmark == Stopmark.ShouldContinue) ||
                                     (words[j].Stopmark == Stopmark.CanStop) ||
                                     (words[j].Stopmark == Stopmark.ShouldStop)
                                   )
                                {
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        string text = sentence.ToString();
                                        int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                        if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    i = j; // start new sentence after j
                                    break; // next i
                                }
                                else if (words[j].Stopmark == Stopmark.MustPause)
                                {
                                    if (
                                         (words[j].Text.Simplify29() == "مَنْ".Simplify29()) ||
                                         (words[j].Text.Simplify29() == "بَلْ".Simplify29())
                                       )
                                    {
                                        continue; // continue building longer senetence
                                    }
                                    else if (
                                              (words[j].Text.Simplify29() == "عِوَجَا".Simplify29()) ||
                                              (words[j].Text.Simplify29() == "مَّرْقَدِنَا".Simplify29()) ||
                                              (words[j].Text.Simplify29() == "مَالِيَهْ".Simplify29())
                                            )
                                    {
                                        Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                        if (sentence != null)
                                        {
                                            string text = sentence.ToString();
                                            int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                            if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                            {
                                                result.Add(sentence);
                                            }
                                        }

                                        i = j; // start new sentence after j
                                        break; // next i
                                    }
                                    else // unknown case
                                    {
                                        throw new Exception("Unknown paused Quran word.");
                                    }
                                }
                                // first CanStopAtEither found at j
                                else if ((!done_CanStopAtEither) && (words[j].Stopmark == Stopmark.CanStopAtEither))
                                {
                                    // ^ ذَٰلِكَ ٱلْكِتَٰبُ لَا رَيْبَ
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        string text = sentence.ToString();
                                        int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                        if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    int kk = -1; // start after ^ (e.g. هُدًۭى)
                                    StringBuilder kk_str = new StringBuilder();
                                    StringBuilder kkk_str = new StringBuilder();
                                    for (int k = j + 1; k < words.Count; k++)
                                    {
                                        str.Append(" " + words[k].Text);
                                        if (kkk_str.Length > 0) // skip first k loop
                                        {
                                            kk_str.Append(" " + words[k].Text);
                                        }
                                        kkk_str.Append(" " + words[k].Text);

                                        if (words[k].Stopmark == Stopmark.None)
                                        {
                                            continue; // next k
                                        }
                                        else // there is a stopmark
                                        {
                                            if (s_numerology_system != null)
                                            {
                                                if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                                {
                                                    if (s_numerology_system.TextMode == "Original")
                                                    {
                                                        str.Append(" " + stopmark_text);
                                                        if (kk_str.Length > 0)
                                                        {
                                                            kk_str.Append(" " + stopmark_text);
                                                        }
                                                        kkk_str.Append(" " + stopmark_text);
                                                    }
                                                }
                                            }

                                            // second CanStopAtEither found at k
                                            if (words[k].Stopmark == Stopmark.CanStopAtEither)
                                            {
                                                // ^ ذَٰلِكَ ٱلْكِتَٰبُ لَا رَيْبَ ۛ^ فِيهِ
                                                sentence = new Sentence(words[i].Verse, words[i].Position, words[k].Verse, words[k].Position + words[k].Text.Length, str.ToString());
                                                if (sentence != null)
                                                {
                                                    string text = sentence.ToString();
                                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                kk = k + 1; // backup k after second ^
                                                continue; // next k
                                            }
                                            else // non-CanStopAtEither stopmark
                                            {
                                                // kkk_str   فِيهِ ۛ^ هُدًۭى لِّلْمُتَّقِينَ
                                                sentence = new Sentence(words[j + 1].Verse, words[j + 1].Position, words[k].Verse, words[k].Position + words[k].Text.Length, kkk_str.ToString());
                                                if (sentence != null)
                                                {
                                                    string text = sentence.ToString();
                                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                // kk_str   هُدًۭى لِّلْمُتَّقِينَ
                                                sentence = new Sentence(words[kk].Verse, words[kk].Position, words[k].Verse, words[k].Position + words[k].Text.Length, kk_str.ToString());
                                                if (sentence != null)
                                                {
                                                    string text = sentence.ToString();
                                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                // skip the whole surrounding non-CanStopAtEither sentence
                                                j = k;
                                                break; // next j
                                            }
                                        }
                                    }

                                    // restart from last
                                    str.Length = 0;
                                    j = i - 1; // will be j++ by reloop
                                    done_CanStopAtEither = true;
                                }
                                else if (words[j].Stopmark == Stopmark.MustStop)
                                {
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        string text = sentence.ToString();
                                        int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                        if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    i = j; // start new sentence after j
                                    break; // next i
                                }
                                else // unknown case
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by frequency - Verses
    public static List<Verse> FindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        return DoFindVerses(search_scope, current_selection, previous_result, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type);
    }
    private static List<Verse> DoFindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindVerses(source, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type);
    }
    private static List<Verse> DoFindVerses(List<Verse> source, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        List<Verse> result = new List<Verse>();
        if (!string.IsNullOrEmpty(phrase))
        {
            if (source != null)
            {
                if (source.Count > 0)
                {
                    if (!String.IsNullOrEmpty(phrase))
                    {
                        foreach (Verse verse in source)
                        {
                            if (verse != null)
                            {
                                string text = verse.Text;
                                int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                {
                                    result.Add(verse);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by frequency - Chapters
    public static List<Chapter> FindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        return DoFindChapters(search_scope, current_selection, previous_result, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type);
    }
    private static List<Chapter> DoFindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result);
        return DoFindChapters(source, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type);
    }
    private static List<Chapter> DoFindChapters(List<Verse> source, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type)
    {
        List<Chapter> result = new List<Chapter>();
        if (!string.IsNullOrEmpty(phrase))
        {
            if (source != null)
            {
                if (source.Count > 0)
                {
                    if (s_book != null)
                    {
                        List<Chapter> source_chapters = s_book.GetCompleteChapters(source);
                        if (!String.IsNullOrEmpty(phrase))
                        {
                            foreach (Chapter chapter in source_chapters)
                            {
                                if (chapter != null)
                                {
                                    string text = chapter.Text;
                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type);
                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                    {
                                        result.Add(chapter);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }

    public static string GetTranslationKey(string translation)
    {
        string result = null;
        if (s_book != null)
        {
            if (s_book.TranslationInfos != null)
            {
                foreach (string key in s_book.TranslationInfos.Keys)
                {
                    if (s_book.TranslationInfos[key].Name == translation)
                    {
                        result = key;
                    }
                }
            }
        }
        return result;
    }
    public static void LoadTranslation(string translation)
    {
        DataAccess.LoadTranslation(s_book, translation);
    }
    public static void UnloadTranslation(string translation)
    {
        DataAccess.UnloadTranslation(s_book, translation);
    }
    public static void SaveTranslation(string translation)
    {
        DataAccess.SaveTranslation(s_book, translation);
    }

    // help messages
    private static List<string> s_help_messages = new List<string>();
    public static List<string> HelpMessages
    {
        get { return s_help_messages; }
    }
    private static void LoadHelpMessages()
    {
        string filename = Globals.HELP_FOLDER + "/" + "Messages.txt";
        if (File.Exists(filename))
        {
            s_help_messages = DataAccess.LoadLines(filename);
        }
    }
}
