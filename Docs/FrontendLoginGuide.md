# Hướng dẫn Frontend - Login API

## Tổng quan

Tài liệu này hướng dẫn cách tích hợp endpoint login vào ứng dụng frontend. API login cung cấp authentication với access token và refresh token.

---

## Endpoint

**URL:** `POST /api/auth/login`

**Base URL:** 
- Development: `https://localhost:5001` (hoặc port được cấu hình)
- Production: `https://your-domain.com/crm-api` (nếu deploy như subapplication)

**Full URL:** `{BaseURL}/api/auth/login`

---

## Request

### Headers
```http
Content-Type: application/json
```

### Body
```json
{
  "userName": "string",
  "password": "string"
}
```

### Ví dụ Request
```json
{
  "userName": "admin",
  "password": "admin"
}
```

---

## Response

### Success Response (200 OK)

```json
{
  "success": true,
  "message": "Đăng nhập thành công",
  "user": {
    "accountId": "guid",
    "employeeId": "guid",
    "userName": "string",
    "fullName": "string",
    "email": "string",
    "department": "string",
    "role": "string",
    "title": "string",
    "permissionId": "guid",
    "permissionName": "string",
    "permissionCode": "string",
    "status": "Active"
  },
  "token": "string (access token)",
  "refreshToken": "string (refresh token)",
  "tokenExpiresAt": "2024-01-01T12:00:00Z",
  "refreshTokenExpiresAt": "2024-01-08T12:00:00Z"
}
```

### Error Responses

#### 400 Bad Request - Validation Error
```json
{
  "success": false,
  "message": "Tên đăng nhập và mật khẩu không được để trống"
}
```

#### 401 Unauthorized - Invalid Credentials
```json
{
  "success": false,
  "message": "Tên đăng nhập hoặc mật khẩu không đúng"
}
```

#### 500 Internal Server Error
```json
{
  "success": false,
  "message": "Đã xảy ra lỗi trong quá trình đăng nhập"
}
```

---

## Token Information

- **Access Token**: Hết hạn sau **1 giờ** (1 hour)
- **Refresh Token**: Hết hạn sau **7 ngày** (7 days)
- **Token Format**: Base64 encoded string (trong production nên dùng JWT)

---

## Ví dụ Code

### 1. Vanilla JavaScript / Fetch API

```javascript
// Hàm login
async function login(userName, password) {
  const baseURL = 'https://localhost:5001'; // Thay đổi theo môi trường
  
  try {
    const response = await fetch(`${baseURL}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        userName: userName,
        password: password
      })
    });

    const data = await response.json();

    if (data.success) {
      // Lưu token vào localStorage hoặc sessionStorage
      localStorage.setItem('accessToken', data.token);
      localStorage.setItem('refreshToken', data.refreshToken);
      localStorage.setItem('user', JSON.stringify(data.user));
      localStorage.setItem('tokenExpiresAt', data.tokenExpiresAt);
      localStorage.setItem('refreshTokenExpiresAt', data.refreshTokenExpiresAt);
      
      return {
        success: true,
        user: data.user,
        token: data.token,
        refreshToken: data.refreshToken
      };
    } else {
      return {
        success: false,
        message: data.message
      };
    }
  } catch (error) {
    console.error('Login error:', error);
    return {
      success: false,
      message: 'Không thể kết nối đến server'
    };
  }
}

// Sử dụng
login('admin', 'admin')
  .then(result => {
    if (result.success) {
      console.log('Đăng nhập thành công:', result.user);
      // Redirect đến trang chủ hoặc dashboard
      window.location.href = '/dashboard';
    } else {
      alert(result.message);
    }
  });
```

### 2. React với Hooks

```jsx
import { useState } from 'react';

const API_BASE_URL = 'https://localhost:5001'; // Thay đổi theo môi trường

function LoginForm() {
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userName, password }),
      });

      const data = await response.json();

      if (data.success) {
        // Lưu token và user info
        localStorage.setItem('accessToken', data.token);
        localStorage.setItem('refreshToken', data.refreshToken);
        localStorage.setItem('user', JSON.stringify(data.user));
        localStorage.setItem('tokenExpiresAt', data.tokenExpiresAt);
        localStorage.setItem('refreshTokenExpiresAt', data.refreshTokenExpiresAt);

        // Redirect hoặc update state
        window.location.href = '/dashboard';
      } else {
        setError(data.message);
      }
    } catch (err) {
      setError('Không thể kết nối đến server');
      console.error('Login error:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleLogin}>
      <div>
        <label>Tên đăng nhập:</label>
        <input
          type="text"
          value={userName}
          onChange={(e) => setUserName(e.target.value)}
          required
        />
      </div>
      <div>
        <label>Mật khẩu:</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
      </div>
      {error && <div style={{ color: 'red' }}>{error}</div>}
      <button type="submit" disabled={loading}>
        {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
      </button>
    </form>
  );
}

export default LoginForm;
```

### 3. React với Axios và Context API

```jsx
// authContext.js
import { createContext, useContext, useState, useEffect } from 'react';
import axios from 'axios';

const AuthContext = createContext();

const API_BASE_URL = 'https://localhost:5001';

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Kiểm tra token khi app khởi động
    const token = localStorage.getItem('accessToken');
    const userStr = localStorage.getItem('user');
    
    if (token && userStr) {
      setUser(JSON.parse(userStr));
    }
    setLoading(false);
  }, []);

  const login = async (userName, password) => {
    try {
      const response = await axios.post(`${API_BASE_URL}/api/auth/login`, {
        userName,
        password
      });

      if (response.data.success) {
        const { user, token, refreshToken, tokenExpiresAt, refreshTokenExpiresAt } = response.data;
        
        localStorage.setItem('accessToken', token);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('user', JSON.stringify(user));
        localStorage.setItem('tokenExpiresAt', tokenExpiresAt);
        localStorage.setItem('refreshTokenExpiresAt', refreshTokenExpiresAt);
        
        setUser(user);
        return { success: true, user };
      } else {
        return { success: false, message: response.data.message };
      }
    } catch (error) {
      console.error('Login error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Không thể kết nối đến server'
      };
    }
  };

  const logout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    localStorage.removeItem('tokenExpiresAt');
    localStorage.removeItem('refreshTokenExpiresAt');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};

// LoginForm.js
import { useState } from 'react';
import { useAuth } from './authContext';

function LoginForm() {
  const { login } = useAuth();
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    const result = await login(userName, password);
    
    if (result.success) {
      window.location.href = '/dashboard';
    } else {
      setError(result.message);
    }
    
    setLoading(false);
  };

  return (
    <form onSubmit={handleSubmit}>
      {/* Form fields */}
    </form>
  );
}
```

### 4. Vue 3 với Composition API

```vue
<template>
  <form @submit.prevent="handleLogin">
    <div>
      <label>Tên đăng nhập:</label>
      <input v-model="userName" type="text" required />
    </div>
    <div>
      <label>Mật khẩu:</label>
      <input v-model="password" type="password" required />
    </div>
    <div v-if="error" style="color: red">{{ error }}</div>
    <button type="submit" :disabled="loading">
      {{ loading ? 'Đang đăng nhập...' : 'Đăng nhập' }}
    </button>
  </form>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const API_BASE_URL = 'https://localhost:5001';
const router = useRouter();

const userName = ref('');
const password = ref('');
const loading = ref(false);
const error = ref('');

const handleLogin = async () => {
  loading.value = true;
  error.value = '';

  try {
    const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        userName: userName.value,
        password: password.value
      })
    });

    const data = await response.json();

    if (data.success) {
      localStorage.setItem('accessToken', data.token);
      localStorage.setItem('refreshToken', data.refreshToken);
      localStorage.setItem('user', JSON.stringify(data.user));
      localStorage.setItem('tokenExpiresAt', data.tokenExpiresAt);
      localStorage.setItem('refreshTokenExpiresAt', data.refreshTokenExpiresAt);

      router.push('/dashboard');
    } else {
      error.value = data.message;
    }
  } catch (err) {
    error.value = 'Không thể kết nối đến server';
    console.error('Login error:', err);
  } finally {
    loading.value = false;
  }
};
</script>
```

### 5. Angular với Service

```typescript
// auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

const API_BASE_URL = 'https://localhost:5001';

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  user?: UserInfo;
  token?: string;
  refreshToken?: string;
  tokenExpiresAt?: string;
  refreshTokenExpiresAt?: string;
}

export interface UserInfo {
  accountId: string;
  employeeId: string;
  userName: string;
  fullName: string;
  email: string;
  department: string;
  role: string;
  title: string;
  permissionId: string;
  permissionName: string;
  permissionCode: string;
  status: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<UserInfo | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // Load user from localStorage on init
    const userStr = localStorage.getItem('user');
    if (userStr) {
      this.currentUserSubject.next(JSON.parse(userStr));
    }
  }

  login(userName: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${API_BASE_URL}/api/auth/login`, {
      userName,
      password
    }).pipe(
      tap(response => {
        if (response.success && response.user && response.token) {
          localStorage.setItem('accessToken', response.token);
          localStorage.setItem('refreshToken', response.refreshToken || '');
          localStorage.setItem('user', JSON.stringify(response.user));
          localStorage.setItem('tokenExpiresAt', response.tokenExpiresAt || '');
          localStorage.setItem('refreshTokenExpiresAt', response.refreshTokenExpiresAt || '');
          this.currentUserSubject.next(response.user);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    localStorage.removeItem('tokenExpiresAt');
    localStorage.removeItem('refreshTokenExpiresAt');
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}

// login.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-login',
  template: `
    <form (ngSubmit)="onSubmit()">
      <div>
        <label>Tên đăng nhập:</label>
        <input [(ngModel)]="userName" name="userName" required />
      </div>
      <div>
        <label>Mật khẩu:</label>
        <input [(ngModel)]="password" name="password" type="password" required />
      </div>
      <div *ngIf="error" style="color: red">{{ error }}</div>
      <button type="submit" [disabled]="loading">
        {{ loading ? 'Đang đăng nhập...' : 'Đăng nhập' }}
      </button>
    </form>
  `
})
export class LoginComponent {
  userName = '';
  password = '';
  loading = false;
  error = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    this.loading = true;
    this.error = '';

    this.authService.login(this.userName, this.password).subscribe({
      next: (response) => {
        if (response.success) {
          this.router.navigate(['/dashboard']);
        } else {
          this.error = response.message;
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Không thể kết nối đến server';
        this.loading = false;
      }
    });
  }
}
```

---

## Axios Interceptor để tự động thêm Token

### Setup Axios Interceptor

```javascript
import axios from 'axios';

const API_BASE_URL = 'https://localhost:5001';

// Tạo axios instance
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor - tự động thêm token vào header
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor - xử lý token hết hạn
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Nếu token hết hạn (401) và chưa retry
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        // Thử refresh token
        const refreshToken = localStorage.getItem('refreshToken');
        const refreshResponse = await axios.post(`${API_BASE_URL}/api/auth/refresh-token`, {
          refreshToken: refreshToken
        });

        if (refreshResponse.data.success) {
          // Lưu token mới
          localStorage.setItem('accessToken', refreshResponse.data.token);
          localStorage.setItem('refreshToken', refreshResponse.data.refreshToken);
          localStorage.setItem('tokenExpiresAt', refreshResponse.data.tokenExpiresAt);
          localStorage.setItem('refreshTokenExpiresAt', refreshResponse.data.refreshTokenExpiresAt);

          // Retry request với token mới
          originalRequest.headers.Authorization = `Bearer ${refreshResponse.data.token}`;
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        // Refresh token thất bại, logout
        localStorage.clear();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;
```

---

## Refresh Token

### Endpoint Refresh Token

**URL:** `POST /api/auth/refresh-token`

**Request:**
```json
{
  "refreshToken": "string"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Token đã được làm mới thành công",
  "token": "new access token",
  "refreshToken": "new refresh token",
  "tokenExpiresAt": "2024-01-01T13:00:00Z",
  "refreshTokenExpiresAt": "2024-01-08T13:00:00Z"
}
```

### Hàm Refresh Token

```javascript
async function refreshAccessToken() {
  const refreshToken = localStorage.getItem('refreshToken');
  
  if (!refreshToken) {
    throw new Error('No refresh token available');
  }

  try {
    const response = await fetch(`${API_BASE_URL}/api/auth/refresh-token`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ refreshToken })
    });

    const data = await response.json();

    if (data.success) {
      localStorage.setItem('accessToken', data.token);
      localStorage.setItem('refreshToken', data.refreshToken);
      localStorage.setItem('tokenExpiresAt', data.tokenExpiresAt);
      localStorage.setItem('refreshTokenExpiresAt', data.refreshTokenExpiresAt);
      
      return data.token;
    } else {
      throw new Error(data.message);
    }
  } catch (error) {
    // Refresh token thất bại, logout user
    localStorage.clear();
    window.location.href = '/login';
    throw error;
  }
}
```

---

## Best Practices

### 1. Lưu trữ Token

- **localStorage**: Dễ bị XSS attack, nhưng tiện lợi
- **sessionStorage**: Tự động xóa khi đóng tab
- **httpOnly Cookie**: An toàn nhất, nhưng cần cấu hình CORS

**Khuyến nghị:** Sử dụng `localStorage` cho development, `httpOnly Cookie` cho production.

### 2. Kiểm tra Token hết hạn

```javascript
function isTokenExpired() {
  const expiresAt = localStorage.getItem('tokenExpiresAt');
  if (!expiresAt) return true;
  
  return new Date(expiresAt) < new Date();
}

// Sử dụng trước khi gọi API
if (isTokenExpired()) {
  await refreshAccessToken();
}
```

### 3. Protected Routes

```javascript
// React Router example
import { Navigate } from 'react-router-dom';

function ProtectedRoute({ children }) {
  const token = localStorage.getItem('accessToken');
  const isExpired = isTokenExpired();
  
  if (!token || isExpired) {
    return <Navigate to="/login" replace />;
  }
  
  return children;
}
```

### 4. Logout

```javascript
function logout() {
  // Xóa tất cả thông tin
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
  localStorage.removeItem('user');
  localStorage.removeItem('tokenExpiresAt');
  localStorage.removeItem('refreshTokenExpiresAt');
  
  // Redirect về trang login
  window.location.href = '/login';
}
```

### 5. Error Handling

```javascript
async function apiCall(url, options = {}) {
  try {
    const token = localStorage.getItem('accessToken');
    
    const response = await fetch(url, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        ...options.headers
      }
    });

    if (response.status === 401) {
      // Token hết hạn, thử refresh
      await refreshAccessToken();
      // Retry request
      return apiCall(url, options);
    }

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Request failed');
    }

    return await response.json();
  } catch (error) {
    console.error('API call error:', error);
    throw error;
  }
}
```

---

## Testing với cURL

```bash
# Login request
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "admin",
    "password": "admin"
  }'

# Sử dụng token trong request tiếp theo
curl -X GET https://localhost:5001/api/auth/test-auth \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

## Lưu ý quan trọng

1. **Admin Bypass**: Trong development, password "admin" sẽ bypass authentication và tự động đăng nhập với account Active đầu tiên.

2. **Token Format**: Hiện tại token là Base64 encoded string. Trong production nên chuyển sang JWT.

3. **CORS**: Đảm bảo backend đã cấu hình CORS để cho phép frontend gọi API.

4. **HTTPS**: Trong production, luôn sử dụng HTTPS để bảo vệ token.

5. **Token Rotation**: Refresh token sẽ được rotate (thay đổi) mỗi lần refresh để tăng bảo mật.

---

## Troubleshooting

### Lỗi CORS
- Kiểm tra CORS configuration trong `Program.cs`
- Đảm bảo frontend URL được thêm vào `AllowedOrigins`

### Token không hợp lệ
- Kiểm tra token có được gửi đúng format: `Bearer {token}`
- Kiểm tra token chưa hết hạn
- Thử refresh token

### 401 Unauthorized
- Token hết hạn → Refresh token
- Token không hợp lệ → Login lại
- Account bị deactivate → Liên hệ admin

---

## Tài liệu liên quan

- [OData Status](./ODataStatus.md)
- [Database Schema](./DatabaseSchema.md)
- [Quotation Item Design](./QuotationItemDesign.md)

