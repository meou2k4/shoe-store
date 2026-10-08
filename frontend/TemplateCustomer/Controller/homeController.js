app.controller('homeController', function ($scope, $http) {
    $scope.sanPhamMoi = [];
    $scope.sanPhamPhoBien = [];
    $scope.sanPhamKhuyenMai = [];
    $scope.isLoading = true; // Trạng thái chờ tải dữ liệu
    $scope.thuongHieu = [];
    $scope.spDanhGiaCao = [];
    
    // Hàm tải hình ảnh từ API
    function loadHinhAnh(idSPCT, callback) {
        fetch(`https://localhost:7196/api/Sanphamchitiets/GetImageById/${idSPCT}`)
            .then(response => response.blob())
            .then(blob => {
                let imgUrl = URL.createObjectURL(blob);
                callback(imgUrl);
            })
            .catch(error => console.error("Lỗi tải ảnh:", error));
    }

    // Hàm xử lý dữ liệu sản phẩm (chung cho tất cả danh sách)
    function processSanPham(data, limit = 4) {
        return data
        .filter(sp => sp.soluong > 0) // Lọc sản phẩm có số lượng lớn hơn 0
        .slice(0, limit).map(sp => {
            sp.hinhAnh = "default-image.jpg"; // Ảnh mặc định
            if (sp.sanphamchitiets && sp.sanphamchitiets.length > 0  ) {
                let spct = sp.sanphamchitiets[0];
                loadHinhAnh(spct.id, function (imgUrl) {
                    sp.hinhAnh = imgUrl;
                    $scope.$apply(); // Cập nhật giao diện
                });
            }
            return sp;
        });
    }

    // Hàm tính tổng số lượng bán của sản phẩm
    $scope.getTotalSPCT = function (product) {
        return product.sanphamchitiets.reduce(function (total, spct) {
            return total + spct.soLuongBan;
        }, 0);
    };
    
    // Gọi API và phân loại sản phẩm
    $http.get("https://localhost:7196/api/Sanphams/GetALLSanPham")
        .then(function (response) {
            const products = response.data;

            const validProducts = products.filter(sp => sp.sanphamchitiets && sp.sanphamchitiets.length > 0);

            $scope.thuongHieu = groupByThuongHieu(validProducts,4);
            console.log($scope.thuongHieu);
            // Xử lý từng loại danh sách sản phẩm
            $scope.sanPhamMoi = processSanPham(
                validProducts.sort((a, b) => new Date(b.ngayThemSanPham) - new Date(a.ngayThemSanPham))
            );
            $scope.spDanhGiaCao = processSanPham(
                validProducts
                .sort((a, b) => b.trungBinhDanhGia - a.trungBinhDanhGia)
                .slice(0, 4)
            );
            $scope.sanPhamPhoBien = processSanPham(
                validProducts
                .sort((a, b) => $scope.getTotalSPCT(b) - $scope.getTotalSPCT(a))
                .slice(0, 3)
            );

            $scope.sanPhamKhuyenMai = processSanPham(
                validProducts.filter(sp => sp.giasale < sp.giaban).sort((a, b) => b.giasale - a.giasale)
            );

            $scope.isLoading = false; // Kết thúc chờ tải
            console.log($scope.sanPhamMoi);
            
        })
        .catch(function (error) {
            console.error("Lỗi khi tải danh sách sản phẩm:", error);
            $scope.errorMessage = "Có lỗi xảy ra khi tải dữ liệu.";
            $scope.isLoading = false; // Dừng trạng thái chờ tải
        });
        $scope.generateStars = function (rating) {
            const fullStars = Math.floor(rating); // Số sao đầy
            const halfStar = rating % 1 >= 0.5; // Có nửa sao hay không
            const emptyStars = 5 - Math.ceil(rating); // Số sao trống
        
            let stars = [];
            for (let i = 0; i < fullStars; i++) stars.push('full'); // Sao đầy
            if (halfStar) stars.push('half'); // Sao nửa
            for (let i = 0; i < emptyStars; i++) stars.push('empty'); // Sao trống
            return stars;
        }

        function groupByThuongHieu(products, limit) {
            const grouped = {};
        
            products.forEach(sp => {
                if (sp.soluong <= 0) return;
                const thuongHieu = sp.thuongHieu || "Khác"; // Nhóm sản phẩm không có thương hiệu vào "Khác"
                const idThuongHieu = sp.idThuongHieu || 0;
                if (!grouped[thuongHieu]) {
                    grouped[thuongHieu] = {
                        idThuongHieu: idThuongHieu,
                        thuongHieu: thuongHieu,
                        sanPhams: []
                    };
                }
        
                if (grouped[thuongHieu].sanPhams.length < limit) {
                    grouped[thuongHieu].sanPhams.push(sp);
                }
            });
        
            return grouped;
        }


        
});
