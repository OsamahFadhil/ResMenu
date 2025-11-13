# ⚡ Quick Start - Test Designer in 5 Minutes

## 🚀 Step-by-Step Quick Test

### 1. Start Servers (2 terminals)

**Terminal 1:**
```bash
cd backend/src/Menufy.API
dotnet run
```
Wait for: `Now listening on...`

**Terminal 2:**
```bash
cd frontend
npm run dev
```
Wait for: `ready started server on...`

---

### 2. Open Browser

Go to: `http://localhost:3000` (or whatever port shows)

---

### 3. Login

Use your restaurant owner account credentials

---

### 4. Create Test Categories (if needed)

If you don't have categories yet:

1. Click **"Categories"** in sidebar
2. Click **"+ Add Category"**
3. Create 3 test categories:
   - **🍕 Pizza** (add 3-4 items)
   - **🥤 Drinks** (add 2-3 items)
   - **🍰 Desserts** (add 2-3 items)

---

### 5. Open Designer

Click **"Menu Designer"** in sidebar

**You should see:**
```
┌──────────────┬─────────────────────────┬───────────────┐
│ Your Menu    │    Live Canvas          │  Customize    │
│              │                         │               │
│ 🍕 Pizza     │   [Drop zone]           │  Global Theme │
│ 🥤 Drinks    │                         │               │
│ 🍰 Desserts  │                         │  [Controls]   │
└──────────────┴─────────────────────────┴───────────────┘
```

---

### 6. Test Drag & Drop ⭐

1. **Drag** "Pizza" from left → **Drop** on center
2. **Drag** "Drinks" → **Drop** on center

**Expected:**
- Both appear in canvas
- Gray preview boxes show
- "Category added!" toast appears

---

### 7. Customize a Category

1. **Click** on "Pizza" category in canvas
2. Right panel changes to show options
3. Try these:
   - Change **Layout** to "Grid"
   - Set **Columns** to 3
   - Change **Card Style** to "Modern"
   - Toggle **Image Size** to "Large"

**Expected:**
- Preview updates (boxes change)
- No errors in console (F12)

---

### 8. Set Global Theme

1. Click outside categories (or "Back to Global Theme")
2. Right panel shows global controls
3. Try:
   - Pick a **Primary Color** (any color)
   - Change **Font** to "Playfair Display"
   - Change **Header Style** to "Banner"

**Expected:**
- Restaurant header preview changes color
- Font changes

---

### 9. Save Your Design 💾

1. Click **"Save & Publish"** (top right)
2. Wait for spinner

**Expected:**
- ✅ Toast: "Design saved and published! 🎉"
- No errors

---

### 10. Verify in Database (Optional)

```bash
# In a new terminal
cd backend/src/Menufy.API
dotnet ef database update  # if not done

# Then connect to your database and check:
```

```sql
SELECT * FROM "MenuDesigns" ORDER BY "CreatedAt" DESC LIMIT 1;
```

Should show your design with `IsActive = true`

---

## ✅ Success Checklist

After following steps 1-9, you should have:
- [ ] Designer page loads
- [ ] Categories appear in left panel
- [ ] Drag & drop works
- [ ] Clicking category shows customization
- [ ] Global theme controls work
- [ ] Save button works
- [ ] Toast notification appears
- [ ] No console errors

---

## 🐛 Quick Troubleshooting

### "No restaurant ID found"
**Fix:** Make sure you're logged in as restaurant owner (not admin)

### Categories don't show in left panel
**Fix:** Create some categories first in the Categories page

### Drag doesn't work
**Fix:** Try a different browser (Chrome/Firefox work best)

### Save button does nothing
**Fix:** 
1. Check browser console (F12) for errors
2. Check backend terminal for errors
3. Verify both servers are running

### "Failed to load menu data"
**Fix:** 
1. Check backend is running on correct port
2. Verify API calls in Network tab (F12)
3. Check authentication token is valid

---

## 🎉 If All Tests Pass

**Congratulations!** 🎊

Your visual menu designer is working perfectly! You've successfully:
- ✅ Built a drag-and-drop interface
- ✅ Implemented per-category customization
- ✅ Created a save/load system
- ✅ Integrated with the backend

### What's Next?

**Option A:** Take a break! You've earned it.

**Option B:** Complete Phase 5 frontend (2-3 hours) to make the public menu use these designs.
- Guide: `PHASE_5_COMPLETION_GUIDE.md`

**Option C:** Show off what you built! It's impressive.

---

## 📸 Screenshot Checklist

For documentation/demo, capture:
1. Designer with empty canvas
2. Dragging a category
3. Category customization panel
4. Grid layout preview
5. Global theme panel
6. Save success toast
7. Saved design in database

---

## 🎬 30-Second Demo Script

"Watch this - I'll create a custom menu in 30 seconds:
1. [Drag Pizza] → Grid layout
2. [Drag Drinks] → List layout  
3. [Click Pizza, change colors]
4. [Set global theme]
5. [Click Save]
→ Done! Custom menu saved."

---

**Ready? Let's test!** 🚀

Open `http://localhost:3000` and follow steps 1-9!

