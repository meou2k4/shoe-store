app.controller("VoucherController", function ($http, $scope, $timeout, $q) {
  $scope.vouchers = [];
  $scope.filteredVouchers = [];
  $scope.ranks = [];
  $scope.newVoucher = {};
  $scope.editingVoucher = null;
  $scope.message = "";
  $scope.itemsPerPage = 5; // Số lượng voucher mỗi trang
  $scope.currentPage = 1;

  // Lấy danh sách voucher
  function loadVouchers() {
    $http.get("https://localhost:7196/api/Giamgia").then(function (res) {
        // Lọc chỉ các voucher có trạng thái từ 0 đến 3 (3 trở xuống)
        $scope.vouchers = res.data.filter(function(voucher) {
            return voucher.trangthai >= 0 && voucher.trangthai <= 3;
        });
        $scope.applyFilters(); // Áp dụng bộ lọc sau khi tải dữ liệu
    }).catch(function(error) {
        console.error('Lỗi khi tải dữ liệu voucher:', error);
        alert('Không thể tải dữ liệu voucher');
    });
}

  // Lấy danh sách rank
  function loadRanks() {
    $http.get("https://localhost:7196/api/Ranks").then(function (res) {
      $scope.ranks = res.data;
      renderRanks(res.data);
    });
  }

  function renderRanks(ranks) {
    var container = document.getElementById("rank_coupon");
    container.innerHTML = ""; // reset container

    ranks.forEach(function (rank) {
      var label = document.createElement("label");
      label.style.display = "block"; // mỗi checkbox xuống dòng riêng

      var checkbox = document.createElement("input");
      checkbox.type = "checkbox";
      checkbox.value = rank.id;
      checkbox.name = "rank"; // hoặc "rank[]" nếu bạn submit form nhiều checkbox
      checkbox.id = "rank_" + rank.id;

      label.appendChild(checkbox);
      label.appendChild(document.createTextNode(" " + rank.tenrank));

      container.appendChild(label);
    });
  }
  document
    .getElementById("saveVoucherBtn")
    .addEventListener("click", function () {
      var mota = document.getElementById("mota").value;
      var donvi = parseInt(document.getElementById("donvi").value);
      var giatri = parseFloat(document.getElementById("giatri").value);
      var soluong = parseInt(document.getElementById("soluong").value);
      var ngaybatdau = document.getElementById("ngaybatdau").value;
      var ngayketthuc = document.getElementById("ngayketthuc").value;
      var now = new Date();

      if (mota.length === 0 || mota.length > 50) {
        alert("Mô tả không được để trống và tối đa 50 ký tự.");
        return;
      }

      // Validate Giá trị
      if (isNaN(giatri)) {
          alert("Vui lòng nhập giá trị hợp lệ.");
          return;
      }

      if (donvi === 0) { // VNĐ
          if (giatri < 1000 || giatri > 9999999) {
              alert("Giá trị VNĐ phải từ 1.000 đến 9.999.999.");
              return;
          }
      } else if (donvi === 1) { // %
          if (giatri < 1 || giatri > 100) {
              alert("Giá trị phần trăm phải từ 1 đến 100.");
              return;
          }
      }

      // Validate Số lượng
      if (isNaN(soluong) || soluong < 10 || soluong > 999) {
          alert("Số lượng phải từ 10 đến 999.");
          return;
      }

      if (!ngaybatdau || ngaybatdau.trim() === "") {
        alert("Ngày bắt đầu không được để trống.");
        return;
    }

      // Validate ngày
      if (ngaybatdau < now) {
          alert("Ngày bắt đầu không được nhỏ hơn hiện tại.");
          return;
      }
      
    
        if (!ngayketthuc || ngayketthuc.trim() === "") {
          alert("Ngày kết thúc không được để trống.");
          return;
      }

      if (ngayketthuc < now) {
          alert("Ngày kết thúc không được nhỏ hơn hiện tại.");
          return;
      }

      if (formatDateTimeLocal(ngayketthuc) < formatDateTimeLocal(ngaybatdau)) {
          alert("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
          return;
      }

      // Lấy danh sách rank đã chọn
      var rankCheckboxes = document.querySelectorAll("#rank_coupon input[type='checkbox']:checked");
      var rankNames = [];

      rankCheckboxes.forEach(function (checkbox) {
        var rankId = checkbox.id.replace('rank_', ''); // lấy ra id thôi (bỏ prefix)

        // Tìm trong ranks list (biến ranks AngularJS) cái rank có id tương ứng
        var selectedRank = $scope.ranks.find(function (rank) {
          return rank.id == rankId;
        });

        if (selectedRank) {
          rankNames.push(selectedRank.tenrank);
        }
      });


      if (rankNames.length === 0) {
        alert("Vui lòng chọn ít nhất một rank!");
        return;
      }

      // Chuẩn bị dữ liệu để gửi API
      var voucherData = {
        Mota: mota,
        Donvi: donvi,
        Giatri: giatri,
        Soluong: soluong,
        Ngaybatdau: ngaybatdau,
        Ngayketthuc: ngayketthuc,
        RankNames: rankNames
      };

      // Gửi 1 lần duy nhất
      fetch("https://localhost:7196/api/Giamgia/AddRankToGiamgia", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(voucherData),
      })
        .then(response => {
          if (!response.ok) {
            throw new Error("Tạo voucher thất bại");
          }
          return response.text();
        })
        .then(data => {
          alert("Thêm mã giảm giá thành công!");
          
          // Reload trang sau khi cập nhật thành công
          window.location.reload();  // Reload lại toàn bộ trang
        })
        .catch(error => {
          console.error("Lỗi khi tạo voucher:", error);
          alert("Có lỗi xảy ra khi tạo mã giảm giá.");
        });
    });

  // Áp dụng bộ lọc
  $scope.applyFilters = function () {
    $scope.filteredVouchers = $scope.vouchers.filter(function (v) {
      return (
        (!$scope.statusFilter || v.trangThai === $scope.statusFilter) &&
        (!$scope.typeFilter || v.loai === $scope.typeFilter) &&
        (!$scope.rankFilter || v.rankId === $scope.rankFilter)
      );
    });
    $scope.currentPage = 1; // Reset trang khi thay đổi bộ lọc
  };

  // Chọn voucher để chỉnh sửa
  // Khi bấm nút sửa voucher
  $scope.editVoucher = function (voucher) {
    $scope.editingVoucher = angular.copy(voucher);
    

    // Chuyển ngày về định dạng phù hợp
    $scope.editingVoucher.ngaybatdau = formatDateTimeLocal(voucher.ngaybatdau);
    $scope.editingVoucher.ngayketthuc = formatDateTimeLocal(voucher.ngayketthuc);

    // Load all ranks
    $http.get("https://localhost:7196/api/Ranks").then(function (res) {
      $scope.allRanks = res.data.map(function (rank) {
        // Khởi tạo selectedEdit = false cho tất cả rank
        return angular.extend({}, rank, { selectedEdit: false });
      });

      // Get voucher-rank relationships
      $http.get("https://localhost:7196/api/Giamgia_rank").then(function (response) {
        console.log('Data từ Giamgia_rank:', response.data); // Debug

        // Lưu toàn bộ dữ liệu quan hệ voucher-rank vào $scope.voucherRanks
        $scope.voucherRanks = response.data.filter(function (item) {
          // Kiểm tra cả null/undefined
          return item && item.iDgiamgia === $scope.editingVoucher.id;
        });

        console.log('voucherRanks đã lọc:', $scope.voucherRanks); // Debug

        // Lấy danh sách ID rank áp dụng
        var appliedRankIds = $scope.voucherRanks.map(function (item) {
          return item && item.idrank; // Kiểm tra null/undefined
        }).filter(function (id) {
          return id !== undefined && id !== null; // Lọc bỏ undefined/null
        });

        console.log('appliedRankIds:', appliedRankIds); // Debug

        // Đánh dấu các rank được chọn
        $scope.allRanks.forEach(function (rank) {
          rank.selectedEdit = appliedRankIds.includes(rank.id);
        });

        // Hiển thị modal
        var modalElement = document.getElementById('editVoucherModal');
        var modal = new bootstrap.Modal(modalElement);
        modal.show();
      });
    });
  };

  // Hàm kiểm tra rank có được chọn không
  $scope.isRankSelected = function (rankId) {
    if (!$scope.voucherRanks) return false;
    return $scope.voucherRanks.some(function (item) {
      return item.iDrank === rankId;
    });
  };


  function formatDateTimeLocal(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    const dd = ('0' + date.getDate()).slice(-2);
    const MM = ('0' + (date.getMonth() + 1)).slice(-2);
    const yyyy = date.getFullYear();
    const HH = ('0' + date.getHours()).slice(-2);
    const mm = ('0' + date.getMinutes()).slice(-2);
    return `${dd}/${MM}/${yyyy} ${HH}:${mm}`;
  }

  function isValidDate(dateString) {
    const date = new Date(dateString);
    return !isNaN(date.getTime());  // Trả về true nếu ngày hợp lệ, ngược lại false
}


  $scope.updateVoucher = function () {
    var updatedVoucher = angular.copy($scope.editingVoucher);
    delete updatedVoucher.rankIds;
  
    // Validate Mô tả
    if (!updatedVoucher.mota || updatedVoucher.mota.length === 0 || updatedVoucher.mota.length > 50) {
      alert("Mô tả không được để trống và tối đa 50 ký tự.");
      return;
    }
  
    // Validate Giá trị
    var giatri = parseFloat(updatedVoucher.giatri);
    var donvi = updatedVoucher.donvi === "VNĐ" ? 0 : 1;
  
    if (isNaN(giatri)) {
      alert("Vui lòng nhập giá trị hợp lệ.");
      return;
    }
  
    if (donvi === 0 && (giatri < 1000 || giatri > 9999999)) {
      alert("Giá trị VNĐ phải từ 1.000 đến 9.999.999.");
      return;
    }
  
    if (donvi === 1 && (giatri < 1 || giatri > 100)) {
      alert("Giá trị phần trăm phải từ 1 đến 100.");
      return;
    }
  
    // Validate Số lượng
    var soluong = parseInt(updatedVoucher.soluong);
    if (isNaN(soluong) || soluong < 10 || soluong > 999) {
      alert("Số lượng phải từ 10 đến 999.");
      return;
    }
  
    var now = new Date();
    var ngaybatdau = new Date(updatedVoucher.ngaybatdau);
    var ngayketthuc = new Date(updatedVoucher.ngayketthuc);
  
    // Validate Ngày bắt đầu
    if (!updatedVoucher.ngaybatdau || updatedVoucher.ngaybatdau.trim() === "") {
      alert("Ngày bắt đầu không được để trống.");
      return;
    }
  
    if (ngaybatdau < now) {
      alert("Ngày bắt đầu không được nhỏ hơn hiện tại.");
      return;
    }
  
    // Validate Ngày kết thúc
    if (!updatedVoucher.ngayketthuc || updatedVoucher.ngayketthuc.trim() === "") {
      alert("Ngày kết thúc không được để trống.");
      return;
    }
  
    if (ngayketthuc < now) {
      alert("Ngày kết thúc không được nhỏ hơn hiện tại.");
      return;
    }
  
    if (ngayketthuc < ngaybatdau) {
      alert("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
      return;
    }
  
    // Validate Rank
    var newlySelectedRanks = $scope.allRanks.filter(r => r.selectedEdit).map(r => r.id);
    if (newlySelectedRanks.length === 0) {
      alert("Vui lòng chọn ít nhất một Rank!");
      return;
    }
  
    var originalSelectedRanks = $scope.voucherRanks.map(r => r.idrank);
    var ranksToAdd = newlySelectedRanks.filter(id => !originalSelectedRanks.includes(id));
    var ranksToRemove = originalSelectedRanks.filter(id => !newlySelectedRanks.includes(id));
  
    var updateChain = $q.when();
  
    // Xóa các rank không còn chọn
    ranksToRemove.forEach(rankId => {
      updateChain = updateChain.then(() => {
        return $http.delete(`https://localhost:7196/api/Giamgia_rank/idgiamgia/${updatedVoucher.id}/idrank/${rankId}`)
          .catch(err => {
            console.error('Lỗi khi xóa rank:', err);
            return $q.resolve(); // Bỏ qua lỗi để tiếp tục
          });
      });
    });
  
    // Thêm các rank mới chọn
    ranksToAdd.forEach(rankId => {
      updateChain = updateChain.then(() => {
        var newRelation = {
          iDgiamgia: updatedVoucher.id,
          iDrank: rankId
        };
        return $http.post('https://localhost:7196/api/Giamgia_rank', newRelation)
          .catch(err => {
            console.error('Lỗi khi thêm rank:', err);
            return $q.resolve();
          });
      });
    });
  
    // Chuẩn bị cập nhật
    updatedVoucher.trangthai = (updatedVoucher.trangthai === "Đang phát hành") ? 0 :
      (updatedVoucher.trangthai === "Chuẩn bị phát hành") ? 1 :
        (updatedVoucher.trangthai === "Dừng phát hành") ? 2 : 1;
  
    updatedVoucher.ngaybatdau = convertToISODate(updatedVoucher.ngaybatdau);
    updatedVoucher.ngayketthuc = convertToISODate(updatedVoucher.ngayketthuc);
    updatedVoucher.donvi = donvi;
    updatedVoucher.rankNames = $scope.allRanks
      .filter(r => r.selectedEdit)
      .map(r => r.tenrank);
  
    updateChain = updateChain.then(() => {
      return $http.put(`https://localhost:7196/api/Giamgia/${updatedVoucher.id}/Admin`, {
        mota: updatedVoucher.mota,
        donvi: updatedVoucher.donvi,
        soluong: updatedVoucher.soluong,
        giatri: updatedVoucher.giatri,
        ngaybatdau: updatedVoucher.ngaybatdau,
        ngayketthuc: updatedVoucher.ngayketthuc,
        rankNames: updatedVoucher.rankNames
      });
    });
  
    updateChain.then(response => {
      if (response && (response.status === 200 || response.status === 204)) {
        alert("Cập nhật mã giảm giá thành công!");
        
        // Reload trang sau khi cập nhật thành công
        window.location.reload();  // Reload lại toàn bộ trang
        
        // Nếu bạn chỉ muốn reload lại một phần dữ liệu (ví dụ: danh sách mã giảm giá)
        // loadVouchers(); // Gọi lại hàm tải dữ liệu sau khi cập nhật thành công
    } else {
        alert("Cập nhật thất bại. Vui lòng thử lại.");
    }
    
    }).catch(err => {
      console.error("Lỗi tổng khi cập nhật:", err);
      alert("Có lỗi xảy ra trong quá trình cập nhật!");
    });
  };

  $scope.changeStatus = function(id) {
    if (confirm('Bạn có chắc chắn muốn thay đổi trạng thái của voucher này?')) {
        $http.put('https://localhost:7196/api/Giamgia/change-status/' + id + '/Admin')
            .then(function(response) {
                alert(response.data);
                // Cập nhật lại danh sách voucher hoặc tìm và cập nhật trạng thái của voucher cụ thể
                window.location.reload();  // Reload lại toàn bộ trang
            })
            .catch(function(error) {
                alert(error.data || 'Có lỗi xảy ra khi thay đổi trạng thái');
            });
    }
};

  function convertToISODate(dateString) {
    if (!dateString) return null;

    // Kiểm tra nếu đã là định dạng ISO thì giữ nguyên
    if (dateString.includes('T')) return dateString;

    // Xử lý định dạng dd/MM/yyyy HH:mm
    const [datePart, timePart] = dateString.split(' ');
    const [day, month, year] = datePart.split('/');
    const [hours, minutes] = timePart.split(':');

    return new Date(year, month - 1, day, hours, minutes).toISOString();
  }



  // Xóa voucher và các quan hệ rank
  $scope.deleteVoucher = function (id) {
    if (!confirm("Bạn có chắc chắn muốn xóa mã giảm giá này không?")) {
      return;
    }

    // Tạo promise chain
    var deleteChain = $q.when();

    // 2. Xóa voucher chính
    deleteChain = deleteChain.then(() => {
      return $http.delete(`https://localhost:7196/api/Giamgia/${id}/Admin`);
    });

    // Xử lý kết quả
    deleteChain.then(() => {
      alert("Đã xóa voucher và các quan hệ thành công!");
      // Reload trang sau khi cập nhật thành công
      window.location.reload();  // Reload lại toàn bộ trang
    }).catch(err => {
      console.error('Lỗi khi xóa voucher:', err);
      alert("Xóa voucher thất bại! Vui lòng thử lại.");
    });
  };


  // Phân trang
  $scope.prevPage = function () {
    if ($scope.currentPage > 1) {
      $scope.currentPage--;
    }
  };

  $scope.nextPage = function () {
    if ($scope.currentPage < $scope.totalPages()) {
      $scope.currentPage++;
    }
  };

  $scope.totalPages = function () {
    return Math.ceil($scope.filteredVouchers.length / $scope.itemsPerPage);
  };

  // Khởi tạo
  function init() {
    loadVouchers();
    loadRanks();
  }

  $scope.closeModal = function () {
    const modalElement = document.getElementById('editVoucherModal');
    let modalInstance = bootstrap.Modal.getInstance(modalElement);

    if (!modalInstance) {
        modalInstance = new bootstrap.Modal(modalElement);
    }

    modalInstance.hide();

    // Đảm bảo loại bỏ backdrop thủ công nếu nó còn sót lại
    const backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach(bd => bd.remove());

    // Xoá class làm mờ giao diện
    document.body.classList.remove('modal-open');
    document.body.style = ''; // reset scroll lock nếu có
};


  init();
});
