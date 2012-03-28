// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace WpfObserverRecognizer
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
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using Spc.Ofp.ObserverRecognizer;
    
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechRecognitionEngine _recognizer;
        private Dispatcher _lengthLabelDispatcher;
        private Dispatcher _speciesLabelDispatcher;
        private Dispatcher _lengthValueDispatcher;
        private Dispatcher _speciesValueDispatcher;
        
        public MainWindow()
        {
            InitializeComponent();

            // Prep Speech Recognition
            // TODO Load culture info from App.Config or something in the GUI
            this._recognizer = new SpeechRecognitionEngine(CultureInfo.CreateSpecificCulture("en-US"));

            this._recognizer.SetInputToDefaultAudioDevice();
            this._recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(speechRecognizer_SpeechRecognized);
            this._recognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(speechRecognizer_SpeechRecognitionRejected);

            // Load grammar, showing a messagebox and exiting if it fails.
            try
            {
                this._recognizer.LoadGrammar(ObserverGrammar.GetGrammar());
            }
            catch (Exception ex)
            {
                string title = "Grammar Load Failure";
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Stop);
                this.Close();
            }

            // Set up dispatchers for access from the recognition thread
            _lengthLabelDispatcher = lblLength.Dispatcher;
            _lengthValueDispatcher = txtLength.Dispatcher;
            _speciesLabelDispatcher = lblSpecies.Dispatcher;
            _speciesValueDispatcher = txtSpeciesCode.Dispatcher;

            ThreadStart start = delegate()
            {
                while (true)
                {
                    this._recognizer.Recognize();
                }
            };

            new Thread(start).Start();

        }

        private void ResetBackgrounds()
        {
            _lengthLabelDispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        lblLength.Background = System.Windows.Media.Brushes.White;
                    }
                )
            );

            _lengthValueDispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        txtLength.Background = System.Windows.Media.Brushes.White;
                        txtLength.Text = String.Empty;
                    }
                )
            );

            _speciesLabelDispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        lblSpecies.Background = System.Windows.Media.Brushes.White;
                    }
                )
            );

            _speciesValueDispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        txtSpeciesCode.Background = System.Windows.Media.Brushes.White;
                        txtSpeciesCode.Text = String.Empty;
                    }
                )
            );

        }

        private void speechRecognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            ResetBackgrounds();
            ShowError();
        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            ResetBackgrounds();
            if (null == e || null == e.Result || null == e.Result.Semantics || !ObserverGrammar.HasSpeciesAndLength(e.Result.Semantics))
            {
                ShowError();
                return;
            }

            var rawResults = ObserverGrammar.GetSpeciesAndLength(e.Result.Semantics);
            string speciesCode = ObserverGrammar.SpeciesNameToCode(rawResults.Item1.ToLower());
            int? length = ObserverGrammar.SpokenNumberToInteger(rawResults.Item2.ToLower());

            if (length.HasValue && !String.Empty.Equals(speciesCode))
            {
                ShowValues(speciesCode, length.Value.ToString());
            }
            else
            {
                ShowError();
            }

        }

        private void ShowValues(string speciesCode, string lengthValue)
        {
            _lengthValueDispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        txtLength.Text = lengthValue;
                    }
                )
            );

            _speciesValueDispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        txtSpeciesCode.Text = speciesCode;
                    }
                )
            );
        }

        private void ShowError()
        {
            _lengthLabelDispatcher.Invoke(
               DispatcherPriority.Normal,
               new Action(
                   delegate()
                   {
                       lblLength.Background = System.Windows.Media.Brushes.Red;
                   }
               )
           );

            _lengthValueDispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        txtLength.Text = "Error";
                    }
                )
            );

            _speciesLabelDispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        lblSpecies.Background = System.Windows.Media.Brushes.Red;
                    }
                )
            );

            _speciesValueDispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        txtSpeciesCode.Text = "Error";
                    }
                )
            );
            
        }

        protected override void OnClosed(EventArgs e)
        {
            if (null != _recognizer)
            {
                try
                {
                    _recognizer.Dispose();
                }
                finally { }
            }
            base.OnClosed(e);
        }
    }
}
