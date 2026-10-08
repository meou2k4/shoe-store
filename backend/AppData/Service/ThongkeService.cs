using AppData.DTO;
using AppData.IRepository;
using AppData.IService;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Service
{
    public class ThongkeService : IThongkeService
    {
        private readonly IThongKeSanPhamRepo _thongKeSanPhamRepo;

        public ThongkeService(IThongKeSanPhamRepo thongKeSanPhamRepo)
        {
            _thongKeSanPhamRepo = thongKeSanPhamRepo;
        }

        public async Task<List<Khachhang>> GetActiveCustomersAsync()
        {
           return await _thongKeSanPhamRepo.GetActiveCustomersAsync();
        }

        public async Task<List<CustomerStatisticsDTO>> GetTop5CustomersAsync(DateTime month)
        {
            return await _thongKeSanPhamRepo.GetTop5CustomersAsync(month);
        }

        public async Task<List<TopSellingProductDTO>> GetTopSellingProductsByTimeAsync(DateTime startDate, DateTime endDate)
        {
            return await _thongKeSanPhamRepo.GetTopSellingProductsByTimeAsync(startDate, endDate);
        }
    }
}
