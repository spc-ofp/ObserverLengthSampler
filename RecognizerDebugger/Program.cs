// -----------------------------------------------------------------------
// <copyright file="ObserverGrammar.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace Spc.Ofp.ObserverRecognizer.Cli
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
    using System;
    using System.Globalization;
    using System.Speech.Recognition;
    using System.Speech.Synthesis;
    using Spc.Ofp.ObserverRecognizer;
    
    public class Program
    {
        private SpeechRecognitionEngine _recognizer;
        private SpeechSynthesizer _synthesizer;
        
        static void Main(string[] args)
        {
            Console.Out.WriteLine("Control-C to exit");
            try
            {
                new Program().Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Worse Than Failure!");
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
            }
        }

        public Program()
        {
            this._recognizer = new SpeechRecognitionEngine(CultureInfo.CreateSpecificCulture("en-US"));
            this._synthesizer = new SpeechSynthesizer();
        }

        public void Run()
        {
            this._recognizer.SetInputToDefaultAudioDevice();
            this._recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(speechRecognizer_SpeechRecognized);
            this._recognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(speechRecognizer_SpeechRecognitionRejected);
            //this._recognizer.Enabled = true;
            

            Console.WriteLine("Loading grammar...");
            this._recognizer.LoadGrammar(ObserverGrammar.GetGrammar());
            Console.WriteLine("Grammar loaded");

            Console.WriteLine("Starting recognition engine...");           

            while (true)
            {
                this._recognizer.Recognize();
            }
        }

        private void speechRecognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (null == e.Result)
            {
                Console.Out.WriteLine("Null result in rejection event");
                return;
            }

            Console.Out.WriteLine("Rejection Event Values:");
            Console.Out.WriteLine(ObserverGrammar.DumpSemantics(e.Result.Semantics));
        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (null == e.Result)
            {
                Console.Error.WriteLine("Recogized speech with null Result.  Huh!?");
                return;
            }

            Console.Out.WriteLine("Recognition Event Values:");
            Console.Out.WriteLine(ObserverGrammar.DumpSemantics(e.Result.Semantics));

            if (!ObserverGrammar.HasSpeciesAndLength(e.Result.Semantics))
                return;

            Tuple<string, string> speciesAndLength = ObserverGrammar.GetSpeciesAndLength(e.Result.Semantics);


            Console.Out.WriteLine("Got species '{0}' and length {1}", speciesAndLength.Item1, speciesAndLength.Item2);

        }
    }
}
