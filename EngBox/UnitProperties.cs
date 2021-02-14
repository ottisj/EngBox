using System;
using System.Collections.Generic;
using System.Text;

namespace EngBox
{
    /// <summary>
    /// Keeps basic information about the unit. 
    /// </summary>
    public class UnitProperties
    {
        public UnitProperties()
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="A">A parameter in equation SI = A*Unit + B</param>
        /// <param name="B"> B parameter in equation SI = A*Unit + B</param>
        /// <param name="Abbr">Unit abbreviation</param>
        public UnitProperties(double A, double B, string Abbr)
        {
            this.A = A;
            this.B = B;
            this.Abbreviation = Abbr;
        }
        /// <summary>
        /// Unit abbreviation like kg.m-3 etc.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// A parameter in equation SI = A*Unit + B
        /// For instance for litres A = 0.001, B = 0 and SI[m3] = 0.001*litres + 0
        /// </summary>
        public double A { get; set; } = 1;
        /// <summary>
        /// B parameter in equation SI = A*Unit + B
        /// </summary>
        public double B { get; set; }

        public override string ToString()
        {
          return  Abbreviation;
        }
    }
}
