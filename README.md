Observer Length Sampler
===================

The Observer Length Sampler solution is a library and a pair
of applications for collecting species length frequency information via
voice recognition.  The target speech API is System.Speech in version
4.0 of the CLR.

The library is contained within the "ObserverRecognizer" project
and contains an SRGS grammar for recognizing numbers and approximately 40
species common to commercial fishing in the southwestern Pacific.

The grammar's primary number recognition mode is configured for
capturing one to three discrete digits that make up a length
measurement.  There is an additional experimental grammar rule (#Number)
for capturing numbers from 1 to 999 as naturally spoken.
See ObserverGrammar.xml for more details.

A command line application for examining recognition data is available
in the "CliObserverRecognizer" project.  It's primary purpose is to
enable debugging outside of the additional constraints of WPF and threading.

The final application is "WpfObserverRecognizer", a WPF application for
demonstrating feasibility of System.Speech for the purpose at hand.

The following tasks have yet to be completed:

+ Fixing the brutally ugly architecture for communicating between the UI and the recognizer.
+ Capturing raw audio samples for offline testing/debugging.
+ Capturing species/length tuples for later scientific analysis



