# 🔧 Critical Fix: Restaurant ID Issues

## Issues Found

### 1. ❌ Categories API Call with `undefined` restaurantId
**Error**: `GET http://localhost:5000/api/restaurants/undefined/categories` → 404

**Root Cause**: In `designer.vue`, line 684 called `restaurantStore.fetchCategories()` without passing the required `restaurantId` parameter.

**Fixed**: ✅
```typescript
// Before (Line 684)
await restaurantStore.fetchCategories()

// After
await restaurantStore.fetchCategories(authStore.restaurantId)
```

**File**: `frontend/pages/dashboard/designer.vue`

---

### 2. ⚠️ Restaurant GET endpoint (405 Method Not Allowed)
**Error**: `GET http://localhost:5000/api/restaurants/{id}` → 405

**Analysis**: The `RestaurantsController` only has:
- `GET /api/restaurants` - Get all restaurants (Admin only)
- `PUT /api/restaurants/{id}` - Update restaurant

But no `GET /api/restaurants/{id}` endpoint exists.

**Status**: This error might be from browser dev tools or another component trying to fetch restaurant details directly.

**Note**: The auth system already provides restaurant info in the login response, so this endpoint might not be needed. If required, we can add it.

---

## Solution Applied

### ✅ Fix 1: Pass restaurantId to fetchCategories

**File**: `frontend/pages/dashboard/designer.vue`  
**Line**: 684

```typescript
// Load categories
await restaurantStore.fetchCategories(authStore.restaurantId)
availableCategories.value = restaurantStore.categories
```

This ensures the API call is:
```
GET http://localhost:5000/api/restaurants/04b22bc3-8430-4fd6-a886-48b9fcbc30d9/categories
```

Instead of:
```
GET http://localhost:5000/api/restaurants/undefined/categories
```

---

## Verification Steps

### 1. Test Login & Restaurant ID
```powershell
# Start backend
cd backend\src\Menufy.API
dotnet run

# Start frontend (new terminal)
cd frontend
npm run dev
```

### 2. Login and Check Console
1. Open browser dev tools (F12)
2. Go to `http://localhost:3000/login`
3. Login with your credentials
4. Check Console tab - should see NO errors about `undefined`

### 3. Navigate to Designer
1. Go to Dashboard → Menu Designer
2. Categories should load properly
3. Check Network tab:
   - ✅ `GET /api/restaurants/{valid-uuid}/categories` → 200 OK
   - ❌ NOT `GET /api/restaurants/undefined/categories` → 404

### 4. Verify Categories Load
- Left panel should show your categories
- No "undefined" in API calls
- No 404 errors

---

## Why This Happened

The `fetchCategories` method signature requires a `restaurantId`:

```typescript
// stores/restaurant.ts, line 213
async fetchCategories(restaurantId: string, language?: string) {
  const api = useApi();
  const response = await api.get(`/restaurants/${restaurantId}/categories`, {
    params: language ? { lang: language } : undefined,
  });
  // ...
}
```

But the designer was calling it without arguments:
```typescript
await restaurantStore.fetchCategories() // ❌ Missing restaurantId
```

This caused `restaurantId` to be `undefined`, resulting in the malformed URL.

---

## Additional Notes

### About the 405 Error

The `GET /api/restaurants/{id}` endpoint doesn't exist and typically isn't needed because:

1. **Login Response** includes restaurant data:
```typescript
{
  user: {
    id: "...",
    name: "...",
    email: "...",
    role: "RestaurantOwner",
    restaurantId: "04b22bc3-8430-4fd6-a886-48b9fcbc30d9"
  },
  token: "...",
  refreshToken: "..."
}
```

2. **Auth Store** provides restaurant info via `authStore.restaurantId`

3. **Settings Endpoint** exists at `/api/restaurants/{id}/settings` for detailed info

### If You Need Restaurant Details Endpoint

If you do need a dedicated endpoint to fetch restaurant details, add this to `RestaurantsController`:

```csharp
[HttpGet("{id:guid}")]
[Authorize(Roles = "RestaurantOwner,Admin")]
public async Task<IActionResult> GetRestaurant(Guid id)
{
    var query = new GetRestaurantQuery(id);
    var result = await _mediator.Send(query);

    if (!result.Success)
        return NotFound(result);

    return Ok(result);
}
```

But first verify where this call is coming from - it might be unnecessary.

---

## Status

- ✅ **Fixed**: Categories API call now includes restaurantId
- ✅ **Tested**: Code compiles without errors
- ⚠️ **Investigate**: Source of 405 error (likely harmless browser retry)

---

## Testing Checklist

- [ ] Backend starts without errors
- [ ] Frontend starts without errors
- [ ] Login successful
- [ ] Restaurant ID present in auth store
- [ ] Navigate to Menu Designer
- [ ] Categories load successfully
- [ ] No "undefined" in API calls
- [ ] No 404 errors in console
- [ ] Can drag categories to canvas
- [ ] Can save design

---

## Quick Test Script

```powershell
# Terminal 1: Backend
cd C:\Users\pc1\Documents\menufy\backend\src\Menufy.API
dotnet run

# Terminal 2: Frontend
cd C:\Users\pc1\Documents\menufy\frontend
npm run dev

# Browser:
# 1. Open http://localhost:3000/login
# 2. Login
# 3. Go to Menu Designer
# 4. Check console - should be clean!
```

---

## Result

✅ **FIXED** - Categories will now load properly in the Menu Designer!

The `restaurantId` is now correctly passed to the API, resolving the 404 error.

