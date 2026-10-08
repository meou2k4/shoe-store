/* dashboard.controller.js */
app.controller("dashboardController",
    function ($scope, $timeout, $http, $q, $filter) {
  
      /* =========== 1. KHỞI TẠO =========== */
      // Placeholder để tránh chớp 0
      $scope.stats = {
          totalEmployees : '…',
          totalProducts  : '…',
          totalOrders    : '…',
          totalRevenue   : '…'
      };
      $scope.recentOrders = [];
  
      /* =========== 2. LẤY THỐNG KÊ =========== */
      const requests = [
          $http.get("https://localhost:7196/api/Nhanviens/tong-trangthai-0"),          // 0
          $http.get("https://localhost:7196/api/Sanphams/tong-trangthai-0-1"),         // 1
          $http.get("https://localhost:7196/api/Hoadons/tong-so-don"),                 // 2
          $http.get("https://localhost:7196/api/Hoadons/tong-doanh-thu-thanh-cong")    // 3
      ];
  
      $q.all(requests).then(function (res) {
          $scope.stats.totalEmployees = res[0].data;
          $scope.stats.totalProducts  = res[1].data;
          $scope.stats.totalOrders    = res[2].data;
          $scope.stats.totalRevenue   =
             res[3].data;  // 1 234 567 ₫
      }).catch(function (err) {
          console.error("Lỗi thống kê:", err);
      });
  
      /* =========== 3. LẤY 10 HOÁ ĐƠN MỚI =========== */
      const ORDER_STATUS = {
          0: "Chờ xác nhận",
          1: "Đơn hàng đã được xác nhận",
          2: "Đơn hàng đang được giao",
          3: "Đơn hàng thành công",
          4: "Đơn hàng đã huỷ",
          5: "Trả hàng"
      };
  
      $http.get("https://localhost:7196/api/Hoadons/latest-10")
           .then(function (res) {
               $scope.recentOrders = res.data.map(function (hd) {
                   return {
                       orderId : hd.maHoaDon,
                       customer: hd.tenKhachHang,
                       total   : hd.tongTien,
                       status  : ORDER_STATUS[hd.trangThai] || "Không rõ"
                   };
               });
           })
           .catch(function (err) {
               console.error("Lỗi lấy đơn hàng:", err);
           });
  
      /* =========== 4. BIỂU ĐỒ DEMO (giữ nguyên) =========== */
      const revenueLabels = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];
      const revenueData   = [1.2, 1.0, 1.8, 2.1, 2.5, 2.0, 2.2];
      const productLabels = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];
      const productData   = [10, 8, 12, 15, 20, 18, 22];
  
      $timeout(function () {
          /* ---- LINE chart ---- */
          let ctxLine = document.getElementById('chartLine').getContext('2d');
          new Chart(ctxLine, {
              type: 'line',
              data: {
                  labels: revenueLabels,
                  datasets: [{
                      label: 'Doanh thu (Triệu VND)',
                      data: revenueData,
                      backgroundColor: 'rgba(54, 162, 235, 0.2)',
                      borderColor:     'rgba(54, 162, 235, 1)',
                      borderWidth: 2,
                      fill: true,
                      tension: 0.1
                  }]
              },
              options: {
                  scales: {
                      y: { beginAtZero: true, title: { display: true, text: 'Triệu VND' } }
                  }
              }
          });
  
          /* ---- BAR chart ---- */
          let ctxBar = document.getElementById('chartBar').getContext('2d');
          new Chart(ctxBar, {
              type: 'bar',
              data: {
                  labels: productLabels,
                  datasets: [{
                      label: 'Sản phẩm bán ra',
                      data: productData,
                      backgroundColor: 'rgba(75, 192, 192, 0.6)',
                      borderColor:     'rgba(75, 192, 192, 1)',
                      borderWidth: 1
                  }]
              },
              options: {
                  scales: {
                      y: { beginAtZero: true, title: { display: true, text: 'Số sản phẩm' } }
                  }
              }
          });
      }, 0);
  });
  