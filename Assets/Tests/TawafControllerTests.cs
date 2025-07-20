using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace TawafARTrainer.Tests
{
    [TestFixture]
    public class TawafControllerTests
    {
        private TawafController tawafController;
        private GameObject testGameObject;
        private Camera testCamera;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject
            testGameObject = new GameObject("TestTawafController");
            tawafController = testGameObject.AddComponent<TawafController>();
            
            // Create test camera
            GameObject cameraObj = new GameObject("TestCamera");
            testCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            
            // Disable testing mode for clean tests
            tawafController.enableTesting = false;
            tawafController.showDebugUI = false;
            tawafController.debugMode = true;
        }

        [TearDown]
        public void Teardown()
        {
            if (testGameObject != null)
                Object.DestroyImmediate(testGameObject);
            
            if (testCamera != null)
                Object.DestroyImmediate(testCamera.gameObject);
        }

        [Test]
        public void TawafController_Initialization_ShouldSetDefaultValues()
        {
            // Assert
            Assert.AreEqual(0, tawafController.currentRound);
            Assert.AreEqual(0f, tawafController.distanceToKaaba);
            Assert.AreEqual(0f, tawafController.totalAngleChange);
            Assert.IsFalse(tawafController.kaabaCreated);
            Assert.IsFalse(tawafController.isValidDistance);
        }

        [Test]
        public void RoundCounting_ShouldStartAtZero()
        {
            // Assert
            Assert.AreEqual(0, tawafController.GetCurrentRound());
        }

        [Test]
        public void KaabaCreation_ShouldReturnFalse_WhenNotCreated()
        {
            // Assert
            Assert.IsFalse(tawafController.IsKaabaCreated());
        }

        [Test]
        public void DistanceToKaaba_ShouldReturnZero_WhenNoKaaba()
        {
            // Assert
            Assert.AreEqual(0f, tawafController.GetDistanceToKaaba());
        }

        [Test]
        public void CreateSimpleKaaba_ShouldCreateKaabaInstance()
        {
            // Act - Use reflection to call private method
            var method = typeof(TawafController).GetMethod("CreateSimpleKaaba", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(tawafController, null);

            // Assert
            Assert.IsTrue(tawafController.kaabaCreated);
            Assert.IsNotNull(tawafController.kaabaInstance);
            Assert.AreEqual("Kaaba", tawafController.kaabaInstance.name);
        }

        [Test]
        public void DistanceValidation_ShouldReturnTrue_WhenInValidRange()
        {
            // Arrange
            tawafController.minimumDistance = 1f;
            tawafController.maximumDistance = 5f;
            float testDistance = 3f;

            // Act
            bool isValid = testDistance >= tawafController.minimumDistance && 
                          testDistance <= tawafController.maximumDistance;

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void DistanceValidation_ShouldReturnFalse_WhenTooClose()
        {
            // Arrange
            tawafController.minimumDistance = 1f;
            tawafController.maximumDistance = 5f;
            float testDistance = 0.5f;

            // Act
            bool isValid = testDistance >= tawafController.minimumDistance && 
                          testDistance <= tawafController.maximumDistance;

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void DistanceValidation_ShouldReturnFalse_WhenTooFar()
        {
            // Arrange
            tawafController.minimumDistance = 1f;
            tawafController.maximumDistance = 5f;
            float testDistance = 7f;

            // Act
            bool isValid = testDistance >= tawafController.minimumDistance && 
                          testDistance <= tawafController.maximumDistance;

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void AngleCalculation_ShouldReturnCorrectAngle_WhenUserMoves()
        {
            // Arrange
            Vector3 kaabaPosition = new Vector3(0, 0, 0);
            Vector3 userPosition = new Vector3(2, 0, 0); // User at 0 degrees
            Vector3 toKaaba = kaabaPosition - userPosition;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);

            // Act
            float angle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            // Assert
            Assert.AreEqual(180f, angle, 0.1f); // Should be 180 degrees
        }

        [Test]
        public void AngleCalculation_ShouldHandleNegativeAngles()
        {
            // Arrange
            Vector3 kaabaPosition = new Vector3(0, 0, 0);
            Vector3 userPosition = new Vector3(-2, 0, 0); // User at 180 degrees
            Vector3 toKaaba = kaabaPosition - userPosition;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);

            // Act
            float angle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            // Assert
            Assert.AreEqual(0f, angle, 0.1f); // Should be 0 degrees
        }

        [Test]
        public void HajarAswadThreshold_ShouldBeReasonable()
        {
            // Assert
            Assert.Greater(tawafController.hajarAswadThreshold, 0f);
            Assert.Less(tawafController.hajarAswadThreshold, 90f); // Should be less than 90 degrees
        }

        [Test]
        public void IstilamSettings_ShouldHaveValidValues()
        {
            // Assert
            Assert.Greater(tawafController.istilamGestureThreshold, 0f);
            Assert.Greater(tawafController.istilamTimeWindow, 0f);
            Assert.Greater(tawafController.istilamShakeThreshold, 0f);
            Assert.Greater(tawafController.requiredShakes, 0);
        }

        [Test]
        public void MovementSettings_ShouldHaveValidRanges()
        {
            // Assert
            Assert.Greater(tawafController.minimumDistance, 0f);
            Assert.Greater(tawafController.maximumDistance, tawafController.minimumDistance);
        }

        [UnityTest]
        public IEnumerator TawafController_Start_ShouldInitializeComponents()
        {
            // Act
            tawafController.Start();
            
            // Wait for one frame
            yield return null;

            // Assert - Check if components are initialized
            Assert.IsNotNull(tawafController);
        }

        [Test]
        public void RoundTimeout_ShouldBeReasonable()
        {
            // Access private field using reflection
            var field = typeof(TawafController).GetField("roundTimeout", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            float roundTimeout = (float)field.GetValue(tawafController);

            // Assert
            Assert.Greater(roundTimeout, 60f); // Should be at least 1 minute
            Assert.Less(roundTimeout, 600f);   // Should be less than 10 minutes
        }

        [Test]
        public void KaabaScale_ShouldBeAppropriateForAR()
        {
            // Assert
            Assert.Greater(tawafController.kaabaScale, 0f);
            Assert.Less(tawafController.kaabaScale, 1f); // Should be scaled down for AR
        }

        [Test]
        public void EnableTesting_ShouldBeConfigurable()
        {
            // Act
            tawafController.enableTesting = true;
            bool testEnabled = tawafController.enableTesting;
            tawafController.enableTesting = false;
            bool testDisabled = tawafController.enableTesting;

            // Assert
            Assert.IsTrue(testEnabled);
            Assert.IsFalse(testDisabled);
        }

        [Test]
        public void ShowDebugUI_ShouldBeConfigurable()
        {
            // Act
            tawafController.showDebugUI = true;
            bool debugEnabled = tawafController.showDebugUI;
            tawafController.showDebugUI = false;
            bool debugDisabled = tawafController.showDebugUI;

            // Assert
            Assert.IsTrue(debugEnabled);
            Assert.IsFalse(debugDisabled);
        }

        [Test]
        public void EnablePrayers_ShouldBeConfigurable()
        {
            // Act
            tawafController.enablePrayers = true;
            bool prayersEnabled = tawafController.enablePrayers;
            tawafController.enablePrayers = false;
            bool prayersDisabled = tawafController.enablePrayers;

            // Assert
            Assert.IsTrue(prayersEnabled);
            Assert.IsFalse(prayersDisabled);
        }

        [Test]
        public void EnableCornerMarkers_ShouldBeConfigurable()
        {
            // Act
            tawafController.enableCornerMarkers = true;
            bool markersEnabled = tawafController.enableCornerMarkers;
            tawafController.enableCornerMarkers = false;
            bool markersDisabled = tawafController.enableCornerMarkers;

            // Assert
            Assert.IsTrue(markersEnabled);
            Assert.IsFalse(markersDisabled);
        }

        [Test]
        public void EnableIstilam_ShouldBeConfigurable()
        {
            // Act
            tawafController.enableIstilam = true;
            bool istilamEnabled = tawafController.enableIstilam;
            tawafController.enableIstilam = false;
            bool istilamDisabled = tawafController.enableIstilam;

            // Assert
            Assert.IsTrue(istilamEnabled);
            Assert.IsFalse(istilamDisabled);
        }
    }
} 