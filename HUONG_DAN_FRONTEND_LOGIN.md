# Hướng Dẫn Frontend Login - VietLab CRM API

## Tổng Quan

Tài liệu này hướng dẫn cách tích hợp chức năng đăng nhập từ frontend với VietLab CRM API.

## Thông Tin API

### Base URL
- **Development**: `http://localhost:5000` hoặc `https://localhost:5001`
- **Production**: `https://your-domain.com/crm-api` (nếu có base path)

### Endpoint Login
```
POST /api/auth/login
```

## Request Format

### Headers
```json
{
  "Content-Type": "application/json"
}
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

## Response Format

### Thành công (200 OK)
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
  "token": "access_token_string",
  "refreshToken": "refresh_token_string",
  "tokenExpiresAt": "2024-01-01T12:00:00Z",
  "refreshTokenExpiresAt": "2024-01-08T12:00:00Z"
}
```

### Lỗi (400 Bad Request / 401 Unauthorized)
```json
{
  "success": false,
  "message": "Tên đăng nhập hoặc mật khẩu không đúng"
}
```

## Cách Sử Dụng Token

Sau khi đăng nhập thành công, bạn cần lưu token và sử dụng nó cho các request tiếp theo.

### Lưu Token
- **Access Token**: Hết hạn sau 1 giờ
- **Refresh Token**: Hết hạn sau 7 ngày

### Gửi Token trong Request
Thêm header `Authorization` với format:
```
Authorization: Bearer {access_token}
```

## Ví Dụ Code

### React với Axios

#### 1. Tạo API Service

```typescript
// services/authService.ts
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000';

interface LoginRequest {
  userName: string;
  password: string;
}

interface LoginResponse {
  success: boolean;
  message: string;
  user?: {
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
  };
  token?: string;
  refreshToken?: string;
  tokenExpiresAt?: string;
  refreshTokenExpiresAt?: string;
}

// Tạo axios instance với interceptor
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor để tự động thêm token vào request
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

// Interceptor để xử lý lỗi 401 và refresh token
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken) {
          const response = await axios.post(`${API_BASE_URL}/api/auth/refresh-token`, {
            refreshToken: refreshToken,
          });

          const { token, refreshToken: newRefreshToken } = response.data;
          localStorage.setItem('accessToken', token);
          localStorage.setItem('refreshToken', newRefreshToken);

          originalRequest.headers.Authorization = `Bearer ${token}`;
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        // Refresh token thất bại, đăng xuất
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await axios.post<LoginResponse>(
      `${API_BASE_URL}/api/auth/login`,
      credentials
    );
    return response.data;
  },

  refreshToken: async (refreshToken: string): Promise<LoginResponse> => {
    const response = await axios.post<LoginResponse>(
      `${API_BASE_URL}/api/auth/refresh-token`,
      { refreshToken }
    );
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  },
};

export default apiClient;
```

#### 2. Tạo Login Component

```typescript
// components/Login.tsx
import React, { useState } from 'react';
import { authService } from '../services/authService';

const Login: React.FC = () => {
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const response = await authService.login({ userName, password });

      if (response.success && response.token && response.user) {
        // Lưu token và thông tin user
        localStorage.setItem('accessToken', response.token);
        localStorage.setItem('refreshToken', response.refreshToken || '');
        localStorage.setItem('user', JSON.stringify(response.user));

        // Chuyển hướng đến trang chủ
        window.location.href = '/';
      } else {
        setError(response.message || 'Đăng nhập thất bại');
      }
    } catch (err: any) {
      setError(
        err.response?.data?.message || 'Đã xảy ra lỗi khi đăng nhập'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <form onSubmit={handleSubmit}>
        <h2>Đăng Nhập</h2>
        
        {error && <div className="error-message">{error}</div>}

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

        <button type="submit" disabled={loading}>
          {loading ? 'Đang đăng nhập...' : 'Đăng Nhập'}
        </button>
      </form>
    </div>
  );
};

export default Login;
```

### Vue 3 với Composition API

#### 1. Tạo API Service

```typescript
// services/authService.ts
import axios, { AxiosInstance } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

interface LoginRequest {
  userName: string;
  password: string;
}

interface LoginResponse {
  success: boolean;
  message: string;
  user?: any;
  token?: string;
  refreshToken?: string;
  tokenExpiresAt?: string;
  refreshTokenExpiresAt?: string;
}

// Tạo axios instance
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor để thêm token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor để refresh token
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken) {
          const response = await axios.post(`${API_BASE_URL}/api/auth/refresh-token`, {
            refreshToken,
          });

          localStorage.setItem('accessToken', response.data.token);
          localStorage.setItem('refreshToken', response.data.refreshToken);
          originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        localStorage.clear();
        window.location.href = '/login';
      }
    }

    return Promise.reject(error);
  }
);

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await axios.post<LoginResponse>(
      `${API_BASE_URL}/api/auth/login`,
      credentials
    );
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  },
};

export default apiClient;
```

#### 2. Tạo Login Component

```vue
<!-- components/Login.vue -->
<template>
  <div class="login-container">
    <form @submit.prevent="handleSubmit">
      <h2>Đăng Nhập</h2>
      
      <div v-if="error" class="error-message">{{ error }}</div>

      <div>
        <label>Tên đăng nhập:</label>
        <input v-model="userName" type="text" required />
      </div>

      <div>
        <label>Mật khẩu:</label>
        <input v-model="password" type="password" required />
      </div>

      <button type="submit" :disabled="loading">
        {{ loading ? 'Đang đăng nhập...' : 'Đăng Nhập' }}
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { authService } from '../services/authService';

const router = useRouter();
const userName = ref('');
const password = ref('');
const error = ref('');
const loading = ref(false);

const handleSubmit = async () => {
  error.value = '';
  loading.value = true;

  try {
    const response = await authService.login({
      userName: userName.value,
      password: password.value,
    });

    if (response.success && response.token && response.user) {
      localStorage.setItem('accessToken', response.token);
      localStorage.setItem('refreshToken', response.refreshToken || '');
      localStorage.setItem('user', JSON.stringify(response.user));
      router.push('/');
    } else {
      error.value = response.message || 'Đăng nhập thất bại';
    }
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Đã xảy ra lỗi khi đăng nhập';
  } finally {
    loading.value = false;
  }
};
</script>
```

### Angular với HttpClient

#### 1. Tạo Auth Service

```typescript
// services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { Router } from '@angular/router';

const API_BASE_URL = 'http://localhost:5000';

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  user?: any;
  token?: string;
  refreshToken?: string;
  tokenExpiresAt?: string;
  refreshTokenExpiresAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenSubject = new BehaviorSubject<string | null>(null);
  public token$ = this.tokenSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    const token = localStorage.getItem('accessToken');
    if (token) {
      this.tokenSubject.next(token);
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${API_BASE_URL}/api/auth/login`, credentials)
      .pipe(
        tap(response => {
          if (response.success && response.token) {
            localStorage.setItem('accessToken', response.token);
            localStorage.setItem('refreshToken', response.refreshToken || '');
            localStorage.setItem('user', JSON.stringify(response.user));
            this.tokenSubject.next(response.token);
          }
        })
      );
  }

  refreshToken(): Observable<LoginResponse> {
    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) {
      this.logout();
      return throwError(() => new Error('No refresh token'));
    }

    return this.http.post<LoginResponse>(`${API_BASE_URL}/api/auth/refresh-token`, {
      refreshToken
    }).pipe(
      tap(response => {
        if (response.success && response.token) {
          localStorage.setItem('accessToken', response.token);
          localStorage.setItem('refreshToken', response.refreshToken || '');
          this.tokenSubject.next(response.token);
        }
      }),
      catchError(error => {
        this.logout();
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    this.tokenSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  getUser(): any {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
```

#### 2. Tạo HTTP Interceptor

```typescript
// interceptors/auth.interceptor.ts
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, switchMap, take, filter } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();

    if (token) {
      request = this.addTokenHeader(request, token);
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          return this.handle401Error(request, next);
        }
        return throwError(() => error);
      })
    );
  }

  private addTokenHeader(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      headers: request.headers.set('Authorization', `Bearer ${token}`)
    });
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response: any) => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(response.token);
          return next.handle(this.addTokenHeader(request, response.token));
        }),
        catchError((err) => {
          this.isRefreshing = false;
          this.authService.logout();
          return throwError(() => err);
        })
      );
    }

    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap((token) => next.handle(this.addTokenHeader(request, token)))
    );
  }
}
```

#### 3. Tạo Login Component

```typescript
// components/login/login.component.ts
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  error: string = '';
  loading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      userName: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.error = '';
      this.loading = true;

      this.authService.login(this.loginForm.value).subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/']);
          } else {
            this.error = response.message || 'Đăng nhập thất bại';
          }
          this.loading = false;
        },
        error: (err) => {
          this.error = err.error?.message || 'Đã xảy ra lỗi khi đăng nhập';
          this.loading = false;
        }
      });
    }
  }
}
```

```html
<!-- components/login/login.component.html -->
<div class="login-container">
  <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
    <h2>Đăng Nhập</h2>
    
    <div *ngIf="error" class="error-message">{{ error }}</div>

    <div>
      <label>Tên đăng nhập:</label>
      <input formControlName="userName" type="text" />
    </div>

    <div>
      <label>Mật khẩu:</label>
      <input formControlName="password" type="password" />
    </div>

    <button type="submit" [disabled]="loading || loginForm.invalid">
      {{ loading ? 'Đang đăng nhập...' : 'Đăng Nhập' }}
    </button>
  </form>
</div>
```

## Refresh Token

Khi access token hết hạn (sau 1 giờ), bạn cần sử dụng refresh token để lấy access token mới.

### Endpoint Refresh Token
```
POST /api/auth/refresh-token
```

### Request Body
```json
{
  "refreshToken": "refresh_token_string"
}
```

### Response
```json
{
  "success": true,
  "message": "Token đã được làm mới thành công",
  "token": "new_access_token",
  "refreshToken": "new_refresh_token",
  "tokenExpiresAt": "2024-01-01T13:00:00Z",
  "refreshTokenExpiresAt": "2024-01-08T12:00:00Z"
}
```

## Xử Lý Lỗi

### Các Mã Lỗi Thường Gặp

- **400 Bad Request**: Request không hợp lệ (thiếu userName hoặc password)
- **401 Unauthorized**: 
  - Tên đăng nhập hoặc mật khẩu không đúng
  - Token không hợp lệ hoặc đã hết hạn
  - Refresh token không hợp lệ
- **500 Internal Server Error**: Lỗi server

### Ví Dụ Xử Lý Lỗi

```typescript
try {
  const response = await authService.login({ userName, password });
  // Xử lý thành công
} catch (error: any) {
  if (error.response) {
    // Server trả về response với status code
    switch (error.response.status) {
      case 400:
        console.error('Request không hợp lệ:', error.response.data.message);
        break;
      case 401:
        console.error('Không được phép:', error.response.data.message);
        break;
      case 500:
        console.error('Lỗi server:', error.response.data.message);
        break;
      default:
        console.error('Lỗi không xác định:', error.response.data.message);
    }
  } else if (error.request) {
    // Request đã được gửi nhưng không nhận được response
    console.error('Không nhận được response từ server');
  } else {
    // Lỗi khi setup request
    console.error('Lỗi:', error.message);
  }
}
```

## CORS và Authentication

### Lưu Ý Quan Trọng

1. **CORS**: 
   - Trong Development: CORS cho phép tất cả origins
   - Trong Production: Chỉ cho phép các origins được cấu hình
   - Nếu `DisableAuthentication = true` trong `appsettings.json`, CORS cũng sẽ bị tắt

2. **Authentication**:
   - Mặc định API yêu cầu Bearer token cho các endpoint được bảo vệ
   - Có thể tắt authentication bằng cách set `DisableAuthentication = true` trong `appsettings.json` (chỉ dùng cho testing)

3. **Base Path**:
   - Trong Development: Không có base path
   - Trong Production: Có thể có base path (ví dụ: `/crm-api`)
   - Cần cấu hình đúng base path trong frontend

## Best Practices

1. **Lưu trữ Token**:
   - Sử dụng `localStorage` hoặc `sessionStorage` cho web app
   - Sử dụng secure storage cho mobile app
   - Không lưu token trong cookie không secure

2. **Refresh Token**:
   - Tự động refresh token trước khi hết hạn
   - Xử lý refresh token trong HTTP interceptor
   - Đăng xuất nếu refresh token thất bại

3. **Bảo Mật**:
   - Không log token ra console trong production
   - Sử dụng HTTPS trong production
   - Validate token trên client trước khi gửi request

4. **Error Handling**:
   - Hiển thị thông báo lỗi thân thiện với người dùng
   - Log lỗi chi tiết cho developer
   - Xử lý các trường hợp edge case (mất mạng, timeout, etc.)

## Ví Dụ Sử Dụng API Client

Sau khi đăng nhập, bạn có thể sử dụng API client để gọi các endpoint khác:

```typescript
// React example
import apiClient from './services/authService';

// Gọi API với token tự động được thêm vào
const fetchData = async () => {
  try {
    const response = await apiClient.get('/api/clients');
    return response.data;
  } catch (error) {
    console.error('Error fetching data:', error);
  }
};
```

## Test với Swagger

Bạn có thể test API login trực tiếp qua Swagger UI:

1. Truy cập: `http://localhost:5000/swagger` (hoặc `https://localhost:5001/swagger`)
2. Tìm endpoint `POST /api/auth/login`
3. Click "Try it out"
4. Nhập thông tin đăng nhập
5. Click "Execute"
6. Copy token từ response và sử dụng trong các request khác

## Hỗ Trợ

Nếu gặp vấn đề, vui lòng kiểm tra:
1. API đang chạy và có thể truy cập
2. CORS được cấu hình đúng
3. Base URL được cấu hình đúng
4. Token được lưu và gửi đúng format
5. Console log để xem lỗi chi tiết

