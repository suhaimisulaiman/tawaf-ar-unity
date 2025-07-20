#!/bin/bash

echo "🔍 Tawaf AR Trainer - Setup Test"
echo "=================================="

# Check if we're in the right directory
if [ ! -f "Packages/manifest.json" ]; then
    echo "❌ Error: Not in Unity project directory"
    exit 1
fi

echo "✅ Unity project found"

# Check for AR Foundation package
if grep -q "com.unity.xr.arfoundation" Packages/manifest.json; then
    echo "✅ AR Foundation package found"
else
    echo "❌ AR Foundation package missing"
fi

# Check for ARKit package
if grep -q "com.unity.xr.arkit" Packages/manifest.json; then
    echo "✅ ARKit package found"
else
    echo "❌ ARKit package missing"
fi

# Check for AR scripts
if [ -f "Assets/ARMovementTracker.cs" ]; then
    echo "✅ ARMovementTracker script found"
else
    echo "❌ ARMovementTracker script missing"
fi

if [ -f "Assets/ARSceneSetup.cs" ]; then
    echo "✅ ARSceneSetup script found"
else
    echo "❌ ARSceneSetup script missing"
fi

# Check for Xcode
if command -v xcodebuild &> /dev/null; then
    echo "✅ Xcode found"
    xcode_version=$(xcodebuild -version | head -n 1)
    echo "   Version: $xcode_version"
else
    echo "❌ Xcode not found - install from Mac App Store"
fi

# Check for iOS Simulator
if command -v xcrun &> /dev/null; then
    echo "✅ iOS development tools found"
else
    echo "❌ iOS development tools missing"
fi

echo ""
echo "📱 Next Steps:"
echo "1. Open Unity and switch platform to iOS"
echo "2. Build the project to Xcode"
echo "3. Connect iPhone and install app"
echo "4. Test AR tracking by walking around Kaaba"
echo ""
echo "🎯 Ready to test on iPhone!" 