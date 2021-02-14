using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Deploy
{
    public class LengthUnitManager
    {
        /// <summary>
        /// Contains definition of length units. First item is always SI value. 
        /// </summary>
        public ObservableCollection<EngBox.UnitProperties> LengthUnits { get; set;}
        public LengthUnitManager()
        {
            LengthUnits = new ObservableCollection<EngBox.UnitProperties>();

            LengthUnits.Add(new EngBox.UnitProperties(1, 0, "m"));
            LengthUnits.Add(new EngBox.UnitProperties(0.001, 0, "mm"));
            LengthUnits.Add(new EngBox.UnitProperties(0.01, 0, "cm"));
            LengthUnits.Add(new EngBox.UnitProperties(0.1, 0, "dm"));
            LengthUnits.Add(new EngBox.UnitProperties(1000, 0, "km"));
            LengthUnits.Add(new EngBox.UnitProperties(0.3048, 0, "ft"));
            LengthUnits.Add(new EngBox.UnitProperties(0.0254, 0, "in"));
            LengthUnits.Add(new EngBox.UnitProperties(0.9144, 0, "yd"));
            LengthUnits.Add(new EngBox.UnitProperties(1609.344, 0, "mile"));
            LengthUnits.Add(new EngBox.UnitProperties(2.54E-5, 0, "mil"));
        }
        
            
    }
}
