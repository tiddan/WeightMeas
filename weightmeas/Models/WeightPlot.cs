using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using DataAnnotationsExtensions;

namespace weightmeas.Models
{
    public class WeightPlot
    {
        [Key]
        public Guid PlotId { get; set; }

        [Date]
        [Required]
        public DateTime PlotStamp { get; set; }

        [Numeric]
        [Required]
        public double Weight { get; set; }

        [Numeric]
        public double? FatPercent { get; set; }

        [Numeric]
        public double? MuscularWeight { get; set; }

        [Numeric]
        public double? WaterWeight { get; set; }

        [DataType(DataType.Text)]
        public string Comment { get; set; }

        [NotMapped]
        public string PrivateToken { get; set; }

    }
}