app.controller('SanPhamSaleController', function ($scope, $http) {
    $scope.sanPhamSale = [];
    $scope.currentPage = 1; // Trang hiện tại
    $scope.pageSize = 9; // Số sản phẩm trên mỗi trang
    $scope.totalPages = 0; // Tổng số trang
    $scope.sortOption = 'newest'; // Tùy chọn sắp xếp mặc định
    $scope.searchParams = {
        thuongHieus: [], 
        sizes: [],       
        giaMin: null,
        giaMax: null
    };
  
    function loadHinhAnh(idSPCT, callback) {
        fetch(`https://localhost:7196/api/Sanphamchitiets/GetImageById/${idSPCT}`)
            .then(response => response.blob())
            .then(blob => {
                let imgUrl = URL.createObjectURL(blob);
                callback(imgUrl);
            })
            .catch(error => console.error("Lỗi tải ảnh:", error));
    }
     $scope.loadCategories = function () {
        $http.get("https://localhost:7196/api/Thuonghieu")
            .then(function (response) {
                //Thương hiệu hoạt động
                $scope.categories0 = []
                $scope.categories = response.data;
                $scope.categories.forEach(function (category) {
                    if (category.tinhtrang === 0) {
                        $scope.categories0.push(category);
                    }
                    console.log("thanh", $scope.categories0);


                });
                console.log($scope.categories);

            })
            .catch(function (error) {
                console.error("Lỗi khi tải danh sách thương hiệu:", error);
            }
            );
    }
    $scope.loadSizes = function () {
        $http.get("https://localhost:7196/api/Size")
            .then(function (response) {
                //Size hoạt động
                $scope.sizes0 = []
                $scope.sizes = response.data;
                $scope.sizes.forEach(function (size) {
                    if (size.trangthai === 0) {
                        $scope.sizes0.push(size);
                    }
                });
                console.log($scope.sizes);
            })
            .catch(function (error) {
                console.error("Lỗi khi tải danh sách kích thước:", error);
            });
    }
     $scope.loadCategories();
    $scope.loadSizes();
    // Lấy danh sách sản phẩm
    $http.get("https://localhost:7196/api/Sanphams/GetALLSanPhamGiamGia")
        .then(function (response) {
            $scope.sanPhamSale = response.data
            .filter(sp => sp.sanphamchitiets && sp.sanphamchitiets.length > 0 && sp.soluong > 0)
            .map(sp => {
                sp.hinhAnh = "default-image.jpg"; // Ảnh mặc định

                // Lấy hình ảnh từ sản phẩm chi tiết đầu tiên
                let spct = sp.sanphamchitiets[0];
                loadHinhAnh(spct.id, function (imgUrl) {
                    sp.hinhAnh = imgUrl;
                    $scope.$apply(); // Cập nhật lại view
                });

                return sp;
            });

        console.log("Sale",$scope.sanPhamSale);

        // Cập nhật tổng số trang
        $scope.totalPages = Math.ceil($scope.sanPhamSale.length / $scope.pageSize);
        })
        .catch(function (error) {
            console.error("Lỗi khi tải danh sách sản phẩm:", error);
            $scope.errorMessage = "Có lỗi xảy ra khi tải dữ liệu.";
        });
        $scope.searchSanPhams = function() {
            const giaMin = $scope.searchParams.giaMin;
            const giaMax = $scope.searchParams.giaMax;
        
            // Validate giá trị nhập liệu
            if (giaMin != null && giaMin < 0) {
                $scope.errorMessage1 = "Giá thấp nhất không được nhỏ hơn 0.";
                return;
            }
            if (giaMax != null && giaMax < 0) {
                $scope.errorMessage1 = "Giá cao nhất không được nhỏ hơn 0.";
                return;
            }
            if (giaMin != null && giaMax != null && giaMin > giaMax) {
                $scope.errorMessage1 = "Giá thấp nhất không được lớn hơn giá cao nhất.";
                return;
            }
        
            $scope.searchParams.thuongHieus = $scope.selectedThuongHieuIds;
            $scope.searchParams.sizes = $scope.selectedSizeIds;
        
            const params = {
                giaMin: $scope.searchParams.giaMin,
                giaMax: $scope.searchParams.giaMax,
                idThuongHieu: $scope.searchParams.thuongHieus,
                idSize: $scope.searchParams.sizes,
                coSale: true,
            };
        
            // Gọi API tìm kiếm sản phẩm
            $http.get("https://localhost:7196/api/Sanphams/SanPhamChiTiet/search", { params: params })
             .then(function (response) {
             $scope.sanPhamSale = response.data
            .filter(sp => sp.sanphamchitiets && sp.sanphamchitiets.length > 0)
            .map(sp => {
                sp.hinhAnh = "default-image.jpg"; // Ảnh mặc định

                // Lấy hình ảnh từ sản phẩm chi tiết đầu tiên
                let spct = sp.sanphamchitiets[0];
                loadHinhAnh(spct.id, function (imgUrl) {
                    sp.hinhAnh = imgUrl;
                    $scope.$apply(); // Cập nhật lại view
                });

                return sp;
            });

        if ($scope.sanPhamSale.length === 0) {
            $scope.errorMessage = "Không tìm thấy sản phẩm nào phù hợp với tiêu chí tìm kiếm.";
        } else {
            $scope.errorMessage = ''; // Clear error message if products are found
        }

        // Cập nhật tổng số trang
        $scope.totalPages = Math.ceil($scope.sanPhamSale.length / $scope.pageSize);
    })
    .catch(function (error) {
        if (error.status === 404) {
            $scope.errorMessage = error.data.message || "Không tìm thấy sản phẩm nào thỏa mãn tiêu chí.";
        } else {
            console.error('Lỗi khi lọc sản phẩm:', error); // In chi tiết lỗi ra console
            $scope.errorMessage = "Có lỗi xảy ra khi lọc sản phẩm.";
        }
    });

        };
        
        
    $scope.applyFilters = function() {
       $scope.searchSanPhams();
        $scope.currentPage = 1; 
        
    };
    $scope.selectedThuongHieuIds = [];
    $scope.selectedSizeIds = [];
    
    $scope.updateSelectedThuongHieu = function () {
        $scope.selectedThuongHieuIds = $scope.categories0
            .filter(b => b.selected)
            .map(b => b.id);
    };
    
    $scope.updateSelectedSize = function () {
        $scope.selectedSizeIds = $scope.sizes0
            .filter(s => s.selected)
            .map(s => s.id);
    };
    
    
  
    // Hàm sắp xếp sản phẩm
    $scope.sortProducts = function() {
      switch ($scope.sortOption) {
          case 'newest': // Sắp xếp mới nhất
              $scope.sanPhamSale.sort((a, b) => new Date(b.ngayThemSanPham) - new Date(a.ngayThemSanPham));
              break;
          case 'oldest': // Sắp xếp cũ nhất
              $scope.sanPhamSale.sort((a, b) => new Date(a.ngayThemSanPham) - new Date(b.ngayThemSanPham));
              break;
          case 'lowToHigh': // Giá từ thấp đến cao
              $scope.sanPhamSale.sort((a, b) => a.giasale - b.giasale);
              break;
          case 'highToLow': // Giá từ cao đến thấp
              $scope.sanPhamSale.sort((a, b) => b.giasale - a.giasale);
              break;
          case 'bestseller': // Bán chạy nhất
              $scope.sanPhamSale.sort((a, b) => $scope.getTotalSPCT(b) - $scope.getTotalSPCT(a));
              break;
          default:
              break;
      }
  };
  
    $scope.getTotalSPCT = function(product) {
      return product.sanphamchitiets.reduce(function(total, spct) {
          return total + spct.soLuongBan;
      }, 0);
  };
    // Lấy danh sách sản phẩm cho trang hiện tại
    $scope.getPagedProducts12 = function () {
        let start = ($scope.currentPage - 1) * $scope.pageSize;
        let end = start + $scope.pageSize;
        return $scope.sanPhamSale.slice(start, end);
    };
  
    // Chuyển sang trang trước
    $scope.previousPage1 = function () {
        if ($scope.currentPage > 1) {
            $scope.currentPage--;
        }
    };
  
    // Chuyển sang trang tiếp theo
    $scope.nextPage1 = function () {
        if ($scope.currentPage < $scope.totalPages) {
            $scope.currentPage++;
        }
    };
  
    // Chuyển tới trang cụ thể
    $scope.goToPage1 = function (page) {
        if (page >= 1 && page <= $scope.totalPages) {
            $scope.currentPage = page;
        }
    };
    $scope.generateStars = function (rating) {
      const fullStars = Math.floor(rating); // Số sao đầy
      const halfStar = rating % 1 >= 0.5; // Có nửa sao hay không
      const emptyStars = 5 - Math.ceil(rating); // Số sao trống
  
      let stars = [];
      for (let i = 0; i < fullStars; i++) stars.push('full'); // Sao đầy
      if (halfStar) stars.push('half'); // Sao nửa
      for (let i = 0; i < emptyStars; i++) stars.push('empty'); // Sao trống
      return stars;
  };
  
});
