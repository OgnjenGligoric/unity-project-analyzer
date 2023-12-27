using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnityProjectAnalyzer.Utils
{
    public class UnityFileExtractor
    {
        public string ExtractNumberFromFatherId(string input)
        {
            // Using Regex to match the number between "{fileID: " and "}"
            Regex regex = new Regex(@"{fileID:\s*(\d+)\s*}");
            Match match = regex.Match(input);

            if (match.Success)
            {
                // Extracting the captured group as a string
                return match.Groups[1].Value;
            }

            return "0"; // Return "0" if no match
        }
    }
}
