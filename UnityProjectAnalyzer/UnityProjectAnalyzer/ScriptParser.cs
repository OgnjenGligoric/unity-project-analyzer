using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnityProjectAnalyzer
{
    public class ScriptParser
    {
        private String _scriptPath { get; set; }

        public ScriptParser(String scriptPath)
        {
            _scriptPath = scriptPath;
        }
        public string GetGUID()
        {
            if (!File.Exists(_scriptPath)) return "";

            string[] lines = File.ReadAllLines(_scriptPath);

            foreach (string line in lines)
            {
                string pattern = @"guid:\s*(\w+)";
                Match match = Regex.Match(line, pattern);

                if (!match.Success) continue;

                string guidValue = match.Groups[1].Value;
                return guidValue;
            }
            return "";
        }
    }
}
