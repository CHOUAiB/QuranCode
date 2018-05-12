using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Model;

public static class DataAccess
{
    static DataAccess()
    {
        if (!Directory.Exists(Globals.DATA_FOLDER))
        {
            Directory.CreateDirectory(Globals.DATA_FOLDER);
        }

        if (!Directory.Exists(Globals.AUDIO_FOLDER))
        {
            Directory.CreateDirectory(Globals.AUDIO_FOLDER);
        }

        if (!Directory.Exists(Globals.TRANSLATIONS_FOLDER))
        {
            Directory.CreateDirectory(Globals.TRANSLATIONS_FOLDER);
        }
    }

    // quran text from http://tanzil.net
    public static List<string> LoadVerseTexts()
    {
        List<string> result = new List<string>();
        string filename = Globals.DATA_FOLDER + "/" + "quran-uthmani.txt";
        if (File.Exists(filename))
        {
            using (StreamReader reader = File.OpenText(filename))
            {
                while (!reader.EndOfStream)
                {
                    // skip # comment lines (tanzil copyrights, other meta info, ...)
                    string line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        if (!line.StartsWith("#"))
                        {
                            line = line.Replace("\r", "");
                            line = line.Replace("\n", "");
                            while (line.Contains("  "))
                            {
                                line = line.Replace("  ", " ");
                            }
                            line = line.Trim();
                            result.Add(line);
                        }
                    }
                }
            }
        }
        return result;
    }
    public static void SaveVerseTexts(Book book, string filename)
    {
        if (book != null)
        {
            StringBuilder str = new StringBuilder();
            foreach (Verse verse in book.Verses)
            {
                str.AppendLine(verse.Text);
            }
            SaveFile(filename, str.ToString());
        }
    }

    // end of verse stopmarks are assumed to be end of sentence (Meem) for now
    public static List<Stopmark> LoadVerseStopmarks()
    {
        List<Stopmark> result = new List<Stopmark>();
        string filename = Globals.DATA_FOLDER + "/" + "verse_stopmarks.txt";
        if (File.Exists(filename))
        {
            using (StreamReader reader = File.OpenText(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        result.Add(StopmarkHelper.GetStopmark(line));
                    }
                }
            }
        }
        return result;
    }
    public static void SaveVerseStopmarks(Book book, string filename)
    {
        if (book != null)
        {
            StringBuilder str = new StringBuilder();
            foreach (Verse verse in book.Verses)
            {
                str.AppendLine(StopmarkHelper.GetStopmarkText(verse.Stopmark));
            }
            SaveFile(filename, str.ToString());
        }
    }

    // recitation infos from http://www.everyayah.com
    public static void LoadRecitationInfos(Book book)
    {
        if (book != null)
        {
            book.RecitationInfos = new Dictionary<string, RecitationInfo>();
            string filename = Globals.AUDIO_FOLDER + "/" + "metadata.txt";
            if (File.Exists(filename))
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    string line = reader.ReadLine(); // skip header row
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('\t');
                        if (parts.Length >= 4)
                        {
                            RecitationInfo recitation = new RecitationInfo();
                            recitation.Url = parts[0];
                            recitation.Folder = parts[0];
                            recitation.Language = parts[1];
                            recitation.Reciter = parts[2];
                            int.TryParse(parts[3], out recitation.Quality);
                            recitation.Name = recitation.Language + " - " + recitation.Reciter;
                            book.RecitationInfos.Add(parts[0], recitation);
                        }
                    }
                }
            }
        }
    }

    // translations info from http://tanzil.net
    public static void LoadTranslationInfos(Book book)
    {
        if (book != null)
        {
            book.TranslationInfos = new Dictionary<string, TranslationInfo>();
            string filename = Globals.TRANSLATIONS_OFFLINE_FOLDER + "/" + "metadata.txt";
            if (File.Exists(filename))
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    string line = reader.ReadLine(); // skip header row
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('\t');
                        if (parts.Length >= 4)
                        {
                            TranslationInfo translation = new TranslationInfo();
                            translation.Url = "?transID=" + parts[0] + "&type=" + TranslationInfo.FileType;
                            translation.Flag = parts[1];
                            translation.Language = parts[2];
                            translation.Translator = parts[3];
                            translation.Name = parts[2] + " - " + parts[3];
                            book.TranslationInfos.Add(parts[0], translation);
                        }
                    }
                }
            }
        }
    }

    // translation books from http://tanzil.net
    public static void LoadTranslations(Book book)
    {
        if (book != null)
        {
            try
            {
                string[] filenames = Directory.GetFiles(Globals.TRANSLATIONS_FOLDER + "/");
                foreach (string filename in filenames)
                {
                    List<string> translated_lines = LoadLines(filename);
                    if (translated_lines != null)
                    {
                        string translation = filename.Substring((Globals.TRANSLATIONS_FOLDER.Length + 1), filename.Length - (Globals.TRANSLATIONS_FOLDER.Length + 1) - 4);
                        if (translation == "metadata.txt") continue;
                        LoadTranslation(book, translation);
                    }
                }
            }
            catch
            {
                // ignore error
            }
        }
    }
    public static void LoadTranslation(Book book, string translation)
    {
        if (book != null)
        {
            try
            {
                string[] filenames = Directory.GetFiles(Globals.TRANSLATIONS_FOLDER + "/");
                bool already_loaded = false;
                foreach (string filename in filenames)
                {
                    if (filename.Contains(translation))
                    {
                        already_loaded = true;
                        break;
                    }
                }
                if (!already_loaded)
                {
                    File.Copy(Globals.TRANSLATIONS_OFFLINE_FOLDER + "/" + translation + ".txt", Globals.TRANSLATIONS_FOLDER + "/" + translation + ".txt", true);
                }

                filenames = Directory.GetFiles(Globals.TRANSLATIONS_FOLDER + "/");
                foreach (string filename in filenames)
                {
                    if (filename.Contains(translation))
                    {
                        List<string> translated_lines = LoadLines(filename);
                        if (translated_lines != null)
                        {
                            if (book.TranslationInfos != null)
                            {
                                if (book.TranslationInfos.ContainsKey(translation))
                                {
                                    if (book.Verses != null)
                                    {
                                        if (book.Verses.Count > 0)
                                        {
                                            for (int i = 0; i < book.Verses.Count; i++)
                                            {
                                                book.Verses[i].Translations[translation] = translated_lines[i];
                                            }

                                            // add bismAllah translation to the first verse of each chapter except chapters 1 and 9
                                            foreach (Chapter chapter in book.Chapters)
                                            {
                                                if ((chapter.Number != 1) && (chapter.Number != 9))
                                                {
                                                    if ((translation != "ar.emlaaei") && (translation != "en.transliteration") && (translation != "en.wordbyword"))
                                                    {
                                                        if (!chapter.Verses[0].Translations[translation].StartsWith(book.Verses[0].Translations[translation]))
                                                        {
                                                            chapter.Verses[0].Translations[translation] = book.Verses[0].Translations[translation] + " " + chapter.Verses[0].Translations[translation];
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch
            {
                // ignore error
            }
        }
    }
    public static void UnloadTranslation(Book book, string translation)
    {
        if (book != null)
        {
            try
            {
                string[] filenames = Directory.GetFiles(Globals.TRANSLATIONS_FOLDER + "/");
                foreach (string filename in filenames)
                {
                    if (filename.Contains(translation))
                    {
                        if (book.TranslationInfos != null)
                        {
                            if (book.TranslationInfos.ContainsKey(translation))
                            {
                                if (book.Verses.Count > 0)
                                {
                                    for (int i = 0; i < book.Verses.Count; i++)
                                    {
                                        book.Verses[i].Translations.Remove(translation);
                                    }
                                    book.TranslationInfos.Remove(translation);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch
            {
                // ignore error
            }
        }
    }
    public static void SaveTranslation(Book book, string translation)
    {
        if (book != null)
        {
            StringBuilder str = new StringBuilder();
            foreach (Verse verse in book.Verses)
            {
                str.AppendLine(verse.Translations[translation]);
            }
            string filename = Globals.TRANSLATIONS_FOLDER + "/" + translation + ".txt";
            SaveFile(filename, str.ToString());
        }
    }

    // word meanings from http://qurandev.appspot.com - modified by Ali Adams
    public static void LoadWordMeanings(Book book)
    {
        if (book != null)
        {
            try
            {
                string filename = Globals.TRANSLATIONS_OFFLINE_FOLDER + "/" + "en.wordbyword.txt";
                if (File.Exists(filename))
                {
                    using (StreamReader reader = File.OpenText(filename))
                    {
                        while (!reader.EndOfStream)
                        {
                            if (book.Verses != null)
                            {
                                foreach (Verse verse in book.Verses)
                                {
                                    string line = reader.ReadLine();
                                    string[] parts = line.Split('\t');

                                    int word_count = 0;
                                    if (verse.Words != null)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            if (word.Text != "و") // WawAsWord
                                            {
                                                word_count++;
                                            }
                                        }

                                        int i = 0;
                                        if (!verse.Book.WithBismAllah)
                                        {
                                            if (verse.NumberInChapter == 1)
                                            {
                                                if ((verse.Chapter.Number != 1) && (verse.Chapter.Number != 9))
                                                {
                                                    i += 4;
                                                }
                                            }
                                        }
                                        if (parts.Length != word_count + i)
                                        {
                                            throw new Exception("File format error.");
                                        }

                                        foreach (Word word in verse.Words)
                                        {
                                            if (word.Text == "و") // WawAsWord
                                            {
                                                word.Meaning = "and";
                                            }
                                            else
                                            {
                                                word.Meaning = parts[i];
                                                i++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("LoadWordMeanings: " + ex.Message);
            }
        }
    }

    // uthmani roots by Ali Adams from emlaaei version of http://www.noorsoft.org version 0.9.1
    public static void LoadRootWords(Book book)
    {
        if (book != null)
        {
            try
            {
                string filename = Globals.DATA_FOLDER + "/" + "root-words.txt";
                if (File.Exists(filename))
                {
                    book.RootWords = new Dictionary<string, List<Word>>();
                    using (StreamReader reader = File.OpenText(filename))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] parts = line.Split('\t');
                            if (parts.Length == 3)
                            {
                                string root = parts[0];
                                //int count = int.Parse(parts[1]);
                                string[] addresses = parts[2].Split(';');

                                List<Word> root_words = new List<Word>();
                                foreach (string address in addresses)
                                {
                                    string[] segments = address.Split(':');
                                    if (segments.Length == 2)
                                    {
                                        int verse_index = int.Parse(segments[0]);
                                        if (book.Verses != null)
                                        {
                                            Verse verse = book.Verses[verse_index];
                                            if (verse != null)
                                            {
                                                int bismAllah_shift = 0;
                                                if (!verse.Book.WithBismAllah)
                                                {
                                                    if (verse.NumberInChapter == 1)
                                                    {
                                                        if ((verse.Chapter.Number != 1) && (verse.Chapter.Number != 9))
                                                        {
                                                            bismAllah_shift -= 4;
                                                        }
                                                    }
                                                }

                                                string[] word_segment_parts = segments[1].Split(',');
                                                foreach (string word_segment_part in word_segment_parts)
                                                {
                                                    if (verse.Words != null)
                                                    {
                                                        if (verse.Words.Count > 0)
                                                        {
                                                            int word_number = int.Parse(word_segment_part);
                                                            int word_index = (word_number - 1) + bismAllah_shift;

                                                            // shift word_index by waw words
                                                            if (verse.Book.WawAsWord)
                                                            {
                                                                for (int i = 0; i <= word_index; i++)
                                                                {
                                                                    if (verse.Words[i].Text == "و")
                                                                    {
                                                                        word_index++;
                                                                    }
                                                                }
                                                            }

                                                            if ((word_index >= 0) && (word_index < verse.Words.Count))
                                                            {
                                                                Word root_word = verse.Words[word_index];
                                                                if (root_word != null)
                                                                {
                                                                    root_words.Add(root_word);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!book.RootWords.ContainsKey(root))
                                {
                                    book.RootWords.Add(root, root_words);
                                }
                            }
                            else
                            {
                                // skip reading copyright notice;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("LoadRootWords: " + ex.Message);
            }
        }
    }
    public static void SaveRootWords(Book book)
    {
        if (book != null)
        {
            if (book.RootWords != null)
            {
                try
                {
                    string filename = Globals.DATA_FOLDER + "/" + "root-words-aliadams" + (!book.WithBismAllah ? "-no-bismAllah" : "ORG") + ".txt";
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                    {
                        foreach (string root in book.RootWords.Keys)
                        {
                            StringBuilder str = new StringBuilder();
                            List<Word> root_words = book.RootWords[root];
                            str.Append(root + "\t" + root_words.Count.ToString() + "\t");
                            for (int i = 0; i < root_words.Count; i++)
                            {
                                int word_chapter_number = root_words[i].Verse.Chapter.Number;
                                int word_verse_number = root_words[i].Verse.Number;
                                int word_verse_number_in_chapter = root_words[i].Verse.NumberInChapter;
                                int word_number_in_verse = root_words[i].NumberInVerse;

                                if (!book.WithBismAllah)
                                {
                                    if ((word_chapter_number != 1) && (word_chapter_number != 9))
                                    {
                                        if (word_verse_number_in_chapter == 1)
                                        {
                                            if (word_number_in_verse > 4)
                                            {
                                                word_number_in_verse -= 4;
                                            }
                                            else
                                            {
                                                word_number_in_verse -= 4;
                                                continue; // skip Bism Allah Ar-Rahmaan Ar-Raheem
                                            }
                                        }
                                    }
                                }

                                str.Append((word_verse_number - 1).ToString() + ":" + word_number_in_verse.ToString());
                                while ((i + 1) < root_words.Count)
                                {
                                    int next_word_chapter_number = root_words[i + 1].Verse.Chapter.Number;
                                    int next_word_verse_number = root_words[i + 1].Verse.Number;
                                    int next_word_verse_number_in_chapter = root_words[i + 1].Verse.NumberInChapter;
                                    if (word_verse_number == next_word_verse_number)
                                    {
                                        int next_word_number_in_verse = root_words[i + 1].NumberInVerse;

                                        if (!book.WithBismAllah)
                                        {
                                            if ((next_word_chapter_number != 1) && (next_word_chapter_number != 9))
                                            {
                                                if (next_word_verse_number_in_chapter == 1)
                                                {
                                                    if (next_word_number_in_verse > 4)
                                                    {
                                                        next_word_number_in_verse -= 4;
                                                    }
                                                    else
                                                    {
                                                        i++;
                                                        continue; // skip Bism Allah Ar-Rahmaan Ar-Raheem
                                                    }
                                                }
                                            }
                                        }

                                        // in all cases
                                        str.Append("," + next_word_number_in_verse.ToString());
                                        i++;
                                    }
                                    else // end of verse, move to outer loop (next verse)
                                    {
                                        break;
                                    }
                                }
                                str.Append(";");
                            }
                            str.Remove(str.Length - ";".Length, ";".Length);
                            writer.WriteLine(str);
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

    // helper methods
    public static List<string> LoadLines(string filename)
    {
        List<string> result = new List<string>();
        try
        {
            if (File.Exists(filename))
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        result.Add(line);
                    }
                }
            }
        }
        catch
        {
            // silence error
        }
        return result;
    }
    private static string LoadFile(string filename)
    {
        StringBuilder str = new StringBuilder();
        if (File.Exists(filename))
        {
            try
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        str.AppendLine(line);
                    }
                }
            }
            catch
            {
                // silence error
            }
        }
        return str.ToString();
    }
    private static void SaveFile(string filename, string content)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
            {
                writer.Write(content);
            }
        }
        catch
        {
            // silence IO error in case running from read-only media (CD/DVD)
        }
    }
}
