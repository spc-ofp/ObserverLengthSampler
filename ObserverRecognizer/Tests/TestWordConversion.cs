// -----------------------------------------------------------------------
// <copyright file="TestWordConversion.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for converting number words into numeric values.
    /// </summary>
    [TestFixture]
    public class TestWordConversion
    {
        private static IList<Tuple<string, int>> TestCases = new List<Tuple<string, int>>()
        {
            Tuple.Create("one two zero", 120),
            Tuple.Create("one zero", 10),
            Tuple.Create("four one", 41),
            Tuple.Create("four", 4),
            Tuple.Create("seven two", 72),
            Tuple.Create("eight nine", 89),
            Tuple.Create("three five two", 352),
            Tuple.Create("six six", 66)
        };
        
        [Test]       
        public void GetNumbers()
        {
            foreach (var entry in TestCases)
            {
                int? result = ObserverGrammar.SpokenNumberToInteger(entry.Item1);
                Assert.True(result.HasValue);
                Assert.AreEqual(entry.Item2, result.Value);
            }
        }
    }
}
