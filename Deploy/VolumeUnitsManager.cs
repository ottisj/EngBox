using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Deploy
{
    public class VolumeUnitsManager
    {
        public ObservableCollection<EngBox.UnitProperties> VolumeUnits { get; set; }
        public VolumeUnitsManager()
        {
            VolumeUnits = new ObservableCollection<EngBox.UnitProperties>();

            VolumeUnits.Add(new EngBox.UnitProperties(1, 0, "m3"));
            VolumeUnits.Add(new EngBox.UnitProperties(0.001, 0, "l"));
            VolumeUnits.Add(new EngBox.UnitProperties(1E-6, 0, "cm3"));
            VolumeUnits.Add(new EngBox.UnitProperties(1/61023.7, 0, "in3"));
            VolumeUnits.Add(new EngBox.UnitProperties(1/35.31, 0, "ft3"));
            VolumeUnits.Add(new EngBox.UnitProperties(1/264.17, 0, "us gal"));
        }
    }
}
