using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Models
{
    public class DropletInfo
    {
        #region Properties
        [Key]
        public int Id { get; set; }

        [Range(0,5)]
        public int Generation { get; set; }

        public virtual ICollection<DataEntry> Measurements { get; set; }
        #endregion

        public DropletInfo(int id, int generation)
        {
            this.Id = id;
            this.Generation = generation;
        }
    }
}
