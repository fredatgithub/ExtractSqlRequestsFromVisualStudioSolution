using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExtractSqlRequests
{
  internal class Program
  {
    static void Main(string[] arguments)
    {
      Action<string> Display = Console.WriteLine;
      Display("Console application which search all code lines with an SQL request in a Visual Studio solution.");
      Display(string.Empty);
      // desktop
      var startPath = "C:\\Repos\\";

      var startDirectory = new DirectoryInfo(startPath);
      var sqlRequestResultFile = new List<string>();
      var sqlSearchedWords = "select,insert,delete,update,alter";
      var comma = ',';
      var codeFilePattern = "*.cs,*.vb";
      var ListOfFiles = new List<string>();
      foreach (string codeLanguage in codeFilePattern.Split(comma))
      {
        ListOfFiles.AddRange(GetFilesFileteredBySize(startDirectory, 0, codeLanguage, SearchOption.AllDirectories));
      }

      foreach (var file in ListOfFiles)
      {
        // we read the file
        var fileLines = new List<string>();
        using (StreamReader sr = new StreamReader(file))
        {
          while (sr.Read() != -1)
          {
            fileLines.Add(sr.ReadLine());
          }
        }

        if (ListOfLinesHasMatch(fileLines, sqlSearchedWords))
        {
          sqlRequestResultFile.AddRange(GetMatchedItems(fileLines, sqlSearchedWords));
        }
      }

      // we write the final file with all sql requests
      var language = codeFilePattern.Replace("*", "").Replace(".", "").Split(',')[1];
      var filename = $"SqlRequests-{language}.txt";
      using (StreamWriter sw = new StreamWriter(filename, true))
      {
        foreach (var item in sqlRequestResultFile)
        {
          sw.WriteLine(item);
        }
      }

      Display($"The file {filename} has been written to the disk. It has {sqlRequestResultFile.Count} lines");
      Display(string.Empty);
      Display("Appuyez sur une touche pour quitter :");
      Console.ReadKey();
    }

    public static List<string> GetMatchedItems(List<string> fileLines, string sqlSearchedWords)
    {
      var result = new List<string>();
      foreach (string item in sqlSearchedWords.Split(','))
      {
        result.AddRange(fileLines.Where(x => x != null && x.Contains(item.ToLower())).ToList());
        result.AddRange(fileLines.Where(x => x != null && x.Contains(item.ToUpper())).ToList());
      }

      return result;
    }

    public static bool ListOfLinesHasMatch(List<string> fileLines, string sqlRequestStartWord)
    {
      foreach (string item in sqlRequestStartWord.Split(','))
      {
        if (fileLines.Any(w => w != null && w.Contains(item.ToLower())))
        {
          return true;
        }

        if (fileLines.Any(m => m != null && m.Contains(item.ToUpper())))
        {
          return true;
        }
      }

      return false;
    }

    private static string[] GetDirectoryFileNameAndExtension(string filePath)
    {
      string directory = Path.GetDirectoryName(filePath);
      string fileName = Path.GetFileNameWithoutExtension(filePath);
      string extension = Path.GetExtension(filePath);

      return new[] { directory, fileName, extension };
    }

    public static List<string> GetFilesFileteredBySize(DirectoryInfo directoryInfo, long sizeGreaterOrEqualTo, string searchPattern, SearchOption subDirectoryOption = SearchOption.TopDirectoryOnly)
    {
      List<string> result = new List<string>();
      foreach (FileInfo fileInfo in directoryInfo.GetFiles(searchPattern, subDirectoryOption))
      {
        if (fileInfo.Length >= sizeGreaterOrEqualTo)
        {
          result.Add(fileInfo.FullName);
        }
      }

      return result;
    }
  }
}
