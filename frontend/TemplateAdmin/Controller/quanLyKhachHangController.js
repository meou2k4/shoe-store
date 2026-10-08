app.controller('quanLyKhachHangController', function ($scope, $http, $location, $timeout, $routeParams, $interval) {
    $scope.khachHangs = [];
    $scope.Ranks = [];
    // Lấy danh sách sản phẩm
    $scope.newkhachHang = {
        id: 0,
        ten: null,
        sdt: null,
        ngaysinh: null,
        tichdiem: 0,
        email: null,
        password: null,
        ngaytaotaikhoan: new Date().toISOString(),
        diemsudung: 0,
        trangthai: 0,
        idrank: 0,
        gioitinh: true,
        avatar: ""
    };
    function getVietnameseDay(dayIndex) {
        const days = ["Chủ Nhật", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy"];
        return days[dayIndex];
    }
    // Hàm cập nhật thời gian
    function updateTime() {
        var now = new Date();
        var dayOfWeek = getVietnameseDay(now.getDay());
        var day = String(now.getDate()).padStart(2, '0');
        var month = String(now.getMonth() + 1).padStart(2, '0'); // Tháng bắt đầu từ 0
        var year = now.getFullYear();
        var hours = String(now.getHours()).padStart(2, '0');
        var minutes = String(now.getMinutes()).padStart(2, '0');
        var seconds = String(now.getSeconds()).padStart(2, '0');

        $scope.time = `${dayOfWeek}, ${day}/${month}/${year} - ${hours}:${minutes}:${seconds}`;
    }

    // Cập nhật mỗi giây
    $interval(updateTime, 1000);

    // Chạy ngay khi trang tải
    updateTime();

    $scope.themKhachHang = function () {

        if (!$scope.formKhachHang.$invalid) {
            Swal.fire({
                title: "Xác nhận",
                text: "Bạn có chắc chắn muốn thêm khách hàng này?",
                icon: "question",
                showCancelButton: true,
                confirmButtonText: "Đồng ý",
                cancelButtonText: "Hủy"
            }).then((result) => {
                if (result.isConfirmed) {
                    $http.post("https://localhost:7196/api/Khachhangs", $scope.newkhachHang)
                        .then(function (response) {
                            console.log("Thêm thành công", response.data);
                            if (response.data) {
                                const result = response.data;
                                Swal.fire({
                                    title: "Thành công",
                                    text: result.Message || "Thêm khách hàng thành công!",
                                    icon: "success"
                                });

                                $scope.LoadKhachHang();
                                $location.url('/KhachHang');
                            } else {
                                Swal.fire({
                                    title: "Thất bại",
                                    text: "Thêm khách hàng không thành công!",
                                    icon: "error"
                                });
                            }

                        })
                        .catch(function (error) {
                            console.error("Lỗi khi thêm sản phẩm:", error);
                            alert("Có lỗi xảy ra khi thêm sản phẩm.");
                        });
                }

            })
        }

    }

    $scope.LoadKhachHang = function () {
        $scope.isLoading = true; // Bắt đầu trạng thái tải
        $http.get("https://localhost:7196/api/Khachhangs")
            .then(function (response) {
                $scope.khachHangs = response.data;
                console.log("Khách hàng", $scope.khachHangs);

            })
            .catch(function (error) {
                console.error("Lỗi khi tải danh sách sản phẩm:", error);
                $scope.errorMessage = "Có lỗi xảy ra khi tải dữ liệu.";
                $scope.isLoading = false;
            });
    }
    $scope.loadRank = function () {
        $http.get("https://localhost:7196/api/Ranks")
            .then(function (response) {
                $scope.Ranks = response.data;
                console.log("Ranks", $scope.Ranks);
            })
            .catch(function (error) {
                console.error("Lỗi khi tải danh sách sản phẩm:", error);
                $scope.errorMessage = "Có lỗi xảy ra khi tải dữ liệu.";
                $scope.isLoading = false; // Kết thúc trạng thái tải
            });
    }
    $scope.LoadKhachHang();
    $scope.loadRank();
    $scope.deleteKhachHang = function (id) {
        Swal.fire({
            title: "Bạn có chắc chắn?",
            text: "Khách hàng sẽ bị xóa và không thể khôi phục!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Xóa",
            cancelButtonText: "Hủy"
        }).then((result) => {
            if (result.isConfirmed) {
                $http.delete("https://localhost:7196/api/Khachhangs/" + id)
                    .then(function (response) {
                        console.log("Xóa thành công", response.data);
                        Swal.fire({
                            title: "Thành công",
                            text: response.data.message || "Xóa khách hàng thành công!",
                            icon: "success"
                        }).then(() => {
                            $scope.LoadKhachHang(); 
                        });
                        
                    })
                    .catch(function (error) {
                        console.error("❌ Lỗi khi xóa khách hàng:", error);
                        Swal.fire({
                            title: "Lỗi",
                            text: "Có lỗi xảy ra khi xóa khách hàng.",
                            icon: "error"
                        });
                    });
            }
        })
    }
    $scope.getKhachHangById = function (id) {
        $http.get("https://localhost:7196/api/Khachhangs/" + id)
            .then(function (response) {
                $scope.newkhachHang = response.data;
                if ($scope.newkhachHang.ngaysinh) {
                    $scope.newkhachHang.ngaysinh = new Date($scope.newkhachHang.ngaysinh);
                }
                if ($scope.newkhachHang.trangthai === "Hoạt động") {
                    $scope.newkhachHang.trangthai = 0;
                }
                if ($scope.newkhachHang.trangthai === "Tài khoản bị khoá") {
                    $scope.newkhachHang.trangthai = 1;
                }

                console.log("Khách hàng", $scope.newkhachHang);
            })
            .catch(function (error) {
                console.error("Lỗi khi tải danh sách sản phẩm:", error);
                $scope.errorMessage = "Có lỗi xảy ra khi tải dữ liệu.";
            });

    }
    $scope.updateKhachHang = function () {
        if (!$scope.formKhachHang.$invalid) {
            Swal.fire({
                title: "Xác nhận",
                text: "Bạn có chắc chắn muốn sửa khách hàng này?",
                icon: "question",
                showCancelButton: true,
                confirmButtonText: "Đồng ý",
                cancelButtonText: "Hủy"
            }).then((result) => {
                if (result.isConfirmed) {
                    $http.put("https://localhost:7196/api/Khachhangs/" + $scope.newkhachHang.id, $scope.newkhachHang)
                        .then(function (response) {
                            if (response.data) {
                                const result = response.data;
                                Swal.fire({
                                    title: "Thành công",
                                    text: result.Message || "Sửa khách hàng thành công!",
                                    icon: "success"
                                });

                                $scope.LoadKhachHang();
                                $location.url('/KhachHang');
                            } else {
                                Swal.fire({
                                    title: "Thất bại",
                                    text: "Sửa khách hàng không thành công!",
                                    icon: "error"
                                });
                            }
                        })
                        .catch(function (error) {
                            console.error("Lỗi khi cập nhật sản phẩm:", error);
                            alert("Có lỗi xảy ra khi cập nhật sản phẩm.");
                        });
                }
            })
        }
    }
    $scope.validateDate = function () {
        if (!$scope.newkhachHang.ngaysinh) return;

        const today = new Date();
        const birthDate = new Date($scope.newkhachHang.ngaysinh);
        const age = today.getFullYear() - birthDate.getFullYear();
        const monthDiff = today.getMonth() - birthDate.getMonth();

        // Điều chỉnh tuổi nếu chưa đến ngày sinh trong năm
        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
            age--;
        }

        $scope.formKhachHang.ngaysinh.$setValidity('invalidAge', age >= 10 && age <= 130);
    };
    $scope.validateSDT = function () {
        if (!$scope.newkhachHang.sdt || !$scope.formKhachHang || !$scope.formKhachHang.sdt) return;

        const sdtPattern = /^(0[3|5|7|8|9])+([0-9]{8})$/;
        const isValidPattern = sdtPattern.test($scope.newkhachHang.sdt);

        $scope.formKhachHang.sdt.$setValidity('invalidSdt', isValidPattern);

        const duplicateSDT = $scope.khachHangs.some(kh =>
            kh.sdt === $scope.newkhachHang.sdt && kh.id !== $scope.newkhachHang.id
        );

        $scope.formKhachHang.sdt.$setValidity('duplicateSdt', !duplicateSDT);
    };


    if ($routeParams.id) {
        let id = $routeParams.id;
        $scope.getKhachHangById(id);
    }
});