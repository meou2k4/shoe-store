using AppData;
using AppData.DTO;
using AppData.Dto_Admin;
using AppData.IService;
using AppData.IService_Admin;
using AppData.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiamgiaController : ControllerBase
	{
        private readonly IGiamGiaService _Service;
		private readonly AppDbContext _context;

		private readonly IGiamgiaService _service;
		private readonly ILogger<GiamgiaController> _logger;

		public GiamgiaController(IGiamGiaService Service, AppDbContext context, IGiamgiaService service, ILogger<GiamgiaController> logger)
		{
			_Service = Service;
			_context = context;
			_service = service;
			_logger = logger;
		}

		[HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _Service.GetAllAsync();
            return Ok(result.Select(gg => new
            {
                gg.Id,
                gg.Mota,
                Donvi = gg.Donvi == 0 ? "VNĐ" : "%",
                gg.Giatri,
                gg.Ngaybatdau,
                gg.Ngayketthuc,
                gg.Soluong,
                gg.Trangthai,
                Trangthaistring = gg.Trangthai switch
                {
                    0 => "Đang phát hành",
                    1 => "Chuẩn bị phát hành",
                    2 => "Dừng phát hành",
                    _ => "Không xác định"
                }
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var giamgia = await _Service.GetByIdAsync(id);
                return Ok(new
                {
                    giamgia.Id,
                    giamgia.Mota,
                    Donvi = giamgia.Donvi == 0 ? "VNĐ" : "%",
                    giamgia.Giatri,
                    giamgia.Ngaybatdau,
                    giamgia.Ngayketthuc,
                    giamgia.Soluong,
                    giamgia.Trangthai,
                    Trangthaistring = giamgia.Trangthai switch
                    {
                        0 => "Đang phát hành",
                        1 => "Chuẩn bị phát hành",
                        2 => "Dừng phát hành",
                        _ => "Không xác định"
                    }
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tìm thấy mã giảm giá.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppData.DTO.GiamgiaDTOadd dto)
        {
            await _Service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        [HttpPost("AddRankToGiamgia")]
        public async Task<IActionResult> AddRankToGiamgia([FromBody] AppData.DTO.GiamgiaDTO dto)
        {
            try
            {
                await _Service.AddRankToGiamgia(dto);
                return Ok("Rank added to Giảm Giá thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AppData.DTO.GiamgiaDTOadd dto)
        {
            await _Service.UpdateAsync(id, dto);
            return Ok(new { Message = "Cập nhật thành công" }); // Trả về 200 và message rõ ràng
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _Service.DeleteAsync(id);
            return NoContent();
        }
		[HttpGet("search/Admin")]
		public async Task<IActionResult> SearchGiamgia([FromQuery] string description)
		{


			var result = await _service.SearchByDescriptionAsync(description);

			if (result == null || result.Count == 0)
			{
				return NotFound("Không tìm thấy giảm giá với mô tả này");
			}

			return Ok(result);
		}

		[HttpPut("change-status/{id}/Admin")]
		public async Task<IActionResult> ChangeTrangthai(int id)
		{
			try
			{
				await _service.ChangeTrangthaiAsync(id);
				return Ok("Trạng thái giảm giá đã được cập nhật");
			}
			catch (KeyNotFoundException)
			{
				return NotFound("Giảm giá không tồn tại");
			}
		}
		[HttpDelete("{id}/Admin")]
		public async Task<IActionResult> DeleteGiamgia(int id)
		{
			// Call service to delete the Giamgia
			var result = await _service.DeleteGiamgiaAsync(id);

			if (!result)
			{

				return BadRequest("Không thể xóa voucher vì nó đã được sử dụng");
			}

			return NoContent();
		}
		[HttpPut("{id}/Admin")]
		public async Task UpdateGiamgiaRankAsync(int id, Giamgia_RankDTO dto)
		{
			var entity = await _context.giamgias.FindAsync(id);
			if (entity == null)
				throw new KeyNotFoundException("Không tìm thấy mã giảm giá.");

			// Cập nhật các trường
			entity.Mota = dto.Mota;
			entity.Donvi = dto.Donvi;
			entity.Soluong = dto.Soluong;
			entity.Giatri = dto.Giatri;
			entity.Ngaybatdau = dto.Ngaybatdau;
			entity.Ngayketthuc = dto.Ngayketthuc;
			entity.Trangthai = dto.Trangthai;

			// Có thể xử lý cập nhật rank riêng hoặc ở đây nếu cần

			await _context.SaveChangesAsync();
		}

		[HttpPost("AddRankToGiamgia/Admin")]
		public async Task<IActionResult> AddRankToGiamgia([FromBody] AppData.Dto_Admin.Giamgia_RankDTO dto)
		{
			if (dto == null)
			{
				return BadRequest("Dữ liệu không hợp lệ.");
			}

			try
			{
				await _service.AddRankToGiamgia(dto);
				return Ok(new { message = "Thêm rank thành công." });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message); // Trả về lỗi nếu validation không hợp lệ
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Đã có lỗi xảy ra.", details = ex.Message });
			}
		}

		[HttpGet("{id}/Admin")]
		public async Task<IActionResult> GetByIdAdmin(int id)
		{
			try
			{
				var giamgia = await _service.GetByIdAsync(id);
				return Ok(new
				{
					giamgia.Mota,
					giamgia.Donvi,
					giamgia.Giatri,
					giamgia.Soluong,
					giamgia.Ngaybatdau,
					giamgia.Ngayketthuc,
					giamgia.Trangthai
				});
			}
			catch (KeyNotFoundException)
			{
				return NotFound("Không tìm thấy mã giảm giá.");
			}
		}
		[HttpGet("{id}/Ranks/Admin")]
		public IActionResult GetRanksByVoucherId(int id)
		{
			var ranks = _context.giamgia_Ranks
			.Where(gr => gr.IDgiamgia == id)
								.Join(_context.ranks, gr => gr.Idrank, r => r.Id,
									  (gr, r) => new { r.Tenrank })
								.Select(x => x.Tenrank)
								.ToList();

			if (ranks == null || !ranks.Any())
				return NotFound("Không tìm thấy rank cho voucher này");

			return Ok(ranks);
		}
		[HttpGet("Admin")]
		public async Task<IActionResult> GetAllAdmin()
		{
			var result = await _service.GetAllAsync();
			return Ok(result.Select(gg => new
			{
				gg.Id,
				gg.Mota,
				gg.Donvi,
				gg.Soluong,
				gg.Giatri,
				gg.Ngaybatdau,
				gg.Ngayketthuc,
				gg.Trangthai,
			}));
		}
		[HttpPut("update-trangthai/Admin")]
		public async Task<IActionResult> StartUpdatingTrangthai()
		{
			// Gọi phương thức để bắt đầu cập nhật trạng thái liên tục
			await _service.UpdateTrangthaiContinuouslyAsync();

			return Ok("Đang cập nhật trạng thái...");
		}
		[HttpGet("vouchers-by-customer/{customerId}/admin")]     // route gọn hơn
		public async Task<ActionResult<IEnumerable<AppData.DTO.GiamgiaDTO>>>
		GetVouchersByCustomerId_Admin(int customerId)     // tên khác
		{
			var vouchers = await _service.GetVouchersByCustomerIdAsync(customerId);
			return vouchers.Any() ? Ok(vouchers)
								  : NotFound("Không tìm thấy voucher phù hợp.");
		}


	}
}
