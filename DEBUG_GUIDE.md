# AR Debugging Guide

## Issue: No UI Text Visible on iPhone

### Step 1: Test Basic GUI
1. **Create new scene** in Unity
2. **Add BasicARTest script** to empty GameObject
3. **Build and test** - should see red "BASIC AR TEST" text
4. **If no text appears** → GUI system issue

### Step 2: Check Scene Setup
**In Unity Editor:**
1. **Select your scene**
2. **Check Hierarchy** - should have GameObject with script
3. **Check Inspector** - script should be attached and enabled
4. **Check Console** - look for "Script is running!" message

### Step 3: Check Build Settings
**In Unity:**
1. **File → Build Settings**
2. **Check if scene is added** to build
3. **Check Player Settings**:
   - **Other Settings → Scripting Backend**: IL2CPP
   - **Other Settings → Target Architectures**: ARM64

### Step 4: Check Xcode Console
**In Xcode:**
1. **Connect iPhone** and run app
2. **Open Console** (Window → Devices and Simulators)
3. **Look for Unity logs**:
   - "Script is running!"
   - Any error messages
   - AR Foundation logs

### Step 5: Common Issues

**Issue: Script not running**
- Check if script is attached to GameObject
- Check if GameObject is active
- Check Console for compilation errors

**Issue: GUI not rendering**
- Check if OnGUI is being called
- Try different GUI methods
- Check if camera is rendering

**Issue: AR not initializing**
- Check device compatibility
- Check camera permissions
- Check AR Foundation setup

### Step 6: Alternative Test
If GUI still doesn't work:
1. **Create a simple cube** in scene
2. **Make it bright red**
3. **Build and test** - should see red cube
4. **If cube appears** → GUI issue, not AR issue

## Quick Test Commands

Run these in terminal to check setup:
```bash
./test_ar_setup.sh
```

## Expected Results

**Success:**
- ✅ Red "BASIC AR TEST" text visible
- ✅ White status text updates
- ✅ Yellow "GUI is working!" text
- ✅ Test button responds to touch

**Failure:**
- ❌ No text visible at all
- ❌ Only camera feed visible
- ❌ App crashes or freezes

## Next Steps

1. **Try BasicARTest first** - simpler than SimpleARTest
2. **Check Xcode console** for error messages
3. **Verify scene setup** in Unity
4. **Test on different device** if possible

Let me know what you see with BasicARTest! 