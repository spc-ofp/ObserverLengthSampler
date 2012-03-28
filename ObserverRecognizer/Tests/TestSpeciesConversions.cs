// -----------------------------------------------------------------------
// <copyright file="TestSpeciesConversions.cs" company="Secretariat of the Pacific Community">
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
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for converting species names to FAO species codes.
    /// </summary>
    [TestFixture]
    public class TestSpeciesConversions
    {
        private static IList<Tuple<string, string>> Samples = new List<Tuple<string, string>>()
        {
            Tuple.Create("YellOwFin", "YFT"),
            Tuple.Create("YELLOWFIN", "YFT"),
            Tuple.Create("Man O War", "PSC"),
            Tuple.Create("KawaKawa", "KAW"),
            Tuple.Create("sailfish", "SFA"),
            Tuple.Create("Hammerhead Shark", "SPN"),
            Tuple.Create("BigEye TreVally", "CXS"),
        };

        [Test]
        public void RunSamplesList()
        {
            foreach (var sample in Samples)
            {
                var result = ObserverGrammar.SpeciesNameToCode(sample.Item1);
                Assert.AreEqual(sample.Item2, result);
            }
        }
    }
}
