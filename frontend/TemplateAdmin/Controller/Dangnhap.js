app.controller('dangnhapController', function ($scope, $http, $rootScope, $location, $timeout) {
    $scope.user = {
        email: '',
        password: ''
    };
    
    $scope.errorMessage = '';
    $scope.isLoading = false;

    $scope.login = function () {
        $scope.isLoading = true;
        $scope.errorMessage = '';
        
        $http.post('https://localhost:7196/api/Nhanviens/Nhanvien/login', {
            Email: $scope.user.email,
            Password: $scope.user.password
        }).then(function (response) {
            $scope.isLoading = false;
            
            if (response.data && response.data.trangthai === 1) {
                $scope.errorMessage = "Tài khoản này ngưng hoạt động. Vui lòng liên hệ admin.";
                return;
            }

            if (response.data && response.data.trangthai === 2) {
                $scope.errorMessage = "Tài khoản này đã bị khóa. Vui lòng liên hệ admin.";
               return
            }

            if (response.data && response.data.message === "Đăng nhập thành công" && response.data.trangthai === 0) {
                $rootScope.isLoggedIn = true;
                $rootScope.userInfo = {
                    id: response.data.id,
                    Ten: response.data.ten,
                    email: response.data.email,
                    avatar: response.data.avatar,
                    role: response.data.role
                };

                localStorage.setItem('userInfo1', JSON.stringify($rootScope.userInfo));
                localStorage.setItem('lastLoginTime', Date.now());
                
                // Thêm thông báo phân quyền
                let roleMessage = "";
                if (response.data.role === 0) {
                    roleMessage = "Bạn đang đăng nhập bằng tài khoản Admin";
                } else if (response.data.role === 1) {
                    roleMessage = "Bạn đang đăng nhập bằng tài khoản Quản lý";
                } else if (response.data.role === 2) {
                    roleMessage = "Bạn đang đăng nhập bằng tài khoản Nhân viên";
                } 

                Swal.fire({
                    title: "Thành Công",
                    html: `Đăng nhập thành công.<br><br>${roleMessage}`,
                    icon: "success"
                }).then(() => {
                    // Chuyển hướng dựa trên role nếu cần
                    $location.path('#!')
                    $scope.$apply();
                });
            } else {
                $scope.errorMessage = "Tài khoản không đúng hoặc mật khẩu không đúng";
            }
        }).catch(function (error) {
            $scope.isLoading = false;
            $scope.errorMessage = "Tài khoản không đúng hoặc mật khẩu không đúng";
        });
    };
});