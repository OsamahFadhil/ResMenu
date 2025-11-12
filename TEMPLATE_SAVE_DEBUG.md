# 🐛 Template Save Debugging Guide

## Issue: Template fields clear when clicking Save (no API request sent)

### What I Added:

**Console Logging** to track the entire flow:

1. **When modal opens**: Logs initial form state
2. **When Save clicked**: Logs form data before validation
3. **In store**: Logs API request and response
4. **Error handling**: Logs any errors with details

### How to Debug:

#### Step 1: Open Browser Console
```
1. Press F12
2. Go to Console tab
3. Keep it open
```

#### Step 2: Try Creating a Template
```
1. Go to http://localhost:3000/dashboard/templates
2. Click "Create Template"
3. Fill in:
   - Name: "Test"
   - Click "Add Category"
   - Fill category name: "Category 1"
   - Click "Add Item"
   - Fill item name: "Item 1"
   - Fill item price: 10
4. Click "Save"
```

#### Step 3: Check Console Output

**Expected Console Output** (if working):
```
=== OPENING CREATE MODAL ===
Initial form: { name: '', description: '', ... }
=== SAVE TEMPLATE CALLED ===
Form data: { name: 'Test', structure: { categories: [...] } }
Starting save process...
Creating new template...
=== STORE: createTemplate called ===
Payload: { ... }
Making POST request to /menu-templates
Response received: { success: true, data: { ... } }
Template added to store: { ... }
Template created: { ... }
=== SAVE TEMPLATE FINISHED ===
```

**If fields clear immediately** (current issue):
```
=== OPENING CREATE MODAL ===
Initial form: { ... }
(nothing else - Save button not triggering)
```

### Possible Causes:

#### Cause 1: Button Not Triggering
- Button might have wrong event handler
- Check if `@click` is properly bound

#### Cause 2: Modal Closing Prematurely
- Modal might be closing before save completes
- Check if there's a backdrop click or ESC key

#### Cause 3: Form Reset Before Save
- Something might be resetting form before save runs
- Check if there's a watcher or computed property

#### Cause 4: Event Propagation Issue
- Click event might be bubbling and triggering close
- Need to add `.stop` or `.prevent`

### What to Look For in Console:

1. **If you see "=== SAVE TEMPLATE CALLED ==="**:
   - ✅ Button is working
   - Check what happens next

2. **If you DON'T see "=== SAVE TEMPLATE CALLED ==="**:
   - ❌ Button click not triggering
   - Check button binding

3. **If you see validation error alert**:
   - Check form data in console log
   - Verify categories and items exist

4. **If you see "=== STORE: createTemplate called ==="**:
   - ✅ Store method reached
   - Check API response

5. **If you see network error**:
   - Check if backend is running
   - Check API endpoint URL

### Quick Fixes to Try:

#### Fix 1: Add .prevent to button
```vue
<UiButton @click.prevent="saveTemplate" variant="primary" :loading="saving">
  Save
</UiButton>
```

#### Fix 2: Check Modal closeOnBackdrop
```vue
<Modal
  v-model="showModal"
  :close-on-backdrop="false"
>
```

#### Fix 3: Verify button type
```vue
<UiButton type="button" @click="saveTemplate">
  Save
</UiButton>
```

### After Testing:

**Share this information**:
1. Screenshot of console output
2. What you see in Network tab (F12 → Network)
3. Any error messages
4. Does modal close immediately or stay open?

This will help identify the exact issue!

