app.controller('dangkyController', function ($scope, $http) {
    $scope.user = {};
    $scope.errorMessage = '';
    $scope.successMessage = '';

    function getAgeValidationMessage(birthDate) {
        var today = new Date();
        var dob = new Date(birthDate);
        var age = today.getFullYear() - dob.getFullYear();
        var m = today.getMonth() - dob.getMonth();

        // Điều chỉnh nếu ngày sinh chưa đến tháng hiện tạiok
        if (m < 0 || (m === 0 && today.getDate() < dob.getDate())) {
            age--;
        }

        // Kiểm tra nếu tuổi quá nhỏ hoặc quá lớn
        if (age < 10) {
            return 'Bạn phải từ 10 tuổi trở lên để đăng ký';
        } else if (age > 130) {
            return 'Tuổi của bạn không hợp lệ. Bạn không thể có tuổi trên 130';
        }
        return ''; // Nếu tuổi hợp lệ, trả về chuỗi rỗng
    }

    // Hàm để kiểm tra họ tên (Ten), yêu cầu độ dài từ 5 đến 50 ký tự
    function isValidFullName(fullName) {
        if (!fullName) {
            return 'Họ tên không được để trống';
        } else if (fullName.length < 6) {
            return 'Họ tên phải có ít nhất 6 ký tự';
        } else if (fullName.length > 30) {
            return 'Họ tên không được dài quá 50 ký tự';
        }
        return ''; // Nếu hợp lệ, trả về chuỗi rỗng
    }
    function isValidPhoneNumber(phoneNumber) {
        var phoneRegex = /^0[0-9]{9}$/; // Kiểm tra số điện thoại có số 0 ở đầu và 9 chữ số tiếp theo
        return phoneRegex.test(phoneNumber);
    }
    
    function isValidEmail(email) {
        var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
        return emailPattern.test(email);
    }

    // Hàm để kiểm tra mật khẩu
    function isValidPassword(password) {
        return password && password.length >= 6;
    }

    $scope.register = function () {
        // Kiểm tra ngày sinh
        var ageErrorMessage = getAgeValidationMessage($scope.user.ngaysinh);
        if (ageErrorMessage) {
            $scope.errorMessage = ageErrorMessage;
            return;
        }
        
        // Kiểm tra họ tên
        var fullNameErrorMessage = isValidFullName($scope.user.ten);
        if (fullNameErrorMessage) {
            $scope.errorMessage = fullNameErrorMessage;
            return;
        }
        
        // Kiểm tra giới tính (đảm bảo là boolean)
        if ($scope.user.gioitinh === undefined || $scope.user.gioitinh === null) {
            $scope.errorGender = 'Vui lòng chọn giới tính.';
            return;
        }
        
        // Chuyển đổi giới tính sang boolean nếu cần
        var gioitinhValue = $scope.user.gioitinh;
        if (typeof gioitinhValue === 'string') {
            gioitinhValue = (gioitinhValue.toLowerCase() === 'true');
        }
        
        // Kiểm tra mật khẩu
        if (!isValidPassword($scope.user.password)) {
            $scope.errorMessage = 'Mật khẩu phải có ít nhất 6 ký tự';
            return;
        }
        
        if (!isValidPhoneNumber($scope.user.sdt)) {
            $scope.errorMessage = 'Số điện thoại phải có 10 chữ số';
            return;
        }
        
        if (!isValidEmail($scope.user.email)) {
            $scope.errorMessage = 'Email không hợp lệ';
            return;
        }
        
        // Kiểm tra mật khẩu khớp
        if ($scope.user.password !== $scope.user.confirmPassword) {
            $scope.errorMessage = 'Mật khẩu không khớp';
            return;
        }
    
        // Gửi yêu cầu đăng ký
        $http.post('https://localhost:7196/api/Login/_KhachHang/register', {
            Ten: $scope.user.ten,
            Sdt: $scope.user.sdt,
            Ngaysinh: $scope.user.ngaysinh,
            Email: $scope.user.email,
            Password: $scope.user.password,
            gioitinh: gioitinhValue, // Sử dụng giá trị đã chuyển đổi
        }).then(function (response) {
            $scope.successMessage = 'Đăng ký thành công';
            $scope.errorMessage = '';
            setTimeout(function () {
                window.location.href = '#!login';
            }, 2000);
        }).catch(function (error) {
            $scope.errorMessage = error.data || 'Đã xảy ra lỗi khi đăng ký';
            $scope.successMessage = '';
        });
    };
});
