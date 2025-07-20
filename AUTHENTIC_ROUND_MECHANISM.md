# Authentic Round Mechanism - 360° from Hajar al-Aswad

## 🎯 Authentic Tawaf Requirements

### **Traditional Tawaf Rules:**
1. **Must start from Hajar al-Aswad** (Black Stone corner)
2. **Must complete exactly 360°** around the Kaaba
3. **Must walk counterclockwise** around the Kaaba
4. **Must complete 7 rounds** for full Tawaf

## 🔧 Implementation Details

### **Round Start Requirements:**
```csharp
public bool mustStartFromHajarAswad = true; // Rounds must start from Hajar al-Aswad
public float roundCompletionThreshold = 360f; // Require full 360° of movement
```

### **Verification Process:**
1. **Detect Hajar al-Aswad pass** (within 5° of 0°)
2. **Verify current position** is actually at Hajar al-Aswad
3. **Start round tracking** only if at correct position
4. **Track 360° rotation** from starting point
5. **Complete round** when exactly 360° is reached

## 📊 Round Counting Logic

### **Step-by-Step Process:**

#### **1. Hajar al-Aswad Detection:**
```csharp
bool nearHajarAswad = Mathf.Abs(Mathf.DeltaAngle(currentAngle, 0f)) < hajarAswadThreshold;
```
- **Threshold**: 5° from 0° (Hajar al-Aswad position)
- **Detection zone**: 355° - 5° (10° total)

#### **2. Round Start Verification:**
```csharp
if (hajarAswadPasses == 1 && !roundInProgress && mustStartFromHajarAswad)
{
    bool nearHajarAswad = Mathf.Abs(Mathf.DeltaAngle(currentAngle, 0f)) < hajarAswadThreshold;
    if (nearHajarAswad)
    {
        // Start round tracking
    }
}
```
- **Must be at Hajar al-Aswad** when starting
- **No round start** from other corners
- **Verification required** before tracking begins

#### **3. Rotation Tracking:**
```csharp
float angleDifference = Mathf.DeltaAngle(lastAngle, currentAngle);
if (Mathf.Abs(angleDifference) > 1f) // Only count significant movements
{
    totalRotationThisRound += Mathf.Abs(angleDifference);
}
```
- **Accumulates rotation** in degrees
- **Filters noise** (movements < 1°)
- **Tracks absolute movement** around Kaaba

#### **4. Round Completion:**
```csharp
if (totalRotationThisRound >= roundCompletionThreshold) // 360°
{
    CompleteRound();
    // Reset for next round
}
```
- **Requires exactly 360°** of movement
- **No shortcuts** or alternative methods
- **Authentic completion** only

## 🚫 What's NOT Allowed

### **Invalid Round Starts:**
- ❌ Starting from Yemeni Corner (90°)
- ❌ Starting from Shami Corner (180°)
- ❌ Starting from Iraqi Corner (270°)
- ❌ Starting from any position not at Hajar al-Aswad

### **Invalid Round Completions:**
- ❌ Less than 360° of movement
- ❌ More than 360° of movement
- ❌ Passing Hajar al-Aswad twice without 360°
- ❌ Manual round completion shortcuts

## ✅ What's Required

### **Valid Round Process:**
1. ✅ **Start at Hajar al-Aswad** (0° ± 5°)
2. ✅ **Walk counterclockwise** around Kaaba
3. ✅ **Complete exactly 360°** of movement
4. ✅ **Return to starting position** (Hajar al-Aswad)
5. ✅ **Round count increments** by 1

### **UI Indicators:**
- **"Round started from Hajar al-Aswad ✓"** - Confirms valid start
- **"Round Progress: X°/360°"** - Shows completion percentage
- **"Round In Progress: True"** - Indicates active tracking

## 🎮 Testing the Mechanism

### **Valid Test Scenario:**
1. **Walk to Hajar al-Aswad** (red corner marker)
2. **Start walking counterclockwise** around Kaaba
3. **Complete full circle** (360° movement)
4. **Return to Hajar al-Aswad**
5. **Round count should increment**

### **Invalid Test Scenarios:**
1. **Start from wrong corner** → No round tracking
2. **Walk less than 360°** → No round completion
3. **Walk more than 360°** → Round completes at 360°
4. **Start from middle of circle** → No round tracking

## 🔍 Debug Information

### **Console Logs:**
```
Starting new round from Hajar al-Aswad at angle: 2.3°
Round Progress - Rotation: 180.5°/360°, Angle: 182.8°
Round completed! Total rotation: 360.2° (360° required)
```

### **UI Display:**
- **Round Progress**: Shows current rotation vs 360° required
- **Hajar al-Aswad Passes**: Counts passes at correct corner
- **Round Status**: Shows if round is actively being tracked

## 🎯 Success Criteria

### **Authentic Tawaf Experience:**
✅ **Rounds only start from Hajar al-Aswad**  
✅ **Exactly 360° required for completion**  
✅ **No shortcuts or manual overrides**  
✅ **Accurate round counting (1-7)**  
✅ **Real movement tracking**  

This ensures the Tawaf AR Trainer provides an authentic experience that matches traditional Tawaf requirements! 🕌📱✨ 