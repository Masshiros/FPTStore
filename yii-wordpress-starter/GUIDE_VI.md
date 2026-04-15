# GUIDE CHO NEWBIE: Yii2 + Docker + WordPress + API + CRUD

## 1) Mình đã build được những gì

Trong thư mục `/home/runner/work/FPTStore/FPTStore/yii-wordpress-starter` đã có:

- `yii-app/`: project Yii2 Basic (bắt đầu từ `yiisoft/yii2-app-basic`)
- `docker-compose.yml`: chạy đồng thời:
  - Yii app: `http://localhost:8080`
  - WordPress: `http://localhost:8081`
  - phpMyAdmin: `http://localhost:8082`
  - MySQL: cổng `3307` trên máy host
- CRUD mẫu cho `Post`:
  - URL giao diện: `http://localhost:8080/post/index`
  - File chính: `models/Post.php`, `controllers/PostController.php`, `views/post/*`
  - Migration: `migrations/m260415_000001_create_post_table.php`
- REST API mẫu cho `Post`:
  - URL: `GET http://localhost:8080/api/posts`
  - Controller API: `controllers/api/PostController.php`
  - Rule REST: trong `config/web.php`

---

## 2) Vì sao project basic có login nhưng “chưa có gì”?

Đúng rồi: `yii2-app-basic` chỉ là skeleton.

Nó có sẵn:
- Login/logout demo
- Contact page
- Layout + cấu hình cơ bản

Nó **không tự có business feature** (CRUD, API thật, module riêng...), bạn phải tự generate thêm bằng Gii hoặc code tay.

---

## 3) Chạy project từ đầu bằng Docker

### Bước 1: vào thư mục
```bash
cd /home/runner/work/FPTStore/FPTStore/yii-wordpress-starter
```

### Bước 2: chạy stack
```bash
docker compose up -d
```

### Bước 3: migrate database cho Yii
```bash
docker compose exec yii php yii migrate --interactive=0
```

### Bước 4: kiểm tra
- Yii home: `http://localhost:8080`
- CRUD Post: `http://localhost:8080/post/index`
- API Post: `http://localhost:8080/api/posts`
- WordPress setup: `http://localhost:8081`
- phpMyAdmin: `http://localhost:8082`

---

## 4) Cách tạo lại project từ con số 0 (newbie flow)

### 4.1 Tạo Yii basic
```bash
composer create-project --prefer-dist yiisoft/yii2-app-basic yii-app
```

### 4.2 Bật DB trong `config/db.php`
Dùng env cho dễ chạy docker:
- `DB_HOST`
- `DB_PORT`
- `DB_NAME`
- `DB_USER`
- `DB_PASSWORD`

### 4.3 Dùng Gii để generate CRUD
1. Chạy app
2. Vào `http://localhost:8080/gii`
3. Generate theo thứ tự:
   - Model
   - CRUD

### 4.4 Tạo API
- Tạo controller kế thừa `yii\rest\ActiveController`
- Khai báo `modelClass`
- Thêm `yii\rest\UrlRule` trong `config/web.php`

### 4.5 Tạo migration
```bash
php yii migrate/create create_post_table
php yii migrate
```

---

## 5) Test API nhanh

### Tạo Post qua API
```bash
curl -X POST http://localhost:8080/api/posts \
  -H "Content-Type: application/json" \
  -d '{"title":"Post 1","content":"Demo content","status":1}'
```

### Lấy danh sách Post
```bash
curl http://localhost:8080/api/posts
```

### Lọc theo status
```bash
curl "http://localhost:8080/api/posts?status=1"
```

---

## 6) Bug thường gặp và cách fix

### Lỗi 1: `Could not resolve host: asset-packagist.org`
Nguyên nhân: mạng/DNS hoặc bị chặn domain.
Fix:
- Kiểm tra DNS/network
- Thử lại trong container khác mạng
- Hoặc cấu hình mirror package phù hợp

### Lỗi 2: `SQLSTATE[HY000] [2002] Connection refused`
Nguyên nhân: DB chưa chạy hoặc sai host.
Fix:
- Trong Docker phải dùng host `db`, không dùng `localhost`
- Kiểm tra `docker compose ps`

### Lỗi 3: 404 với Pretty URL
Fix:
- Đã bật `urlManager`
- Kiểm tra Apache rewrite và `.htaccess`
- Tạm thời test bằng `/index.php?r=post/index`

### Lỗi 4: Gii không vào được
Fix:
- Chạy ở `YII_ENV_DEV`
- Kiểm tra module `gii` trong `config/web.php`

---

## 7) Hướng dẫn build feature tiếp theo

Bạn có thể follow roadmap:

1. **Auth nâng cao**
   - RBAC role/permission
   - JWT cho API
2. **WordPress integration**
   - Đồng bộ category/post qua REST API của WordPress
   - SSO hoặc shared user strategy
3. **E-commerce domain**
   - Product, Category, Cart, Order
   - Checkout + payment sandbox
4. **API versioning**
   - `/api/v1/*`, `/api/v2/*`
5. **Test & quality**
   - Codeception (functional/API)
   - Logging + error tracking
6. **Deploy**
   - Docker image cho môi trường production
   - Nginx reverse proxy + SSL

---

## 8) Gợi ý lệnh hữu ích hằng ngày

```bash
# vào app container
docker compose exec yii bash

# chạy migrate
docker compose exec yii php yii migrate --interactive=0

# tạo migration mới
docker compose exec yii php yii migrate/create add_xxx_to_post

# clear cache
docker compose exec yii php yii cache/flush-all

# xem log
docker compose logs -f yii
```

---

Nếu bạn muốn, bước tiếp theo mình có thể build luôn:
- JWT auth cho API
- Product/Category CRUD đầy đủ
- Kết nối WordPress REST API 2 chiều (sync post/product)
