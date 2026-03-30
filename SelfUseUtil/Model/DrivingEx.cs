using NewLife.Map.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Model
{
    public class DrivingEx : Driving
    {
        /// <summary>路线步骤</summary>
        public IList<Step> Steps { get; set; } = new List<Step>();
    }

    public class Step
    {
        public string instruction { get; set; }
        public string orientation { get; set; }
        public string road_name { get; set; }
        public string step_distance { get; set; }
        public Cost cost { get; set; }
        public string polyline { get; set; }
    }

    public class Path
    {
        public string distance { get; set; }
        public string restriction { get; set; }
        public List<Step> steps { get; set; }
        public Cost cost { get; set; }
    }

    public class Route
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public string taxi_cost { get; set; }
        public List<Path> paths { get; set; }
    }

    public class Root
    {
        public Route route { get; set; }
    }

    public class Cost
    {
        public string duration { get; set; }
        public string tolls { get; set; }
        public string toll_distance { get; set; }
        public string toll_road { get; set; }
        public string traffic_lights { get; set; }
    }
}
