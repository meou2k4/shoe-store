using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AppData.Models
{
    public class Color
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Tenmau { get; set; }

        public string Mamau {  get; set; }

        public int Trangthai { get; set; }

        public virtual ICollection<Sanphamchitiet> Sanphamchitiets { get; set; }

    }
}
