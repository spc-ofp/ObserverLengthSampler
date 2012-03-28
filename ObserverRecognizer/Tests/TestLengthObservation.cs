// -----------------------------------------------------------------------
// <copyright file="TestLengthObservation.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace Spc.Ofp.ObserverRecognizer.Tests
{
    /*
     * This file is part of Observer Length Sampler.
     *
     * Observer Length Sampler is free software: you can redistribute it and/or modify
     * it under the terms of the GNU Affero General Public License as published by
     * the Free Software Foundation, either version 3 of the License, or
     * (at your option) any later version.
     *  
     * Observer Length Sampler is distributed in the hope that it will be useful,
     * but WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
     * GNU Affero General Public License for more details.
     *  
     * You should have received a copy of the GNU Affero General Public License
     * along with Observer Length Sampler.  If not, see <http://www.gnu.org/licenses/>.
     */
    using System.Collections.Generic;
    using System.Globalization;
    using System.Speech.Recognition;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for speech recognition.
    /// Uses the magic voodoo of Microsoft's "EmulateRecognize" method.
    /// </summary>
    [TestFixture]
    public class TestLengthObservation
    {
        private SpeechRecognitionEngine _recognizer;
        
        [TestFixtureSetUp]
        public void SetUp()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.LoadGrammar(ObserverGrammar.GetGrammar());
        }

        [Test]
        public void GetSamplesFromList()
        {
            var samples = new List<string>()
            {
                "Yellowfin One Zero",
                "Unknown Zero",
                "Bigeye Trevally One Four",
                "Barracuda One Two One",
                "Mako One One One",
                "Unknown Tuna Four Five",
                "Unknown Bird Two Two"
            };

            foreach (var sample in samples)
            {
                // http://stackoverflow.com/questions/9475737/simple-grammar-for-speech-recognition
                // http://msdn.microsoft.com/en-us/library/ms554574.aspx
                var result = _recognizer.EmulateRecognize(sample, CompareOptions.IgnoreCase);
                Assert.NotNull(result);
                Assert.NotNull(result.Semantics);
                System.Console.WriteLine(ObserverGrammar.DumpSemantics(result.Semantics));
                Assert.True(ObserverGrammar.HasSpeciesAndLength(result.Semantics));
            }
           
        }

        [Test]
        public void GetSingleSample()
        {
            string sampleText = "Barracuda Five Two";
            var result = _recognizer.EmulateRecognize(sampleText, CompareOptions.IgnoreCase);
            Assert.NotNull(result);
            Assert.NotNull(result.Semantics);
            System.Console.WriteLine(ObserverGrammar.DumpSemantics(result.Semantics));
            Assert.True(ObserverGrammar.HasSpeciesAndLength(result.Semantics));
            var rawResult = ObserverGrammar.GetSpeciesAndLength(result.Semantics);

            Assert.AreEqual("barracuda", rawResult.Item1.ToLower());
            Assert.AreEqual(52, ObserverGrammar.SpokenNumberToInteger(rawResult.Item2.ToLower()));

        }

        [Test]
        public void GetYellowfinSample()
        {
            string sampleText = "Yellowfin One Two One";
            var result = _recognizer.EmulateRecognize(sampleText, CompareOptions.IgnoreCase);
            Assert.NotNull(result);
            Assert.NotNull(result.Semantics);
            System.Console.WriteLine(ObserverGrammar.DumpSemantics(result.Semantics));
            Assert.True(ObserverGrammar.HasSpeciesAndLength(result.Semantics));
            var rawResult = ObserverGrammar.GetSpeciesAndLength(result.Semantics);

            Assert.AreEqual("yellowfin", rawResult.Item1.ToLower());
            Assert.AreEqual(121, ObserverGrammar.SpokenNumberToInteger(rawResult.Item2.ToLower()));
        }

        [Test]
        public void GetInvalidSample()
        {
            string sampleText = "Pizza Five Five Five";
            var result = _recognizer.EmulateRecognize(sampleText, CompareOptions.IgnoreCase);
            Assert.Null(result);
        }
    }
}
