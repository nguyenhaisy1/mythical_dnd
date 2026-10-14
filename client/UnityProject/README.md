# Unity presentation workspace

Đây là layout để khởi tạo Unity, chưa phải project đã được Editor tạo/kiểm chứng. Chưa có ProjectVersion.txt, Packages lock, scene hay Editor build. Không bịa các file đó để gọi là hoàn thành Unity bootstrap.

Thực hiện FND-16/VS-07 theo docs/runbooks/UNITY-BOOTSTRAP.md: chọn editor, tạo project trong thư mục này, giữ Assets/Game, cài API .NET Standard 2.1, nhập portable core DLLs. UI gửi commands và đọc knowledge projections. Không đặt rule gameplay trong MonoBehaviour.
