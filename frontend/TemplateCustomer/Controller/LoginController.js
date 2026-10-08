app.controller('LoginController', function ($scope, $http, $rootScope, $location, $timeout) {
    $scope.step = 1; // Bước hiện tại
    $scope.user = {};
    $scope.isLoading = false;
    $scope.errorMessage = '';

    // Kiểm tra nếu đã đăng nhập, chuyển hướng về trang chủ
    if ($rootScope.userInfo && $rootScope.userInfo.id) {
        $location.path('/');
        return;
    }

    // Hàm reset mật khẩu khi email thay đổi
    $scope.resetPasswordInput = function () {
        $scope.step = 1;
        $scope.user.password = '';
        $scope.errorMessage = '';
    };

    // Kiểm tra email
    $scope.checkEmail = function () {
        $scope.isLoading = true;
        $http.post(`https://localhost:7196/api/Login/_KhachHang/checkemail?checkEmail=${$scope.user.email}`)
            .then(function (response) {
                $scope.isLoading = false;

                if (response.data && response.data.status === 'password_required') {
                    $scope.step = 2; // Chuyển sang bước nhập mật khẩu
                } else if (response.data && response.data.status === 'new_account') {
                    $location.path('/RegisterPassword').search({ email: $scope.user.email }); // Đăng ký mới
                } else if (response.data && response.data.status === 'not_found') {
                    $scope.errorMessage = "Email không tồn tại.";
                } else {
                    $scope.errorMessage = "Đã xảy ra lỗi, vui lòng thử lại.";
                }
            }).catch(function (error) {
                $scope.isLoading = false;
                $scope.errorMessage = "Không thể kiểm tra email. Vui lòng thử lại.";
            });
    };

    // Đăng nhập
    $scope.login = function () {
        $scope.isLoading = true;
        $http.post('https://localhost:7196/api/Login/_KhachHang/login', {
            Email: $scope.user.email,
            Password: $scope.user.password
        }).then(function (response) {
            $scope.isLoading = false;
            if (response.data && response.data.trangthai === 1) {
                $scope.errorMessage = "Tài khoản này đã bị khóa. Vui lòng liên hệ quản trị viên.";
                return; // Không cho phép đăng nhập
            }

            if (response.data && response.data.message === "Đăng nhập thành công") {
                // Cập nhật trạng thái đăng nhập
                $rootScope.isLoggedIn = true;
                $rootScope.userInfo = {
                    id: response.data.khachHangId,
                    ten: response.data.ten,
                    email: response.data.email,
                    avatar: response.data.avatar
                };

                // Lưu thông tin người dùng vào localStorage
                localStorage.setItem('userInfo', JSON.stringify($rootScope.userInfo));
                localStorage.setItem('lastLoginTime', Date.now()); // Lưu thời gian đăng nhập
                Swal.fire("Thành Công", "Đăng nhập thành công.", "success");
                $location.path('/');
            } else {
                // Thay đổi thông báo lỗi khi đăng nhập thất bại
                $scope.errorMessage = "Tài khoản hoặc mật khẩu không đúng";
            }
        }).catch((error) => {
            $scope.isLoading = false;
            $scope.errorMessage = "Tài khoản hoặc mật khẩu không đúng";
        });
    };
});
