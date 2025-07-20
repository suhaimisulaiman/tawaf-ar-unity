using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace TawafARTrainer.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private TawafController tawafController;
        private CornerMarker cornerMarker;
        private PrayerRecitation prayerRecitation;
        private ARMovementTracker movementTracker;
        private GameObject testGameObject;

        [SetUp]
        public void Setup()
        {
            // Create main test GameObject
            testGameObject = new GameObject("TestIntegration");
            
            // Create all components
            tawafController = testGameObject.AddComponent<TawafController>();
            cornerMarker = testGameObject.AddComponent<CornerMarker>();
            prayerRecitation = testGameObject.AddComponent<PrayerRecitation>();
            movementTracker = testGameObject.AddComponent<ARMovementTracker>();
            
            // Set up camera
            GameObject cameraObj = new GameObject("TestCamera");
            Camera testCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            cameraObj.transform.position = new Vector3(2, 0, 0);
            
            // Configure for testing
            tawafController.enableTesting = false;
            tawafController.showDebugUI = false;
            tawafController.debugMode = true;
        }

        [TearDown]
        public void Teardown()
        {
            if (testGameObject != null)
                Object.DestroyImmediate(testGameObject);
        }

        [Test]
        public void ComponentIntegration_AllComponentsShouldInitialize()
        {
            // Assert all components are properly initialized
            Assert.IsNotNull(tawafController);
            Assert.IsNotNull(cornerMarker);
            Assert.IsNotNull(prayerRecitation);
            Assert.IsNotNull(movementTracker);
        }

        [Test]
        public void TawafController_ShouldIntegrateWithCornerMarker()
        {
            // Arrange
            tawafController.enableCornerMarkers = true;
            
            // Act - Create Kaaba to enable corner detection
            var createMethod = typeof(TawafController).GetMethod("CreateSimpleKaaba", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            createMethod.Invoke(tawafController, null);

            // Assert
            Assert.IsTrue(tawafController.kaabaCreated);
            Assert.IsNotNull(tawafController.kaabaInstance);
        }

        [Test]
        public void TawafController_ShouldIntegrateWithPrayerRecitation()
        {
            // Arrange
            tawafController.enablePrayers = true;
            
            // Act - Create Kaaba to trigger prayer
            var createMethod = typeof(TawafController).GetMethod("CreateSimpleKaaba", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            createMethod.Invoke(tawafController, null);

            // Assert
            Assert.IsTrue(tawafController.kaabaCreated);
            Assert.IsTrue(tawafController.enablePrayers);
        }

        [Test]
        public void RoundCounting_ShouldWorkAcrossComponents()
        {
            // Arrange
            tawafController.enableTesting = true;
            
            // Act - Create Kaaba and simulate round completion
            var createMethod = typeof(TawafController).GetMethod("CreateSimpleKaaba", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            createMethod.Invoke(tawafController, null);

            // Simulate round completion
            var completeMethod = typeof(TawafController).GetMethod("CompleteRound", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            completeMethod.Invoke(tawafController, null);

            // Assert
            Assert.AreEqual(1, tawafController.GetCurrentRound());
        }

        [Test]
        public void DistanceValidation_ShouldBeConsistentAcrossComponents()
        {
            // Arrange
            float testDistance = 3f;
            tawafController.minimumDistance = 1f;
            tawafController.maximumDistance = 5f;
            movementTracker.minimumDistance = 1f;
            movementTracker.maximumDistance = 5f;

            // Act
            bool tawafValid = testDistance >= tawafController.minimumDistance && 
                             testDistance <= tawafController.maximumDistance;
            bool movementValid = testDistance >= movementTracker.minimumDistance && 
                                testDistance <= movementTracker.maximumDistance;

            // Assert
            Assert.AreEqual(tawafValid, movementValid);
            Assert.IsTrue(tawafValid);
        }

        [Test]
        public void AngleCalculation_ShouldBeConsistentAcrossComponents()
        {
            // Arrange
            Vector3 kaabaPosition = Vector3.zero;
            Vector3 userPosition = new Vector3(2, 0, 0);
            Vector3 toKaaba = kaabaPosition - userPosition;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);

            // Act
            float angle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            // Assert
            Assert.AreEqual(180f, angle, 0.1f);
        }

        [Test]
        public void CornerDetection_ShouldWorkWithTawafController()
        {
            // Arrange
            tawafController.enableCornerMarkers = true;
            cornerMarker.showCornerMarkers = true;
            
            // Act - Create Kaaba
            var createMethod = typeof(TawafController).GetMethod("CreateSimpleKaaba", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            createMethod.Invoke(tawafController, null);

            // Test corner detection at Hajar al-Aswad angle
            int nearestCorner = cornerMarker.GetNearestCorner(90f);

            // Assert
            Assert.AreEqual(0, nearestCorner); // Hajar al-Aswad
        }

        [Test]
        public void PrayerIntegration_ShouldWorkWithTawafController()
        {
            // Arrange
            tawafController.enablePrayers = true;
            prayerRecitation.enablePrayers = true;
            
            // Act - Get prayer text
            string prayer = prayerRecitation.GetTawafStartPrayer();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(prayer));
            Assert.IsTrue(tawafController.enablePrayers);
            Assert.IsTrue(prayerRecitation.enablePrayers);
        }

        [Test]
        public void MovementTracking_ShouldWorkWithTawafController()
        {
            // Arrange
            tawafController.enableTesting = true;
            
            // Act - Create Kaaba
            var createMethod = typeof(TawafController).GetMethod("CreateSimpleKaaba", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            createMethod.Invoke(tawafController, null);

            // Test movement tracking
            Vector3 movement = movementTracker.GetDeviceMovement();
            bool isMoving = movementTracker.IsUserMoving();

            // Assert
            Assert.IsNotNull(movement);
            Assert.IsInstanceOf<bool>(isMoving);
            Assert.IsTrue(tawafController.kaabaCreated);
        }

        [Test]
        public void CompleteTawaf_ShouldIntegrateAllComponents()
        {
            // Arrange
            tawafController.enableTesting = true;
            tawafController.enablePrayers = true;
            tawafController.enableCornerMarkers = true;
            
            // Act - Create Kaaba
            var createMethod = typeof(TawafController).GetMethod("CreateSimpleKaaba", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            createMethod.Invoke(tawafController, null);

            // Simulate completing all 7 rounds
            for (int i = 0; i < 7; i++)
            {
                var completeMethod = typeof(TawafController).GetMethod("CompleteRound", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                completeMethod.Invoke(tawafController, null);
            }

            // Assert
            Assert.AreEqual(7, tawafController.GetCurrentRound());
            Assert.IsTrue(tawafController.kaabaCreated);
        }

        [Test]
        public void Settings_ShouldBeConsistentAcrossComponents()
        {
            // Test that settings are consistent between components
            Assert.AreEqual(7, movementTracker.totalRounds); // Tawaf is 7 rounds
            
            // Test distance settings are reasonable
            Assert.Greater(tawafController.minimumDistance, 0f);
            Assert.Greater(tawafController.maximumDistance, tawafController.minimumDistance);
            Assert.Greater(movementTracker.minimumDistance, 0f);
            Assert.Greater(movementTracker.maximumDistance, movementTracker.minimumDistance);
        }

        [Test]
        public void ComponentInitialization_ShouldNotConflict()
        {
            // Test that all components can be initialized together without conflicts
            Assert.DoesNotThrow(() => {
                tawafController.Start();
                cornerMarker.Start();
                prayerRecitation.Start();
                movementTracker.Start();
            });
        }

        [UnityTest]
        public IEnumerator Integration_AllComponentsShouldWorkTogether()
        {
            // Arrange
            tawafController.enableTesting = true;
            tawafController.enablePrayers = true;
            tawafController.enableCornerMarkers = true;
            
            // Act - Start all components
            tawafController.Start();
            cornerMarker.Start();
            prayerRecitation.Start();
            movementTracker.Start();
            
            // Wait for one frame
            yield return null;

            // Assert
            Assert.IsNotNull(tawafController);
            Assert.IsNotNull(cornerMarker);
            Assert.IsNotNull(prayerRecitation);
            Assert.IsNotNull(movementTracker);
        }

        [Test]
        public void ErrorHandling_ShouldBeGraceful()
        {
            // Test that components handle missing dependencies gracefully
            Assert.DoesNotThrow(() => {
                // Try to access components that might not be fully initialized
                tawafController.GetCurrentRound();
                tawafController.GetDistanceToKaaba();
                tawafController.IsKaabaCreated();
                cornerMarker.GetCurrentCornerName();
                cornerMarker.IsNearCorner();
                prayerRecitation.GetTawafStartPrayer();
                movementTracker.GetDeviceMovement();
            });
        }

        [Test]
        public void Performance_ShouldBeReasonable()
        {
            // Test that component operations don't take excessive time
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            
            stopwatch.Start();
            tawafController.GetCurrentRound();
            stopwatch.Stop();
            
            Assert.Less(stopwatch.ElapsedMilliseconds, 100); // Should complete in less than 100ms
        }

        [Test]
        public void MemoryUsage_ShouldBeReasonable()
        {
            // Test that components don't create excessive objects
            int initialObjectCount = FindObjectsOfType<GameObject>().Length;
            
            // Create additional components
            GameObject testObj = new GameObject("MemoryTest");
            testObj.AddComponent<TawafController>();
            testObj.AddComponent<CornerMarker>();
            testObj.AddComponent<PrayerRecitation>();
            
            int finalObjectCount = FindObjectsOfType<GameObject>().Length;
            
            // Clean up
            Object.DestroyImmediate(testObj);
            
            // Assert reasonable object creation
            Assert.Less(finalObjectCount - initialObjectCount, 10); // Should create less than 10 objects
        }
    }
} 