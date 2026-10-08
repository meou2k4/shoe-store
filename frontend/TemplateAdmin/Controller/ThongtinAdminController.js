app.controller('thongtinAdminController', function ($scope, $http, $location) {
    const userInfoString = localStorage.getItem("userInfo1");
    const userInfo = JSON.parse(userInfoString);
    $scope.previewImage = null; // Dùng để hiển thị ảnh tạm thời

    $scope.datanv = function () {
        $http.get("https://localhost:7196/api/Nhanviens/" + userInfo.id)
            .then(function (response) {
                response.data.ngaysinh = new Date(response.data.ngaysinh);
                response.data.ngaytaotaikhoan = new Date(response.data.ngaytaotaikhoan);
                $scope.thongtin = response.data;

                // Gán ảnh từ server vào thẻ img bằng id
                if ($scope.thongtin.avatar) {
                    document.getElementById("thongtinanh").src = "https://localhost:7196/picture/" + $scope.thongtin.avatar;
                }
            })
            .catch(function (error) {
                console.error("Lỗi khi lấy dữ liệu nhân viên:", error);
            });
    };

    $scope.triggerFileInput = function () {
        document.getElementById('avatarInput').click();
    };

    $scope.onImageChange = function (files) {
        if (files && files.length > 0) {
            const file = files[0];

            // Lưu file vào scope
            $scope.thongtin.fileanh = file;

            // Hiển thị ảnh mới qua id="thongtinanh"
            const imageUrl = URL.createObjectURL(file);
            
            // Tạo URL tạm thời để hiển thị ảnh preview
            $scope.previewImage = URL.createObjectURL(file);

            document.getElementById("thongtinanh").src = imageUrl;

            console.log("Ảnh mới đã được chọn:", file);

            $scope.$apply(); // Cập nhật giao diện
        }
    };



    $scope.updateProfile = function () {
        console.log('Dữ liệu edit trước khi gửi:', $scope.edit);
        var formData = new FormData();

        formData.append("Id", $scope.thongtin.id);
        formData.append("Hovaten", $scope.thongtin.hovaten);
        formData.append("Ngaysinh", new Date($scope.thongtin.ngaysinh).toISOString());
        formData.append("Diachi", $scope.thongtin.diachi);
        formData.append("Gioitinh", $scope.thongtin.gioitinh === 'true'); // boolean
        formData.append("Sdt", $scope.thongtin.sdt);
        formData.append("Email", $scope.thongtin.email);

        // Xử lý ảnh:
        if ($scope.thongtin.fileanh) {
            formData.append("avatarFile", $scope.thongtin.fileanh);
        }

        $http.put("https://localhost:7196/api/Nhanviens/Updatethongtin/" + $scope.thongtin.id, formData, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (res) {
            Swal.fire({
                title: "Thành công",
                text: res.data.message,
                icon: "success"
            }).then((result) => {
                if (result.isConfirmed) {
                    $('#EditNVModal').modal('hide');
                    location.reload();
                    window.scroll(0, 0);
                }
            });
        }).catch(function (error) {
            Swal.fire("Lỗi", error.data.message, "error");
        });
    };

    $scope.updatePasswordBtn = function (){
        var currentPassword = document.getElementById("currentPassword").value.trim();
        var newPassword = document.getElementById("newPassword").value.trim();
        var confirmPassword = document.getElementById("confirmPassword").value.trim();

        // Kiểm tra rỗng
        if (!currentPassword || !newPassword || !confirmPassword) {
            Swal.fire("Lỗi", "Vui lòng nhập đầy đủ các trường.", "error");
            return;
        }

        // Kiểm tra xác nhận mật khẩu
        if (newPassword !== confirmPassword) {
            Swal.fire("Lỗi", "Xác nhận mật khẩu không khớp.", "error");
            return;
        }

        // Giả sử đã có email trong localStorage/session hoặc biến toàn cục
        var email = $scope.thongtin.email; // Hoặc lấy từ $scope nếu có: $scope.thongtin.email

        if (!email) {
            Swal.fire("Lỗi", "Không tìm thấy email người dùng.", "error");
            return;
        }

        var data = {
            Email: email,
            OldPassword: currentPassword,
            NewPassword: newPassword
        };

        fetch("https://localhost:7196/api/Nhanviens/doimatkhau", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        })
        .then(response => {
            if (!response.ok) {
                return response.json().then(err => {
                    throw err;
                });
            }
            return response.json();
        })
        .then(result => {
            Swal.fire("Thành công", result.message, "success").then(res => {
                if (res.isConfirmed) {
                    // ✅ Xoá localStorage
                    localStorage.removeItem("userInfo1");
                    location.reload();
                }
            });
        })
        .catch(error => {
            let message = error.message || "Đã xảy ra lỗi khi đổi mật khẩu.";
            Swal.fire("Lỗi", message, "error");
        });
    }

    $scope.datanv();
});
