using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace TawafARTrainer.Tests
{
    [TestFixture]
    public class CornerMarkerTests
    {
        private CornerMarker cornerMarker;
        private GameObject testGameObject;
        private TawafController mockTawafController;
        private GameObject mockKaaba;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject
            testGameObject = new GameObject("TestCornerMarker");
            cornerMarker = testGameObject.AddComponent<CornerMarker>();
            
            // Create mock TawafController
            GameObject controllerObj = new GameObject("MockTawafController");
            mockTawafController = controllerObj.AddComponent<TawafController>();
            
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
            
            if (mockTawafController != null)
                Object.DestroyImmediate(mockTawafController.gameObject);
            
            if (mockKaaba != null)
                Object.DestroyImmediate(mockKaaba);
        }

        [Test]
        public void CornerMarker_Initialization_ShouldSetDefaultValues()
        {
            // Assert
            Assert.IsTrue(cornerMarker.showCornerMarkers);
            Assert.AreEqual(2f, cornerMarker.markerDisplayDistance);
            Assert.AreEqual(1f, cornerMarker.markerFadeTime);
            Assert.AreEqual("", cornerMarker.currentCornerName);
            Assert.IsFalse(cornerMarker.isMarkerVisible);
            Assert.AreEqual(0f, cornerMarker.markerAlpha);
        }

        [Test]
        public void CornerNames_ShouldHaveFourCorners()
        {
            // Assert
            Assert.AreEqual(4, cornerMarker.cornerNames.Length);
            Assert.AreEqual("Hajar al-Aswad", cornerMarker.cornerNames[0]);
            Assert.AreEqual("Yemeni Corner", cornerMarker.cornerNames[1]);
            Assert.AreEqual("Shami Corner", cornerMarker.cornerNames[2]);
            Assert.AreEqual("Iraqi Corner", cornerMarker.cornerNames[3]);
        }

        [Test]
        public void CornerAngles_ShouldHaveFourAngles()
        {
            // Assert
            Assert.AreEqual(4, cornerMarker.cornerAngles.Length);
            Assert.AreEqual(90f, cornerMarker.cornerAngles[0]); // Hajar al-Aswad
            Assert.AreEqual(180f, cornerMarker.cornerAngles[1]); // Yemeni
            Assert.AreEqual(270f, cornerMarker.cornerAngles[2]); // Shami
            Assert.AreEqual(0f, cornerMarker.cornerAngles[3]); // Iraqi
        }

        [Test]
        public void CornerColors_ShouldHaveFourColors()
        {
            // Assert
            Assert.AreEqual(4, cornerMarker.cornerColors.Length);
            Assert.AreEqual(Color.red, cornerMarker.cornerColors[0]);
            Assert.AreEqual(Color.green, cornerMarker.cornerColors[1]);
            Assert.AreEqual(Color.blue, cornerMarker.cornerColors[2]);
            Assert.AreEqual(Color.yellow, cornerMarker.cornerColors[3]);
        }

        [Test]
        public void GetNearestCorner_ShouldReturnHajarAswad_WhenAt90Degrees()
        {
            // Arrange
            float angle = 90f;

            // Act
            int nearestCorner = cornerMarker.GetNearestCorner(angle);

            // Assert
            Assert.AreEqual(0, nearestCorner); // Hajar al-Aswad index
        }

        [Test]
        public void GetNearestCorner_ShouldReturnYemeni_WhenAt180Degrees()
        {
            // Arrange
            float angle = 180f;

            // Act
            int nearestCorner = cornerMarker.GetNearestCorner(angle);

            // Assert
            Assert.AreEqual(1, nearestCorner); // Yemeni Corner index
        }

        [Test]
        public void GetNearestCorner_ShouldReturnShami_WhenAt270Degrees()
        {
            // Arrange
            float angle = 270f;

            // Act
            int nearestCorner = cornerMarker.GetNearestCorner(angle);

            // Assert
            Assert.AreEqual(2, nearestCorner); // Shami Corner index
        }

        [Test]
        public void GetNearestCorner_ShouldReturnIraqi_WhenAt0Degrees()
        {
            // Arrange
            float angle = 0f;

            // Act
            int nearestCorner = cornerMarker.GetNearestCorner(angle);

            // Assert
            Assert.AreEqual(3, nearestCorner); // Iraqi Corner index
        }

        [Test]
        public void GetNearestCorner_ShouldHandleBoundaryAngles()
        {
            // Test angles near boundaries
            Assert.AreEqual(0, cornerMarker.GetNearestCorner(45f)); // Near Hajar al-Aswad
            Assert.AreEqual(1, cornerMarker.GetNearestCorner(135f)); // Near Yemeni
            Assert.AreEqual(2, cornerMarker.GetNearestCorner(225f)); // Near Shami
            Assert.AreEqual(3, cornerMarker.GetNearestCorner(315f)); // Near Iraqi
        }

        [Test]
        public void GetCurrentCornerName_ShouldReturnEmptyString_WhenNoCorner()
        {
            // Assert
            Assert.AreEqual("", cornerMarker.GetCurrentCornerName());
        }

        [Test]
        public void IsNearCorner_ShouldReturnFalse_WhenNoCorner()
        {
            // Assert
            Assert.IsFalse(cornerMarker.IsNearCorner());
        }

        [Test]
        public void GetCurrentCornerIndex_ShouldReturnMinusOne_WhenNoCorner()
        {
            // Assert
            Assert.AreEqual(-1, cornerMarker.GetCurrentCornerIndex());
        }

        [Test]
        public void MarkerDisplayDistance_ShouldBeReasonable()
        {
            // Assert
            Assert.Greater(cornerMarker.markerDisplayDistance, 0f);
            Assert.Less(cornerMarker.markerDisplayDistance, 10f); // Should be reasonable for AR
        }

        [Test]
        public void MarkerFadeTime_ShouldBeReasonable()
        {
            // Assert
            Assert.Greater(cornerMarker.markerFadeTime, 0f);
            Assert.Less(cornerMarker.markerFadeTime, 5f); // Should be quick but not instant
        }

        [Test]
        public void ShowCornerMarkers_ShouldBeConfigurable()
        {
            // Act
            cornerMarker.showCornerMarkers = true;
            bool markersEnabled = cornerMarker.showCornerMarkers;
            cornerMarker.showCornerMarkers = false;
            bool markersDisabled = cornerMarker.showCornerMarkers;

            // Assert
            Assert.IsTrue(markersEnabled);
            Assert.IsFalse(markersDisabled);
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
        public void AngleCalculation_ShouldHandlePositiveAngles()
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
        public void CornerAngles_ShouldBeInValidRange()
        {
            // Assert all corner angles are between 0 and 360 degrees
            foreach (float angle in cornerMarker.cornerAngles)
            {
                Assert.GreaterOrEqual(angle, 0f);
                Assert.Less(angle, 360f);
            }
        }

        [Test]
        public void CornerAngles_ShouldBeUnique()
        {
            // Assert all corner angles are unique
            for (int i = 0; i < cornerMarker.cornerAngles.Length; i++)
            {
                for (int j = i + 1; j < cornerMarker.cornerAngles.Length; j++)
                {
                    Assert.AreNotEqual(cornerMarker.cornerAngles[i], cornerMarker.cornerAngles[j]);
                }
            }
        }

        [Test]
        public void CornerNames_ShouldNotBeEmpty()
        {
            // Assert all corner names are not empty
            foreach (string name in cornerMarker.cornerNames)
            {
                Assert.IsFalse(string.IsNullOrEmpty(name));
            }
        }

        [Test]
        public void CornerNames_ShouldBeUnique()
        {
            // Assert all corner names are unique
            for (int i = 0; i < cornerMarker.cornerNames.Length; i++)
            {
                for (int j = i + 1; j < cornerMarker.cornerNames.Length; j++)
                {
                    Assert.AreNotEqual(cornerMarker.cornerNames[i], cornerMarker.cornerNames[j]);
                }
            }
        }

        [UnityTest]
        public IEnumerator CornerMarker_Start_ShouldInitialize()
        {
            // Act
            cornerMarker.Start();
            
            // Wait for one frame
            yield return null;

            // Assert
            Assert.IsNotNull(cornerMarker);
        }

        [Test]
        public void MarkerAlpha_ShouldBeInValidRange()
        {
            // Assert
            Assert.GreaterOrEqual(cornerMarker.markerAlpha, 0f);
            Assert.LessOrEqual(cornerMarker.markerAlpha, 1f);
        }

        [Test]
        public void MarkerStartTime_ShouldBeInitialized()
        {
            // Assert
            Assert.AreEqual(0f, cornerMarker.markerStartTime);
        }
    }
} 