using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
    public class SizeDTO
    {
        public int Id { get; set; }

        public int Sosize { get; set; }

        public int Trangthai { get; set; }
        public bool IsUsedInProduct { get; set; }
    }
}
