# 🧪 Unit Testing Framework for Tawaf AR Trainer

## 📋 Overview

This directory contains a comprehensive unit testing framework for the Tawaf AR Trainer project. The tests are designed to ensure code quality, functionality, and reliability across all major components.

## 🏗️ Test Structure

### **Test Files**
- `TawafControllerTests.cs` - Core Tawaf logic and round counting
- `CornerMarkerTests.cs` - Corner detection and marker system
- `PrayerRecitationTests.cs` - Prayer text and audio system
- `ARMovementTrackerTests.cs` - Movement tracking and AR integration
- `IntegrationTests.cs` - Component interaction and system integration

### **Configuration**
- `TestRunner.asmdef` - Assembly definition for test framework
- `README.md` - This documentation file

## 🎯 Test Categories

### **1. Unit Tests**
Individual component testing with isolated functionality:

| Component | Test Focus | Coverage |
|-----------|------------|----------|
| **TawafController** | Round counting, Kaaba creation, distance validation | Core logic |
| **CornerMarker** | Corner detection, angle calculations, marker visibility | UI/UX |
| **PrayerRecitation** | Prayer text generation, audio playback | Content |
| **ARMovementTracker** | Movement tracking, device sensors | AR functionality |

### **2. Integration Tests**
Cross-component testing and system behavior:

| Test Type | Purpose | Coverage |
|-----------|---------|----------|
| **Component Integration** | Component interaction | System cohesion |
| **Round Counting** | End-to-end round completion | User workflow |
| **Distance Validation** | Consistent validation across components | Data integrity |
| **Performance** | Response time and memory usage | Optimization |

## 🚀 Running Tests

### **In Unity Editor**
1. Open **Window > General > Test Runner**
2. Select **EditMode** or **PlayMode** tests
3. Click **Run All** or select specific tests

### **Command Line**
```bash
# Run all tests
unity-test-runner -runTests -testPlatform PlayMode -projectPath /path/to/project

# Run specific test assembly
unity-test-runner -runTests -testPlatform EditMode -assemblyFilter TawafARTrainer.Tests
```

### **CI/CD Integration**
```yaml
# Example GitHub Actions workflow
- name: Run Unity Tests
  run: |
    unity-test-runner -runTests -testPlatform PlayMode -projectPath .
    unity-test-runner -runTests -testPlatform EditMode -projectPath .
```

## 📊 Test Coverage

### **Core Functionality**
- ✅ **Round Counting**: 7-round Tawaf completion
- ✅ **Distance Validation**: User positioning relative to Kaaba
- ✅ **Angle Calculation**: User movement around Kaaba
- ✅ **Gesture Detection**: Istilam shake recognition
- ✅ **Corner Detection**: Hajar al-Aswad and other corners
- ✅ **Prayer System**: Arabic text and translations

### **AR Integration**
- ✅ **Movement Tracking**: Device sensor integration
- ✅ **Spatial Positioning**: 3D coordinate calculations
- ✅ **Kaaba Creation**: AR object placement
- ✅ **Camera Integration**: AR camera setup

### **UI/UX**
- ✅ **Corner Markers**: Visual feedback system
- ✅ **Progress Tracking**: Round completion indicators
- ✅ **Status Display**: Distance and angle feedback
- ✅ **Prayer Display**: Multilingual text rendering

## 🧪 Test Methods

### **Setup and Teardown**
```csharp
[SetUp]
public void Setup()
{
    // Create test environment
    // Initialize components
    // Set up mock objects
}

[TearDown]
public void Teardown()
{
    // Clean up test objects
    // Reset state
    // Dispose resources
}
```

### **Test Attributes**
```csharp
[Test]           // Standard unit test
[UnityTest]      // Coroutine-based test
[TestCase]       // Parameterized test
[Category]       // Test categorization
```

### **Assertions**
```csharp
Assert.AreEqual(expected, actual);           // Value equality
Assert.IsTrue(condition);                    // Boolean check
Assert.IsNotNull(object);                    // Null check
Assert.Greater(value, threshold);            // Range validation
Assert.DoesNotThrow(action);                 // Exception handling
```

## 📈 Test Metrics

### **Coverage Goals**
- **Line Coverage**: >80%
- **Branch Coverage**: >70%
- **Function Coverage**: >90%

### **Performance Benchmarks**
- **Test Execution**: <5 seconds for full suite
- **Memory Usage**: <50MB per test
- **Response Time**: <100ms for individual operations

## 🔧 Test Configuration

### **Test Environment**
```csharp
// Test configuration in Setup()
tawafController.enableTesting = false;    // Disable debug features
tawafController.showDebugUI = false;      // Clean test environment
tawafController.debugMode = true;         // Enable test mode
```

### **Mock Objects**
```csharp
// Create mock Kaaba for testing
GameObject mockKaaba = GameObject.CreatePrimitive(PrimitiveType.Cube);
mockKaaba.name = "Kaaba";
mockKaaba.transform.position = Vector3.zero;
```

### **Reflection Usage**
```csharp
// Access private methods for testing
var method = typeof(TawafController).GetMethod("CreateSimpleKaaba", 
    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
method.Invoke(tawafController, null);
```

## 🐛 Debugging Tests

### **Common Issues**
1. **Missing Dependencies**: Ensure all required packages are installed
2. **Test Environment**: Check camera and GameObject setup
3. **Timing Issues**: Use `yield return null` for frame-based tests
4. **Reflection Errors**: Verify method names and access modifiers

### **Test Logs**
```csharp
// Enable debug logging in tests
Debug.Log($"Test: {testName} - Result: {result}");
```

## 📝 Adding New Tests

### **Test Template**
```csharp
[Test]
public void ComponentName_MethodName_ShouldDoSomething_WhenCondition()
{
    // Arrange
    // Set up test data and conditions
    
    // Act
    // Execute the method being tested
    
    // Assert
    // Verify expected outcomes
}
```

### **Naming Convention**
- **Format**: `Component_Method_ExpectedBehavior_WhenCondition`
- **Example**: `TawafController_CompleteRound_ShouldIncrementCount_WhenValidRound`

### **Test Categories**
- **Unit Tests**: Individual method testing
- **Integration Tests**: Component interaction
- **Performance Tests**: Speed and memory
- **Edge Case Tests**: Boundary conditions

## 🎯 Best Practices

### **Test Design**
- ✅ **Single Responsibility**: Each test focuses on one behavior
- ✅ **Independence**: Tests don't depend on each other
- ✅ **Readability**: Clear test names and structure
- ✅ **Maintainability**: Easy to update when code changes

### **Test Data**
- ✅ **Realistic Values**: Use realistic test data
- ✅ **Edge Cases**: Test boundary conditions
- ✅ **Error Conditions**: Test failure scenarios
- ✅ **Performance**: Test with realistic data volumes

### **Assertions**
- ✅ **Specific**: Test exact expected values
- ✅ **Descriptive**: Clear failure messages
- ✅ **Complete**: Test all relevant outcomes
- ✅ **Efficient**: Minimize redundant checks

## 📚 Resources

### **Unity Testing Documentation**
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/index.html)
- [NUnit Framework](https://docs.nunit.org/)
- [Test Runner](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/workflow-run-playmode-tests.html)

### **Testing Patterns**
- [AAA Pattern](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/reference-attribute-test.html)
- [Mock Objects](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/reference-attribute-testcase.html)
- [Test Categories](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/reference-attribute-category.html)

---

**Last Updated**: December 2024  
**Test Framework Version**: Unity Test Framework 1.5.1  
**Coverage**: Comprehensive unit and integration testing 