using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModel
{
    public class ThuocTinhViewModel
    {
        public int IDThuocTinh { get; set; }
        public string NameThuocTinh { get; set; }
        public List<ThuocTinhChiTietViewModel> thuocTinhChiTietViewModels { get; set; }
    }
    public class ThuocTinhChiTietViewModel
    {
        public int idspct { get; set; }
        public string TenThucTinhChiTiet { get; set; }
    }
}
     