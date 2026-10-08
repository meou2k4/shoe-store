app.controller("doimatkhauController", function($scope, $http, $location, $rootScope) {

    $scope.formData = {
        oldPassword: "",
        newPassword: "",
        confirmPassword: ""
    };

    $scope.errorMessage = "";
    $scope.successMessage = "";

    $scope.updatePassword = function() {
        // Kiểm tra mật khẩu mới và xác nhận trùng nhau
        if ($scope.formData.newPassword !== $scope.formData.confirmPassword) {
            $scope.errorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
            $scope.successMessage = "";
            return;
        }

        // Gọi API hoặc xử lý logic đổi mật khẩu
        // Ví dụ giả lập:
        $http.post("/api/admin/doimatkhau", $scope.formData)
            .then(function(response) {
                if(response.data.success) {
                    $scope.successMessage = "Đổi mật khẩu thành công!";
                    $scope.errorMessage = "";
                    // Xóa form
                    $scope.formData = {
                        oldPassword: "",
                        newPassword: "",
                        confirmPassword: ""
                    };
                    // Chuyển hướng nếu muốn
                    // $location.path("/thongtinAdmin");
                } else {
                    $scope.errorMessage = response.data.message || "Có lỗi xảy ra!";
                    $scope.successMessage = "";
                }
            })
            .catch(function(err) {
                console.log(err);
                $scope.errorMessage = "Lỗi server!";
                $scope.successMessage = "";
            });
    };
});
