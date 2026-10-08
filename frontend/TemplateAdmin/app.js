const app = angular.module("myApp", ['ngRoute']);
//ifanfgf
app.config(function ($routeProvider) {
    $routeProvider
        .when("/", {
            templateUrl: "./Views/dashboard.html",
            controller: "dashboardController"
        })
        .when("/addtrahang", {
          templateUrl: "./Views/trahang.html",
          controller: "trahangCtrl"
        })
        .when("/dangnhap", {
            templateUrl: "./Views/dangnhap.html",
            controller: "dangnhapController"
        })
        .when("/trahang", {
            templateUrl: "./Views/trahang.html",
            controller: "trahangController"
        })
        .when("/quenmatkhau", {
            templateUrl: "./Views/quenmatkhau1.html"
        })
        .when("/datlaimatkhau", {
            templateUrl: "./Views/datlaimatkhau.html"
        })
        .when("/doimatkhau", {
            templateUrl: "./Views/doimatkhau.html",
              controller: "doimatkhauController"
        })
        .when("/sale", {
            templateUrl: "./Views/sale1.html",
            controller: "saleController"
        })
        .when("/quanlyhoadon", {
            templateUrl: "./Views/quanlyhoadon.html",
            controller: "quanlyhoadonController"
        })
        .when("/passwordChanging", {
            templateUrl: "./Views/passwordChanging.html"
        })
        .when("/nhanvien", {
            templateUrl: "./Views/nhanvien.html",
            controller: 'nhanvienController'
        })
        .when("/Size", {
            templateUrl: "./Views/Size.html",
            controller: 'QuanLyThuocTinhController'
        })
        .when("/Mau", {
            templateUrl: "./Views/Mau.html",
            controller: 'QuanLyThuocTinhController'
        })
        .when("/ChatLieu", {
            templateUrl: "./Views/ChatLieu.html",
            controller: 'QuanLyThuocTinhController'
        })
        .when("/thuongHieu", {
            templateUrl: "./Views/thuongHieu.html",
            controller: 'QuanLyThuocTinhController'
        })
        .when("/addSanPham", {
            templateUrl: "./Views/sanPham/addSanPham.html",
            controller: "QuanLySanPhamController"
        })
        .when("/editSanPham/:id", {
            templateUrl: "./Views/sanPham/editSanPham.html",
            controller: "QuanLySanPhamController"
        })
        .when("/chiTietSanPham/:id", {
            templateUrl: "./Views/sanPham/chiTietSanPham.html",
            controller: "QuanLySanPhamController"
        })
        .when("/sanpham", {
            templateUrl: "./Views/sanPham/quanLySanPham.html",
            controller: "QuanLySanPhamController"
        })
        .when("/Rank", {
            templateUrl: "./Views/Rank.html",
            controller: 'RankController'
            
        })
        .when("/KhachHang", {
            templateUrl: "./Views/khachHang/quanLyKhachHang.html",
            controller: 'quanLyKhachHangController'
        })
        .when("/addKhachHang", {
            templateUrl: "./Views/khachHang/addKhachHang.html",
            controller: 'quanLyKhachHangController'
        })
        .when("/editKhachHang/:id", {
            templateUrl: "./Views/khachHang/editKhachHang.html",
            controller: 'quanLyKhachHangController'
        })
        .when("/thongtinAdmin", {
            templateUrl: "./Views/ThongtinAdmin.html",
            controller: 'thongtinAdminController'
            
        })
        .when("/Vocher", {
            templateUrl: "./Views/VocherAdmin.html",
             controller: 'VoucherController'
        })
          .when("/dashboard", {
            templateUrl: "./Views/dashboard.html",
            controller: "dashboardController"
        })
        .when("/addTrahang/:id", {
          templateUrl: "./Views/addTrahang.html",
          controller: "addTrahangCtrl"
      })
        .when("/hoadonoff", {
          templateUrl: "./Views/hoadonoff.html"
      })
      .when("/banhangtaiquay", {
        templateUrl: "./Views/banhangtaiquay.html"
    })

        .otherwise("/")
});
app.run(function($rootScope, $location) {
    var storedUser = localStorage.getItem('userInfo1');
    if (storedUser) {
        $rootScope.userInfo = JSON.parse(storedUser);
        $rootScope.isLoggedIn = true;
    }

    $rootScope.$on('$routeChangeStart', function(event, next, current) {
        var publicPages = ["/dangnhap", "/quenmatkhau", "/datlaimatkhau","/doimatkhau"];
        var path = $location.path();
        if ($rootScope.userInfo && ["/dangnhap", "/quenmatkhau", "/datlaimatkhau"].includes(path)) {
            event.preventDefault();
            $location.path("/");
            return;
        }
        
        var restrictedPage = publicPages.indexOf(path) === -1;
        if (restrictedPage && !$rootScope.userInfo) {
            event.preventDefault();
           $location.path("/dangnhap");
            // $location.path("/doimatkhau");
        }doimatkhau
    });

    $rootScope.dangxuat = function () {
        $rootScope.isLoggedIn = false;
        $rootScope.userInfo = null;
        localStorage.removeItem('userInfo1');
        console.log("Đăng xuất thành công");
        $location.path('/dangnhap');
    };
});


