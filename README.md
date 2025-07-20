# Tawaf AR Trainer 🕌📱

An augmented reality (AR) application for training and practicing the Islamic ritual of Tawaf around the Kaaba using iPhone AR technology.

## 🌟 Features

### **Core Functionality:**
- **Real-time AR Kaaba**: Virtual Kaaba appears in your physical space
- **Movement Tracking**: Tracks your real-world movement around the Kaaba
- **Authentic Round Counting**: Counts Tawaf rounds from Hajar al-Aswad to Hajar al-Aswad
- **Visual Corner Markers**: Shows corner names when approaching each corner
- **Prayer Recitation System**: Automatic prayers at corners and round completion

### **Technical Features:**
- **Unity AR Foundation**: Real phone movement tracking
- **ARKit Integration**: iPhone AR support
- **Precise Angle Detection**: 5° threshold for corner detection
- **Rotation-Based Counting**: Requires 350° movement for round completion
- **Smooth UI Animations**: Fade-in/out corner markers and prayer text

## 📱 Requirements

- **Device**: iPhone with ARKit support (iPhone 6s or later)
- **iOS Version**: iOS 11.0 or later
- **Unity Version**: 2022.3 LTS or later
- **Space**: Minimum 2m x 2m clear area for circling

## 🎮 How to Use

### **Setup:**
1. **Point camera at floor** to initialize AR
2. **Tap screen** to create Kaaba in front of you
3. **Walk around the black cube** in complete circles

### **Tawaf Practice:**
1. **Start at Hajar al-Aswad** (red corner marker)
2. **Walk counterclockwise** around the Kaaba
3. **Complete 7 rounds** for full Tawaf
4. **Follow corner markers** for guidance
5. **Listen to prayers** at each corner

### **UI Elements:**
- **Round Counter**: Shows current round (1-7)
- **Distance Indicator**: Keep 0.5-3m from Kaaba
- **Corner Markers**: Appear when approaching corners
- **Prayer Text**: Displays during corner prayers
- **Progress Bar**: Shows round completion percentage

## 🏗️ Project Structure

```
TawafARTrainer/
├── Assets/
│   ├── TawafController.cs        # Main Tawaf logic and round counting
│   ├── CornerMarker.cs           # Visual corner markers system
│   ├── PrayerRecitation.cs       # Prayer playback and timing
│   ├── PrayerUI.cs               # Prayer text display
│   ├── ARMovementTracker.cs      # Real phone movement tracking
│   ├── ARSceneSetup.cs           # AR session initialization
│   └── SimpleCornerTest.cs       # Corner marker testing
├── Scenes/
│   ├── SampleScene.unity         # Main AR scene
│   └── ARScene.unity             # Alternative AR scene
├── XR/                           # AR Foundation settings
└── Documentation/
    ├── PRAYER_SETUP_GUIDE.md     # Prayer system guide
    ├── CORNER_MARKER_GUIDE.md    # Corner marker guide
    └── ROUND_COUNTING_FIX.md     # Round counting fix details
```

## 🔧 Configuration

### **Round Counting Settings:**
```csharp
public float hajarAswadThreshold = 5f;        // Corner detection precision
public float roundCompletionThreshold = 350f; // Required rotation for round
public bool requireFullRotation = true;       // Enforce movement requirement
```

### **Movement Settings:**
```csharp
public float minimumDistance = 0.5f;  // Minimum distance from Kaaba
public float maximumDistance = 3f;    // Maximum distance from Kaaba
```

### **Corner Markers:**
```csharp
public float markerDisplayDistance = 2f;  // Distance to show corner marker
public float markerFadeTime = 1f;        // Animation duration
```

## 🎯 Corner Layout

### **Traditional Kaaba Corners:**
1. **Hajar al-Aswad** (Black Stone) - 0° - Red marker
2. **Yemeni Corner** - 90° - Green marker  
3. **Shami Corner** - 180° - Blue marker
4. **Iraqi Corner** - 270° - Yellow marker

## 🚀 Development

### **Building for iOS:**
1. **Open Unity** and load the project
2. **Switch platform** to iOS in Build Settings
3. **Configure AR Foundation** settings
4. **Build project** to Xcode
5. **Deploy to iPhone** via Xcode

### **Testing:**
- **Unity Editor**: Use keyboard controls for testing
- **iPhone**: Real AR movement tracking
- **Debug Mode**: Enable for detailed logging

## 📋 Recent Updates

### **Round Counting Fix (Latest):**
- ✅ Fixed 600+ rounds issue
- ✅ Implemented precise 5° corner detection
- ✅ Added 350° rotation requirement
- ✅ Improved round progress tracking
- ✅ Enhanced UI feedback

### **Corner Marker System:**
- ✅ Visual corner markers with names
- ✅ Color-coded corner indicators
- ✅ Smooth fade animations
- ✅ Integration with prayer system

### **Prayer System:**
- ✅ Automatic corner prayers
- ✅ Round completion prayers
- ✅ Tawaf completion celebration
- ✅ Prayer text display

## 🤝 Contributing

1. **Fork the repository**
2. **Create feature branch** (`git checkout -b feature/AmazingFeature`)
3. **Commit changes** (`git commit -m 'Add AmazingFeature'`)
4. **Push to branch** (`git push origin feature/AmazingFeature`)
5. **Open Pull Request**

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Unity Technologies** for AR Foundation
- **Apple** for ARKit support
- **Islamic scholars** for Tawaf guidance
- **AR/VR community** for technical inspiration

## 📞 Support

For questions or issues:
- **GitHub Issues**: Report bugs or feature requests
- **Documentation**: Check the guides in the Documentation folder
- **Testing**: Use SimpleCornerTest.cs for isolated testing

---

**May this application help you practice Tawaf with ease and authenticity! 🕌📱✨** 