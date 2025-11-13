# 🧪 Testing Guide - Menu Designer

## Pre-Testing Checklist

### 1. Ensure Both Servers Are Running

**Terminal 1 - Backend:**
```bash
cd backend/src/Menufy.API
dotnet run
```
Expected output: `Now listening on: http://localhost:5000` (or similar)

**Terminal 2 - Frontend:**
```bash
cd frontend
npm run dev
```
Expected output: `Local: http://localhost:3000` (or similar)

### 2. Verify Database Is Up-to-Date

```bash
cd backend/src/Menufy.API
dotnet ef database update
```

Should show: `No migrations were applied. The database is already up to date.`

---

## 🎯 Test Plan: Designer Functionality

### Test 1: Basic Access ✅

**Steps:**
1. Open browser to `http://localhost:3000`
2. Login with restaurant owner credentials
3. Navigate to "Menu Designer" in sidebar

**Expected Result:**
- Designer page loads without errors
- Three-panel layout visible:
  - Left: "Your Menu" panel
  - Center: "Live Canvas" with drop zone
  - Right: "Customize" panel

**If Fails:**
- Check browser console for errors
- Verify `restaurantId` exists in auth store
- Check network tab for failed API calls

---

### Test 2: Load Existing Categories ✅

**Pre-requisite:** Have at least 2-3 categories with items

**If No Categories Exist:**
1. Go to `/dashboard/categories`
2. Create 2-3 test categories:
   - **Pizza** (with 4-5 items)
   - **Drinks** (with 3-4 items)
   - **Desserts** (with 2-3 items)

**Steps:**
1. Refresh designer page
2. Check left panel

**Expected Result:**
- Categories appear in left panel
- Each shows icon and item count
- Categories are draggable (cursor changes on hover)

**If Fails:**
- Check `fetchCategories` API call
- Verify categories endpoint returns data
- Check console for errors

---

### Test 3: Drag & Drop ⭐

**Steps:**
1. Drag "Pizza" category from left panel
2. Drop onto center canvas
3. Repeat for "Drinks" category

**Expected Result:**
- While dragging, canvas shows "drop zone" style (border changes)
- On drop, category appears in canvas
- Toast notification: "Category added!"
- Category shows with:
  - Title
  - Layout badge (initially "list")
  - Preview items (gray placeholders)
  - Up/Down buttons
  - Remove button

**If Fails:**
- Check browser supports HTML5 drag & drop
- Verify `onCategoryDragStart`, `onDragOver`, `onDrop` functions
- Check console for JavaScript errors
- Try refreshing page

---

### Test 4: Category Customization ✅

**Steps:**
1. Click on "Pizza" category in canvas
2. Right panel should change to show customization options
3. Try each setting:
   - Change layout to "Grid"
   - Set columns to 3
   - Change card style to "Modern"
   - Change image size to "Large"
   - Change image shape to "Rounded"
   - Toggle "Show Images" off/on
   - Toggle "Show Prices" off/on

**Expected Result:**
- Right panel shows "Category: Pizza"
- All controls respond to clicks
- Preview in canvas updates (gray boxes change size/layout)
- No errors in console

**If Fails:**
- Check `selectedCategory` reactive ref
- Verify v-model bindings on controls
- Check computed properties for preview classes

---

### Test 5: Global Theme ✅

**Steps:**
1. Click anywhere outside a category (or click "Back to Global Theme")
2. Right panel shows global theme controls
3. Try changing:
   - Primary color (pick any color)
   - Accent color
   - Background color
   - Font family (try "Playfair Display")
   - Header style (try "Banner")
   - Logo position (try "Left")

**Expected Result:**
- Right panel shows "Global Theme"
- Color pickers work
- Dropdowns change values
- Restaurant header preview updates colors
- No errors

**If Fails:**
- Check `globalTheme` reactive ref
- Verify color input bindings
- Check `headerStyle` computed property

---

### Test 6: Save Design 💾

**Steps:**
1. Ensure you have at least 1 category in canvas
2. Customize it (change layout, colors, etc.)
3. Click "Save & Publish" button (top right)

**Expected Result:**
- Button shows loading spinner
- Toast notification: "Design saved and published! 🎉"
- Browser console shows:
  ```
  Saving new menu design for restaurant {restaurantId}
  Template created: {result}
  ```
- Button returns to normal state

**Check Database:**
```sql
SELECT * FROM "MenuDesigns" 
WHERE "RestaurantId" = 'your-restaurant-id' 
ORDER BY "CreatedAt" DESC 
LIMIT 1;
```

Should show your saved design with:
- `IsActive = true`
- `Version = 1` (or incremented)
- `LayoutConfiguration` as JSON
- `GlobalTheme` as JSON

**If Fails:**
- Check network tab for POST to `/api/menu-design`
- Verify request payload includes all data
- Check backend logs for errors
- Verify `restaurantId` is correct

---

### Test 7: Load Existing Design ✅

**Steps:**
1. Save a design (Test 6)
2. Refresh the page
3. Wait for loading

**Expected Result:**
- Loading spinner shows briefly
- Designer loads with previously saved design:
  - Categories appear in canvas
  - Layouts match what you saved
  - Theme colors match
  - Toast: "Loaded existing design"

**If Fails:**
- Check `getActiveDesign` API call
- Verify GET `/api/menu-design/restaurant/{restaurantId}` returns data
- Check console for deserialization errors
- Verify `MenuDesign.IsActive = true` in database

---

### Test 8: Reorder Categories ✅

**Steps:**
1. Add 3 categories to canvas
2. Click up/down arrows to reorder
3. Move bottom category to top

**Expected Result:**
- Categories change position
- Display order updates
- No duplicates or missing categories

**If Fails:**
- Check `moveCategoryUp` and `moveCategoryDown` functions
- Verify array manipulation logic

---

### Test 9: Remove Category ✅

**Steps:**
1. Add 2 categories
2. Click remove (trash icon) on one
3. Confirm deletion

**Expected Result:**
- Confirmation dialog appears
- Category is removed from canvas
- Remaining category stays intact
- If removed category was selected, selection clears

**If Fails:**
- Check `removeCategory` function
- Verify array splice logic

---

### Test 10: Preview Menu 👁️

**Steps:**
1. Save a design
2. Click "Preview" button (top right)

**Expected Result:**
- New tab opens
- Public menu loads with theme colors
- ⚠️ **Note:** Per-category layouts won't work yet (that's Phase 5 frontend)
- But global theme should apply

**Expected Issues (Normal):**
- All categories render as list (not grid/cards)
- This is because Phase 5 frontend isn't done yet
- Colors and fonts should work

---

## 🐛 Common Issues & Solutions

### Issue: "No restaurant ID found"
**Solution:** 
- Ensure you're logged in as restaurant owner (not admin)
- Check `authStore.restaurantId` in browser console
- Verify user has a restaurant associated

### Issue: Categories not loading
**Solution:**
- Check `/api/restaurants/{restaurantId}/categories` endpoint
- Verify categories exist in database
- Check CORS if API on different domain

### Issue: Drag & drop not working
**Solution:**
- Try different browser (Chrome/Firefox recommended)
- Check if categories have `draggable="true"` attribute
- Verify event handlers are attached

### Issue: Save button does nothing
**Solution:**
- Check browser console for errors
- Verify network tab shows POST request
- Check backend is running and accessible
- Verify authentication token is valid

### Issue: Design doesn't persist
**Solution:**
- Check database connection
- Verify MenuDesigns table exists
- Check backend logs for errors
- Ensure migration was applied

---

## ✅ Success Criteria

After all tests pass, you should be able to:
- ✅ Access designer page
- ✅ See your categories in left panel
- ✅ Drag categories to canvas
- ✅ Click to select and customize
- ✅ Change layouts (list/grid/cards)
- ✅ Adjust colors and fonts
- ✅ Save design to database
- ✅ Reload and see saved design
- ✅ Reorder and remove categories

---

## 🎯 What to Test Next

After confirming designer works:
1. **Test with Multiple Restaurants** (if applicable)
2. **Test with Many Categories** (10+)
3. **Test with Empty Categories** (0 items)
4. **Test Mobile Responsive** (drag may not work on touch)
5. **Test Different Browsers** (Chrome, Firefox, Safari)

---

## 📊 Test Results Template

```
Date: _______________
Tester: _______________

Test 1 - Basic Access:        [ ] Pass  [ ] Fail  Notes: __________
Test 2 - Load Categories:     [ ] Pass  [ ] Fail  Notes: __________
Test 3 - Drag & Drop:         [ ] Pass  [ ] Fail  Notes: __________
Test 4 - Category Customize:  [ ] Pass  [ ] Fail  Notes: __________
Test 5 - Global Theme:        [ ] Pass  [ ] Fail  Notes: __________
Test 6 - Save Design:         [ ] Pass  [ ] Fail  Notes: __________
Test 7 - Load Design:         [ ] Pass  [ ] Fail  Notes: __________
Test 8 - Reorder:             [ ] Pass  [ ] Fail  Notes: __________
Test 9 - Remove:              [ ] Pass  [ ] Fail  Notes: __________
Test 10 - Preview:            [ ] Pass  [ ] Fail  Notes: __________

Overall Status: [ ] All Pass  [ ] Some Fail  [ ] Major Issues

Notes:
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________
```

---

## 🎥 Demo Script

**For showcasing to stakeholders:**

1. **Login** → "Here's our restaurant dashboard"
2. **Navigate to Designer** → "This is our new visual menu designer"
3. **Show Categories** → "These are our actual menu categories"
4. **Drag Pizza** → "I can drag them onto the canvas"
5. **Click Pizza** → "Click to customize this category"
6. **Change to Grid** → "Make it a grid with 3 columns"
7. **Change Card Style** → "Choose modern card style"
8. **Drag Drinks** → "Add another category"
9. **Keep as List** → "This one stays as a list"
10. **Global Theme** → "Set overall colors and fonts"
11. **Save** → "Save and publish instantly"
12. **Preview** → "And here's how customers will see it"

**Key Message:** "In 2 minutes, we designed a completely custom menu without any technical knowledge!"

---

## 🚀 Next Steps After Testing

If all tests pass:
- ✅ Designer is production-ready!
- ⏭️ Move to Phase 5 frontend (public menu integration)
- 📝 Document any issues found

If tests fail:
- 🐛 Debug specific issues
- 💬 Report errors with console logs
- 🔧 Fix and re-test

---

**Happy Testing! 🧪**

