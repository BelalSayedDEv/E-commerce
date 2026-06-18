// Global API Endpoint Settings
const API_BASE = 'http://localhost:5249/api';

// Application State Management
const state = {
    token: localStorage.getItem('token') || null,
    username: localStorage.getItem('username') || null,
    decodedToken: null,
    activeTab: 'products',
    products: [],
    categories: [],
    activeCategory: null,
    searchQuery: '',
    cart: { products: [], totalPrice: 0 },
    orders: [],
    activeProduct: null,
    activeComments: [],
    // Dashboard state (admin-only)
    dashboard: {
        allOrders: [],
        users: [],
        allComments: [],
        categoriesMap: {},
        usersMap: {},
        stats: {
            totalProducts: 0,
            totalOrders: 0,
            totalUsers: 0
        },
        loading: {
            products: false,
            orders: false,
            users: false,
            comments: false
        }
    }
};

// ================= THEME MANAGEMENT =================
function initTheme() {
    const savedTheme = localStorage.getItem('theme') || 'dark';
    document.documentElement.setAttribute('data-theme', savedTheme);
    updateThemeIcon(savedTheme);
    
    document.getElementById('themeToggle').addEventListener('click', () => {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        updateThemeIcon(newTheme);
        showToast('تم تغيير المظهر', `تفعيل الوضع ${newTheme === 'dark' ? 'الداكن' : 'المضيء'}`, 'info');
    });
}

function updateThemeIcon(theme) {
    const icon = document.getElementById('themeIcon');
    if (theme === 'dark') {
        icon.setAttribute('data-lucide', 'sun');
    } else {
        icon.setAttribute('data-lucide', 'moon');
    }
    if (window.lucide) {
        lucide.createIcons();
    }
}

// ================= JWT DECODER UTILITY =================
function decodeJwt(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        console.error('Error decoding JWT token:', e);
        return null;
    }
}

// Check if current user has Admin role
function isUserAdmin() {
    return getClaim('role') === 'Admin';
}

// Get claim values dynamically from decoded token
function getClaim(claimKey) {
    if (!state.decodedToken) return null;
    
    // Check direct key or standard claim schemas
    if (state.decodedToken[claimKey] !== undefined) {
        return state.decodedToken[claimKey];
    }
    
    const schemas = {
        userId: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
        username: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
        role: 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
    };
    
    return state.decodedToken[schemas[claimKey]] || null;
}

// ================= TOAST NOTIFICATION SYSTEM =================
function showToast(title, message, type = 'info') {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    
    let iconName = 'info';
    if (type === 'success') iconName = 'check-circle';
    if (type === 'error') iconName = 'alert-triangle';
    
    toast.innerHTML = `
        <i data-lucide="${iconName}"></i>
        <div class="toast-content">
            <div class="toast-title">${title}</div>
            <div class="toast-message">${message}</div>
        </div>
        <button class="toast-close" onclick="this.parentElement.remove()">
            <i data-lucide="x" style="width:16px;height:16px;"></i>
        </button>
    `;
    
    container.appendChild(toast);
    
    // Trigger Lucide parsing for new icons
    if (window.lucide) {
        lucide.createIcons({
            attrs: {
                class: 'lucide-icon'
            }
        });
    }
    
    // Animation in
    setTimeout(() => toast.classList.add('active'), 10);
    
    // Auto removal
    setTimeout(() => {
        toast.classList.remove('active');
        setTimeout(() => toast.remove(), 500);
    }, 4000);
}

// ================= API CALL WRAPPER =================
async function apiCall(endpoint, method = 'GET', body = null, useAuth = true) {
    const headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    };
    
    if (useAuth && state.token) {
        headers['Authorization'] = `Bearer ${state.token}`;
    }
    
    const config = {
        method,
        headers
    };
    
    if (body) {
        config.body = JSON.stringify(body);
    }
    
    try {
        const response = await fetch(`${API_BASE}${endpoint}`, config);
        
        // Handle unauthorized token issues
        if (response.status === 401) {
            showToast('انتهت الجلسة', 'يرجى تسجيل الدخول مرة أخرى', 'error');
            logout();
            return { ok: false, status: 401, errorMsg: 'Unauthorized access.' };
        }
        
        if (response.status === 204) {
            return { ok: true, status: 204, data: null };
        }
        
        const responseText = await response.text();
        let payload = null;
        try {
            payload = responseText ? JSON.parse(responseText) : null;
        } catch (e) {
            console.error('Failed to parse JSON response:', responseText);
            return { ok: response.ok, status: response.status, errorMsg: 'تنسيق استجابة غير صالح من الخادم.' };
        }
        
        if (!response.ok) {
            // Map ASP.NET model validation errors or direct messages
            let errorMsg = 'حدث خطأ غير متوقع.';
            if (payload) {
                if (payload.message) errorMsg = payload.message;
                else if (payload.errors) {
                    errorMsg = Object.values(payload.errors).flat().join(' | ');
                } else if (payload.title) {
                    errorMsg = payload.title;
                }
            }
            return { ok: false, status: response.status, data: payload, errorMsg };
        }
        
        // Handle standard ApiResponse<T> wrapped model
        if (payload && payload.hasOwnProperty('isSuccess')) {
            if (payload.isSuccess) {
                return { ok: true, status: response.status, data: payload.data, message: payload.message };
            } else {
                return { ok: false, status: response.status, errorMsg: payload.message || 'فشلت العملية' };
            }
        }
        
        return { ok: true, status: response.status, data: payload };
        
    } catch (error) {
        console.error('Fetch Error:', error);
        return { ok: false, status: 0, errorMsg: 'عذراً، فشل الاتصال بالخادم. يرجى التأكد من تشغيل المشروع الخلفي.' };
    }
}

// ================= AUTHENTICATION FLOW =================
async function login() {
    const usernameInput = document.getElementById('loginUsername');
    const passwordInput = document.getElementById('loginPassword');
    const submitBtn = document.getElementById('loginSubmitBtn');
    
    const username = usernameInput.value.trim();
    const password = passwordInput.value;
    
    if (!username || !password) {
        showToast('خطأ في الإدخال', 'يرجى إدخال اسم المستخدم وكلمة المرور', 'error');
        return;
    }
    
    submitBtn.disabled = true;
    submitBtn.innerHTML = `<i class="skeleton" style="width: 16px; height: 16px; border-radius: 50%;"></i> جاري التحقق...`;
    
    const { ok, data, errorMsg } = await apiCall('/Account/Login', 'POST', { userName: username, password }, false);
    
    submitBtn.disabled = false;
    submitBtn.innerHTML = `<i data-lucide="log-in"></i><span>دخول</span>`;
    if (window.lucide) lucide.createIcons();
    
    if (ok && data && data.token) {
        state.token = data.token;
        state.username = username;
        state.decodedToken = decodeJwt(data.token);
        
        localStorage.setItem('token', data.token);
        localStorage.setItem('username', username);
        
        showToast('مرحباً بك', `تم تسجيل الدخول بنجاح كـ ${username}`, 'success');
        showAppSection();
    } else {
        showToast('فشل تسجيل الدخول', errorMsg || 'اسم المستخدم أو كلمة المرور غير صحيحة', 'error');
    }
}

async function register() {
    const regUsername = document.getElementById('regUsername').value.trim();
    const regFullName = document.getElementById('regFullName').value.trim();
    const regEmail = document.getElementById('regEmail').value.trim();
    const regAddress = document.getElementById('regAddress').value.trim();
    const regPassword = document.getElementById('regPassword').value;
    const regConfirmPassword = document.getElementById('regConfirmPassword').value;
    const submitBtn = document.getElementById('regSubmitBtn');
    
    if (!regUsername || !regFullName || !regEmail || !regAddress || !regPassword || !regConfirmPassword) {
        showToast('خطأ في الإدخال', 'جميع الحقول مطلوبة لإنشاء حساب', 'error');
        return;
    }
    
    if (regPassword !== regConfirmPassword) {
        showToast('خطأ في الإدخال', 'كلمتا المرور غير متطابقتين', 'error');
        return;
    }
    
    submitBtn.disabled = true;
    submitBtn.innerHTML = `<i class="skeleton" style="width: 16px; height: 16px; border-radius: 50%;"></i> جاري الإنشاء...`;
    
    const payload = {
        userName: regUsername,
        fullName: regFullName,
        email: regEmail,
        address: regAddress,
        password: regPassword,
        confirmPassword: regConfirmPassword
    };
    
    const { ok, errorMsg } = await apiCall('/Account/Register', 'POST', payload, false);
    
    submitBtn.disabled = false;
    submitBtn.innerHTML = `<i data-lucide="user-plus"></i><span>إنشاء حساب</span>`;
    if (window.lucide) lucide.createIcons();
    
    if (ok) {
        showToast('تم التسجيل بنجاح', 'تم إنشاء حسابك الجديد! يمكنك الآن تسجيل الدخول.', 'success');
        document.getElementById('registerForm').reset();
    } else {
        showToast('فشل إنشاء الحساب', errorMsg, 'error');
    }
}

function logout() {
    state.token = null;
    state.username = null;
    state.decodedToken = null;
    
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    
    document.getElementById('loginForm').reset();
    document.getElementById('registerForm').reset();
    
    document.getElementById('appSection').classList.add('hidden');
    document.getElementById('authSection').classList.remove('hidden');
    document.getElementById('logoutBtn').style.display = 'none';
    document.getElementById('navUserBadge').style.display = 'none';
    
    showToast('تم تسجيل الخروج', 'إلى اللقاء مجدداً!', 'info');
}

// ================= APP INITIALIZATION & NAVIGATION =================
function showAppSection() {
    state.decodedToken = decodeJwt(state.token);
    
    document.getElementById('authSection').classList.add('hidden');
    document.getElementById('appSection').classList.remove('hidden');
    
    document.getElementById('logoutBtn').style.display = 'inline-flex';
    document.getElementById('navUserBadge').style.display = 'inline-flex';
    document.getElementById('navUsername').textContent = state.username;
    
    // Show/hide dashboard tab based on admin role
    const dashboardTab = document.getElementById('tab-dashboard');
    if (dashboardTab) {
        dashboardTab.style.display = isUserAdmin() ? 'inline-flex' : 'none';
    }
    
    // Navigate default to products
    switchTab('products');
    loadCartCount();
}

function switchTab(tabId) {
    state.activeTab = tabId;
    
    // Manage tab button active classes
    document.querySelectorAll('.tab-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    const activeBtn = document.getElementById(`tab-${tabId}`);
    if (activeBtn) activeBtn.classList.add('active');
    
    // Hide all tab views and show the active one
    document.querySelectorAll('.tab-view').forEach(view => {
        view.classList.add('hidden');
    });
    const activeView = document.getElementById(`view-${tabId}`);
    if (activeView) activeView.classList.remove('hidden');
    
    // Route tab actions
    if (tabId === 'products') {
        loadProductsAndCategories();
    } else if (tabId === 'cart') {
        loadCartItems();
    } else if (tabId === 'orders') {
        loadOrdersHistory();
    } else if (tabId === 'profile') {
        loadProfileData();
    } else if (tabId === 'dashboard') {
        loadDashboardData();
    }
}

// ================= PRODUCTS & CATEGORIES MODULE =================
async function loadProductsAndCategories() {
    const grid = document.getElementById('productsGridList');
    
    // Render Skeletons Loader while fetching
    grid.innerHTML = Array(6).fill().map(() => `
        <div class="product-card">
            <div class="skeleton skeleton-img"></div>
            <div class="skeleton skeleton-title"></div>
            <div class="skeleton skeleton-text" style="width: 90%;"></div>
            <div class="skeleton skeleton-text" style="width: 70%;"></div>
            <div class="skeleton skeleton-btn" style="margin-top: 1rem;"></div>
        </div>
    `).join('');
    
    // Fetch items in parallel
    const [productsRes, categoriesRes] = await Promise.all([
        apiCall('/Product'),
        apiCall('/Category')
    ]);
    
    if (productsRes.ok) {
        state.products = productsRes.data || [];
    } else {
        showToast('حدث خطأ في جلب المنتجات', productsRes.errorMsg, 'error');
        grid.innerHTML = `<div class="empty-state" style="grid-column: 1/-1;"><i data-lucide="info"></i><p>فشل جلب المنتجات</p></div>`;
        if (window.lucide) lucide.createIcons();
        return;
    }
    
    if (categoriesRes.ok) {
        state.categories = categoriesRes.data || [];
        renderCategoryFilters();
    }
    
    renderProducts();
}

function renderCategoryFilters() {
    const container = document.getElementById('categoryPillsList');
    
    let pillsHtml = `<button class="pill-btn ${state.activeCategory === null ? 'active' : ''}" onclick="filterByCategory(null)">الكل</button>`;
    
    state.categories.forEach(cat => {
        // Look up Category Name
        pillsHtml += `<button class="pill-btn ${state.activeCategory === cat.id ? 'active' : ''}" onclick="filterByCategory(${cat.id})">${cat.name}</button>`;
    });
    
    container.innerHTML = pillsHtml;
}

function filterByCategory(catId) {
    state.activeCategory = catId;
    renderCategoryFilters();
    renderProducts();
}

function renderProducts() {
    const grid = document.getElementById('productsGridList');
    
    // Apply client filters (Category + Search query)
    let filtered = state.products;
    
    if (state.activeCategory !== null) {
        filtered = filtered.filter(p => p.categoryID === state.activeCategory);
    }
    
    if (state.searchQuery) {
        const query = state.searchQuery.toLowerCase();
        filtered = filtered.filter(p => p.name.toLowerCase().includes(query) || p.description.toLowerCase().includes(query));
    }
    
    if (filtered.length === 0) {
        grid.innerHTML = `
            <div class="empty-state" style="grid-column: 1/-1;">
                <i data-lucide="shopping-bag"></i>
                <p>لا توجد منتجات مطابقة لخيارات البحث الحالية</p>
            </div>
        `;
        if (window.lucide) lucide.createIcons();
        return;
    }
    
    grid.innerHTML = filtered.map(p => {
        // Resolve Category display
        const cat = state.categories.find(c => c.id === p.categoryID);
        const catName = cat ? cat.name : 'متنوع';
        
        // Show out-of-stock badge if stock is 0
        const outOfStock = p.quantity <= 0;
        const badge = outOfStock ? `<span class="product-badge" style="background-color: var(--error);">نفدت الكمية</span>` : '';
        
        return `
            <div class="product-card" onclick="openProductDetails(${p.id}, event)">
                ${badge}
                <h3 class="product-title">${p.name}</h3>
                <p class="product-desc">${p.description}</p>
                <div class="product-price-row">
                    <span class="product-price">$${p.price}</span>
                    <span class="product-category">${catName}</span>
                </div>
                <div class="product-footer" onclick="event.stopPropagation()">
                    <input type="number" id="qty-${p.id}" value="1" min="1" max="${outOfStock ? 0 : 99}" class="form-group qty-input" style="margin-bottom:0;" ${outOfStock ? 'disabled' : ''}>
                    <button class="btn btn-success btn-sm" onclick="addProductToCart(${p.id})" style="flex:1;" ${outOfStock ? 'disabled' : ''}>
                        <i data-lucide="shopping-cart"></i>
                        <span>أضف للسلة</span>
                    </button>
                </div>
            </div>
        `;
    }).join('');
    
    if (window.lucide) lucide.createIcons();
}

// ================= SHOPPING CART MODULE =================
async function loadCartCount() {
    const { ok, data } = await apiCall('/Cart');
    const badge = document.getElementById('cartBadgeCount');
    if (ok && data && data.products) {
        badge.textContent = data.products.length;
        badge.style.display = data.products.length > 0 ? 'inline-block' : 'none';
    } else {
        badge.style.display = 'none';
    }
}

async function addProductToCart(productId) {
    const qtyInput = document.getElementById(`qty-${productId}`);
    const quantity = parseInt(qtyInput.value) || 1;
    
    const { ok, errorMsg } = await apiCall('/Cart', 'POST', { productId, quantity });
    
    if (ok) {
        showToast('تمت الإضافة', 'تمت إضافة المنتج بنجاح إلى سلة المشتريات', 'success');
        loadCartCount();
    } else {
        showToast('خطأ في الإضافة', errorMsg, 'error');
    }
}

async function loadCartItems() {
    const listContainer = document.getElementById('cartItemsListContainer');
    const itemsCountEl = document.getElementById('cartSummaryItemsCount');
    const totalValEl = document.getElementById('cartSummaryTotalPrice');
    
    listContainer.innerHTML = `
        <div class="skeleton skeleton-title" style="width:40%;"></div>
        <div class="skeleton skeleton-text"></div>
        <div class="skeleton skeleton-text" style="width:80%;"></div>
    `;
    
    const { ok, data, errorMsg } = await apiCall('/Cart');
    
    if (!ok) {
        showToast('فشل جلب السلة', errorMsg, 'error');
        listContainer.innerHTML = `<div class="empty-state"><i data-lucide="alert-circle"></i><p>فشل جلب سلة المشتريات</p></div>`;
        itemsCountEl.textContent = '0';
        totalValEl.textContent = '$0.00';
        if (window.lucide) lucide.createIcons();
        return;
    }
    
    state.cart = data || { products: [], totalPrice: 0 };
    itemsCountEl.textContent = state.cart.products.length;
    totalValEl.textContent = `$${state.cart.totalPrice}`;
    
    if (state.cart.products.length === 0) {
        listContainer.innerHTML = `
            <div class="empty-state">
                <i data-lucide="shopping-cart"></i>
                <p>سلة التسوق فارغة تماماً. ابدأ بإضافة بعض المنتجات المتاحة!</p>
                <button class="btn btn-secondary btn-sm" onclick="switchTab('products')">تسوق الآن</button>
            </div>
        `;
        if (window.lucide) lucide.createIcons();
        return;
    }
    
    listContainer.innerHTML = state.cart.products.map(item => `
        <div class="cart-item-row">
            <div class="cart-item-info">
                <h4 class="cart-item-name">${item.productName}</h4>
                <div class="cart-item-meta">السعر الفردي: $${item.price} | الكمية المطلوبة: ${item.quantity}</div>
            </div>
            <div class="cart-item-actions">
                <span class="cart-item-price">$${item.subTotal}</span>
                <button class="btn btn-danger btn-circle btn-sm" onclick="deleteCartItem(${item.id})" title="حذف من السلة">
                    <i data-lucide="trash-2" style="width:16px;height:16px;"></i>
                </button>
            </div>
        </div>
    `).join('');
    
    if (window.lucide) lucide.createIcons();
}

async function deleteCartItem(itemId) {
    const { ok, errorMsg } = await apiCall(`/Cart/${itemId}`, 'DELETE');
    if (ok) {
        showToast('تم الحذف', 'تم إزالة السلعة من السلة', 'success');
        loadCartItems();
        loadCartCount();
    } else {
        showToast('فشل الحذف', errorMsg, 'error');
    }
}

async function runCheckout() {
    const checkoutBtn = document.getElementById('checkoutBtn');
    if (state.cart.products.length === 0) {
        showToast('السلة فارغة', 'لا توجد منتجات في السلة لإتمام الطلب', 'warning');
        return;
    }
    
    checkoutBtn.disabled = true;
    checkoutBtn.innerHTML = `<i class="skeleton" style="width:16px;height:16px;border-radius:50%;"></i> جاري معالجة الطلب...`;
    
    const { ok, data, errorMsg } = await apiCall('/Order/checkout', 'POST');
    
    checkoutBtn.disabled = false;
    checkoutBtn.innerHTML = `<i data-lucide="credit-card"></i><span>إتمام عملية الشراء</span>`;
    if (window.lucide) lucide.createIcons();
    
    if (ok && data) {
        showToast('تم الشراء بنجاح', `تم إنشاء طلبك رقم #${data.id} بنجاح!`, 'success');
        loadCartCount();
        switchTab('orders');
    } else {
        showToast('فشل إتمام الشراء', errorMsg || 'انتهى المخزون أو السلة فارغة', 'error');
    }
}

// ================= ORDER HISTORY MODULE =================
async function loadOrdersHistory() {
    const container = document.getElementById('ordersListContainer');
    
    container.innerHTML = `
        <div class="card skeleton" style="height:120px;margin-bottom:1rem;"></div>
        <div class="card skeleton" style="height:120px;"></div>
    `;
    
    const { ok, data, errorMsg } = await apiCall('/Order/History');
    
    if (!ok) {
        showToast('فشل جلب الطلبات', errorMsg, 'error');
        container.innerHTML = `<div class="empty-state"><i data-lucide="alert-circle"></i><p>فشل جلب سجل الطلبات</p></div>`;
        if (window.lucide) lucide.createIcons();
        return;
    }
    
    state.orders = data || [];
    
    if (state.orders.length === 0) {
        container.innerHTML = `
            <div class="empty-state card">
                <i data-lucide="package"></i>
                <p>لا يوجد لديك أي طلبات سابقة حتى الآن.</p>
            </div>
        `;
        if (window.lucide) lucide.createIcons();
        return;
    }
    
    container.innerHTML = state.orders.map(order => {
        // Parse date
        const dateObj = new Date(order.orderDate);
        const formattedDate = isNaN(dateObj) ? 'غير محدد' : dateObj.toLocaleString('ar-EG', { dateStyle: 'medium', timeStyle: 'short' });
        
        // Match status class
        let statusClass = 'status-pending';
        let statusText = 'قيد الانتظار';
        
        const statusClean = (order.status || 'Pending').toLowerCase().trim();
        if (statusClean === 'completed' || statusClean === 'مكتمل') {
            statusClass = 'status-completed';
            statusText = 'مكتمل';
        } else if (statusClean === 'shipped' || statusClean === 'تم الشحن') {
            statusClass = 'status-shipped';
            statusText = 'تم الشحن';
        } else if (statusClean === 'cancelled' || statusClean === 'ملغي') {
            statusClass = 'status-cancelled';
            statusText = 'ملغي';
        } else {
            statusText = order.status || 'قيد الانتظار';
        }
        
        return `
            <div class="card order-card">
                <div class="order-header">
                    <div>
                        <strong>طلب رقم #${order.id}</strong>
                        <span style="color:var(--text-secondary); margin-right:15px; font-size:0.85rem;">التاريخ: ${formattedDate}</span>
                    </div>
                    <span class="status-badge ${statusClass}">${statusText}</span>
                </div>
                <div class="order-items-list">
                    ${order.items.map(item => `
                        <div class="order-item-detail">
                            <span><i data-lucide="check" style="width:14px;height:14px;vertical-align:middle;margin-left:5px;"></i> ${item}</span>
                        </div>
                    `).join('')}
                </div>
                <div class="order-footer">
                    <span>القيمة الكلية للطلب:</span>
                    <strong style="font-size:1.2rem; color:var(--success);">$${order.totalPrice}</strong>
                </div>
            </div>
        `;
    }).join('');
    
    if (window.lucide) lucide.createIcons();
}

// ================= COMMENTS PANEL MODULE =================
async function openProductDetails(productId, event) {
    // Prevent modal triggering from internal buttons on cards
    if (event && event.target.closest('.product-footer')) return;
    
    const modal = document.getElementById('productDetailsModal');
    const prod = state.products.find(p => p.id === productId);
    if (!prod) return;
    
    state.activeProduct = prod;
    
    // Inject metadata
    document.getElementById('modalProductTitle').textContent = prod.name;
    document.getElementById('modalProductPrice').textContent = `$${prod.price}`;
    document.getElementById('modalProductDesc').textContent = prod.description;
    
    const cat = state.categories.find(c => c.id === prod.categoryID);
    document.getElementById('modalProductCategory').textContent = cat ? cat.name : 'متنوع';
    
    const stockEl = document.getElementById('modalProductStock');
    const qtyInput = document.getElementById('modalQtyInput');
    const addBtn = document.getElementById('modalAddToCartBtn');
    
    if (prod.quantity <= 0) {
        stockEl.textContent = 'نفدت الكمية المتوفرة';
        stockEl.style.color = 'var(--error)';
        qtyInput.disabled = true;
        qtyInput.max = 0;
        qtyInput.value = 0;
        addBtn.disabled = true;
    } else {
        stockEl.textContent = `الكمية المتوفرة: ${prod.quantity}`;
        stockEl.style.color = 'var(--text-secondary)';
        qtyInput.disabled = false;
        qtyInput.max = prod.quantity;
        qtyInput.value = 1;
        addBtn.disabled = false;
    }
    
    // Trigger comment loading
    await loadComments(productId);
    
    // Display Modal
    modal.classList.add('active');
    document.body.style.overflow = 'hidden';
}

function closeProductDetailsModal() {
    const modal = document.getElementById('productDetailsModal');
    modal.classList.remove('active');
    document.body.style.overflow = 'auto';
    state.activeProduct = null;
    state.activeComments = [];
    document.getElementById('newCommentInput').value = '';
}

async function loadComments(productId) {
    const commentsContainer = document.getElementById('modalCommentsList');
    const countBadge = document.getElementById('commentsCountBadge');
    
    commentsContainer.innerHTML = `<div class="skeleton skeleton-text"></div><div class="skeleton skeleton-text" style="width:60%;"></div>`;
    countBadge.textContent = '0';
    
    const { ok, data } = await apiCall(`/Comment/${productId}`);
    
    if (ok && data && data.comments) {
        state.activeComments = data.comments;
        countBadge.textContent = data.comments.length;
        
        if (data.comments.length === 0) {
            commentsContainer.innerHTML = `<div class="empty-state" style="padding:1.5rem;"><p style="font-size:0.9rem;">لا توجد تعليقات بعد لهذا المنتج. كن أول من يعلق!</p></div>`;
            return;
        }
        
        commentsContainer.innerHTML = data.comments.map(c => {
            const dateObj = new Date(c.createdAt);
            const formattedDate = isNaN(dateObj) ? 'قبل قليل' : dateObj.toLocaleString('ar-EG', { dateStyle: 'short', timeStyle: 'short' });
            
            return `
                <div class="comment-bubble">
                    <div class="comment-info">
                        <div class="comment-text">${c.comment}</div>
                        <div class="comment-date">${formattedDate}</div>
                    </div>
                    <button class="btn btn-secondary btn-circle btn-sm" onclick="deleteComment(${c.id})" style="border:none;background:transparent;color:var(--error);" title="حذف التعليق">
                        <i data-lucide="trash-2" style="width:14px;height:14px;"></i>
                    </button>
                </div>
            `;
        }).join('');
        
        if (window.lucide) lucide.createIcons();
    } else {
        commentsContainer.innerHTML = `<p style="font-size:0.9rem;color:var(--error);text-align:center;">فشل تحميل مراجعات المنتج.</p>`;
    }
}

async function postComment() {
    const input = document.getElementById('newCommentInput');
    const submitBtn = document.getElementById('submitCommentBtn');
    const text = input.value.trim();
    
    if (!text || !state.activeProduct) return;
    
    submitBtn.disabled = true;
    
    // Note: Backend expects "description" field in payload
    const payload = {
        productId: state.activeProduct.id,
        description: text
    };
    
    const { ok, errorMsg } = await apiCall('/Comment', 'POST', payload);
    
    submitBtn.disabled = false;
    if (ok) {
        showToast('تم نشر التعليق', 'شكراً لمشاركتك رأيك حول المنتج!', 'success');
        input.value = '';
        loadComments(state.activeProduct.id);
    } else {
        showToast('فشل نشر التعليق', errorMsg, 'error');
    }
}

async function deleteComment(commentId) {
    const { ok, errorMsg } = await apiCall(`/Comment/${commentId}`, 'DELETE');
    
    if (ok) {
        showToast('تم حذف التعليق', 'تمت إزالة مراجعتك بنجاح', 'success');
        if (state.activeProduct) {
            loadComments(state.activeProduct.id);
        }
    } else {
        showToast('فشل حذف التعليق', 'عذراً، لا يمكنك حذف هذا التعليق لأنه لا يخص حسابك!', 'error');
    }
}

// ================= PROFILE MANAGEMENT MODULE =================
async function loadProfileData() {
    const form = document.getElementById('profileForm');
    const saveBtn = document.getElementById('profileSaveBtn');
    const initialsEl = document.getElementById('profileInitials');
    const fullnameDisplayEl = document.getElementById('profileFullnameDisplay');
    const roleDisplayEl = document.getElementById('profileRoleDisplay');
    
    saveBtn.disabled = true;
    
    const { ok, data, errorMsg } = await apiCall('/Profile');
    
    saveBtn.disabled = false;
    
    if (!ok) {
        showToast('فشل جلب الملف الشخصي', errorMsg, 'error');
        return;
    }
    
    // Populate Form fields
    document.getElementById('profileUsername').value = data.userName || state.username;
    document.getElementById('profileFullName').value = data.fullName || '';
    document.getElementById('profileEmail').value = data.email || '';
    document.getElementById('profileAddress').value = data.address || '';
    
    // Set initials from full name
    const nameStr = data.fullName || state.username || '؟';
    initialsEl.textContent = nameStr.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase();
    fullnameDisplayEl.textContent = data.fullName || state.username;
    
    // Decode user role from claims
    const userRole = getClaim('role') || 'عميل';
    roleDisplayEl.textContent = userRole === 'Admin' ? 'مدير النظام (Admin)' : 'عميل متميز (User)';
}

async function saveProfileChanges() {
    const saveBtn = document.getElementById('profileSaveBtn');
    const fullName = document.getElementById('profileFullName').value.trim();
    const email = document.getElementById('profileEmail').value.trim();
    const address = document.getElementById('profileAddress').value.trim();
    
    if (!fullName || !email || !address) {
        showToast('خطأ في الإدخال', 'يرجى ملء جميع الحقول المطلوبة', 'error');
        return;
    }
    
    saveBtn.disabled = true;
    saveBtn.innerHTML = `<i class="skeleton" style="width:16px;height:16px;border-radius:50%;"></i> جاري الحفظ...`;
    
    const payload = { fullName, email, address };
    const { ok, errorMsg } = await apiCall('/Profile', 'PATCH', payload);
    
    saveBtn.disabled = false;
    saveBtn.innerHTML = `<i data-lucide="save"></i><span>حفظ التعديلات</span>`;
    if (window.lucide) lucide.createIcons();
    
    if (ok) {
        showToast('تم الحفظ', 'تم تحديث بياناتك الشخصية بنجاح', 'success');
        loadProfileData();
    } else {
        showToast('فشل التحديث', errorMsg, 'error');
    }
}

// ================= DASHBOARD MODULE (ADMIN) =================
async function loadDashboardData() {
    const dashboardView = document.getElementById('view-dashboard');
    const container = document.getElementById('dashboardContainer');
    
    // Show loading state
    container.innerHTML = `
        <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; margin-bottom: 2rem;">
            <div class="skeleton card" style="height: 80px;"></div>
            <div class="skeleton card" style="height: 80px;"></div>
            <div class="skeleton card" style="height: 80px;"></div>
        </div>
        <div class="skeleton card" style="height: 200px; margin-bottom: 1rem;"></div>
        <div class="skeleton card" style="height: 200px;"></div>
    `;
    
    // Fetch all required data in parallel
    const [productsRes, ordersRes, usersRes, categoriesRes] = await Promise.all([
        apiCall('/Product'),
        apiCall('/Order/admin/History'),
        apiCall('/Profile/Profiles'),
        apiCall('/Category')
    ]);
    
    // Process products
    if (productsRes.ok) {
        state.products = productsRes.data || [];
        state.dashboard.stats.totalProducts = state.products.length;
    }
    
    // Process categories and build lookup map
    // Note: ShowCategoryDto is missing 'id' field - this is a backend limitation
    // We map by using categoryID -> index position in categories array
    if (categoriesRes.ok && categoriesRes.data) {
        state.categories = categoriesRes.data;
        state.dashboard.categoriesMap = {};
        // Map by index - assuming categories are returned in consistent order
        state.categories.forEach((c, idx) => {
            state.dashboard.categoriesMap[idx + 1] = c.name; // 1-based index
        });
    }
    
    // Process orders
    if (ordersRes.ok) {
        state.dashboard.allOrders = ordersRes.data || [];
        state.dashboard.stats.totalOrders = state.dashboard.allOrders.length;
    }
    
    // Process users
    if (usersRes.ok) {
        state.dashboard.users = usersRes.data || [];
        state.dashboard.stats.totalUsers = state.dashboard.users.length;
        // Build user lookup map (note: GetProfileForAdminDto has UserName, not UserId)
        state.dashboard.usersMap = {};
        state.dashboard.users.forEach((u, idx) => {
            state.dashboard.usersMap[idx] = u;
        });
    }
    
    // Render dashboard
    renderDashboard();
    
    // Load comments after initial render
    await loadAllProductComments();
    renderDashboard(); // Re-render with comments
}

function renderDashboard() {
    const container = document.getElementById('dashboardContainer');
    
    // Statistics cards
    const statsHtml = `
        <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; margin-bottom: 2rem;">
            <div class="card" style="text-align: center; padding: 1.5rem;">
                <div style="font-size: 2.5rem; font-weight: 800; color: var(--primary);">${state.dashboard.stats.totalProducts}</div>
                <div style="color: var(--text-secondary);">إجمالي المنتجات</div>
            </div>
            <div class="card" style="text-align: center; padding: 1.5rem;">
                <div style="font-size: 2.5rem; font-weight: 800; color: var(--success);">${state.dashboard.stats.totalOrders}</div>
                <div style="color: var(--text-secondary);">إجمالي الطلبات</div>
            </div>
            <div class="card" style="text-align: center; padding: 1.5rem;">
                <div style="font-size: 2.5rem; font-weight: 800; color: var(--warning);">${state.dashboard.stats.totalUsers}</div>
                <div style="color: var(--text-secondary);">إجمالي المستخدمين</div>
            </div>
        </div>
    `;
    
    // Product management section
    const productsHtml = renderProductTableAdmin();
    
    // Comment management section (aggregated across all products)
    const commentsHtml = renderCommentAdminTable();
    
    // Order management section
    const ordersHtml = renderOrderTableAdmin();
    
    container.innerHTML = `
        ${statsHtml}
        <div class="card" style="margin-bottom: 1.5rem;">
            <h2 class="card-title">إدارة المنتجات</h2>
            ${productsHtml}
        </div>
        <div class="card" style="margin-bottom: 1.5rem;">
            <h2 class="card-title">إدارة التعليقات (${state.dashboard.allComments?.length || 0})</h2>
            ${commentsHtml}
        </div>
        <div class="card">
            <h2 class="card-title">إدارة الطلبات</h2>
            ${ordersHtml}
        </div>
    `;
    
    if (window.lucide) lucide.createIcons();
}

function renderProductTableAdmin() {
    if (!state.products || state.products.length === 0) {
        return `<div class="empty-state"><i data-lucide="package"></i><p>لا توجد منتجات لعرضها</p></div>`;
    }
    
    return `
        <div style="overflow-x: auto;">
            <table class="admin-table">
                <thead>
                    <tr>
                        <th>الاسم</th>
                        <th>الفئة</th>
                        <th>السعر</th>
                        <th>الكمية</th>
                        <th>الإجراءات</th>
                    </tr>
                </thead>
                <tbody>
                    ${state.products.map(p => `
                        <tr>
                            <td>${p.name}</td>
                            <td>${state.dashboard.categoriesMap[p.categoryID] || 'متنوع'}</td>
                            <td>$${p.price}</td>
                            <td>${p.quantity}</td>
                            <td>
                                <button class="btn btn-secondary btn-sm" onclick="openEditProductModal(${p.id})" title="تعديل">
                                    <i data-lucide="edit-2" style="width:14px;height:14px;"></i>
                                </button>
                                <button class="btn btn-danger btn-sm" onclick="deleteProductAdmin(${p.id})" title="حذف" style="margin-right:0.5rem;">
                                    <i data-lucide="trash-2" style="width:14px;height:14px;"></i>
                                </button>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    `;
}

function renderOrderTableAdmin() {
    if (!state.dashboard.allOrders || state.dashboard.allOrders.length === 0) {
        return `<div class="empty-state"><i data-lucide="package"></i><p>لا توجد طلبات لعرضها</p></div>`;
    }
    
    return `
        <div style="overflow-x: auto;">
            <table class="admin-table">
                <thead>
                    <tr>
                        <th>رقم الطلب</th>
                        <th>التاريخ</th>
                        <th>القيمة</th>
                        <th>الحالة</th>
                        <th>الإجراءات</th>
                    </tr>
                </thead>
                <tbody>
                    ${state.dashboard.allOrders.map(o => {
                        const statusClass = getStatusClass(o.status);
                        return `
                            <tr>
                                <td>#${o.id}</td>
                                <td>${formatDate(o.orderDate)}</td>
                                <td>$${o.totalPrice}</td>
                                <td><span class="status-badge ${statusClass}">${o.status || 'Pending'}</span></td>
                                <td>
                                    <select onchange="updateOrderStatus(${o.id}, this.value)" style="padding:0.25rem; border-radius:var(--border-radius-sm); border:1px solid var(--border-color);">
                                        <option value="Pending" ${o.status === 'Pending' ? 'selected' : ''}>Pending</option>
                                        <option value="Shipped" ${o.status === 'Shipped' ? 'selected' : ''}>Shipped</option>
                                        <option value="Completed" ${o.status === 'Completed' ? 'selected' : ''}>Completed</option>
                                        <option value="Cancelled" ${o.status === 'Cancelled' ? 'selected' : ''}>Cancelled</option>
                                    </select>
                                </td>
                            </tr>
                        `;
                    }).join('')}
                </tbody>
            </table>
        </div>
    `;
}

function getStatusClass(status) {
    const s = (status || '').toLowerCase();
    if (s === 'completed' || s === 'مكتمل') return 'status-completed';
    if (s === 'shipped' || s === 'تم الشحن') return 'status-shipped';
    if (s === 'cancelled' || s === 'ملغي') return 'status-cancelled';
    return 'status-pending';
}

function formatDate(dateString) {
    const dateObj = new Date(dateString);
    return isNaN(dateObj) ? 'غير محدد' : dateObj.toLocaleDateString('ar-EG');
}

// Product edit modal
function openEditProductModal(productId) {
    const product = state.products.find(p => p.id === productId);
    if (!product) return;
    
    state.dashboard.editingProduct = product;
    
    document.getElementById('editProductName').value = product.name;
    document.getElementById('editProductDesc').value = product.description;
    document.getElementById('editProductPrice').value = product.price;
    document.getElementById('editProductQty').value = product.quantity;
    document.getElementById('editProductCategoryDisplay').value = state.dashboard.categoriesMap[product.categoryID] || 'غير محدد';
    
    document.getElementById('editProductModal').classList.add('active');
    document.body.style.overflow = 'hidden';
}

function closeEditProductModal() {
    document.getElementById('editProductModal').classList.remove('active');
    document.body.style.overflow = 'auto';
    state.dashboard.editingProduct = null;
}

async function saveProductChanges() {
    if (!state.dashboard.editingProduct) return;
    
    const product = state.dashboard.editingProduct;
    const payload = {
        name: document.getElementById('editProductName').value,
        description: document.getElementById('editProductDesc').value,
        price: parseInt(document.getElementById('editProductPrice').value) || product.price,
        quantity: parseInt(document.getElementById('editProductQty').value) || product.quantity,
        categoryID: product.categoryID // Read-only per requirements
    };
    
    const { ok, errorMsg } = await apiCall(`/Product/${product.id}`, 'PUT', payload);
    
    if (ok) {
        showToast('تم التحديث', 'تم تحديث المنتج بنجاح', 'success');
        closeEditProductModal();
        loadDashboardData();
    } else {
        showToast('فشل التحديث', errorMsg, 'error');
    }
}

async function deleteProductAdmin(productId) {
    const product = state.products.find(p => p.id === productId);
    if (!product || !confirm(`هل أنت متأكد من حذف المنتج "${product.name}"؟`)) return;
    
    const { ok, errorMsg } = await apiCall(`/Product/${productId}`, 'DELETE');
    
    if (ok) {
        showToast('تم الحذف', 'تم حذف المنتج بنجاح', 'success');
        loadDashboardData();
    } else {
        showToast('فشل الحذف', errorMsg, 'error');
    }
}

async function updateOrderStatus(orderId, newStatus) {
    if (!confirm(`هل تريد تغيير حالة الطلب #${orderId} إلى "${newStatus}"؟`)) return;
    
    const { ok, errorMsg } = await apiCall(`/Order/${orderId}/Status?Status=${newStatus}`, 'PATCH');
    
    if (ok) {
        showToast('تم التحديث', 'تم تحديث حالة الطلب بنجاح', 'success');
        loadDashboardData();
    } else {
        showToast('فشل التحديث', errorMsg, 'error');
    }
}

// Admin comment aggregation and rendering
async function loadAllProductComments() {
    const comments = [];
    if (!state.products || state.products.length === 0) return;
    
    const commentPromises = state.products.map(async (p) => {
        const { ok, data } = await apiCall(`/Comment/${p.id}`);
        if (ok && data && data.comments) {
            return data.comments.map(c => ({ ...c, productName: p.name }));
        }
        return [];
    });
    
    const results = await Promise.all(commentPromises);
    results.forEach(res => comments.push(...res));
    state.dashboard.allComments = comments;
}

function renderCommentAdminTable() {
    const comments = state.dashboard.allComments || [];
    
    if (comments.length === 0) {
        return `<div class="empty-state"><i data-lucide="message-circle"></i><p>لا توجد تعليقات لعرضها</p></div>`;
    }
    
    return `
        <div style="overflow-x: auto; max-height: 300px;">
            <table class="admin-table">
                <thead>
                    <tr>
                        <th>المنتج</th>
                        <th>التعليق</th>
                        <th>التاريخ</th>
                        <th>الإجراءات</th>
                    </tr>
                </thead>
                <tbody>
                    ${comments.map(c => {
                        const date = formatDate(c.createdAt);
                        return `
                            <tr>
                                <td>${c.productName || 'غير محدد'}</td>
                                <td style="max-width: 300px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">${c.comment}</td>
                                <td>${date}</td>
                                <td>
                                    <button class="btn btn-danger btn-sm" onclick="deleteCommentAdmin(${c.id})" title="حذف">
                                        <i data-lucide="trash-2" style="width:14px;height:14px;"></i>
                                    </button>
                                </td>
                            </tr>
                        `;
                    }).join('')}
                </tbody>
            </table>
        </div>
    `;
}

async function deleteCommentAdmin(commentId) {
    if (!confirm('هل أنت متأكد من حذف هذا التعليق؟')) return;
    
    const { ok, errorMsg } = await apiCall(`/Comment/${commentId}`, 'DELETE');
    
    if (ok) {
        showToast('تم الحذف', 'تم حذف التعليق بنجاح', 'success');
        loadDashboardData();
    } else {
        showToast('فشل الحذف', errorMsg || 'لا يمكنك حذف هذا التعليق', 'error');
    }
}

// ================= APP INITIALIZATION EVENT BINDINGS =================
document.addEventListener('DOMContentLoaded', () => {
    // 1. Initialize visual theme
    initTheme();
    
    // 2. Bind authentication forms submit
    document.getElementById('loginForm').addEventListener('submit', login);
    document.getElementById('registerForm').addEventListener('submit', register);
    document.getElementById('logoutBtn').addEventListener('click', logout);
    
    // Bind tab navigation clicks
    document.getElementById('tab-products').addEventListener('click', () => switchTab('products'));
    document.getElementById('tab-cart').addEventListener('click', () => switchTab('cart'));
    document.getElementById('tab-orders').addEventListener('click', () => switchTab('orders'));
    document.getElementById('tab-profile').addEventListener('click', () => switchTab('profile'));
    document.getElementById('tab-dashboard').addEventListener('click', () => switchTab('dashboard'));
    
    // 4. Bind catalog search
    const searchInput = document.getElementById('productSearchInput');
    let searchTimeout = null;
    searchInput.addEventListener('input', (e) => {
        state.searchQuery = e.target.value;
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => {
            renderProducts();
        }, 300); // Debounce search changes for typing fluidity
    });
    
    // 5. Bind cart operations
    document.getElementById('checkoutBtn').addEventListener('click', runCheckout);
    
    // 6. Bind product details modal events
    document.getElementById('closeModalBtn').addEventListener('click', closeProductDetailsModal);
    
    // Close modal when clicking outer backdrop overlay
    document.getElementById('productDetailsModal').addEventListener('click', (e) => {
        if (e.target.id === 'productDetailsModal') {
            closeProductDetailsModal();
        }
    });
    
    // Add to cart from modal
    document.getElementById('modalAddToCartBtn').addEventListener('click', () => {
        if (state.activeProduct) {
            const qty = parseInt(document.getElementById('modalQtyInput').value) || 1;
            // Set qty on catalog input to match and reuse handler
            const catQtyInput = document.getElementById(`qty-${state.activeProduct.id}`);
            if (catQtyInput) catQtyInput.value = qty;
            addProductToCart(state.activeProduct.id);
        }
    });
    
    // Post comment from modal
    document.getElementById('addCommentForm').addEventListener('submit', postComment);
    
    // Bind profile editing
    document.getElementById('profileForm').addEventListener('submit', saveProfileChanges);
    
    // 7. Auto login verification
    if (state.token) {
        showAppSection();
    }
});







