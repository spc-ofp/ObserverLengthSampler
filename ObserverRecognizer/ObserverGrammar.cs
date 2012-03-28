// -----------------------------------------------------------------------
// <copyright file="ObserverGrammar.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace Spc.Ofp.ObserverRecognizer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Speech.Recognition;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// ObserverGrammar manages the details of loading a speech recognition grammar.
    /// If we really wanted to be pedantic, it could be named ObserverGrammarFactory
    /// </summary>
    public sealed class ObserverGrammar
    {
        /*
         * http://msdn.microsoft.com/en-us/library/hh538495.aspx
         * http://msdn.microsoft.com/en-us/library/hh378350.aspx
         * http://msdn.microsoft.com/en-us/library/hh378351.aspx
         * http://msdn.microsoft.com/en-us/library/hh362824.aspx
         * http://msdn.microsoft.com/en-us/library/bb814030.aspx
         * http://cafe.bevocal.com/docs/grammar/xml.html
         * http://gotspeech.net/forums/thread/10339.aspx
         * https://studio.tellme.com/grammars/
         * http://franksworld.com/blog/archive/2009/06/30/11617.aspx
         */

        private const string GrammarResourceName = "Spc.Ofp.ObserverRecognizer.ObserverGrammar.xml";

        public const string SpeciesKey = "Species";
        public const string LengthKey = "FishLength";

        private static IDictionary<string, int> StringToIntegers = new Dictionary<string, int>()
        {
            { "zero", 0 },
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 }, 
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
        };

        /// <summary>
        /// IMPORTANT!!!!!!
        /// The species names in this dictionary need to be kept up to date with the
        /// values (and tag values) in the grammar XML.  That's my motivation for
        /// pushing the conversion here, and not having it in another library.
        /// </summary>
        public static IDictionary<string, string> NameToCode = new Dictionary<string, string>()
        {
            { "yellowfin", "YFT" },
            { "bigeye", "BET" },
            { "frigate tuna", "FRI" },
            { "bullet tuna", "BLT" },
            { "kawakawa", "KAW" },
            { "albacore", "ALB" },
            { "wahoo", "WAH" },
            { "blue marlin", "BUM" },
            { "black marlin", "BLM" },
            { "striped marlin", "MLS" },
            { "sailfish", "SFA" },
            { "short billed spearfish", "SSP" },
            { "broadbill swordfish", "SWO" },
            { "sergeant major", "ABU" },
            { "amberjack", "AMX" },
            { "barracuda", "BAR" },
            { "batfish", "BAT" },
            { "bigeye trevally", "CXS" },
            { "mahi mahi", "DOL" },
            { "rainbow runner", "RRU" },
            { "file fish", "FLF" },
            { "triggerfish", "TRI" },
            { "drummer", "KYC" },
            { "mackeral scad", "MSD" },
            { "man o war", "PSC" },
            { "triple tail", "LOB" },
            { "pomfret", "BRZ" },
            { "oceanic whitetip", "OCS" },
            { "blue whaler shark", "BSH" },
            { "silky shark", "FAL" },
            { "mako shark", "MAK" },
            { "hammerhead shark", "SPN" },
            { "thresher shark", "THR" },
            { "whale shark", "RHN" },
            { "manta ray", "MAN" },
            { "sunfish", "MOX" },
            { "squid", "SQU" },
            { "frigate and bullet tuna", "FRZ" },
            { "unknown tuna", "TUN" },
            { "unknown trevally", "TRE" },
            { "unknown fish", "UNS" },
            { "unknown bird", "BIZ" },
            { "unknown", "UNS" }
        };
        
        /// <summary>
        /// Loads the one and only grammar from an embedded resource.
        /// There's no error handling -- that's the caller's responsibility.
        /// </summary>
        /// <returns></returns>
        public static Grammar GetGrammar()
        {
            Grammar grammar = null;
            using (Stream stream = typeof(ObserverGrammar).Assembly.GetManifestResourceStream(GrammarResourceName))
            {
                grammar = new Grammar(stream);
            }
            return grammar;
        }

        /// <summary>
        /// Check the recognition result for both a species name and a length value
        /// </summary>
        /// <param name="semanticValue"></param>
        /// <returns></returns>
        public static bool HasSpeciesAndLength(SemanticValue semanticValue)
        {
            bool hasSpecies = semanticValue.ContainsKey(SpeciesKey);
            bool hasLength = semanticValue.ContainsKey(LengthKey);
            System.Diagnostics.Debug.WriteLine("HasSpecies? {0}  HasLength? {1}", hasSpecies, hasLength);
            return hasSpecies && hasLength;
        }

        public static Tuple<string, string> GetSpeciesAndLength(SemanticValue semanticValue)
        {
            if (!HasSpeciesAndLength(semanticValue))
                return Tuple.Create(String.Empty, String.Empty);

            // As long as the grammar is valid, there should be no need for a defensive
            // null check.  If it is throwing an NRE, then check the grammar definition.
            // Don't ask me how I know...
            var species = semanticValue[SpeciesKey].Value.ToString();
            var length = semanticValue[LengthKey].Value.ToString();
            System.Diagnostics.Debug.WriteLine("Species: [{0}], Length: [{1}]", species, length);
            return Tuple.Create(species, length);
        }

        public static string DumpSemantics(SemanticValue semanticValue)
        {
            StringBuilder builder = new StringBuilder();
            if (null == semanticValue)
            {
                builder.Append("Null SemanticValue argument");
            }
            else
            {
                builder.AppendFormat("SemanticValue.Count: {0}\n", semanticValue.Count);
                var keys =
                    from sv in semanticValue
                    select sv.Key;
                foreach (var key in keys)
                {
                    builder.Append("Key: ").AppendLine(key);
                    builder.Append("Value: ");
                    SemanticValue obj = semanticValue[key];
                    builder.AppendLine(
                        null == obj ?
                            "Null Value" :
                            obj.Value.ToString()
                    );
                    builder.AppendLine("===============");
                }
            }
            return builder.ToString();
        }

        public static int? SpokenNumberToInteger(string spokenNumber)
        {
            if (String.IsNullOrEmpty(spokenNumber))
                return null;

            /*
             * Some people, when confronted with a problem, think 
             * “I know, I'll use regular expressions.”   Now they have two problems.
             * -- Jamie Zawinski
             */
            string[] digits = Regex.Split(spokenNumber, @"\s+");

            int place = 1;
            int returnValue = 0;
            foreach (string digit in digits.Reverse())
            {
                var digitKey = digit.ToLower();
                if (StringToIntegers.ContainsKey(digitKey))
                {
                    returnValue += (StringToIntegers[digitKey] * place);
                    place *= 10;
                }
            }

            return returnValue;
        }

        public static string SpeciesNameToCode(string name)
        {
            string speciesCode = String.Empty;

            if (String.IsNullOrEmpty(name))
                return speciesCode;

            name = name.ToLower();
            if (NameToCode.ContainsKey(name))
            {
                speciesCode = NameToCode[name];
            }
            
            return speciesCode;


        }
    }
}
