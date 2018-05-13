public enum Edition { Lite }

public static class Globals
{
    public static string VERSION = "6.19.619.4"; // updated by Version.bat (with AssemblyInfo.cs of all projects)
    public static Edition EDITION = Edition.Lite;
    public static string SHORT_VERSION
    {
        get
        {
            int pos = VERSION.LastIndexOf(".");
            if (EDITION == Edition.Lite)
            {
                return ("v" + VERSION.Remove(pos) + "L");
            }
            else
            {
                return ("v" + VERSION.Remove(pos) + "!"); // Invalid Edition
            }
        }
    }
    public static string LONG_VERSION
    {
        get
        {
            return (VERSION + " - " + EDITION.ToString() + " Edition");
        }
    }

    // Global Variables
    public static string OUTPUT_FILE_EXT = ".csv"; // to open in Excel
    public static string DELIMITER = "\t";
    public static string SUB_DELIMITER = "|";
    public static string DATE_FORMAT = "yyyy-MM-dd";
    public static string TIME_FORMAT = "HH:mm:ss";
    public static string DATETIME_FORMAT = DATE_FORMAT + " " + TIME_FORMAT;
    public static string NUMBER_FORMAT = "000";

    // Global Folders
    public static string FONTS_FOLDER = "Fonts";
    public static string IMAGES_FOLDER = "Images";
    public static string DATA_FOLDER = "Data";
    public static string AUDIO_FOLDER = "Audio";
    public static string TRANSLATIONS_FOLDER = "Translations";
    public static string TRANSLATIONS_OFFLINE_FOLDER = "Translations/Offline";
    public static string TRANSLATIONS_FLAGS_FOLDER = "Translations/Flags";
    public static string RULES_FOLDER = "Rules";
    public static string VALUES_FOLDER = "Values";
    public static string STATISTICS_FOLDER = "Statistics";
    public static string BOOKMARKS_FOLDER = "Bookmarks";
    public static string HISTORY_FOLDER = "History";
    public static string HELP_FOLDER = "Help";
}
