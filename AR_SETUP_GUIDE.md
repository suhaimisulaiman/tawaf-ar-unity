# AR Setup Guide for Tawaf Trainer

## Hardware Requirements

**For iPhone AR development:**
- iPhone 6s or newer (supports ARKit)
- No additional hardware required
- Built-in sensors: Camera, Accelerometer, Gyroscope, Compass, LiDAR (iPhone 12 Pro+)

## Software Requirements

1. **Unity 2022.3 LTS or newer** ✅ (You have this)
2. **AR Foundation package** ✅ (Added to manifest.json)
3. **ARKit XR Plugin** ✅ (Already installed)
4. **Xcode** (for building to iPhone)

## Step-by-Step Setup

### 1. Unity Project Setup

The AR Foundation package has been added to your `Packages/manifest.json`. Unity will automatically download and install it.

### 2. Create AR Scene

1. **Create a new scene** or use the provided `ARScene.unity`
2. **Add AR Scene Setup component** to an empty GameObject
3. **The script will automatically create** all necessary AR components

### 3. Configure Build Settings

1. **File → Build Settings**
2. **Switch Platform** to iOS
3. **Player Settings**:
   - **Other Settings → Camera Usage Description**: "This app uses the camera for AR tracking"
   - **Other Settings → Location Usage Description**: "This app uses location for AR positioning"
   - **Other Settings → Microphone Usage Description**: "This app uses microphone for AR audio"

### 4. Xcode Setup

1. **Build the project** to iOS
2. **Open in Xcode**
3. **Sign the app** with your Apple Developer account
4. **Set Bundle Identifier** (e.g., com.yourname.tawafartrainer)

## How It Works

### Real Movement Tracking

The new `ARMovementTracker.cs` script:

1. **Uses AR Foundation** to track the phone's real-world position
2. **Creates an AR anchor** for the Kaaba to keep it stable in real space
3. **Tracks camera movement** around the anchored Kaaba
4. **Calculates real distance** and angle changes
5. **Detects when user completes** a full circle around the Kaaba

### Key Features

- **Real-world positioning**: The Kaaba stays fixed in your real environment
- **Accurate distance tracking**: Uses AR Foundation's world tracking
- **Device sensor integration**: Uses accelerometer and gyroscope
- **Automatic round detection**: Detects when you complete 360° around the Kaaba

## Testing on iPhone

### Prerequisites
- iPhone 6s or newer
- iOS 11.0 or newer
- Good lighting conditions
- Textured surfaces for AR tracking

### How to Test

1. **Build and install** the app on your iPhone
2. **Point camera** at a textured surface (floor, table, etc.)
3. **The Kaaba will appear** in front of you
4. **Walk around the Kaaba** in a circle
5. **Watch the progress** update in real-time
6. **Complete 7 rounds** to finish Tawaf

### Troubleshooting

**AR not working:**
- Check lighting (AR needs good light)
- Move to a textured surface
- Restart the app
- Check device compatibility

**Tracking issues:**
- Move slowly around the Kaaba
- Keep the Kaaba in view
- Avoid rapid movements
- Ensure stable lighting

## Code Structure

### ARMovementTracker.cs
- Handles real-world movement tracking
- Manages AR anchors and positioning
- Calculates round completion
- Integrates device sensors

### ARSceneSetup.cs
- Automatically sets up AR components
- Configures AR Session and Session Origin
- Enables device sensors
- Monitors AR session state

## Next Steps

1. **Test on iPhone** to verify AR tracking works
2. **Adjust sensitivity** if needed (change angle thresholds)
3. **Add visual feedback** for better user experience
4. **Improve Kaaba model** with 3D assets
5. **Add audio guidance** for better immersion

## Performance Tips

- **Keep Kaaba size reasonable** (2-3 meters)
- **Limit distance range** (2-8 meters is good)
- **Use efficient 3D models** for the Kaaba
- **Test in various lighting** conditions
- **Monitor battery usage** (AR can be power-intensive)

## Success Indicators

✅ **AR Session starts** without errors  
✅ **Kaaba appears** and stays stable  
✅ **Movement tracking** responds to phone movement  
✅ **Round counter** increments when circling  
✅ **Distance validation** works correctly  
✅ **7 rounds complete** successfully  

Your Tawaf AR Trainer is now ready for real iPhone movement tracking! 🎉 