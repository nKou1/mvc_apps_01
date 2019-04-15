using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mvc_apps_01.Models
{
    public class TmDbComingSoon
    {
        public int ID { get; set; }
        [Required]
        [Range(0, 9999999)]
        public int MovieId { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        [Range(0, 999.999)]
        public decimal Popularity { get; set; }
        [Url]
        [StringLength(100)]
        public string BackdropPath { get; set; }
        [Url]
        [StringLength(100)]
        public string PosterPath { get; set; }
        [DataType(DataType.Date)]
        public DateTime UpdateDate { get; set; }
    }
}
