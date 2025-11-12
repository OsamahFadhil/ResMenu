# 🔴 CRITICAL DEBUG STEPS

## The Real Problems

Based on your screenshots, here are the ACTUAL issues:

1. **Settings page is completely empty** - Not even loading
2. **Templates page shows theme customizers but doesn't save**
3. **Public menu shows "No items"**

---

## 🔍 Step-by-Step Debugging

### **Step 1: Check if Backend is Running**

```bash
cd backend/src/Menufy.API
dotnet run
```

**Expected:** Should start on `https://localhost:5001`

---

### **Step 2: Check if Frontend is Running**

```bash
cd frontend
npm run dev
```

**Expected:** Should start on `http://localhost:3000`

---

### **Step 3: Check Authentication**

1. Open browser console (F12)
2. Go to `http://localhost:3000/debug`
3. Check the "Auth Store" section
4. **CRITICAL:** Check if `restaurantId` exists

**If restaurantId is NULL:**
- You need to logout and login again
- Or register a new account
- The backend MUST return restaurantId in login response

---

### **Step 4: Test Settings API Directly**

1. Go to `http://localhost:3000/debug`
2. Click "Test Get Settings"
3. Check the response

**Expected Response:**
```json
{
  "restaurantId": "some-guid",
  "activeTemplateId": null,
  "customTheme": null,
  "displaySettings": {...},
  "currency": "USD",
  "defaultLanguage": "en"
}
```

**If you get 404 or 401:**
- Backend endpoint doesn't exist
- You're not authenticated
- RestaurantId is missing

---

### **Step 5: Check Backend Endpoints**

Open: `https://localhost:5001/swagger`

**Check these endpoints exist:**
- GET `/api/restaurants/{id}/settings`
- PUT `/api/restaurants/{id}/settings`
- POST `/api/restaurants/{id}/apply-template`
- GET `/api/menu/{slug}`

**If they DON'T exist:**
- Backend migration wasn't applied
- Controller wasn't created
- Need to rebuild backend

---

### **Step 6: Apply Database Migration**

```bash
cd backend/src/Menufy.Infrastructure
dotnet ef database update --startup-project ../Menufy.API
```

**Expected:** Should apply `EnhancedMenuGenerator` migration

---

### **Step 7: Test Template Creation**

1. Go to Templates page
2. Open browser console
3. Click "Create Template"
4. Fill in name
5. Scroll down to theme customizers
6. Change a color
7. Add a category
8. Add an item
9. Click Create
10. **Watch console for errors**

**Common Errors:**
- "restaurantId is required" → You're not logged in properly
- "Failed to create template" → Backend error
- Nothing happens → JavaScript error in console

---

### **Step 8: Check Network Tab**

1. Open browser DevTools (F12)
2. Go to Network tab
3. Try to create a template
4. Look for the POST request to `/api/menu-templates`
5. Check the request payload
6. Check the response

**If Request Fails:**
- Check status code (401 = not authenticated, 404 = endpoint missing, 500 = server error)
- Check response body for error message

---

## 🔧 Quick Fixes

### **Fix 1: Logout and Login Again**

```typescript
// In browser console:
localStorage.clear()
// Then reload page and login again
```

### **Fix 2: Rebuild Backend**

```bash
cd backend
dotnet clean
dotnet build
cd src/Menufy.API
dotnet run
```

### **Fix 3: Clear Frontend Cache**

```bash
cd frontend
rm -rf .nuxt
rm -rf node_modules/.vite
npm run dev
```

### **Fix 4: Check API Base URL**

File: `frontend/nuxt.config.ts`

```typescript
runtimeConfig: {
  public: {
    apiBase: process.env.NUXT_PUBLIC_API_BASE || 'https://localhost:5001'
  }
}
```

---

## 🎯 The Most Likely Problem

**You probably don't have a restaurantId in your auth store.**

### **To Fix:**

1. **Option A: Logout and Login**
   - Click logout
   - Login again
   - Check if restaurantId exists now

2. **Option B: Register New Account**
   - Go to register page
   - Create new account with restaurant name
   - This WILL create a restaurant and set restaurantId

3. **Option C: Check Database**
   ```sql
   SELECT * FROM Users;
   SELECT * FROM Restaurants;
   ```
   - Make sure your user has a restaurant
   - Check if Restaurant.OwnerId matches your User.Id

---

## 📊 Expected Data Flow

```
1. User registers/logs in
   ↓
2. Backend returns: { user: { id, name, email, role, restaurantId }, token, refreshToken }
   ↓
3. Frontend stores in localStorage and authStore
   ↓
4. Settings page reads authStore.restaurantId
   ↓
5. Calls GET /api/restaurants/{restaurantId}/settings
   ↓
6. Backend returns settings
   ↓
7. Page displays settings
```

**If ANY step fails, the whole thing breaks.**

---

## 🚨 Emergency Solution

If nothing works, here's the nuclear option:

```bash
# 1. Stop everything
# Kill backend and frontend

# 2. Reset database
cd backend/src/Menufy.Infrastructure
dotnet ef database drop --force --startup-project ../Menufy.API
dotnet ef database update --startup-project ../Menufy.API

# 3. Rebuild backend
cd ../..
dotnet clean
dotnet build
cd src/Menufy.API
dotnet run

# 4. In another terminal, restart frontend
cd frontend
rm -rf .nuxt
npm run dev

# 5. Register a NEW account
# Go to http://localhost:3000/register
# Fill in ALL fields including restaurant name
# Register

# 6. Go to debug page
# http://localhost:3000/debug
# Check if restaurantId exists

# 7. If restaurantId exists, go to settings page
# It should work now
```

---

## 📝 Checklist

Before asking for help, verify:

- [ ] Backend is running
- [ ] Frontend is running
- [ ] Database migration applied
- [ ] You're logged in
- [ ] restaurantId exists in authStore (check /debug page)
- [ ] Backend endpoints exist (check /swagger)
- [ ] No errors in browser console
- [ ] No errors in backend logs
- [ ] API base URL is correct
- [ ] You can see your restaurant in database

---

## 🆘 If Still Not Working

1. Go to `http://localhost:3000/debug`
2. Take screenshot of ALL sections
3. Check browser console for errors
4. Check backend terminal for errors
5. Share the error messages

The debug page will show EXACTLY what's wrong.

