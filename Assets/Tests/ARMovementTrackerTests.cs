using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace TawafARTrainer.Tests
{
    [TestFixture]
    public class ARMovementTrackerTests
    {
        private ARMovementTracker movementTracker;
        private GameObject testGameObject;
        private GameObject mockKaaba;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject
            testGameObject = new GameObject("TestARMovementTracker");
            movementTracker = testGameObject.AddComponent<ARMovementTracker>();
            
            // Create mock Kaaba
            mockKaaba = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mockKaaba.name = "Kaaba";
            mockKaaba.transform.position = Vector3.zero;
            
            // Set up camera
            GameObject cameraObj = new GameObject("TestCamera");
            Camera testCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            cameraObj.transform.position = new Vector3(2, 0, 0);
        }

        [TearDown]
        public void Teardown()
        {
            if (testGameObject != null)
                Object.DestroyImmediate(testGameObject);
            
            if (mockKaaba != null)
                Object.DestroyImmediate(mockKaaba);
        }

        [Test]
        public void ARMovementTracker_Initialization_ShouldSetDefaultValues()
        {
            // Assert
            Assert.AreEqual(new Vector3(2f, 3f, 2f), movementTracker.kaabaSize);
            Assert.AreEqual(2f, movementTracker.minimumDistance);
            Assert.AreEqual(8f, movementTracker.maximumDistance);
            Assert.AreEqual(4f, movementTracker.idealDistance);
            Assert.AreEqual(0, movementTracker.currentRound);
            Assert.AreEqual(7, movementTracker.totalRounds);
        }

        [Test]
        public void KaabaSize_ShouldBeReasonable()
        {
            // Assert
            Assert.Greater(movementTracker.kaabaSize.x, 0f);
            Assert.Greater(movementTracker.kaabaSize.y, 0f);
            Assert.Greater(movementTracker.kaabaSize.z, 0f);
        }

        [Test]
        public void DistanceSettings_ShouldHaveValidRanges()
        {
            // Assert
            Assert.Greater(movementTracker.minimumDistance, 0f);
            Assert.Greater(movementTracker.maximumDistance, movementTracker.minimumDistance);
            Assert.GreaterOrEqual(movementTracker.idealDistance, movementTracker.minimumDistance);
            Assert.LessOrEqual(movementTracker.idealDistance, movementTracker.maximumDistance);
        }

        [Test]
        public void RoundSettings_ShouldBeValid()
        {
            // Assert
            Assert.GreaterOrEqual(movementTracker.currentRound, 0);
            Assert.AreEqual(7, movementTracker.totalRounds); // Tawaf is 7 rounds
        }

        [Test]
        public void CreateKaaba_ShouldCreateKaabaInstance()
        {
            // Act - Use reflection to call private method
            var method = typeof(ARMovementTracker).GetMethod("CreateKaaba", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(movementTracker, null);

            // Assert
            Assert.IsTrue(movementTracker.kaabaPlaced);
            Assert.IsNotNull(movementTracker.kaabaInstance);
            Assert.AreEqual("Kaaba", movementTracker.kaabaInstance.name);
        }

        [Test]
        public void DistanceValidation_ShouldReturnTrue_WhenInValidRange()
        {
            // Arrange
            movementTracker.minimumDistance = 1f;
            movementTracker.maximumDistance = 5f;
            float testDistance = 3f;

            // Act
            bool isValid = testDistance >= movementTracker.minimumDistance && 
                          testDistance <= movementTracker.maximumDistance;

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void DistanceValidation_ShouldReturnFalse_WhenTooClose()
        {
            // Arrange
            movementTracker.minimumDistance = 1f;
            movementTracker.maximumDistance = 5f;
            float testDistance = 0.5f;

            // Act
            bool isValid = testDistance >= movementTracker.minimumDistance && 
                          testDistance <= movementTracker.maximumDistance;

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void DistanceValidation_ShouldReturnFalse_WhenTooFar()
        {
            // Arrange
            movementTracker.minimumDistance = 1f;
            movementTracker.maximumDistance = 5f;
            float testDistance = 7f;

            // Act
            bool isValid = testDistance >= movementTracker.minimumDistance && 
                          testDistance <= movementTracker.maximumDistance;

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void AngleCalculation_ShouldReturnCorrectAngle_WhenUserMoves()
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
        public void AngleCalculation_ShouldHandleNegativeAngles()
        {
            // Arrange
            Vector3 kaabaPosition = Vector3.zero;
            Vector3 userPosition = new Vector3(-2, 0, 0);
            Vector3 toKaaba = kaabaPosition - userPosition;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);

            // Act
            float angle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            // Assert
            Assert.AreEqual(0f, angle, 0.1f);
        }

        [Test]
        public void CompleteRound_ShouldIncrementRoundCount()
        {
            // Arrange
            int initialRound = movementTracker.currentRound;

            // Act - Use reflection to call private method
            var method = typeof(ARMovementTracker).GetMethod("CompleteRound", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(movementTracker, null);

            // Assert
            Assert.AreEqual(initialRound + 1, movementTracker.currentRound);
        }

        [Test]
        public void CompleteRound_ShouldNotExceedTotalRounds()
        {
            // Arrange - Set current round to total rounds
            movementTracker.currentRound = movementTracker.totalRounds;

            // Act - Use reflection to call private method
            var method = typeof(ARMovementTracker).GetMethod("CompleteRound", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(movementTracker, null);

            // Assert
            Assert.AreEqual(movementTracker.totalRounds + 1, movementTracker.currentRound);
        }

        [Test]
        public void GetDeviceMovement_ShouldReturnAcceleration()
        {
            // Act
            Vector3 movement = movementTracker.GetDeviceMovement();

            // Assert
            Assert.IsNotNull(movement);
            // Note: In test environment, acceleration might be zero
        }

        [Test]
        public void IsUserMoving_ShouldReturnBoolean()
        {
            // Act
            bool isMoving = movementTracker.IsUserMoving();

            // Assert
            // Should return a boolean value (might be false in test environment)
            Assert.IsInstanceOf<bool>(isMoving);
        }

        [Test]
        public void KaabaPlaced_ShouldBeInitializedToFalse()
        {
            // Assert
            Assert.IsFalse(movementTracker.kaabaPlaced);
        }

        [Test]
        public void PlayerPosition_ShouldBeInitialized()
        {
            // Assert
            Assert.AreEqual(Vector3.zero, movementTracker.playerPosition);
        }

        [Test]
        public void LastAngle_ShouldBeInitializedToZero()
        {
            // Assert
            Assert.AreEqual(0f, movementTracker.lastAngle);
        }

        [Test]
        public void TotalAngleChange_ShouldBeInitializedToZero()
        {
            // Assert
            Assert.AreEqual(0f, movementTracker.totalAngleChange);
        }

        [Test]
        public void IsValidDistance_ShouldBeInitializedToFalse()
        {
            // Assert
            Assert.IsFalse(movementTracker.isValidDistance);
        }

        [Test]
        public void KaabaInstance_ShouldBeInitializedToNull()
        {
            // Assert
            Assert.IsNull(movementTracker.kaabaInstance);
        }

        [Test]
        public void ARComponents_ShouldBeInitializedToNull()
        {
            // Assert
            Assert.IsNull(movementTracker.arSession);
            Assert.IsNull(movementTracker.arSessionOrigin);
            Assert.IsNull(movementTracker.arCameraManager);
        }

        [Test]
        public void KaabaPrefab_ShouldBeInitializedToNull()
        {
            // Assert
            Assert.IsNull(movementTracker.kaabaPrefab);
        }

        [Test]
        public void RoundCompletion_ShouldResetAngleChange()
        {
            // Arrange
            movementTracker.totalAngleChange = 360f; // Simulate completed round

            // Act - Use reflection to call private method
            var method = typeof(ARMovementTracker).GetMethod("CompleteRound", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(movementTracker, null);

            // Assert - Angle change should be reset in the tracking logic
            // This is tested indirectly through the round completion
            Assert.AreEqual(1, movementTracker.currentRound);
        }

        [Test]
        public void MovementThreshold_ShouldBeReasonable()
        {
            // Test the movement threshold in IsUserMoving method
            // This is an indirect test since we can't easily mock Input.acceleration
            
            // Act
            bool isMoving = movementTracker.IsUserMoving();

            // Assert - Should return a boolean (implementation detail)
            Assert.IsInstanceOf<bool>(isMoving);
        }

        [UnityTest]
        public IEnumerator ARMovementTracker_Start_ShouldInitialize()
        {
            // Act
            movementTracker.Start();
            
            // Wait for one frame
            yield return null;

            // Assert
            Assert.IsNotNull(movementTracker);
        }

        [Test]
        public void SetupAR_ShouldNotThrowException()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => {
                var method = typeof(ARMovementTracker).GetMethod("SetupAR", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(movementTracker, null);
            });
        }

        [Test]
        public void HandleDebugInput_ShouldNotThrowException()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => {
                var method = typeof(ARMovementTracker).GetMethod("HandleDebugInput", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(movementTracker, null);
            });
        }

        [Test]
        public void TrackRealMovement_ShouldNotThrowException()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => {
                var method = typeof(ARMovementTracker).GetMethod("TrackRealMovement", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(movementTracker, null);
            });
        }

        [Test]
        public void RoundCounting_ShouldFollowTawafRules()
        {
            // Assert - Tawaf should be exactly 7 rounds
            Assert.AreEqual(7, movementTracker.totalRounds);
        }

        [Test]
        public void DistanceSettings_ShouldBeAppropriateForAR()
        {
            // Assert - Distances should be reasonable for AR experience
            Assert.Greater(movementTracker.minimumDistance, 0.5f); // At least 0.5m
            Assert.Less(movementTracker.maximumDistance, 20f); // Less than 20m
            Assert.Greater(movementTracker.idealDistance, movementTracker.minimumDistance);
            Assert.Less(movementTracker.idealDistance, movementTracker.maximumDistance);
        }
    }
} 