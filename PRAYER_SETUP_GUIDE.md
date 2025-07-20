# Prayer Recitation System Setup Guide

## 🎯 What's Been Added

### **Prayer System Features:**
1. **Corner Prayers** - Specific prayers for each Kaaba corner
2. **General Prayers** - Regular Tawaf prayers during movement
3. **Round Completion Prayers** - Celebratory prayers when rounds finish
4. **Tawaf Completion Prayer** - Final prayer when all 7 rounds are done
5. **Visual Prayer Display** - Prayer text appears on screen

## 📱 How to Test

### **Step 1: Setup**
1. **Add TawafController component** to your scene (if not already added)
2. **Enable prayers** in TawafController inspector
3. **Build and test** on iPhone

### **Step 2: Test Corner Prayers**
1. **Walk around the Kaaba** in circles
2. **Watch for prayer text** appearing on screen
3. **Different prayers** will play at each corner:
   - **Hajar al-Aswad** (Black Stone) - "Bismillahi Allahu Akbar"
   - **Yemeni Corner** - "Rabbana atina fid-dunya hasanatan..."
   - **Shami Corner** - "Allahumma inni as'aluka al-jannah..."
   - **Iraqi Corner** - "Allahumma inni as'aluka al-'afwa..."

### **Step 3: Test Round Completion**
1. **Complete one full circle** around the Kaaba
2. **Watch for round completion prayer** - "Round X completed. Alhamdulillah!"
3. **Continue for 7 rounds**

### **Step 4: Test Tawaf Completion**
1. **Complete all 7 rounds**
2. **Final prayer** will appear - "Tawaf completed! Allahu Akbar!"

## 🎮 What You'll See

### **On Screen:**
- **Prayer text** appears in center of screen
- **Yellow prayer type** (e.g., "Hajar al-Aswad")
- **White prayer text** (the actual prayer)
- **Black background** for readability
- **Fade in/out effect** for smooth display

### **In Console (Xcode):**
- **Debug logs** showing which prayers are playing
- **Corner detection** logs
- **Round completion** messages

## 🔧 Configuration Options

### **In TawafController Inspector:**
- **Enable Prayers**: Turn prayer system on/off
- **Debug Mode**: Easier round completion for testing

### **In PrayerRecitation Inspector:**
- **Enable Prayers**: Master prayer switch
- **Play Corner Prayers**: Corner-specific prayers
- **Play General Prayers**: Regular Tawaf prayers
- **Prayer Interval**: Time between general prayers (30s default)
- **Corner Threshold**: How close to corner to trigger prayer (45° default)

## 📋 Prayer List

### **Corner Prayers:**
1. **Hajar al-Aswad**: "Bismillahi Allahu Akbar"
2. **Yemeni Corner**: "Rabbana atina fid-dunya hasanatan wa fil-akhirati hasanatan wa qina 'adhaban-nar"
3. **Shami Corner**: "Allahumma inni as'aluka al-jannah wa a'udhu bika minan-nar"
4. **Iraqi Corner**: "Allahumma inni as'aluka al-'afwa wal-'afiyah fid-dunya wal-akhirati"

### **General Prayers:**
- "Subhan Allah wal-hamdu lillahi wa la ilaha illa Allahu wa Allahu Akbar"
- "Allahumma salli 'ala Muhammadin wa 'ala ali Muhammad"
- "Rabbana taqabbal minna innaka antas-Sami'ul-'Alim"
- "Allahumma inni as'aluka ridaka wal-jannah wa a'udhu bika min sakhatika wan-nar"

## 🎯 Success Indicators

✅ **Prayer text appears** on screen when circling  
✅ **Different prayers** play at different corners  
✅ **Round completion prayers** trigger after each round  
✅ **Final prayer** plays after 7 rounds  
✅ **Prayer status** shows in bottom UI  

## 🔄 Next Steps

1. **Test the prayer system** with current setup
2. **Add audio files** for actual voice recitations
3. **Customize prayers** for different preferences
4. **Add more authentic** Tawaf prayers and duas

Your Tawaf AR Trainer now has a complete prayer recitation system! 🎉📱 