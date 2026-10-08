using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AppData.DTO
{
    public class ColorDTO
    {
        public int Id { get; set; }

        public string Tenmau { get; set; }

        public string Mamau {  get; set; }

        public int Trangthai { get; set; }

        public bool IsUsedInProduct { get; set; }

    }
}
