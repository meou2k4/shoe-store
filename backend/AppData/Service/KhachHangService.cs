using AppData.DTO;
using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.Repository;

namespace AppData.Service
{
    public class KhachHangService : IKhachHangService
    {
        private readonly IKhachHangRepo _repos;
        private readonly IConfiguration _configuration;
        private readonly IGioHangRepo _GHrepos;
        private readonly IRankRepository _rankrepository;

        public KhachHangService(IKhachHangRepo repos, IConfiguration configuration, IGioHangRepo gHrepos, IRankRepository rankrepository)
        {
            _repos = repos;
            _configuration = configuration;
            _GHrepos = gHrepos;
            _rankrepository = rankrepository;
        }

        public async Task AddKhachhangAsync(KhachhangDTO dto)
        {
            var kh = new Khachhang()
            {
                Ten = dto.Ten,
                Sdt = dto.Sdt,
                Ngaysinh = dto.Ngaysinh,
                Tichdiem = 0,
                Email = dto.Email,
                Password = dto.Password,
                Diemsudung = 0,
                Trangthai = 0,
                Idrank = dto.Idrank,
                Avatar = dto.Avatar,
                Gioitinh = dto.Gioitinh,
                Ngaytaotaikhoan = dto.Ngaytaotaikhoan

            };
            var ranks = await _rankrepository.GetAllRanksAsync();
            var selectedRank = ranks.FirstOrDefault(r => kh.Tichdiem >= r.MinMoney && kh.Tichdiem <= r.MaxMoney && r.Trangthai == 0);

            if (selectedRank == null)
            {
                throw new Exception("Không tìm thấy Rank phù hợp với Tích điểm.");
            }
            kh.Idrank = selectedRank.Id;
            await _repos.AddAsync(kh);
            if (dto.Password != null)
            {

                var gh = new Giohang()
                {
                    Soluong = 0,
                    Idkh = kh.Id
                };

                await _GHrepos.AddAsync(gh);
            }
           
        }

        public async Task<bool> ChangePasswordAsync(DoimkKhachhang changePasswordDto)
        {
            var doi = await _repos.GetByEmailAsync(changePasswordDto.Email);
            if (doi == null)
            {
                throw new Exception("Không tìm thấy nhân viên với email này.");
            }

            // Kiểm tra mật khẩu cũ
            if (doi.Password != changePasswordDto.OldPassword)
            {
                throw new Exception("Mật khẩu cũ không đúng.");
            }

            // Cập nhật mật khẩu mới
            doi.Password = changePasswordDto.NewPassword;
            await _repos.UpdateAsync(doi);

            return true;
        }

        public async Task DeleteKhachhangAsync(int id)
        {
            await _repos.DeleteAsync(id);
        }

        public async Task<IEnumerable<Khachhang>> GetAllKhachhangsAsync()
        {
            var a = await _repos.GetAllAsync();
            return a.Select(x => new Khachhang()
            {
                Id = x.Id,
                Ten = x.Ten,
                Sdt = x.Sdt,
                Ngaysinh = x.Ngaysinh,
                Tichdiem = x.Tichdiem,
                Email = x.Email,
                Password = x.Password,
                Diemsudung = x.Diemsudung,
                Trangthai = x.Trangthai,
                Idrank = x.Idrank,
                Avatar = x.Avatar,
                Gioitinh = x.Gioitinh,
                Ngaytaotaikhoan = x.Ngaytaotaikhoan
            });
        }

        public async Task<KhachhangDTO> GetKhachhangByIdAsync(int id)
        {
            var x = await _repos.GetByIdAsync(id);
            return new KhachhangDTO()
            {
                Id =x.Id,
                Ten = x.Ten,
                Sdt = x.Sdt,
                Ngaysinh = x.Ngaysinh,
                Tichdiem = x.Tichdiem,
                Email = x.Email,
                Password = x.Password,
                Diemsudung = x.Diemsudung,
                Trangthai = x.Trangthai,
                Idrank = x.Idrank,
                Avatar = x.Avatar,
                Gioitinh = x.Gioitinh
            };
        }


        public string GenerateOtp()
        {
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            return otp;
        }
        public async Task<IEnumerable<KhachhangDTO>> TimKiemAsync(string search)
        {
            var a = await _repos.TimKiemAsync(search);
            return a.Select(x => new KhachhangDTO()
            {
                Ten = x.Ten,
                Sdt = x.Sdt,
                Ngaysinh = x.Ngaysinh,
                Tichdiem = x.Tichdiem,
                Email = x.Email,
                Password = x.Password,
                Diemsudung = x.Diemsudung,
                Trangthai = x.Trangthai,
                Idrank = x.Idrank,
                Avatar = x.Avatar,
                Gioitinh = x.Gioitinh
            });
        }

        public async Task UpdateKhachhangAsync(int id, KhachhangDTO dto)
        {
            var a = await _repos.GetByIdAsync(id);
            if (a == null) throw new KeyNotFoundException("Khách hàng không tồn tại.");

            a.Ten = dto.Ten ?? a.Ten;
            a.Sdt = dto.Sdt ?? a.Sdt;
            a.Ngaysinh = dto.Ngaysinh;
            a.Email = dto.Email ?? a.Email;
            a.Avatar = dto.Avatar;
            a.Gioitinh = dto.Gioitinh;
            a.Trangthai = dto.Trangthai;

            await _repos.UpdateAsync(a);
        }

        public async Task UpdateThongTinKhachhangAsync(int id, KhachhangDTO dto)
        {
            var a = await _repos.GetByIdAsync(id);
            if (a == null) throw new KeyNotFoundException("Khách hàng không tồn tại.");

            a.Ten = dto.Ten ?? a.Ten;
            a.Sdt = dto.Sdt ?? a.Sdt;
            a.Ngaysinh = dto.Ngaysinh;
            a.Tichdiem = a.Tichdiem;
            a.Email = dto.Email ?? a.Email;
            a.Password = a.Password;
            a.Diemsudung = a.Diemsudung;
            a.Trangthai = a.Trangthai;
            a.Idrank = a.Idrank;
            a.Avatar = dto.Avatar;
            a.Gioitinh = dto.Gioitinh;

            await _repos.UpdateAsync(a);
        }


        public async Task<KhachhangDTO> FindByEmailAsync(string email)
        {

            var dto = await _repos.GetByEmailAsync(email);
            if (dto == null)
                return null;

            return new KhachhangDTO
            {

                Ten = dto.Ten,
                Sdt = dto.Sdt,
                Ngaysinh = dto.Ngaysinh,
                Tichdiem = dto.Tichdiem,
                Email = dto.Email,
                Password = dto.Password,
                Diemsudung = dto.Diemsudung,
                Trangthai = dto.Trangthai,
                Idrank = dto.Idrank,
                Avatar = dto.Avatar,
                Gioitinh = dto.Gioitinh
            };

        }

        async Task<(bool isSent, object otp)> IKhachHangService.SendOtpAsync(string email)
        {
            try
            {
                // Kiểm tra cấu hình
                var senderEmail = _configuration["EmailSettings:SenderEmail"]
                    ?? throw new InvalidOperationException("Sender email not configured");
                var senderPassword = _configuration["EmailSettings:SenderPassword"]
                    ?? throw new InvalidOperationException("Sender password not configured");
                var smtpServer = _configuration["EmailSettings:SmtpServer"]
                    ?? throw new InvalidOperationException("SMTP server not configured");
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]
                    ?? throw new InvalidOperationException("SMTP port not configured"));

                var otp = GenerateOtp();
                var subject = "Mã OTP xác thực quên mật khẩu";
                var body = $"Mã OTP của bạn là: {otp}. Vui lòng không chia sẻ mã này với bất kỳ ai.";

                using var client = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage(senderEmail, email, subject, body);
                using var message = mailMessage;
                await client.SendMailAsync(message);

                return (true, otp);
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây
                Console.WriteLine($"Error sending email: {ex.Message}");
                return (false, string.Empty);
            }
        }
    }
}
